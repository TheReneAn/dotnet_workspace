using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commends
{
    public class NewNotebookCommand : ICommand
    {
        public NotesVM ViewModel { get; set; }

        public event EventHandler CanExecuteChanged;

        public NewNotebookCommand(NotesVM vm)
        {
            ViewModel = vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ViewModel.CreateNotebook();
        }
    }
}
