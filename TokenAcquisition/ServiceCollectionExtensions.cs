using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.TokenAcquisition
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTokenAcquisitionHandler(this IServiceCollection services, params IConfigurationSection[] sections)
        {
            AddProtectedResources(services, sections);
            services.AddTransient<TokenAcquisitionHandler>();
            return services;
        }

        private static void AddProtectedResources(IServiceCollection services, params IConfigurationSection[] sections)
        {
            var list = new ProtectedResourceList();

            foreach (var section in sections)
            {
                var options = section.Get<ProtectedResourceOptions>();
                var item = new ProtectedResource(options);
                list.Items.Add(item);
            }

            services.AddSingleton(list);
        }

    }
}
