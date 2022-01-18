namespace Starnight.Internal.Rest;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Starnight.Internal.Utils;

/// <summary>
/// Represents a rest client for the discord API.
/// </summary>
public class RestClient
{
	private static readonly Regex __route_regex;
	private static HttpClient __http_client;
	private readonly ILogger? __logger;
	private readonly ConcurrentDictionary<String, RatelimitBucket> __ratelimit_buckets;

	public event Action<RatelimitBucket, HttpResponseMessage> SharedRatelimitHit = null!;
	public event Action<Guid> RequestDenied = null!;

	static RestClient()
	{
		__route_regex = new(@":([a-z_]+)");

		HttpClientHandler handler = new()
		{
			UseCookies = false,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
		};

		__http_client = new(handler)
		{
			BaseAddress = new Uri(DiscordApiConstants.BaseUri),
			Timeout = TimeSpan.FromSeconds(15)
		};
	}

	public static void SetClientProxy(IWebProxy proxy)
	{
		HttpClientHandler handler = new()
		{
			UseCookies = false,
			AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
			UseProxy = true,
			Proxy = proxy
		};

		TimeSpan timeout = __http_client.Timeout;

		__http_client = new(handler)
		{
			BaseAddress = new Uri(DiscordApiConstants.BaseUri),
			Timeout = timeout
		};
	}

	public static void SetTimeout(TimeSpan timeout)
		=> __http_client.Timeout = timeout;

	public RestClient(ILogger logger, Boolean enableQueue)
	{
		this.__logger = logger;
		this.__ratelimit_buckets = new();
	}

	// the GUID is intended for internal callback verification.
	public async Task<HttpResponseMessage> MakeRequestAsync(IRestRequest request, Guid guid)
	{
		if(!__route_regex.IsMatch(request.Route))
		{
			this.__logger?.LogError(LoggingEvents.RestClientRequestDenied,
				"Invalid request route. Please contact the library developers.");
			RequestDenied(guid);
			return null!;
		}

		HttpRequestMessage message = request.Build();

		// --- unreadable ternary ahead --- //

		RatelimitBucket v = !this.__ratelimit_buckets.ContainsKey(request.Route)
			? await this.createAndRegisterBucket(request.Route) // if this bucket is not registered yet: create one
			: this.__ratelimit_buckets[request.Route]; // we're good.

		// --- unreadable ternary over --- //

		if(!v.AllowRequest())
		{
#if DEBUG
			this.__logger?.LogError(LoggingEvents.RestClientRequestDenied,
				"Request {guid} to {route} was denied by the pre-emptive ratelimiter", guid, request.Route);
#else
				this.__logger?.LogWarning(LoggingEvents.RestClientRequestDenied,
					"The request was denied.");
#endif
			RequestDenied(guid);
		}

		HttpResponseMessage response = await __http_client.SendAsync(message);

		v.ProcessResponse(response);

		return response;
	}

	private Task<RatelimitBucket> createAndRegisterBucket(String route)
	{
		RatelimitBucket v = this.__ratelimit_buckets.AddOrUpdate(route,
			xm => new RatelimitBucket
			{
				Path = route,
				IsRatelimitDetermined = false
			},
			(x, y) => this.__ratelimit_buckets[route]);

		v.RatelimitHit += this.ratelimitHitHandler;
		v.SharedRatelimitHit += this.sharedRatelimitHitHandler;

		return Task.FromResult(v);
	}

	private void sharedRatelimitHitHandler(RatelimitBucket arg1, HttpResponseMessage arg2)
		=> this.SharedRatelimitHit(arg1, arg2);

	private void ratelimitHitHandler(RatelimitBucket arg1, HttpResponseMessage arg2, String arg3) => throw new NotImplementedException();
}
