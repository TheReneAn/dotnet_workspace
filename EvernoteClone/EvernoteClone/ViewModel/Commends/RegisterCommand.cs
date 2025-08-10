using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commends
{
    public class RegisterCommand : ICommand
    {
        public LoginVM ViewModel { get; set; }
        public event EventHandler CanExecuteChanged;

        public RegisterCommand(LoginVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //TODO: Call register from ViewModel
        }
    }
}
