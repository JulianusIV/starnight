namespace Starnight.Internal.Gateway.Payloads.Clientbound;

using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

/// <summary>
/// Dispatched if resumption was successful.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public record struct DiscordResumedGatewayEvent : IDiscordGatewayPayload
{
	/// <inheritdoc/>
	[JsonPropertyName("op")]
	public DiscordGatewayOpcode Opcode { get; init; }

	/// <inheritdoc/>
	[JsonPropertyName("s")]
	public Int32 Sequence { get; init; }

	/// <inheritdoc/>
	[JsonPropertyName("t")]
	public String EventName { get; init; }
}
