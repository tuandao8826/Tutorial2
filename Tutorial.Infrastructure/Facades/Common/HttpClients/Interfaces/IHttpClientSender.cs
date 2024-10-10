using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients.Interfaces
{
    public interface IHttpClientSender
    {
        /// <summary>
        /// Use to set client instance if not using default client
        /// </summary>
        IHttpClientSender UseClient(HttpClient httpClient, bool enableLogging);

        /// <summary>
        /// set http method (default is get)
        /// </summary>
        IHttpClientSender UseMethod(HttpMethod method);

        /// <summary>
        /// set request uri (default is empty)
        /// </summary>
        IHttpClientSender WithUri(string uri);

        /// <summary>
        /// set request uri (default is empty)
        /// </summary>
        IHttpClientSender WithUri(Uri uri);

        /// <summary>
        /// Header is object or keyvaluepair
        /// </summary>
        /// <param name="headers">Names/values of HTTP headers to set. Typically an anonymous object or IDictionary.</param>
        /// <param name="replaceUnderscoreWithHyphen">If true, underscores in property names will be replaced by hyphens. Default is true.</param>
        /// <exception cref="ArgumentNullException"><paramref name="headers"/> is <c>null</c>.</exception>
        IHttpClientSender WithHeaders(object headers, bool replaceUnderscoreWithHyphen = true);

        /// <summary>
        /// set request content (default is empty)
        /// </summary>
        IHttpClientSender WithContent(HttpContent content);

        Task<HttpResult> SendAsync(CancellationToken cancellationToken = default);
    }
}
