using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HlLib
{
    public static class Http
    {
        public class SendHandler : HttpMessageHandler
        {
            public SendHandler(Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return _handler(this, request, cancellationToken);
            }

            private Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;
        }

        public static async Task<HttpResponseMessage> GetAsync(
            string uri,
            CancellationToken cancellationToken = default(CancellationToken),
            bool configureAwait = false,
            Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> messageHandler = null)
        {
            var client = (messageHandler != null) ?
                new HttpClient(new SendHandler(messageHandler))
                : new HttpClient();

            return await client.GetAsync(uri, cancellationToken)
                .ConfigureAwait(configureAwait);
        }

        public static async Task<HttpResponseMessage> PostAsync(
            string uri,
            object parameters = null,
            CancellationToken cancellationToken = default(CancellationToken),
            bool configureAwait = false,
            Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> messageHandler = null)
        {
            return await PostAsync(
                uri,
                parameters.ToDictionary(),
                cancellationToken,
                configureAwait,
                messageHandler);
        }

        public static async Task<HttpResponseMessage> PostAsync(
            string uri,
            IDictionary<string, string> parameters = null,
            CancellationToken cancellationToken = default(CancellationToken),
            bool configureAwait = false,
            Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> messageHandler = null)
        {
            return await PostAsync(
                uri,
                new FormUrlEncodedContent(parameters),
                cancellationToken,
                configureAwait,
                messageHandler);
        }

        public static async Task<HttpResponseMessage> PostAsync(
            string uri,
            HttpContent content,
            CancellationToken cancellationToken = default(CancellationToken),
            bool configureAwait = false,
            Func<HttpMessageHandler, HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> messageHandler = null)
        {
            var client = (messageHandler != null) ?
                new HttpClient(new SendHandler(messageHandler))
                : new HttpClient();

            return await client.PostAsync(
                uri,
                content,
                cancellationToken)
                .ConfigureAwait(configureAwait);
        }
    }
}
