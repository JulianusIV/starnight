namespace Starnight.Internal.Rest.Resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Starnight.Caching.Abstractions;
using Starnight.Internal.Exceptions;
using Starnight.Internal.Entities.Channels;
using Starnight.Internal.Entities.Channels.Threads;
using Starnight.Internal.Entities.Guilds.Invites;
using Starnight.Internal.Entities.Messages;
using Starnight.Internal.Entities.Users;
using Starnight.Internal.Rest.Payloads.Channels;

using static Starnight.Internal.DiscordApiConstants;

using HttpMethodEnum = HttpMethod;

/// <summary>
/// Represents a request wrapper for all requests to the Channel resource.
/// </summary>
public class DiscordChannelRestResource : AbstractRestResource
{
	private readonly RestClient __rest_client;

	/// <inheritdoc/>
	public DiscordChannelRestResource
	(
		RestClient client,
		ICacheService cache
	)
		: base(cache)
		=> this.__rest_client = client;

	/// <summary>
	/// Returns a channel object for the given ID. If the channel is a thread channel, a
	/// <see cref="DiscordThreadMember"/> object is included in the returned channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordChannel> GetChannelAsync
	(
		Int64 channelId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Modifies a group DM channel with the given parameters.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the group DM in question.</param>
	/// <param name="payload">Payload object containing the modification parameters.</param>
	/// <returns>The modified channel object.</returns>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyGroupDMRequestPayload payload
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}"),
			Payload = JsonSerializer.Serialize(payload),
			Method = HttpMethodEnum.Patch,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Modifies a guild channel with the given parameters.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="payload">Payload object containing the modification parameters.</param>
	/// <param name="reason">Optional audit log reason for the edit.</param>
	/// <returns>The modified channel object.</returns>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyGuildChannelRequestPayload payload,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}"),
			Payload = JsonSerializer.Serialize(payload),
			Method = HttpMethodEnum.Patch,
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Modifies a thread channel with the given parameters.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="payload">Payload object containing the modification parameters.</param>
	/// <param name="reason">Optional audit log reason for the edit.</param>
	/// <returns>The modified channel object.</returns>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyThreadChannelRequestPayload payload,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}"),
			Payload = JsonSerializer.Serialize(payload),
			Method = HttpMethodEnum.Patch,
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Deletes a channel. Deleting guild channels cannot be undone. DM channels, however, cannot be deleted
	/// and are restored by opening a direct message channel again.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="reason">Optional audit log reason if this is a guild channel.</param>
	/// <returns>The associated channel object.</returns>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordChannel> DeleteChannelAsync
	(
		Int64 channelId,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}"),
			Method = HttpMethodEnum.Delete,
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Returns a set amount of messages, optionally before, after or around a certain message.
	/// </summary>
	/// <remarks>
	/// <c>around</c>, <c>before</c> and <c>after</c> are mutually exclusive. Only one may be passed. If multiple are passed,
	/// only the first one in the parameter list is respected, independent of the order they are passed in client code.
	/// </remarks>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="count">Maximum amount of messages to return.</param>
	/// <param name="around">Snowflake identifier of the center message of the requested block.</param>
	/// <param name="before">Snowflake identifier of the first older message than the requested block.</param>
	/// <param name="after">Snowflake identifier of the first newer message than the requested block.</param>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<IEnumerable<DiscordMessage>> GetChannelMessagesAsync
	(
		Int64 channelId,
		Int32 count,
		Int64? around = null,
		Int64? before = null,
		Int64? after = null
	)
	{
		QueryBuilder builder = new($"{BaseUri}/{Channels}/{channelId}/{Messages}");

		_ = builder.AddParameter("limit", count.ToString())
			.AddParameter("around", around.ToString())
			.AddParameter("before", before.ToString())
			.AddParameter("after", after.ToString());

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}",
			Url = builder.Build(),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordMessage>>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Gets a message by snowflake identifier.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordMessage> GetChannelMessageAsync
	(
		Int64 channelId,
		Int64 messageId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordMessage>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Creates a new message in a channel.
	/// </summary>
	/// <param name="channelId">snowflake identifier of the message's target channel.</param>
	/// <param name="payload">Message creation payload including potential attachment files.</param>
	/// <returns>The newly created message object.</returns>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordMessage> CreateMessageAsync
	(
		Int64 channelId,
		CreateMessageRequestPayload payload
	)
	{
		String payloadBody = JsonSerializer.Serialize(payload);

		IRestRequest request =

			payload.Files is null ?

				new RestRequest
				{
					Path = $"/{Channels}/{channelId}/{Messages}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}"),
					Payload = payloadBody,
					Method = HttpMethodEnum.Post,
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{Messages}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				} :

				new MultipartRestRequest
				{
					Path = $"/{Channels}/{channelId}/{Messages}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}"),
					Payload = String.IsNullOrWhiteSpace(payloadBody)
						? new()
						: new()
						{
							["payload_json"] = payloadBody
						},
					Method = HttpMethodEnum.Post,
					Files = payload.Files.ToList(),
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{MessageId}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordMessage>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Publishes a message in an announcement channel to following channels.
	/// </summary>
	/// <param name="channelId">Source announcement channel for this message.</param>
	/// <param name="messageId">Snowflake identifier of the message.</param>
	/// <exception cref="StarnightSharedRatelimitHitException">Thrown if the shared resource ratelimit is exceeded.</exception>
	public async ValueTask<DiscordMessage> CrosspostMessageAsync
	(
		Int64 channelId,
		Int64 messageId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Crosspost}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Crosspost}"),
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Crosspost}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordMessage>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Creates a reaction with the given emoji on the specified message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="emoji">String representation of the emoji.</param>
	/// <returns>Whether the reaction was added successfully.</returns>
	public async ValueTask<Boolean> CreateReactionAsync
	(
		Int64 channelId,
		Int64 messageId,
		String emoji
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{Me}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}/{emoji}/{Me}"),
			Method = HttpMethodEnum.Put,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{Me}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return response.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Deletes your own reaction with the specified emoji on the specified message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="emoji">String representation of the emoji.</param>
	/// <returns>Whether the reaction was removed successfully.</returns>
	public async ValueTask<Boolean> DeleteOwnReactionAsync
	(
		Int64 channelId,
		Int64 messageId,
		String emoji
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{Me}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}/{emoji}/{Me}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{Me}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return response.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Deletes the specified user's reaction with the specified emoji on the specified message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="userId">Snowflake identifier of the user in question.</param>
	/// <param name="emoji">String representation of the emoji.</param>
	/// <returns>Whether the reaction was removed successfully.</returns>
	public async ValueTask<Boolean> DeleteUserReactionAsync
	(
		Int64 channelId,
		Int64 messageId,
		Int64 userId,
		String emoji
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}/{emoji}/{userId}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return response.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Gets a list of users that reacted with the given emoji.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="emoji">String representation of the queried emoji.</param>
	/// <param name="after">Specifies a minimum user ID to return from, to paginate queries.</param>
	/// <param name="limit">Maximum amount of users to return. Defaults to 25.</param>
	public async ValueTask<IEnumerable<DiscordUser>> GetReactionsAsync
	(
		Int64 channelId,
		Int64 messageId,
		String emoji,
		Int64? after = null,
		Int32? limit = null
	)
	{
		QueryBuilder builder = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}/{emoji}");

		_ = builder.AddParameter("after", after.ToString())
			.AddParameter("limit", limit.ToString());

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}",
			Url = builder.Build(),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordUser>>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Deletes all reactions on the given message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	public async ValueTask DeleteAllReactionsAsync
	(
		Int64 channelId,
		Int64 messageId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		_ = await this.__rest_client.MakeRequestAsync(request);
	}

	/// <summary>
	/// Deletes all reactions with a specific emoji from the specified message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="emoji">String representation of the emoji in question.</param>
	public async ValueTask DeleteEmojiReactionsAsync
	(
		Int64 channelId,
		Int64 messageId,
		String emoji
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Reactions}/{emoji}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Reactions}/{Emoji}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		_ = await this.__rest_client.MakeRequestAsync(request);
	}

	/// <summary>
	/// Edits the given message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="payload">Edit payload.</param>
	public async ValueTask<DiscordMessage> EditMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		EditMessageRequestPayload payload
	)
	{
		String payloadBody = JsonSerializer.Serialize(payload);

		IRestRequest request =

			payload.Files is null ?

				new RestRequest
				{
					Path = $"/{Channels}/{channelId}/{Messages}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}"),
					Payload = payloadBody,
					Method = HttpMethodEnum.Patch,
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{Messages}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				} :

				new MultipartRestRequest
				{
					Path = $"/{Channels}/{channelId}/{Messages}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}"),
					Payload = String.IsNullOrWhiteSpace(payloadBody)
						? new()
						: new()
						{
							["payload_json"] = payloadBody
						},
					Method = HttpMethodEnum.Patch,
					Files = payload.Files.ToList(),
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{MessageId}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordMessage>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Deletes a message, potentially passing an audit log reason.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the message was successfully deleted.</returns>
	public async ValueTask<Boolean> DeleteMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}"),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Bulk deletes the given messages.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageIds">Up to 100 message IDs to delete. If any messages older than two weeks are included,
	/// or any of the IDs are duplicated, the entire request will fail.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the messages were deleted successfully.</returns>
	public async ValueTask<Boolean> BulkDeleteMessagesAsync
	(
		Int64 channelId,
		IEnumerable<Int64> messageIds,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{BulkDelete}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{BulkDelete}"),
			Payload = JsonSerializer.Serialize(messageIds),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{BulkDelete}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Edits a permission overwrite for a guild channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier for the channel in question.</param>
	/// <param name="overwriteId">Snowflake identifier of the entity (role/user) this overwrite targets.</param>
	/// <param name="payload">Edit payload.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the overwrite was successfully edited.</returns>
	public async ValueTask<Boolean> EditChannelPermissionsAsync
	(
		Int64 channelId,
		Int64 overwriteId,
		EditChannelPermissionsRequestPayload payload,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Permissions}/{OverwriteId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Permissions}/{overwriteId}"),
			Payload = JsonSerializer.Serialize(payload),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Put,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Permissions}/{OverwriteId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Returns a list of invite objects with invite metadata pointing to this channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	public async ValueTask<IEnumerable<DiscordInvite>> GetChannelInvitesAsync
	(
		Int64 channelId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Invites}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Invites}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Invites}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordInvite>>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Creates an invite on the specified channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="payload">Additional invite metadata.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>The newly created invite object.</returns>
	public async ValueTask<DiscordInvite> CreateChannelInviteAsync
	(
		Int64 channelId,
		CreateChannelInviteRequestPayload payload,
		String? reason = null
	)
	{
		String serializedPayload = JsonSerializer.Serialize(payload);

		// always pass an empty json object
		if(String.IsNullOrWhiteSpace(serializedPayload))
		{
			serializedPayload = "{}";
		}

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Invites}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Invites}"),
			Method = HttpMethodEnum.Post,
			Payload = serializedPayload,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Invites}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordInvite>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Deletes a channel permission overwrite.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	/// <param name="overwriteId">Snowflake identifier of the object this overwrite points to.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the deletion was successful.</returns>
	public async ValueTask<Boolean> DeleteChannelPermissionOverwriteAsync
	(
		Int64 channelId,
		Int64 overwriteId,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Permissions}/{OverwriteId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Permissions}/{overwriteId}"),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Permissions}/{OverwriteId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Follows a news channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the news channel to follow.</param>
	/// <param name="targetChannelId">Snowflake identifier of the channel you want messages to be cross-posted into.</param>
	/// <returns></returns>
	public async ValueTask<DiscordFollowedChannel> FollowNewsChannelAsync
	(
		Int64 channelId,
		Int64 targetChannelId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Followers}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Followers}"),
			Payload =
			$$"""
			{ "webhook_channel_id": "{{targetChannelId}}" }
			""",
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Followers}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordFollowedChannel>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Triggers the typing indicator for the current user in the given channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the channel in question.</param>
	public async ValueTask TriggerTypingIndicatorAsync
	(
		Int64 channelId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Typing}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Typing}"),
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Typing}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		_ = await this.__rest_client.MakeRequestAsync(request);
	}

	/// <summary>
	/// Returns all pinned messages as message objects.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the messages' parent channel.</param>
	public async ValueTask<IEnumerable<DiscordMessage>> GetPinnedMessagesAsync
	(
		Int64 channelId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Pins}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Pins}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Pins}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordMessage>>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Pins a message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the message was successfully pinned.</returns>
	public async ValueTask<Boolean> PinMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Pins}/{MessageId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Pins}/{messageId}"),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Pins}/{MessageId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Unpins a message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the message's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the message in question.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>Whether the message was successfully unpinned.</returns>
	public async ValueTask<Boolean> UnpinMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Pins}/{MessageId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Pins}/{messageId}"),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Pins}/{MessageId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Adds the given user to a specified group DM channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the group DM channel in question.</param>
	/// <param name="userId">Snowflake identifier of the user in question.</param>
	/// <param name="payload">Request payload, containing the access token needed.</param>
	public async ValueTask AddGroupDMRecipientAsync
	(
		Int64 channelId,
		Int64 userId,
		AddGroupDMRecipientRequestPayload payload
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Recipients}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Recipients}/{userId}"),
			Payload = JsonSerializer.Serialize(payload),
			Method = HttpMethodEnum.Put,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Recipients}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		_ = await this.__rest_client.MakeRequestAsync(request);
	}

	/// <summary>
	/// Removes the given user from the given group DM channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the group DM channel in question.</param>
	/// <param name="userId">Snowflake identifier of the user in question.</param>
	public async ValueTask RemoveGroupDMRecipientAsync
	(
		Int64 channelId,
		Int64 userId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Recipients}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Recipients}/{userId}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Recipients}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		_ = await this.__rest_client.MakeRequestAsync(request);
	}

	/// <summary>
	/// Creates a new thread channel from the given message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the thread's parent channel.</param>
	/// <param name="messageId">Snowflake identifier of the thread's parent message.</param>
	/// <param name="payload">Request payload for this request.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>The newly created thread channel.</returns>
	public async ValueTask<DiscordChannel> StartThreadFromMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		StartThreadFromMessageRequestPayload payload,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Threads}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Messages}/{messageId}/{Threads}"),
			Payload = JsonSerializer.Serialize(payload),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Messages}/{MessageId}/{Threads}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Creates a new thread channel without a message.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the thread's parent channel.</param>
	/// <param name="payload">Request payload for this request.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>The newly created thread channel.</returns>
	public async ValueTask<DiscordChannel> StartThreadWithoutMessageAsync
	(
		Int64 channelId,
		StartThreadWithoutMessageRequestPayload payload,
		String? reason = null
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{channelId}/{Threads}",
			Url = new($"{BaseUri}/{Channels}/{channelId}/{Threads}"),
			Payload = JsonSerializer.Serialize(payload),
			Headers = reason is not null ? new()
			{
				["X-Audit-Log-Reason"] = reason
			}
			: new(),
			Method = HttpMethodEnum.Post,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Threads}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Creates a new thread with a starting message in a forum channel.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the parent forum channel.</param>
	/// <param name="payload">A <see cref="CreateMessageRequestPayload"/> combined with a
	/// <see cref="StartThreadFromMessageRequestPayload"/>. A new message is created, then a thread is started from it.</param>
	/// <param name="reason">Optional audit log reason.</param>
	/// <returns>The newly created thread channel.</returns>
	public async ValueTask<DiscordChannel> StartThreadInForumChannelAsync
	(
		Int64 channelId,
		StartThreadInForumChannelRequestPayload payload,
		String? reason = null
	)
	{
		String payloadBody = JsonSerializer.Serialize(payload);

		IRestRequest request =

			payload.Files is null ?

				new RestRequest
				{
					Path = $"/{Channels}/{channelId}/{Threads}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Threads}"),
					Payload = payloadBody,
					Method = HttpMethodEnum.Post,
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{Threads}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				} :

				new MultipartRestRequest
				{
					Path = $"/{Channels}/{channelId}/{Threads}",
					Url = new($"{BaseUri}/{Channels}/{channelId}/{Threads}"),
					Payload = String.IsNullOrWhiteSpace(payloadBody)
						? new()
						: new()
						{
							["payload_json"] = payloadBody
						},
					Method = HttpMethodEnum.Post,
					Files = payload.Files.ToList(),
					Context = new()
					{
						["endpoint"] = $"/{Channels}/{channelId}/{Threads}",
						["cache"] = this.RatelimitBucketCache,
						["exempt-from-global-limit"] = false,
						["is-webhook-request"] = false
					}
				};

		HttpResponseMessage response = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordChannel>(await response.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Joins the current user into a thread.
	/// </summary>
	/// <param name="threadId">Snowflake identifier of the thread channel to be joined.</param>
	/// <returns>Whether the operation was successful.</returns>
	public async ValueTask<Boolean> JoinThreadAsync
	(
		Int64 threadId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}/{Me}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}/{Me}"),
			Method = HttpMethodEnum.Put,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}/{Me}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Adds another member into a thread.
	/// </summary>
	/// <param name="threadId">Snowflake identifier of the thread to be joined.</param>
	/// <param name="userId">Snowflake identifier of the user to join into the thread.</param>
	/// <returns>Whether the operation was successful.</returns>
	public async ValueTask<Boolean> AddToThreadAsync
	(
		Int64 threadId,
		Int64 userId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}/{userId}"),
			Method = HttpMethodEnum.Put,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Leaves a thread as the current bot.
	/// </summary>
	/// <param name="threadId">Snowflake identifier of the thread to be left.</param>
	/// <returns>Whether the operation was successful.</returns>
	public async ValueTask<Boolean> LeaveThreadAsync
	(
		Int64 threadId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}/{Me}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}/{Me}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}/{Me}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Removes another user from a thread.
	/// </summary>
	/// <param name="threadId">Snowflake identifier of the thread to be left.</param>
	/// <param name="userId">Snowflake identifier of the user to be removed.</param>
	/// <returns>Whether the operation was successful.</returns>
	public async ValueTask<Boolean> RemoveFromThreadAsync
	(
		Int64 threadId,
		Int64 userId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}/{userId}"),
			Method = HttpMethodEnum.Delete,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return message.StatusCode == HttpStatusCode.NoContent;
	}

	/// <summary>
	/// Returns a thread member object for the specified user.
	/// </summary>
	/// <param name="threadId">Snowflake identifier of the thread to obtain data from.</param>
	/// <param name="userId">Snowflake identifier of the user to obtain data for.</param>
	public async ValueTask<DiscordThreadMember> GetThreadMemberAsync
	(
		Int64 threadId,
		Int64 userId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}/{UserId}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}/{userId}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}/{UserId}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<DiscordThreadMember>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Returns a list of all thread members for the specified thread.
	/// </summary>
	/// <param name="threadId">Snowflake identifier fo the thread to obtain data from.</param>
	public async ValueTask<IEnumerable<DiscordThreadMember>> ListThreadMembersAsync
	(
		Int64 threadId
	)
	{
		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{ThreadMembers}",
			Url = new($"{BaseUri}/{Channels}/{threadId}/{ThreadMembers}"),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{threadId}/{ThreadMembers}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<IEnumerable<DiscordThreadMember>>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Returns all public, archived threads for this channel including respective thread member objects.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the thread's parent channel.</param>
	/// <param name="before">Timestamp to filter threads by: only threads archived before this timestamp will be returned.</param>
	/// <param name="limit">Maximum amount of threads to return.</param>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListPublicArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before = null,
		Int32? limit = null
	)
	{
		QueryBuilder builder = new($"{BaseUri}/{Channels}/{channelId}/{Threads}/{Archived}/{Public}");

		_ = builder.AddParameter("before", before.ToString())
			.AddParameter("limit", limit.ToString());

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{Threads}/{Archived}/{Public}",
			Url = builder.Build(),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Threads}/{Archived}/{Public}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<ListArchivedThreadsResponsePayload>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Returns all private, accessible, archived threads for this channel including respective thread member objects.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of the thread's parent channel.</param>
	/// <param name="before">Timestamp to filter threads by: only threads archived before this timestamp will be returned.</param>
	/// <param name="limit">Maximum amount of threads to return.</param>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListPrivateArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before = null,
		Int32? limit = null
	)
	{
		QueryBuilder builder = new($"{BaseUri}/{Channels}/{channelId}/{Threads}/{Archived}/{Private}");

		_ = builder.AddParameter("before", before.ToString())
			.AddParameter("limit", limit.ToString());

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{Threads}/{Archived}/{Private}",
			Url = builder.Build(),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Threads}/{Archived}/{Private}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<ListArchivedThreadsResponsePayload>(await message.Content.ReadAsStringAsync())!;
	}

	/// <summary>
	/// Returns a list of joined, private, archived threads.
	/// </summary>
	/// <param name="channelId">Snowflake identifier of their parent channel.</param>
	/// <param name="before">Timestamp to act as upper boundary for archival dates.</param>
	/// <param name="limit">Maximum amount of threads to return from this request.</param>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListJoinedPrivateArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before = null,
		Int32? limit = null
	)
	{
		QueryBuilder builder = new($"{BaseUri}/{Channels}/{channelId}/{Threads}/{Archived}/{Private}");

		_ = builder.AddParameter("before", before.ToString())
			.AddParameter("limit", limit.ToString());

		IRestRequest request = new RestRequest
		{
			Path = $"/{Channels}/{ChannelId}/{Users}/{Me}/{Threads}/{Archived}/{Private}",
			Url = builder.Build(),
			Method = HttpMethodEnum.Get,
			Context = new()
			{
				["endpoint"] = $"/{Channels}/{channelId}/{Users}/{Me}/{Threads}/{Archived}/{Private}",
				["cache"] = this.RatelimitBucketCache,
				["exempt-from-global-limit"] = false,
				["is-webhook-request"] = false
			}
		};

		HttpResponseMessage message = await this.__rest_client.MakeRequestAsync(request);

		return JsonSerializer.Deserialize<ListArchivedThreadsResponsePayload>(await message.Content.ReadAsStringAsync())!;
	}
}
