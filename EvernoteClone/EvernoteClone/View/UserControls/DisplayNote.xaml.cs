using EvernoteClone.Model;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteClone.View.UserControls
{
    /// <summary>
    /// Interaction logic for DisplayNote.xaml
    /// </summary>
    public partial class DisplayNote : UserControl
    {
        public Note Note
        {
            get { return (Note)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NoteProperty =
            DependencyProperty.Register("Note", typeof(Note), typeof(DisplayNote), new PropertyMetadata(null, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplayNote? noteUserControl = d as DisplayNote;

            if (noteUserControl != null)
            {
                noteUserControl.DataContext = noteUserControl.Note;
            }
        }

        public DisplayNote()
        {
            InitializeComponent();
        }
    }
}
