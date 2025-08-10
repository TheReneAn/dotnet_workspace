using EvernoteClone.Model;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commends
{
    public class RegisterCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RegisterCommand(LoginVM vm)
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

            // Check all necessary fields and password match for activation
            return !string.IsNullOrEmpty(user.Username) &&
                   !string.IsNullOrEmpty(user.Password) &&
                   !string.IsNullOrEmpty(user.Name) &&
                   !string.IsNullOrEmpty(user.Lastname) &&
                   !string.IsNullOrEmpty(user.ConfirmPassword) &&
                   user.Password == user.ConfirmPassword;
        }

        public void Execute(object parameter)
        {
            ViewModel.Register();
        }
    }
}
