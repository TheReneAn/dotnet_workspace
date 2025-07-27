// Imports the popular Newtonsoft.Json library, used for parsing JSON data from the API responses.
using Newtonsoft.Json;
// Imports the namespace for HttpClient, which is used to make web requests to the API.
using System.Net.Http;
// Imports the application's data models (City, CurrentConditions) to structure the received data.
using WeatherApp.Model;

namespace WeatherApp.ViewModel.Helpers
{
    /// <summary>
    /// A helper class that handles all interactions with the AccuWeather API.
    /// It encapsulates the logic for building request URLs, making HTTP calls, and deserializing JSON responses.
    /// All methods are static, so an instance of the class is not needed.
    /// </summary>
    public class AccuWeatherHelper
    {
        // Defines the base URL for all AccuWeather API endpoints.
        public const string BASE_URL = "http://dataservice.accuweather.com/";

        // Defines the URL template for the city autocomplete search endpoint.
        // {0} is a placeholder for the API key, and {1} is for the user's search query.
        public const string AUTOCOMPLETE_ENDPOINT = "locations/v1/cities/autocomplete?apikey={0}&q={1}";

        // Defines the URL template for the current weather conditions endpoint.
        // {0} is a placeholder for the city's unique location key, and {1} is for the API key.
        public const string CURRENT_CONDITIONS_ENDPOINT = "currentconditions/v1/{0}?apikey={1}";

        // Your private AccuWeather API Key.
        // WARNING: Hardcoding API keys directly in source code is a significant security risk.
        // In a production application, this should be stored securely using a configuration file,
        // environment variables, or a secrets management service.
        public const string API_KEY = "BgIEQ61fGKFBIFjwwYUcv0x4fSAyvOZK";

        /// <summary>
        /// Asynchronously fetches a list of cities that match a given search query.
        /// </summary>
        /// <param name="query">The search term entered by the user (e.g., "Calgary").</param>
        /// <returns>A Task that resolves to a List of City objects.</returns>
        public static async Task<List<City>> GetCities(string query)
        {
            List<City> cities = new List<City>();

            // Construct the full request URL by formatting the endpoint string with the API key and query.
            string url = BASE_URL + string.Format(AUTOCOMPLETE_ENDPOINT, API_KEY, query);

            // Best Practice Note: Creating a new HttpClient for each request is inefficient and can
            // lead to socket exhaustion. A better approach is to use a single, static HttpClient instance
            // or IHttpClientFactory for the application's lifetime.
            using (HttpClient client = new HttpClient())
            {
                // Send an asynchronous GET request to the AccuWeather API.
                var response = await client.GetAsync(url);
                // Read the response content as a JSON string.
                string json = await response.Content.ReadAsStringAsync();

                // Deserialize the JSON string into a list of City objects using Newtonsoft.Json.
                cities = JsonConvert.DeserializeObject<List<City>>(json);
            }

            return cities;
        }

        /// <summary>
        /// Asynchronously fetches the current weather conditions for a specific city.
        /// </summary>
        /// <param name="cityKey">The unique location key for the desired city, obtained from a GetCities query.</param>
        /// <returns>A Task that resolves to a CurrentConditions object.</returns>
        public static async Task<CurrentConditions> GetCurrentConditions(string cityKey)
        {
            CurrentConditions currentConditions = new CurrentConditions();

            // Construct the full request URL for the current conditions endpoint.
            string url = BASE_URL + string.Format(CURRENT_CONDITIONS_ENDPOINT, cityKey, API_KEY);

            using (HttpClient client = new HttpClient())
            {
                // Send the GET request and await the response.
                var response = await client.GetAsync(url);
                // Read the response content as a JSON string.
                string json = await response.Content.ReadAsStringAsync();

                // This specific API endpoint returns the result inside a JSON array, even if there's only one item.
                // Therefore, we first deserialize the JSON into a List of CurrentConditions objects...
                // ...and then take the first element from the list. .FirstOrDefault() safely handles cases
                // where the list might be empty, returning null instead of throwing an exception.
                currentConditions = JsonConvert.DeserializeObject<List<CurrentConditions>>(json).FirstOrDefault();
            }

            return currentConditions;
        }
    }
}