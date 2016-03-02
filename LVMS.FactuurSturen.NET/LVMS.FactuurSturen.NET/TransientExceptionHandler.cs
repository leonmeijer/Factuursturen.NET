using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LVMS.FactuurSturen.Exceptions;
using Newtonsoft.Json;
using Polly;
using PortableRest;

namespace LVMS.FactuurSturen
{
    internal static class TransientExceptionHandler
    {
        internal static async Task<T> ExecuteWithPolicyAsync<T>(this RestClient restClient, FactuurSturenClient factuurSturenClient, RestRequest restRequest,
            CancellationToken cancellationToken = default(CancellationToken), bool retryHttpPostAndPut = true, bool byPassCheckInitialized = false) where T : class
        {
            PreSendRequest<T>(factuurSturenClient, restRequest, byPassCheckInitialized);

            if (!factuurSturenClient.UsePollyTransientFaultHandling)
                return await restClient.ExecuteAsync<T>(restRequest, cancellationToken);

            try
            {
                var retVal = await SendRequestWithPolicy<T>(restClient, restRequest, cancellationToken);
                return retVal.Content;
            }
            catch (Exception e)
            {
                throw new RequestFailedLibException("Even with transient fault handling enabled, the request failed.", e);
            }
        }

        internal static async Task<RestResponse<T>> SendWithPolicyAsync<T>(this RestClient restClient, FactuurSturenClient factuurSturenClient, RestRequest restRequest,
            CancellationToken cancellationToken = default(CancellationToken), bool retryHttpPostAndPut = true, bool byPassCheckInitialized = false) where T : class
        {
            PreSendRequest<T>(factuurSturenClient, restRequest, byPassCheckInitialized);

            if (!factuurSturenClient.UsePollyTransientFaultHandling)
                return await restClient.SendAsync<T>(restRequest, cancellationToken);

            try
            {
                var retVal = await SendRequestWithPolicy<T>(restClient, restRequest, cancellationToken);
                return retVal;
            }
            catch (Exception e)
            {
                throw new RequestFailedLibException("Even with transient fault handling enabled, the request failed.", e);
            }
        }

        private static async Task<RestResponse<T>> SendRequestWithPolicy<T>(RestClient restClient, RestRequest restRequest, CancellationToken cancellationToken)
            where T : class
        {
            var retVal = await Policy
                .Handle<JsonReaderException>()
                .Or<RequestFailedLibException>()
                .Or<TaskCanceledException>()
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync<RestResponse<T>>(
                    () => SendRequestAndCheckForException<T>(restClient, restRequest, cancellationToken));
            return retVal;
        }

        private static void PreSendRequest<T>(FactuurSturenClient factuurSturenClient, RestRequest restRequest, bool byPassCheckInitialized) where T : class
        {
            if (factuurSturenClient == null)
                throw new ArgumentNullException(nameof(factuurSturenClient));

            if (!byPassCheckInitialized)
                factuurSturenClient.CheckInitialized();

            restRequest.ContentType = ContentTypes.Json;
            restRequest.AddHeader("Accept", "application/json");
        }

        private static async Task<RestResponse<T>> SendRequestAndCheckForException<T>(RestClient restClient, RestRequest restRequest,
            CancellationToken cancellationToken) where T : class
        {
            var response = await restClient.SendAsync<T>(restRequest, cancellationToken);
            if (response.Exception != null)
                throw response.Exception;
            else if (response.HttpResponseMessage.IsSuccessStatusCode)
                return response;
            else
            {
                var rateLimitRemainingHeader =
                    response.HttpResponseMessage.Headers.FirstOrDefault(h => h.Key == "X-RateLimit-Remaining");
                var rateLimitHeader = rateLimitRemainingHeader.Value?.FirstOrDefault();
                if (response.HttpResponseMessage.StatusCode == HttpStatusCode.Forbidden && rateLimitHeader == "0")
                    throw new RateLimitExceededLibException(response.HttpResponseMessage.StatusCode);
                else if (response.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.InternalServerError ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.ServiceUnavailable ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.BadGateway ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.GatewayTimeout ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.NoContent ||
                     response.HttpResponseMessage.StatusCode == HttpStatusCode.RequestTimeout)
                throw new RequestFailedLibException(response.HttpResponseMessage.StatusCode);
            else
                throw new FactuurSturenLibException("Unexpected HTTP status code. Code: " + response.HttpResponseMessage.StatusCode);
            }
            
        }
    }
}
