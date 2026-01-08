using Microsoft.Extensions.DependencyInjection;

namespace OmniBus.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Application layer registrations
            // Services are registered in Infrastructure layer
            return services;
        }
    }
}