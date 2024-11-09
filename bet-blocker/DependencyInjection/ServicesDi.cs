using bet_blocker.Business;
using bet_blocker.Business.Interfaces;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;

namespace bet_blocker.DependencyInjection
{
    public class ServicesDi
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ICaller, Caller>();
            services.AddTransient<IBetBusiness, BetBusiness>();
        }
    }
}

