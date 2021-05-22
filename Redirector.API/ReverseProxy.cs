using System;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Redirector.API
{
    public class ReverseProxy
    {
        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13 | SslProtocols.Tls | SslProtocols.Tls11,
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
        });
        
        public static string teamserver = "127.0.0.1"; // point this to your listener
        public static async Task Invoke(HttpContext context)
        {
            var uri = new Uri("https://" + teamserver + context.Request.Path.ToUriComponent());

            var request  = CopyRequest(context, uri);
            var remoteRsp = await _httpClient.SendAsync(request);
            var rsp    = context.Response;

            foreach (var header in remoteRsp.Headers)
            {
                rsp.Headers.Add(header.Key, header.Value.ToArray());
            }

            rsp.ContentType  = remoteRsp.Content.Headers.ContentType?.ToString();
            rsp.ContentLength = remoteRsp.Content.Headers.ContentLength;

            await remoteRsp.Content.CopyToAsync(rsp.Body);
        }


        static HttpRequestMessage CopyRequest(HttpContext context, Uri targetUri)
        {
            var req = context.Request;
            var requestMessage = new HttpRequestMessage()
            {
                Method   = new HttpMethod(req.Method),
                Content  = new StreamContent(req.Body),
                RequestUri = targetUri,
            };

            foreach (var header in req.Headers)
            {
                requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            requestMessage.Headers.Host = targetUri.Host;

            return requestMessage;
        }
    }
}