using Microsoft.Extensions.DependencyInjection;
using Tutorial.Infrastructure.Facades.Common.HttpClients.Interfaces;

namespace Tutorial.Infrastructure.Facades.Common.HttpClients
{
    public static class Startup
    {
        public static IServiceCollection AddHttpClientSender(this IServiceCollection services)
        {
            services.AddTransient<IHttpClientSender, HttpClientSender>();
            return services;
        }

        //public static IServiceCollection AddClientSetting(this IServiceCollection services)
        //{
        //    services
        //        .AddOptions<HttpClientSettings>()
        //        .BindConfiguration(nameof(HttpClientSettings)) // Microsoft.Extensions.Options.ConfigurationExtensions
        //        .ValidateDataAnnotationsRecursively()
        //        .ValidateOnStart();

        //    return services;
        //}
    }
}