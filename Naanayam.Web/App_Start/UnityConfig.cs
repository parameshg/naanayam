using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Naanayam.Data;
using Naanayam.Server;
using NLog;

namespace Naanayam.Web.App_Start
{
    public class UnityConfig
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();

            RegisterTypes(container);
            
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            container.RegisterInstance<ILogger>(LogManager.GetLogger("*"));
 
            container.RegisterType<IDatabase, Database>(new InjectionConstructor(Properties.Settings.Default.Database));

            container.RegisterType<IServer, Agent>(new InjectionConstructor(container.Resolve<IDatabase>(), new Context("anonymous")));
        }
    }
}