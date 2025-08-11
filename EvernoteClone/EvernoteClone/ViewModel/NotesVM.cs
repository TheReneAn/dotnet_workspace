using EvernoteClone.Model;
using EvernoteClone.ViewModel.Commends;
using EvernoteClone.ViewModel.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace EvernoteClone.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        public ObservableCollection<Notebook> Notebooks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }

        private Notebook selectedNotebook;
        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                // 1. First, check if the selected notebook is actually changing
                if (selectedNotebook == value)
                {
                    return;
                }

                selectedNotebook = value;
                OnPropertyChanged(nameof(SelectedNotebook));

                // 2. Reset SelectedNote to null to prevent referencing an old, invalid note
                SelectedNote = null;

                // 3. Now, get the notes for the new notebook
                GetNotes();
            }
        }

        private Note selectedNote;
        public Note? SelectedNote
        {
            get { return selectedNote; }
            set
            {
                if (selectedNote == value)
                {
                    return;
                }

                selectedNote = value;
                OnPropertyChanged(nameof(selectedNote));
                if (selectedNote != null)
                {
                    SelectedNotebookChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        private Visibility isVisible;
        public Visibility IsVisible
        {
            get { return isVisible; }
            set 
            { 
                isVisible = value; 
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        public NewNotebookCommand NewNotebookCommand { get; set; }
        public NewNoteCommand NewNoteCommand { get; set; }
        public EditCommand EditCommand { get; set; }
        public EndEditingCommand EndEditingCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler SelectedNotebookChanged;

        public NotesVM()
        {
            NewNoteCommand = new NewNoteCommand(this);
            NewNotebookCommand = new NewNotebookCommand(this);
            EditCommand = new EditCommand(this);
            EndEditingCommand = new EndEditingCommand(this);

            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();

            IsVisible = Visibility.Collapsed;

            GetNotebooks();
        }

        public async void CreateNotebook()
        {
            Notebook newNotebook = new()
            {
                Name = "Notebook",
                UserId = App.UserId
            };

            await DatabaseHelper.Insert(newNotebook);

            GetNotebooks();
        }

        public async void CreateNote(string notebookId)
        {
            Note newNote = new()
            {
                NotebookId = notebookId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Title = $"Note for {DateTime.Now.ToString()}"
            };

            await DatabaseHelper.Insert(newNote);

            GetNotes();
        }

        public async void GetNotebooks()
        {
            var notebooks = (await DatabaseHelper.Read<Notebook>()).Where(n => n.UserId == App.UserId).ToList();

            Notebooks.Clear();
            foreach (var notebook in notebooks)
            {
                Notebooks.Add(notebook);
            }
        }

        private async void GetNotes()
        {
            if (SelectedNotebook != null)
            {
                var notes = (await DatabaseHelper.Read<Note>()).Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

                Notes.Clear();
                foreach (var note in notes)
                {
                    Notes.Add(note);
                }

                if (Notes.Any())
                {
                    SelectedNote = Notes.First();
                }
                else
                {
                    // If the notebook is empty, explicitly set SelectedNote to null.
                    SelectedNote = null;
                }
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartEditing()
        {
            IsVisible = Visibility.Visible;
        }

        public void StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
            DatabaseHelper.Update(notebook);
            GetNotebooks();
        }
    }
}
