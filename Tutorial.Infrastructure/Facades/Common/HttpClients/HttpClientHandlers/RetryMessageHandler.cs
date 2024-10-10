using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients.HttpClientHandlers
{
    public class RetryMessageHandler : DelegatingHandler
    {
        public RetryMessageHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Policy
                .Handle<HttpRequestException>()  // Xử lý HttpRequestException
                .Or<TaskCanceledException>()     // Xử lý TaskCanceledException do timeout
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode) // Retry nếu mã phản hồi không thành công
                .WaitAndRetryAsync(
                        3, // Số lần retry
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    )
                .ExecuteAsync(() => base.SendAsync(request, cancellationToken)
        );
    }
}
