using System.Windows;
using RssReader.ViewModel;

namespace RssReader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DependencyInjector.Register<IRssHelper, RssHelper>();
            MainWindow = DependencyInjector.Retrieve<MainWindow>();
            MainWindow.Show();
        }
    }
}