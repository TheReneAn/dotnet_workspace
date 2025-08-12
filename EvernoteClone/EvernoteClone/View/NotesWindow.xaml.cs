using EvernoteClone.ViewModel;
using EvernoteClone.ViewModel.Helpers;
using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using Azure.Storage.Blobs;

namespace EvernoteClone.View
{
    public partial class NotesWindow : Window
    {
        NotesVM viewModel;

        private SpeechRecognitionEngine? recognizer;
        bool isRecognizing = false;

        public NotesWindow()
        {
            InitializeComponent();

            viewModel = (NotesVM)Resources["vm"];
            viewModel.SelectedNotebookChanged += ViewModel_SelectedNotebookChanged;

            // Speech recognizer initialization should be done carefully
            InitializeSpeechRecognizer();

            // --- UI Setup ---
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            List<double> fontSizes = [8, 9, 10, 11, 12, 15, 28, 48, 72];
            FontSizeComboBox.ItemsSource = fontSizes;
        }

        private void InitializeSpeechRecognizer()
        {
            try
            {
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
                    MessageBox.Show("A speech recognizer could not be found for your system's current culture. The voice input feature will be unavailable.", "Speech Recognition Unavailable", MessageBoxButton.OK, MessageBoxImage.Warning);
                    if (SpeechButton != null)
                    {
                        SpeechButton.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Voice recognition initialization failed. Please ensure a microphone is connected and properly configured.\n\nError details: {ex.Message}", "Voice Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (SpeechButton != null)
                {
                    SpeechButton.IsEnabled = false;
                }
                recognizer = null; // Ensure recognizer is null on failure
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow = new();
                loginWindow.ShowDialog();

                viewModel.GetNotebooks();
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

        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            // Null-check added for recognizer
            if (recognizer == null)
            {
                MessageBox.Show("Speech recognizer is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

        private void ContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int ammountCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;
            StatusTextBlock.Text = $"Document Length: {ammountCharacters} characters";
        }

        #region Save and Load

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = $"{viewModel.SelectedNote.Id}.rtf";
            string rtfFile = Path.Combine(Environment.CurrentDirectory, fileName);

            using (FileStream fileStream = new(rtfFile, FileMode.Create))
            {
                var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
                contents.Save(fileStream, DataFormats.Rtf);
            }

            viewModel.SelectedNote.FileLocation = await UpdateFile(rtfFile, fileName);
            await DatabaseHelper.Update(viewModel.SelectedNote);
        }

        /// <summary>
        /// Microsoft Azure Blob Storage connection string and container name are hardcoded for demonstration purposes.
        /// </summary>
        /// <param name="rtfFilesPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private async Task<string> UpdateFile(string rtfFilesPath, string fileName)
        {
            // Access Key
            string connectionString = "";
            string containerName = "notes";

            var container = new BlobContainerClient(connectionString, containerName);
            // container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(fileName);
            await blobClient.UploadAsync(rtfFilesPath);

            // URL to the uploaded file
            return $"https://evernotestorage2025.blob.core.windows.net/notes/{fileName}";
        }

        private async void ViewModel_SelectedNotebookChanged(object? sender, EventArgs e)
        {
            ContentRichTextBox.Document.Blocks.Clear();

            // Null-check for SelectedNote
            if (viewModel.SelectedNote != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedNote.FileLocation))
                {
                    string donwloadPath = $"{viewModel.SelectedNote.Id}.rtf";
                    await new BlobClient(new Uri(viewModel.SelectedNote.FileLocation)).DownloadToAsync(donwloadPath);
                    using (FileStream fileStream = new(donwloadPath, FileMode.Open))
                    {
                        var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
                        contents.Load(fileStream, DataFormats.Rtf);
                    }
                }
            }
        }

        #endregion Save and Load

        #region ToolBarTray

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.SelectedNote == null)
            {
                return;
            }

            if (sender is ToggleButton toggleButton)
            {
                var isButtonChecked = toggleButton.IsChecked ?? false;
                if (isButtonChecked)
                {
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
                }
                else
                {
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
                }
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var isButtonChecked = toggleButton.IsChecked ?? false;
                if (isButtonChecked)
                {
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
                }
                else
                {
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
                }
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
            {
                var isButtonChecked = toggleButton.IsChecked ?? false;
                if (isButtonChecked)
                {
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
                }
                else
                {
                    ((TextDecorationCollection)ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty)).TryRemove(TextDecorations.Underline, out TextDecorationCollection textDecorations);
                    ContentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
                }
            }
        }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Bold
            var selectedWeight = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldButton.IsChecked = (selectedWeight is FontWeight fontWeight) && fontWeight == FontWeights.Bold;

            // Italic
            var selectedStyle = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicButton.IsChecked = (selectedStyle is FontStyle fontStyle) && fontStyle == FontStyles.Italic;

            // Underline
            var selectedDecorations = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection;
            UnderlineButton.IsChecked = selectedDecorations?.Any(d => d.Location == TextDecorationLocation.Underline) ?? false;

            // Font Family and Size
            FontFamilyComboBox.SelectedItem = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);

            // Null check for FontSize property value
            var fontSizeProperty = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            FontSizeComboBox.Text = (fontSizeProperty is double size) ? size.ToString() : string.Empty;
        }

        #endregion ToolBarTray

        #region ComboBoxes

        private void FontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem is FontFamily selectedFontFamily)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, selectedFontFamily);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(FontSizeComboBox.Text, out double newSize))
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, newSize);
            }
        }

        #endregion ComboBoxes
    }
}