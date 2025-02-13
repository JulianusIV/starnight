namespace Starnight.Internal.Rest;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Contrib.WaitAndRetry;

using Starnight.Caching.Providers.Memory;
using Starnight.Exceptions;
using Starnight.Internal;
using Starnight.Internal.Rest.Resources;
using Starnight.Internal.Rest.Resources.Implementation;

/// <summary>
/// Contains an extension method on IServiceCollection to register Starnight's rest client
/// </summary>
public static class RestRegistration
{
	private static readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

	/// <summary>
	/// Registers the Starnight rest client into the given service collection.
	/// </summary>
	public static IServiceCollection AddStarnightRestClient
	(
		this IServiceCollection collection,
		RestClientOptions options
	)
	{
		PollyRateLimitPolicy ratelimiter = new();
		IEnumerable<TimeSpan> retryDelay = Backoff.DecorrelatedJitterBackoffV2
		(
			options.MedianFirstRequestRetryDelay,
			options.RetryCount
		);

		_ = collection
			.Configure<RestClientOptions>
			(
				xm =>
				{
					xm.MedianFirstRequestRetryDelay = options.MedianFirstRequestRetryDelay;
					xm.RatelimitedRetryCount = options.RatelimitedRetryCount;
					xm.RetryCount = options.RetryCount;
				}
			)
			.AddHttpClient<RestClient>()
			.ConfigureHttpClient
			(
				(client) =>
				{
					client.BaseAddress = new
					(
						$"{DiscordApiConstants.BaseUri}/"
					);
					client.DefaultRequestHeaders.UserAgent.Add
					(
						new
						(
							StarnightInternalConstants.UserAgentHeader,
							StarnightInternalConstants.Version
						)
					);
				}
			)
			.AddTransientHttpErrorPolicy
			(
				policy => policy.WaitAndRetryAsync
				(
					retryDelay
				)
				.WrapAsync
				(
					ratelimiter
				)
			)
			.AddPolicyHandler
			(
				Policy.HandleResult<HttpResponseMessage>
				(
					result => result.StatusCode == HttpStatusCode.TooManyRequests
				)
				.WaitAndRetryAsync
				(
					options.RatelimitedRetryCount,
					(_, response, _) =>
					{
						HttpResponseMessage message = response.Result;

						return message.Headers.GetValues
						(
							"X-RateLimit-Scope"
						)
						.SingleOrDefault() == "shared"
							? throw new StarnightSharedRatelimitHitException
							(
								"Shared ratelimit hit, not retrying request.",
								"Polly request retry policy"
							)
							: message == default
								? oneSecond
								: message.Headers.RetryAfter is null || message.Headers.RetryAfter.Delta is null
									? oneSecond
									: message.Headers.RetryAfter.Delta.Value;
					},
					(_, _, _, _) => Task.CompletedTask
				)
			);

		_ = collection
			.AddSingleton<IDiscordChannelRestResource, DiscordChannelRestResource>()
			.AddSingleton<IDiscordGuildRestResource, DiscordGuildRestResource>()
			.AddSingleton<IDiscordApplicationCommandsRestResource, DiscordApplicationCommandsRestResource>()
			.AddSingleton<IDiscordAuditLogRestResource, DiscordAuditLogRestResource>()
			.AddSingleton<IDiscordEmojiRestResource, DiscordEmojiRestResource>()
			.AddSingleton<IDiscordScheduledEventRestResource, DiscordScheduledEventRestResource>()
			.AddSingleton<IDiscordGuildTemplateRestResource, DiscordGuildTemplateRestResource>()
			.AddSingleton<IDiscordInviteRestResource, DiscordInviteRestResource>()
			.AddSingleton<IDiscordStageInstanceRestResource, DiscordStageInstanceRestResource>()
			.AddSingleton<IDiscordStickerRestResource, DiscordStickerRestResource>()
			.AddSingleton<IDiscordUserRestResource, DiscordUserRestResource>()
			.AddSingleton<IDiscordAutoModerationRestResource, DiscordAutoModerationRestResource>()
			.AddSingleton<IDiscordVoiceRestResource, DiscordVoiceRestResource>()
			.AddSingleton<IDiscordWebhookRestResource, DiscordWebhookRestResource>();

		_ = collection.Configure<MemoryCacheProviderOptions>
		(
			xm =>
			{
				_ = xm.SetSlidingExpiration<RatelimitBucket>
				(
					TimeSpan.FromSeconds(1)
				);

				_ = xm.SetAbsoluteExpiration<RatelimitBucket>
				(
					TimeSpan.FromSeconds(1)
				);
			}
		);

		return collection;
	}
}
