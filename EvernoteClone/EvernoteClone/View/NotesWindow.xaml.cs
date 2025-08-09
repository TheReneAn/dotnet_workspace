using System.Speech.Recognition;
using System.Windows;
using System.Windows.Documents;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        SpeechRecognitionEngine recognizer;

        public NotesWindow()
        {
            InitializeComponent();

            var currentCulture = (from recognizerInfo in SpeechRecognitionEngine.InstalledRecognizers()
                                  where recognizerInfo.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                  select recognizerInfo).FirstOrDefault();

            if (currentCulture != null)
            {
                recognizer = new SpeechRecognitionEngine(currentCulture);
                recognizer.SetInputToDefaultAudioDevice();

                GrammarBuilder grammarBuilder = new();
                grammarBuilder.AppendDictation();
                Grammar grammar = new(grammarBuilder);
                recognizer.LoadGrammar(grammar);

                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
            }
            else
            {
                // Handle case where no recognizer for the current culture is found
                MessageBox.Show("No speech recognizer found for the current system culture.");
            }
        }

        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(recognizedText)));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        bool isRecognizing = false;
        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecognizing == false)
            {
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                isRecognizing = true;
            }
            else
            {
                recognizer.RecognizeAsyncStop();
                isRecognizing = false;
            }
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