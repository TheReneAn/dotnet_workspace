using System.Windows;
using RssReader.ViewModel;
using Unity;

namespace RssReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [Dependency]
        public MainVM ViewModel
        {
            set
            {
                DataContext = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
        }
    }
}