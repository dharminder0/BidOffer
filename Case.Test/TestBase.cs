using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
namespace Case.Test
{
    public class TestBase
    {
        public ServiceProvider serviceProvider { get; set; }
        protected IConfiguration configuration;

        public TestBase()
        {
            var services = new ServiceCollection();
            var assembliesToScan = new[] {
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(Case.Business.IDependency)),
                Assembly.GetAssembly(typeof(IConfiguration)),
            };
         
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan).AsPublicImplementedInterfaces();
            var conf = GetIConfigurationRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            services.AddSingleton(typeof(IConfiguration), conf);
            serviceProvider = services.BuildServiceProvider();
            configuration = (IConfiguration)serviceProvider.GetService(typeof(IConfiguration));
        }

        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
        }

        public T Resolve<T>()
        {
            return serviceProvider.GetService<T>();
        }

        public void PrintOutput(object obj)
        {
            Console.WriteLine("---------------------Start------------------------------");
            Console.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
            Console.WriteLine("---------------------END------------------------------");
        }
    }
}
