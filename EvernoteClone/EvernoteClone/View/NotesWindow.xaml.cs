using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        public NotesWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async Task SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ContentRichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int ammountCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBlock.Text = $"Document Length: {ammountCharacters} characters";
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
        }
    }
}
