using EvernoteClone.Model;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commends
{
    public class LoginCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public LoginCommand(LoginVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            User? user = parameter as User;

            if (user == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }

            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.Login();
        }
    }
}