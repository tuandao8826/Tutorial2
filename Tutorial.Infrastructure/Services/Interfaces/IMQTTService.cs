using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure.Services.Interfaces
{
    public interface IMQTTService
    {
        Task ConnectAsync();
        Task CreateSubscribeOptionsBuilder(string topic);
        Task SendMessageAsync(string topic, string payload);
        Task<string> WaitForMessageAsync();
    }
}
