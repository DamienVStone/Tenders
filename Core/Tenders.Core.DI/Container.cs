using Microsoft.Extensions.DependencyInjection;
using System;
using Tenders.Core.Abstractions.Services;
using Tenders.Core.Services;
using Tenders.Integration.API.Interfaces;
using Tenders.Integration.API.Services;

namespace Tenders.Core.DI
{
    public class Container
    {
        private static object o = new object();
        private static ServiceProvider sc;
        private static bool _isInited = false;
        public static void Init(Action<IServiceCollection> registerExternal, ServiceCollection serviceCollection = null)
        {
            if (_isInited)
                return;

            lock (o)
            {
                if (_isInited)
                    return;

                serviceCollection = serviceCollection ?? new ServiceCollection();
                registerExternal(serviceCollection);
                sc = serviceCollection.BuildServiceProvider();
                _isInited = true;
            }
        }

        public static T GetService<T>()
        {
            if (!_isInited)
                throw new Exception("Container isn't inited");

            return sc.GetService<T>();
        }

        public static class Registration
        {
            public static void Register(IServiceCollection serviceCollection)
            {
                serviceCollection.AddSingleton<IProxyService, ProxyService>();
                serviceCollection.AddSingleton<ILoggerService, LoggerService>();
                serviceCollection.AddSingleton<IDataProvider, DataProvider>();
#if DEBUG
                serviceCollection.AddSingleton<IConfigService, LocalFileConfigService>();
#else
                serviceCollection.AddSingleton<IConfigService, EnvironmentVariablesConfigService>();
#endif
            }
        }
    }
}
