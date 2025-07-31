using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class NewNotebookCommand : ICommand
    {
        public NotesVM NoteVM { get; set; }

        public event EventHandler? CanExecuteChanged;

        public NewNotebookCommand(NotesVM notesVM)
        {
            NoteVM = notesVM;
        }

        public bool CanExecute(object? parameter)
        {
            return true; // Always allow execution for now
        }

        public void Execute(object? parameter)
        {
            NoteVM.CreateNotebook();
        }
    }
}
