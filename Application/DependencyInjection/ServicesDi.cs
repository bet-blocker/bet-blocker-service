using Application.Business;
using Application.Business.Interfaces;
using Application.Services;
using Application.Services.Interfaces;

namespace Application.DependencyInjection
{
    public class ServicesDi
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ICaller, Caller>();
            services.AddSingleton<IBetBusiness, BetBusiness>();
        }
    }
}

