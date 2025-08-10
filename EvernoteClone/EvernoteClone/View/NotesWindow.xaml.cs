using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EvernoteClone.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        // SpeechRecognitionEngine to handle voice input.
        private SpeechRecognitionEngine recognizer;

        // Flag to track whether speech recognition is active
        bool isRecognizing = false;

        public NotesWindow()
        {
            InitializeComponent();

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
                MessageBox.Show("No speech recognizer found for the current system culture.");
            }

            // --- UI Setup ---
            // Populate the font family ComboBox with system fonts.
            var fontFamilies = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyComboBox.ItemsSource = fontFamilies;

            // Populate the font size ComboBox with a list of common sizes.
            List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 15, 28, 48, 72 };
            FontSizeComboBox.ItemsSource = fontSizes;
        }

        // Event handler for when speech is successfully recognized.
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
        private void ContentRichTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Calculate the total number of characters in the document.
            int ammountCharacters = (new TextRange(ContentRichTextBox.Document.ContentStart, ContentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBlock.Text = $"Document Length: {ammountCharacters} characters";
        }

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

            if (isButtonChecked == false)
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

            if (isButtonChecked == false)
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
