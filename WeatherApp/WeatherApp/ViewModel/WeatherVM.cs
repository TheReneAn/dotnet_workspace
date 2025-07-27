// The 'using' statements import necessary namespaces from the .NET framework and the project itself.
using System.Collections.ObjectModel; // Provides ObservableCollection for data binding that updates the UI.
using System.ComponentModel; // Provides INotifyPropertyChanged for notifying the UI of property changes.
using WeatherApp.Model; // Imports the data models (e.g., City, CurrentConditions).
using WeatherApp.ViewModel.Commands; // Imports custom command classes.
using WeatherApp.ViewModel.Helpers; // Imports helper classes, like the one for API calls.

namespace WeatherApp.ViewModel
{
    /// <summary>
    /// ViewModel for the main weather view. It handles the application's logic and data.
    /// Implements INotifyPropertyChanged to notify the UI when data changes.
    /// </summary>
    public class WeatherVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the WeatherVM class.
        /// </summary>
        public WeatherVM()
        {
            // This 'if' block checks if the code is running in a designer (like Visual Studio's XAML designer).
            // This allows us to show sample data in the designer, making UI development easier.
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
            {
                // Create dummy data for the selected city and current conditions for display in the designer.
                SelectedCity = new City { LocalizedName = "Calgary" };
                CurrentConditions = new CurrentConditions
                {
                    WeatherText = "Partly Cloudy",
                    Temperature = new Temperature
                    {
                        Metric = new Units
                        {
                            Value = "21"
                        }
                    }
                };
            }

            // Initialize the command that will be bound to the search button.
            SearchCommand = new SearchCommand(this);
            // Initialize the collection that will hold the list of cities found in a search.
            Cities = new ObservableCollection<City>();
        }

        /// <summary>
        /// Gets or sets the search query string entered by the user.
        /// This property is bound to a TextBox in the UI.
        /// </summary>
        private string query;
        public string Query
        {
            get { return query; }
            set
            {
                query = value;
                // Call OnPropertyChanged to notify the UI that the Query property has changed.
                OnPropertyChanged(nameof(Query));
            }
        }

        /// <summary>
        /// A collection of cities that match the search query.
        /// Using an ObservableCollection automatically updates the UI when cities are added or removed.
        /// This is typically bound to a ListBox or ComboBox.
        /// </summary>
        public ObservableCollection<City> Cities { get; set; }

        /// <summary>
        /// Gets or sets the current weather conditions for the selected city.
        /// This property is bound to various UI elements to display the weather details.
        /// </summary>
        private CurrentConditions currentConditions;
        public CurrentConditions CurrentConditions
        {
            get { return currentConditions; }
            set
            {
                currentConditions = value;
                OnPropertyChanged(nameof(CurrentConditions));
            }
        }

        /// <summary>
        /// Gets or sets the city that the user has selected from the search results list.
        /// The 'set' block contains important application logic: whenever a new city is selected,
        /// it automatically triggers the GetCurrentConditions method to fetch and display the weather for that city.
        /// </summary>
        private City selectedCity;
        public City SelectedCity
        {
            get { return selectedCity; }
            set
            {
                selectedCity = value;
                OnPropertyChanged(nameof(SelectedCity));

                // If a city is actually selected (not null), proceed to get its weather.
                if (selectedCity != null)
                {
                    // This method call is a "side-effect" of setting the SelectedCity property.
                    // It kicks off the process of fetching the weather for the newly selected city.
                    GetCurrentConditions();
                }
            }
        }

        /// <summary>
        /// Asynchronously fetches the current weather conditions for the SelectedCity.
        /// This method is marked 'async Task' because it performs a network operation (API call)
        /// that should not block the UI thread.
        /// </summary>
        private async Task GetCurrentConditions()
        {
            // Clear the search box text to provide a clean user experience after a selection.
            Query = string.Empty;

            // Hide the list of city search results, as a selection has been made.
            Cities.Clear();

            // The 'await' keyword pauses the execution of this method until the API call in
            // AccuWeatherHelper.GetCurrentConditions is complete, without freezing the UI.
            // It fetches the weather using the unique 'Key' of the selected city.
            CurrentConditions = await AccuWeatherHelper.GetCurrentConditions(SelectedCity.Key);
        }

        /// <summary>
        /// The command that executes the city search.
        /// This is bound to a Button in the UI.
        /// </summary>
        public SearchCommand SearchCommand { get; set; }

        /// <summary>
        /// Asynchronously queries the AccuWeather helper to get a list of cities based on the Query property.
        /// 'async void' is used here because this method is typically called by a command or event handler and is not awaited.
        /// </summary>
        public async void MakeQuery()
        {
            // Call the helper method to fetch city data from an API.
            var cities = await AccuWeatherHelper.GetCities(Query);

            // Clear the previous search results.
            Cities.Clear();

            // Check if the result is not null before iterating.
            if (cities != null)
            {
                // Add each city from the results to the ObservableCollection.
                // The UI will update automatically for each city added.
                foreach (var city in cities)
                {
                    Cities.Add(city);
                }
            }
        }

        /// <summary>
        /// Event that fires when a property value changes. Required by the INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Helper method to raise the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            // Invoke the event, which signals the UI to update its bindings for the given property.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}