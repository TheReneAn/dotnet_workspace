// Imports the namespace containing interfaces and classes for handling command-based interactions in WPF,
using System.Windows.Input;

namespace WeatherApp.ViewModel.Commands
{
    /// <summary>
    /// Represents a command that executes the city search logic in the WeatherVM.
    /// This class implements the ICommand interface, which is a core part of the MVVM pattern.
    /// It decouples the View (e.g., a Button) from the ViewModel's logic (the action to be performed).
    /// </summary>
    public class SearchCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the ViewModel associated with this command.
        /// The command needs a reference to the ViewModel to call its methods (like MakeQuery)
        /// and access its properties.
        /// </summary>
        public WeatherVM VM { get; set; }

        /// <summary>
        /// An event that is raised when changes occur that affect whether the command should execute.
        /// This implementation uses the CommandManager.RequerySuggested event. This is a simple way
        /// to have the WPF framework automatically re-evaluate the CanExecute method whenever it
        //  detects a change in the UI that might affect the command's state (like a change in focus).
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Initializes a new instance of the SearchCommand class.
        /// </summary>
        /// <param name="vm">The ViewModel that this command will operate on.</param>
        public SearchCommand(WeatherVM vm)
        {
            // Store the provided ViewModel instance so it can be used in the Execute method.
            VM = vm;
        }

        /// <summary>
        /// Determines whether the command can be executed in its current state.
        /// The return value of this method is used by WPF to automatically enable or disable
        /// the UI control (e.g., a Button) that this command is bound to.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. In this case, it's expected to be the search query string from the TextBox.
        /// </param>
        /// <returns>true if the command can be executed; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            // Cast the command parameter to a string.
            string? query = parameter as string;

            // The command should only be executable if the search query is not null, empty, or just whitespace.
            // This prevents the user from initiating a search with no input.
            if (string.IsNullOrWhiteSpace(query))
            {
                return false;
            }

            // If there is text in the query, the command can be executed.
            return true;
        }

        /// <summary>
        /// The method that is called when the command is invoked (e.g., when the user clicks the search button).
        /// This contains the actual logic that the command performs.
        /// </summary>
        /// <param name="parameter">Data used by the command. This parameter is not used here but is required by the interface.</param>
        public void Execute(object? parameter)
        {
            // Call the MakeQuery method on the ViewModel to perform the city search.
            VM.MakeQuery();
        }
    }
}