using EvernoteClone.ViewModel;
using EvernoteClone.ViewModel.Helpers;
using System.IO;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        NotesVM viewModel;

        // SpeechRecognitionEngine to handle voice input.
        private SpeechRecognitionEngine recognizer;

        // Flag to track whether speech recognition is active
        bool isRecognizing = false;

        public NotesWindow()
        {
            InitializeComponent();

            viewModel = (NotesVM)Resources["vm"];
            viewModel.SelectedNotebookChanged += ViewModel_SelectedNotebookChanged;

            try
            {
                // Find the speech recognizer that matches the current system's culture.
                var currentCulture = (from recognizerInfo in SpeechRecognitionEngine.InstalledRecognizers()
                                      where recognizerInfo.Culture.Equals(Thread.CurrentThread.CurrentCulture)
                                      select recognizerInfo).FirstOrDefault();

                if (currentCulture != null)
                {
                    // Initialize the recognizer with the found culture.
                    recognizer = new SpeechRecognitionEngine(currentCulture);

                    // Set the audio input to the default microphone.
                    recognizer.SetInputToDefaultAudioDevice();

                    // Create a grammar for dictation (free-form speech).
                    GrammarBuilder grammarBuilder = new();
                    grammarBuilder.AppendDictation();
                    Grammar grammar = new(grammarBuilder);
                    recognizer.LoadGrammar(grammar);

                    // Subscribe to the event that fires when speech is recognized.
                    recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                }
                else
                {
                    // Display a message if no compatible speech recognizer is found.
                    MessageBox.Show("A speech recognizer could not be found for your system's current culture. The voice input feature will be unavailable.", "Speech Recognition Unavailable", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // The SpeechToggleButton is now accessible after InitializeComponent()
                    if (SpeechButton != null)
                    {
                        SpeechButton.IsEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions that occur during initialization, such as no microphone connected.
                MessageBox.Show($"Voice recognition initialization failed. Please ensure a microphone is connected and properly configured.\n\nError details: {ex.Message}", "Voice Input Error", MessageBoxButton.OK, MessageBoxImage.Error);

                // Disable the speech button to prevent the user from trying to use it.
                if (SpeechButton != null)
                {
                    SpeechButton.IsEnabled = false;
                }
            }

            // --- UI Setup ---
            // Populate the font family ComboBox with system fonts.
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            // Populate the font size ComboBox with a list of common sizes.
            List<double> fontSizes = [8, 9, 10, 11, 12, 15, 28, 48, 72];
            FontSizeComboBox.ItemsSource = fontSizes;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if(string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow = new();
                loginWindow.ShowDialog();
                
                viewModel.GetNotebooks();
            }
        }

        /// <summary>
        /// Event handler for when speech is successfully recognized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            // Get the recognized text.
            string recognizedText = e.Result.Text;
            // Append the recognized text as a new paragraph in the RichTextBox.
            // Assuming 'ContentRichTextBox' is the name of your RichTextBox.
            ContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(recognizedText)));
        }

        // Event handler for the 'Exit' menu item.
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Shut down the application.
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Event handler for the 'Speech' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRecognizing == false)
            {
                // Start continuous asynchronous speech recognition.
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                isRecognizing = true;
            }
            else
            {
                // Stop the asynchronous speech recognition.
                recognizer.RecognizeAsyncStop();
                isRecognizing = false;
            }
        }

        /// <summary>
        /// Event handler for when the RichTextBox content changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Calculate the total number of characters in the document.
            int ammountCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBlock.Text = $"Document Length: {ammountCharacters} characters";
        }

        #region Save and Load

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string rtfFile = Path.Combine(Environment.CurrentDirectory, $"{viewModel.SelectedNote.Id}.rtf");
            viewModel.SelectedNote.FileLocation = rtfFile;
            DatabaseHelper.Update(viewModel.SelectedNote);

            FileStream fileStream = new(rtfFile, FileMode.Create);
            var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
            contents.Save(fileStream, DataFormats.Rtf);
        }

        private void ViewModel_SelectedNotebookChanged(object? sender, EventArgs e)
        {
            // Clear the RichTextBox if no file location is set.
            ContentRichTextBox.Document.Blocks.Clear();

            if (viewModel.SelectedNotebook != null)
            {
                if (!string.IsNullOrEmpty(viewModel.SelectedNote.FileLocation))
                {
                    FileStream fileStream = new FileStream(viewModel.SelectedNote.FileLocation, FileMode.Open);
                    var contents = new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd);
                    contents.Load(fileStream, DataFormats.Rtf);
                }
            }
        }

        #endregion Save and Load

        #region ToolBarTray

        /// <summary>
        /// Event handler for the 'Bold' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the button is a ToggleButton and its checked state.
            bool isButtonChecked = ((ToggleButton)sender).IsChecked ?? false;

            if (isButtonChecked)
            {
                // Apply bold font weight to the selected text.
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            }
            else
            {
                // Apply normal font weight to the selected text.
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
            }
        }

        /// <summary>
        /// Event handler for the 'Italic' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the button is a ToggleButton and its checked state.
            bool isButtonChecked = ((ToggleButton)sender).IsChecked ?? false;

            if (isButtonChecked)
            {
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            }
            else
            {
                // Apply normal font style to the selected text.
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
            }
        }

        /// <summary>
        /// Event handler for the 'Underline' button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the button is a ToggleButton and its checked state.
            bool isButtonChecked = ((ToggleButton)sender).IsChecked ?? false;

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

        /// <summary>
        /// Event handler for when the RichTextBox selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Bold
            var selectedWeight = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            BoldButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && selectedWeight.Equals(FontWeights.Bold);

            // Italic
            var selectedStyle = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            ItalicButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            // Underline
            var selectedDecorations = ContentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineButton.IsChecked = (selectedWeight != DependencyProperty.UnsetValue) && (selectedDecorations.Equals(TextDecorations.Underline));

            // Font Family and Size
            FontFamilyComboBox.SelectedItem = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            FontSizeComboBox.Text = ContentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty).ToString();
        }

        #endregion ToolBarTray

        #region ComboBoxes

        private void FontFamilyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (FontFamilyComboBox.SelectedItem != null)
            {
                // Apply the selected font family to the selected text in the RichTextBox.
                ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, FontFamilyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Apply the selected font size to the selected text in the RichTextBox.
            ContentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, FontSizeComboBox.Text);
        }

        #endregion ComboBoxes
    }
}
