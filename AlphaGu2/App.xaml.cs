using AlphaGu2.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace AlphaGu2
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
