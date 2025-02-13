namespace Starnight.Caching.Shims;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Starnight.Caching;
using Starnight.Caching.Services;
using Starnight.Internal.Entities.Channels;
using Starnight.Internal.Entities.Channels.Threads;
using Starnight.Internal.Entities.Guilds.Invites;
using Starnight.Internal.Entities.Messages;
using Starnight.Internal.Entities.Users;
using Starnight.Internal.Rest.Payloads.Channels;
using Starnight.Internal.Rest.Resources;

/// <summary>
/// Represents a shim over the present channel commands rest resource, updating and corroborating from
/// cache where possible.
/// </summary>
public partial class CachingChannelRestResource : IDiscordChannelRestResource
{
	public readonly IDiscordChannelRestResource __underlying;
	public readonly IStarnightCacheService __cache;

	public CachingChannelRestResource
	(
		IDiscordChannelRestResource underlying,
		IStarnightCacheService cache
	)
	{
		this.__underlying = underlying;
		this.__cache = cache;
	}

	/// <inheritdoc/>
	public async ValueTask<Boolean> BulkDeleteMessagesAsync
	(
		Int64 channelId,
		IEnumerable<Int64> messageIds,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		Boolean value = await this.__underlying.BulkDeleteMessagesAsync
		(
			channelId,
			messageIds,
			reason,
			ct
		);

		foreach(Int64 id in messageIds)
		{
			_ = await this.__cache.EvictObjectAsync<DiscordMessage>
			(
				KeyHelper.GetMessageKey
				(
					id
				)
			);
		}

		return value;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordInvite> CreateChannelInviteAsync
	(
		Int64 channelId,
		CreateChannelInviteRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordInvite invite = await this.__underlying.CreateChannelInviteAsync
		(
			channelId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetInviteKey
			(
				invite.Code
			),
			invite
		);

		return invite;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordMessage> CreateMessageAsync
	(
		Int64 channelId,
		CreateMessageRequestPayload payload,
		CancellationToken ct = default
	)
	{
		DiscordMessage message = await this.__underlying.CreateMessageAsync
		(
			channelId,
			payload,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetMessageKey
			(
				message.Id
			),
			message
		);

		return message;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordMessage> CrosspostMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		CancellationToken ct = default
	)
	{
		DiscordMessage message = await this.__underlying.CrosspostMessageAsync
		(
			channelId,
			messageId,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetMessageKey
			(
				message.Id
			),
			message
		);

		return message;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> DeleteChannelAsync
	(
		Int64 channelId,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		_ = await this.__cache.EvictObjectAsync<DiscordChannel>
		(
			KeyHelper.GetChannelKey
			(
				channelId
			)
		);

		return await this.__underlying.DeleteChannelAsync
		(
			channelId,
			reason,
			ct
		);
	}

	/// <inheritdoc/>
	public async ValueTask<Boolean> DeleteMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		Boolean value = await this.__underlying.DeleteMessageAsync
		(
			channelId,
			messageId,
			reason,
			ct
		);

		if(!value)
		{
			return value;
		}

		_ = await this.__cache.EvictObjectAsync<DiscordMessage>
		(
			KeyHelper.GetMessageKey
			(
				messageId
			)
		);

		return value;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordMessage> EditMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		EditMessageRequestPayload payload,
		CancellationToken ct = default
	)
	{
		DiscordMessage message = await this.__underlying.EditMessageAsync
		(
			channelId,
			messageId,
			payload,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetMessageKey
			(
				messageId
			),
			message
		);

		return message;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> GetChannelAsync
	(
		Int64 channelId,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.GetChannelAsync
		(
			channelId,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channelId
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordInvite>> GetChannelInvitesAsync
	(
		Int64 channelId,
		CancellationToken ct = default
	)
	{
		IEnumerable<DiscordInvite> invites = await this.__underlying.GetChannelInvitesAsync
		(
			channelId,
			ct
		);

		await Parallel.ForEachAsync
		(
			invites,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetInviteKey
				(
					xm.Code
				),
				xm
			)
		);

		return invites;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordMessage> GetChannelMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		CancellationToken ct = default
	)
	{
		DiscordMessage message = await this.__underlying.GetChannelMessageAsync
		(
			channelId,
			messageId,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetMessageKey
			(
				messageId
			),
			message
		);

		return message;
	}

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordMessage>> GetChannelMessagesAsync
	(
		Int64 channelId,
		Int32 count,
		Int64? around = null,
		Int64? before = null,
		Int64? after = null,
		CancellationToken ct = default
	)
	{
		IEnumerable<DiscordMessage> messages = await this.__underlying.GetChannelMessagesAsync
		(
			channelId,
			count,
			around,
			before,
			after,
			ct
		);

		await Parallel.ForEachAsync
		(
			messages,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetMessageKey
				(
					xm.Id
				),
				xm
			)
		);

		return messages;
	}

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordMessage>> GetPinnedMessagesAsync
	(
		Int64 channelId,
		CancellationToken ct = default
	)
	{
		IEnumerable<DiscordMessage> messages = await this.__underlying.GetPinnedMessagesAsync
		(
			channelId,
			ct
		);

		await Parallel.ForEachAsync
		(
			messages,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetMessageKey
				(
					xm.Id
				),
				xm
			)
		);

		return messages;
	}

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordUser>> GetReactionsAsync
	(
		Int64 channelId,
		Int64 messageId,
		String emoji,
		Int64? after = null,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		IEnumerable<DiscordUser> users = await this.__underlying.GetReactionsAsync
		(
			channelId,
			messageId,
			emoji,
			after,
			limit,
			ct
		);

		await Parallel.ForEachAsync
		(
			users,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetUserKey
				(
					xm.Id
				),
				xm
			)
		);

		return users;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordThreadMember> GetThreadMemberAsync
	(
		Int64 threadId,
		Int64 userId,
		Boolean? withMember = null,
		CancellationToken ct = default
	)
	{
		DiscordThreadMember threadMember = await this.__underlying.GetThreadMemberAsync
		(
			threadId,
			userId,
			withMember,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetThreadMemberKey
			(
				threadId,
				userId
			),
			threadMember
		);

		return threadMember;
	}

	/// <inheritdoc/>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListJoinedPrivateArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		ListArchivedThreadsResponsePayload response = await this.__underlying.ListJoinedPrivateArchivedThreadsAsync
		(
			channelId,
			before,
			limit,
			ct
		);

		await Parallel.ForEachAsync
		(
			response.Threads,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetChannelKey
				(
					xm.Id
				),
				xm
			)
		);

		await Parallel.ForEachAsync
		(
			response.ThreadMembers,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetThreadMemberKey
				(
					xm.ThreadId,
					xm.UserId
				),
				xm
			)
		);

		return response;
	}

	/// <inheritdoc/>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListPrivateArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		ListArchivedThreadsResponsePayload response = await this.__underlying.ListPrivateArchivedThreadsAsync
		(
			channelId,
			before,
			null,
			ct
		);

		await Parallel.ForEachAsync
		(
			response.Threads,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetChannelKey
				(
					xm.Id
				),
				xm
			)
		);

		await Parallel.ForEachAsync
		(
			response.ThreadMembers,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetThreadMemberKey
				(
					xm.ThreadId,
					xm.UserId
				),
				xm
			)
		);

		return response;
	}

	/// <inheritdoc/>
	public async ValueTask<ListArchivedThreadsResponsePayload> ListPublicArchivedThreadsAsync
	(
		Int64 channelId,
		DateTimeOffset? before,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		ListArchivedThreadsResponsePayload response = await this.__underlying.ListPublicArchivedThreadsAsync
		(
			channelId,
			before,
			limit,
			ct
		);

		await Parallel.ForEachAsync
		(
			response.Threads,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetChannelKey
				(
					xm.Id
				),
				xm
			)
		);

		await Parallel.ForEachAsync
		(
			response.ThreadMembers,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetThreadMemberKey
				(
					xm.ThreadId,
					xm.UserId
				),
				xm
			)
		);

		return response;
	}

	/// <inheritdoc/>
	public async ValueTask<IEnumerable<DiscordThreadMember>> ListThreadMembersAsync
	(
		Int64 threadId,
		Boolean? withMember = null,
		Int64? after = null,
		Int32? limit = null,
		CancellationToken ct = default
	)
	{
		IEnumerable<DiscordThreadMember> members = await this.__underlying.ListThreadMembersAsync
		(
			threadId,
			withMember,
			after,
			limit,
			ct
		);

		await Parallel.ForEachAsync
		(
			members,
			async (xm, _) => await this.__cache.CacheObjectAsync
			(
				KeyHelper.GetThreadMemberKey
				(
					xm.ThreadId,
					xm.UserId
				),
				xm
			)
		);

		return members;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyGroupDMRequestPayload payload,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.ModifyChannelAsync
		(
			channelId,
			payload,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channelId
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyGuildChannelRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.ModifyChannelAsync
		(
			channelId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channelId
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> ModifyChannelAsync
	(
		Int64 channelId,
		ModifyThreadChannelRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.ModifyChannelAsync
		(
			channelId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channelId
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> StartThreadFromMessageAsync
	(
		Int64 channelId,
		Int64 messageId,
		StartThreadFromMessageRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.StartThreadFromMessageAsync
		(
			channelId,
			messageId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channel.Id
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> StartThreadInForumChannelAsync
	(
		Int64 channelId,
		StartThreadInForumChannelRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.StartThreadInForumChannelAsync
		(
			channelId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channel.Id
			),
			channel
		);

		return channel;
	}

	/// <inheritdoc/>
	public async ValueTask<DiscordChannel> StartThreadWithoutMessageAsync
	(
		Int64 channelId,
		StartThreadWithoutMessageRequestPayload payload,
		String? reason = null,
		CancellationToken ct = default
	)
	{
		DiscordChannel channel = await this.__underlying.StartThreadWithoutMessageAsync
		(
			channelId,
			payload,
			reason,
			ct
		);

		await this.__cache.CacheObjectAsync
		(
			KeyHelper.GetChannelKey
			(
				channel.Id
			),
			channel
		);

		return channel;
	}
}
