namespace Starnight.Internal.Gateway.Payloads.Clientbound.Dispatch;

using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

using Starnight.Internal.Entities.Channels;

/// <summary>
/// Fired when a thread is deleted.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public record struct DiscordThreadDeletedGatewayEvent : IDiscordGatewayDispatchPayload<DiscordChannel>
{
	/// <inheritdoc/>
	[JsonPropertyName("s")]
	public Int32 Sequence { get; init; }

	/// <inheritdoc/>
	[JsonPropertyName("t")]
	public String EventName { get; init; }

	/// <inheritdoc/>
	[JsonPropertyName("d")]
	public DiscordChannel Data { get; init; }

	/// <inheritdoc/>
	[JsonPropertyName("op")]
	public DiscordGatewayOpcode Opcode { get; init; }
}
