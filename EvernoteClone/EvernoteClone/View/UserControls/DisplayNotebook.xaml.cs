using EvernoteClone.Model;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteClone.View.UserControls
{
    /// <summary>
    /// Interaction logic for DisplayNotebook.xaml
    /// </summary>
    public partial class DisplayNotebook : UserControl
    {
        public Notebook Notebook
        {
            get { return (Notebook)GetValue(NotebookProperty); }
            set { SetValue(NotebookProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotebookProperty =
            DependencyProperty.Register("Notebook", typeof(Notebook), typeof(DisplayNotebook), new PropertyMetadata(null, SetValues));

        private static void SetValues(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DisplayNotebook? notebookUserControl = d as DisplayNotebook;

            if (notebookUserControl != null)
            {
                notebookUserControl.DataContext = notebookUserControl.Notebook;
            }
        }

        public DisplayNotebook()
        {
            InitializeComponent();
        }
    }
}
