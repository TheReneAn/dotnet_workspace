using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class LoginCommand : ICommand
    {
        public LoginVM LoginVM { get; set; }

        public event EventHandler? CanExecuteChanged;
        public LoginCommand(LoginVM loginVM)
        {
            LoginVM = loginVM;
        }

        public bool CanExecute(object? parameter)
        {
            return true; // Always allow execution for now
        }

        public void Execute(object? parameter)
        {
            // TODO: Implement login functionality
        }
    }
}
