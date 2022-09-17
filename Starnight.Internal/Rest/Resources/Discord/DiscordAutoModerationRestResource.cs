namespace Starnight.Internal.Rest.Resources.Discord;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Starnight.Caching.Abstractions;
using Starnight.Internal.Entities.Guilds.AutoModeration;
using Starnight.Internal.Rest.Payloads.AutoModeration;

using static DiscordApiConstants;

using HttpMethodEnum = HttpMethod;

/// <inheritdoc cref="IDiscordAutoModerationRestResource"/>
public sealed class DiscordAutoModerationRestResource
	: AbstractRestResource, IDiscordAutoModerationRestResource
{
	private readonly RestClient __rest_client;

	/// <inheritdoc/>
	public DiscordAutoModerationRestResource
	(
		RestClient client,
		ICacheService cache
	)
		: base(cache)
		=> this.__rest_client = client;

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordAutoModerationRule>> ListAutoModerationRulesAsync
	(
		Int64 guildId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}",
			Url = new($"{Guilds}/{guildId}/{AutoModeration}/{Rules}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordAutoModerationRule>>
			(await response.Content.ReadAsStringAsync(), StarnightInternalConstants.DefaultSerializerOptions)!;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordAutoModerationRule> GetAutoModerationRuleAsync
	(
		Int64 guildId,
		Int64 ruleId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
			Url = new($"{Guilds}/{guildId}/{AutoModeration}/{Rules}/{ruleId}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordAutoModerationRule>
			(await response.Content.ReadAsStringAsync(), StarnightInternalConstants.DefaultSerializerOptions)!;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordAutoModerationRule> CreateAutoModerationRuleAsync
	(
		Int64 guildId,
		CreateAutoModerationRuleRequestPayload payload,
		String? reason
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}",
			Url = new($"{Guilds}/{guildId}/{AutoModeration}/{Rules}"),
			Method = HttpMethodEnum.Post,
			Payload = JsonSerializer.Serialize(payload, StarnightInternalConstants.DefaultSerializerOptions),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordAutoModerationRule>
			(await response.Content.ReadAsStringAsync(), StarnightInternalConstants.DefaultSerializerOptions)!;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordAutoModerationRule> ModifyAutoModerationRuleAsync
	(
		Int64 guildId,
		Int64 ruleId,
		ModifyAutoModerationRuleRequestPayload payload,
		String? reason
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
			Url = new($"{Guilds}/{guildId}/{AutoModeration}/{Rules}/{ruleId}"),
			Method = HttpMethodEnum.Patch,
			Payload = JsonSerializer.Serialize(payload, StarnightInternalConstants.DefaultSerializerOptions),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordAutoModerationRule>
			(await response.Content.ReadAsStringAsync(), StarnightInternalConstants.DefaultSerializerOptions)!;
	}

	/// <inheritdoc/>
	public async ValueTask<Boolean> DeleteAutoModerationRuleAsync
	(
		Int64 guildId,
		Int64 ruleId,
		String? reason
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
			Url = new($"{Guilds}/{guildId}/{AutoModeration}/{Rules}/{ruleId}"),
			Method = HttpMethodEnum.Delete,
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Guilds}/{guildId}/{AutoModeration}/{Rules}/{AutoModerationRuleId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return response.StatusCode == HttpStatusCode.NoContent;
	}
}
