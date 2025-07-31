using EvernoteClone.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EvernoteClone.ViewModel.Commands
{
    public class NewNoteCommand : ICommand
    {
        public NotesVM NoteVM { get; set; }

        public event EventHandler? CanExecuteChanged;

        public NewNoteCommand(NotesVM notesVM)
        {
            NoteVM = notesVM;
        }

        public bool CanExecute(object? parameter)
        {
            Notebook? selectedNotebook = parameter as Notebook;
            if (selectedNotebook != null)
            {
                return true;
            }

            return false; // Disable command if no notebook is selected
        }

        public void Execute(object? parameter)
        {
            Notebook? selectedNotebook = parameter as Notebook;
            NoteVM.CreateNote(selectedNotebook?.Id ?? 0);
        }
    }
}
