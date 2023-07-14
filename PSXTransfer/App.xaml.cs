using Autofac;
using Autofac.Features.ResolveAnything;
using PSXTransfer.WPF.MVVM.Data;
using PSXTransfer.WPF.MVVM.Services;
using PSXTransfer.WPF.MVVM.Services.Interfaces;
using PSXTransfer.WPF.MVVM.ViewModels;
using System.Windows;

namespace PSXTransfer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ContainerBuilder builder = new();
            //allow the Autofac container resolve unknown types
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            //register the MyDataService class as the IDataService interface in the DI container
            builder.RegisterType<PSXTransferService>().As<IPSXTransferService>().SingleInstance();
            IContainer container = builder.Build();
            //get a MainViewModel instance
            PSXTransferViewModel mainViewModel = container.Resolve<PSXTransferViewModel>();

            DISource.Resolver = (type) =>
            {
                return container.Resolve(type);
            };
        }
    }
}
