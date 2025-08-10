using EvernoteClone.ViewModel;
using System.Windows;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        LoginVM viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            viewModel = (LoginVM)Resources["vm"];
            viewModel.Authenticated += ViewModel_Authenticated;
        }

        private void ViewModel_Authenticated(object? sender, EventArgs e)
        {
            Close();
        }
    }
}