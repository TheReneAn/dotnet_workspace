using LandmarkAI.Classes;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LandmarkAI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create a new OpenFileDialog object. This dialog allows users to select files.
            OpenFileDialog fileDialog = new OpenFileDialog();
            // Set the filter to display only image files (PNG, JPG, JPEG) in the dialog.
            fileDialog.Filter = "Image Files (*.png; *.jpg)|*.jpg;*.jpeg;*.png;|All Files (*.*)|*.*";
            // Set the initial directory where the file dialog will open.
            fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // Show the file dialog to the user.
            // If the user selects a file and clicks "OK" (ShowDialog() returns true), proceed.
            if (fileDialog.ShowDialog() == true)
            {
                // Get the full path and name of the file the user selected.
                string fileName = fileDialog.FileName;

                // Create a new BitmapImage from the selected file's URI.
                // This image will then be displayed in the 'SelectedImage' control
                SelectedImage.Source = new BitmapImage(new Uri(fileName));

                // Call the asynchronous MakePredictionAsync method with the selected file name.
                Task task = MakePredictionAsync(fileName);
            }
        }

        private async Task MakePredictionAsync(string fileName)
        {
            // The URL for the Custom Vision prediction endpoint
            string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/e164b78c-e740-499c-ac22-387c2b9b0784/classify/iterations/Landmark/image";
            // The prediction key for authenticating with the Custom Vision service
            string predictionKey = "b014fbb2f3cb41cb888e034fabcefc8b";
            string contentType = "application/octet-stream";

            // Read the file as a byte array
            var file = File.ReadAllBytes(fileName);

            // Create an HttpClient to send the HTTP request
            using (HttpClient client = new HttpClient())
            {
                // Add the prediction key to the request headers for authentication
                client.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);

                // Create ByteArrayContent from the file bytes
                using (var content = new ByteArrayContent(file))
                {
                    // Explicitly set the Content-Type header for the content being sent
                    // This is redundant if already set in DefaultRequestHeaders but ensures correctness.
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

                    // Send the POST request asynchronously and await the response
                    var response = await client.PostAsync(url, content);

                    // Read the response content as a string asynchronously
                    var responseString = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON response into a CustomVision object
                    IList<Prediction> predictions = JsonConvert.DeserializeObject<CustomVision>(responseString).Predictions;
                    PredictionsListView.ItemsSource = predictions;
                }
            }
        }
    }
}