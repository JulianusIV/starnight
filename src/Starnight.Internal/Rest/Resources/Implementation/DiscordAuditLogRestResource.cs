namespace Starnight.Internal.Rest.Resources.Implementation;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Starnight.Caching.Providers.Abstractions;
using Starnight.Internal.Entities.Guilds.Audit;

using static DiscordApiConstants;

/// <inheritdoc cref="IDiscordAuditLogRestResource"/>
public sealed class DiscordAuditLogRestResource
	: AbstractRestResource, IDiscordAuditLogRestResource
{
	private readonly RestClient restClient;

	/// <inheritdoc/>
	public DiscordAuditLogRestResource
	(
		RestClient client,
		ICacheProvider cache
	)
		: base(cache)
		=> this.restClient = client;

	/// <inheritdoc/>
	public async ValueTask<DiscordAuditLogObject> GetGuildAuditLogAsync
	(
		Int64 guildId,
		Int64? userId = null,
		DiscordAuditLogEvent? actionType = null,
		Int64? before = null,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		QueryBuilder builder = new
		(
			$"{Guilds}/{guildId}/{AuditLogs}"
		);

		_ = builder.AddParameter
			(
				"user_id",
				userId.ToString()
			)
			.AddParameter
			(
				"action_type",
				((Int32?)actionType).ToString()
			)
			.AddParameter
			(
				"before",
				before.ToString()
			)
			.AddParameter
			(
				"limit",
				limit.ToString()
			);

		IRestRequest request = new RestRequest
		{
			Url = builder.Build(),
			Method = HttpMethod.Get,
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AuditLogs}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.restClient.MakeRequestAsync
		(
			request,
			ct
		);

		return JsonSerializer.Deserialize<DiscordAuditLogObject>
		(
			await response.Content.ReadAsStringAsync
			(
				ct
			),
			StarnightInternalConstants.DefaultSerializerOptions
		)!;
	}
}
