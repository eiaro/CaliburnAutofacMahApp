using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using CaliburnAutofacMahApp.ViewModels;

namespace CaliburnAutofacMahApp
{
    public class Bootstrapper : BootstrapperBase
    {
        private IContainer container;

        protected override void BuildUp(object instance)
        {
            container.InjectProperties(instance);
        }

        protected override void Configure()
        {
            base.Configure();
            var builder = new ContainerBuilder();
            builder.RegisterType<MainWindowViewModel>();
            builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
            container = builder.Build();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return key == null ? container.Resolve(service) : container.ResolveKeyed(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var serviceType = typeof(IEnumerable<>).MakeGenericType(service);
            return (IEnumerable<object>)container.Resolve(serviceType);
        }

        private static string[] GetAllDllEntries()
        {
            var runDir = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(runDir).Where(file => Regex.IsMatch(file, @"^.+\.(exe|dll)$")).ToArray();

            return files;
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return GetAllDllEntries().Select(Assembly.LoadFrom);
        }
    }
}
