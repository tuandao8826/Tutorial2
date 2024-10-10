using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients.Interfaces
{
    public interface IHttpResult
    {
        Task<string?> ReadAsStringAsync(CancellationToken cancellationToken = default);
        Task<TResponse?> ReadFromJsonAsync<TResponse>(JsonSerializerOptions? options = null, CancellationToken cancellationToken = default);
        Task<Stream?> ReadAsStreamAsync(CancellationToken cancellationToken = default);
        Task<byte[]> ReadAsByteArrayAsync(CancellationToken cancellationToken = default);
    }
}
