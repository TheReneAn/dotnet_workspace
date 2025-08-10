using EvernoteClone.Model;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commends
{
    public class EndEditingCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private NotesVM ViewModel { get; set; }

        public EndEditingCommand(NotesVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Notebook? notebook = parameter as Notebook;
            if (notebook != null)
            {
                ViewModel.StopEditing(notebook);
            }
        }
    }
}
