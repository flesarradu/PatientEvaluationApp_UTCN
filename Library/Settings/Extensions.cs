using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace PatientEvaluationApp_UTCN.Lib
{
    public static class Extensions
    {
        public static IServiceCollection AddAppMain(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.Scan(scan => scan.FromCallingAssembly()
                                      .AddClasses(c => c.InNamespaces(
                                          "PatientEvaluationApp_UTCN.DataModels",
                                          "PatientEvaluationApp_UTCN.ViewModels",
                                          "PatientEvaluationApp_UTCN.Alerts"))
                                      .AsMatchingInterface()
                                      .WithTransientLifetime());

            return services;
        }
        public static IServiceCollection AddAppSettings(this IServiceCollection services, IAppSettings settings)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            services.AddSingleton(settings);

            return services;
        }
        public static IServiceCollection AddCommonLibrary(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ILocalStorageService, LocalStorageService>();

            return services;
        }
    }
}
