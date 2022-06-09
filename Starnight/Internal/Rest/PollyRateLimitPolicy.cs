namespace Starnight.Internal.Rest;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

using Polly;

public class PollyRateLimitPolicy : AsyncPolicy<HttpResponseMessage>
{
	private readonly ConcurrentDictionary<String, String> __endpoint_buckets;
	private readonly RatelimitBucket __global_bucket;

	private readonly static TimeSpan __one_second = TimeSpan.FromSeconds(1);

	public PollyRateLimitPolicy()
	{
		// 50 per second is discord's defined global ratelimit
		this.__global_bucket = new(50, 50, DateTimeOffset.UtcNow.AddSeconds(1), "global");
		this.__endpoint_buckets = new();
	}

	protected override async Task<HttpResponseMessage> ImplementationAsync(
		Func<Context, CancellationToken, Task<HttpResponseMessage>> action,
		Context context,
		CancellationToken cancellationToken,
		Boolean continueOnCapturedContext = true)
	{
		if(!context.TryGetValue("endpoint", out Object endpointRaw) || endpointRaw is not String endpoint)
		{
			throw new InvalidOperationException("No endpoint passed.");
		}

		if(!context.TryGetValue("cache", out Object cacheRaw) || cacheRaw is not IMemoryCache cache)
		{
			throw new InvalidOperationException("No cache passed.");
		}

		// fail-earlies done, policy time
		DateTimeOffset instant = DateTimeOffset.UtcNow;
		Boolean exemptFromGlobalLimit = false;
		String bucketHash;

		if(context.TryGetValue("exempt-from-global-limit", out Object exemptRaw) && exemptRaw is Boolean exempt)
		{
			exemptFromGlobalLimit = exempt;
		}

		if(!exemptFromGlobalLimit)
		{
			if(this.__global_bucket.ResetTime < instant)
			{
				this.__global_bucket.ResetBucket(instant + __one_second);
			}

			if(!this.__global_bucket.AllowNextRequest())
			{
				HttpResponseMessage response = new(HttpStatusCode.TooManyRequests);

				response.Headers.RetryAfter = new RetryConditionHeaderValue(this.__global_bucket.ResetTime - instant);
				response.Headers.Add("X-Starnight-Internal-Response", "global");

				return response;
			}
		}

		bucketHash = this.__endpoint_buckets.TryGetValue(endpoint, out String? bucketHashNullable)
			? bucketHashNullable
			: endpoint;

		if(cache.TryGetValue(bucketHash, out RatelimitBucket? bucket))
		{
			if(!bucket?.AllowNextRequest() ?? false)
			{
				HttpResponseMessage response = new(HttpStatusCode.TooManyRequests);

				response.Headers.RetryAfter = new RetryConditionHeaderValue(bucket!.ResetTime - instant);
				response.Headers.Add("X-Starnight-Internal-Response", "bucket");

				return response;
			}
		}

		HttpResponseMessage message = await action(context, cancellationToken).ConfigureAwait(continueOnCapturedContext);

		if(!RatelimitBucket.ExtractRatelimitBucket(message.Headers, out RatelimitBucket? extractedBucket))
		{
			return message;
		}

		if(extractedBucket.Hash is null)
		{
			_ = this.__endpoint_buckets.TryRemove(endpoint, out _);

			// expire a second later, to pre-act a server/local time desync
			_ = cache.Set(endpoint, extractedBucket, extractedBucket.ResetTime + __one_second);

			return message;
		}

		_ = this.__endpoint_buckets.AddOrUpdate(endpoint, extractedBucket.Hash, (_, _) => extractedBucket.Hash);
		_ = cache.Set(endpoint, extractedBucket, extractedBucket.ResetTime + __one_second);

		if(extractedBucket.Hash != bucketHash)
		{
			cache.Remove(bucketHash);
		}

		return message;
	}
}
