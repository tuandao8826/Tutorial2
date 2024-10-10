using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients.HttpClientHandlers
{
    public class CheckUrlHandler : DelegatingHandler
    {
        public CheckUrlHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage();

            var host = request.RequestUri.Host.ToLower();
            var scheme = request.RequestUri.Scheme.ToLower();

            if (scheme == "http")
            {
                response.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("URL không an toàn!"));
                return await Task.FromResult(response);
            }

            if (host == "ABC")
            {
                response.Content = new ByteArrayContent(Encoding.UTF8.GetBytes("Host không được phép"));
                return await Task.FromResult(response);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
