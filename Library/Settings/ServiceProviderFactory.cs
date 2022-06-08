using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace PatientEvaluationApp_UTCN.Lib
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class ServiceProviderFactory
    {
        #region Fields
        private static ServiceCollection serviceCollection;
        #endregion

        #region Properties
        public static IServiceProvider ServiceProvider { get; private set; }
        #endregion

        #region Public Methods
        public static IServiceCollection Create()
        {
            ServiceProviderFactory.serviceCollection = new ServiceCollection();
            return ServiceProviderFactory.serviceCollection;
        }

        public static void BuildServiceProvider()
        {
            ServiceProviderFactory.ServiceProvider = ServiceProviderFactory.serviceCollection.BuildServiceProvider();
        }
        #endregion
    }
}