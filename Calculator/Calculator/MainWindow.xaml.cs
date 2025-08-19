using System.Windows;
using System.Windows.Controls;

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Represents the mathematical operator selected for a calculation.
        /// </summary>
        public enum SelectedOperator
        {
            Adddition,
            Subtracttion,
            Multiplication,
            Division
        }
        private SelectedOperator _selectedOperator;

        /// <summary>
        /// Provides basic mathematical operations, including addition, subtraction, multiplication, and division.
        /// </summary>
        public class SimpleMath
        {
            public static double Add(double a, double b) => a + b;
            public static double Subtract(double a, double b) => a - b;
            public static double Multiply(double a, double b) => a * b;
            public static double Divide(double a, double b)
            {
                if (b == 0)
                {
                    // Handle division by zero
                    MessageBox.Show("Division by 0 is not supported.", "Wrong Operation", MessageBoxButton.OK, MessageBoxImage.Error);
                    return double.NaN; // Return NaN to indicate an error
                }

                return a / b;
            }
        }

        private double _lastNumber;
        private double _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <remarks>This constructor sets up the main window and initializes its components. It also
        /// attaches event handlers to various button controls to handle user interactions.</remarks>
        public MainWindow()
        {
            InitializeComponent();

            // Button event handlers
            AcButton.Click += AcButton_Click;
            NegativeButton.Click += NegativeButton_Click;
            PercentageButton.Click += PercentageButton_Click;
            EqualButton.Click += EqualButton_Click;
        }

        /// <summary>
        /// Handles the click event for the AC button, resetting the displayed result to zero.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void AcButton_Click(object sender, RoutedEventArgs e)
        {
            ResultLabel.Content = "0";
            _lastNumber = 0;
            _result = 0;
        }

        /// <summary>
        /// Handles the click event for the "Equal" button, performing the selected mathematical operation and updating
        /// the result display.
        /// </summary>
        /// label is updated to display "Error".</remarks>
        /// <param name="sender">The source of the event, typically the "Equal" button.</param>
        /// <param name="e">The event data associated with the button click.</param>
        private void EqualButton_Click(object sender, RoutedEventArgs e)
        {
            // Try to parse the Content as an double
            if (!double.TryParse(ResultLabel.Content.ToString(), out var newNumber))
            {
                return;
            }

            switch (_selectedOperator)
            {
                case SelectedOperator.Adddition:
                    _result = SimpleMath.Add(_lastNumber, newNumber);
                    break;
                case SelectedOperator.Subtracttion:
                    _result = SimpleMath.Subtract(_lastNumber, newNumber);
                    break;
                case SelectedOperator.Multiplication:
                    _result = SimpleMath.Multiply(_lastNumber, newNumber);
                    break;
                case SelectedOperator.Division:
                    _result = SimpleMath.Divide(_lastNumber, newNumber);
                    break;
            }

            // Update the result label with the calculated result
            if (_result == double.NaN)
            {
                ResultLabel.Content = "Error"; // Handle division by zero
            }
            else
            {
                ResultLabel.Content = _result.ToString();
            }
        }

        /// <summary>
        /// Handles the click event for the negative button, converting the displayed number to its negative value.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultLabel.Content.ToString(), out _lastNumber))
            {
                _result = -_lastNumber;
                ResultLabel.Content = _result.ToString();
            }
            else
            {
                ResultLabel.Content = "Error";
            }
        }

        /// <summary>
        /// Handles the click event for the percentage button.
        /// </summary>
        /// <param name="sender">The source of the event, typically the percentage button.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(ResultLabel.Content.ToString(), out var tempNumber))
            {
                // Calculate the percentage of the last number
                tempNumber /= 100;

                // If there was a previous number, multiply the percentage by it
                if (_lastNumber != 0)
                {
                    // If there was a previous number, multiply the percentage by it
                    tempNumber *= _lastNumber;
                }
                ResultLabel.Content = tempNumber.ToString();
            }
            else
            {
                ResultLabel.Content = "Error"; // If parsing fails, show an error
            }
        }

        /// <summary>
        /// Handles the click event for the button that appends a decimal point to the current value.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button being clicked.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void PointButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the current content of the result label, default to "0" if null
            var current = ResultLabel.Content?.ToString() ?? "0";

            // If the current content already contains a decimal point, do nothing
            if (current.Contains('.'))
            {
                return;
            }

            // If the current content is empty or "0", start with "0."
            if (string.IsNullOrEmpty(current) || current == "0")
            {
                ResultLabel.Content = "0.";
            }
            else
            {
                // Otherwise, append a decimal point to the current content
                ResultLabel.Content = current + ".";
            }
        }

        /// <summary>
        /// Handles the click event for operator buttons in a calculator interface. (e.g., +, -, *, /)
        /// </summary>
        /// <param name="sender">The button that triggered the event. Must be an operator button.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void OperationButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if sender is a Button and its Content is not null
            if (sender is Button button && button.Content != null)
            {
                // Try to parse the Content as an double
                if (double.TryParse(ResultLabel.Content.ToString(), out _lastNumber))
                {
                    ResultLabel.Content = "0";
                }
                else
                {
                    ResultLabel.Content = "Error";
                }
            }

            // Determine which operator button was clicked and set the selectedOperator accordingly
            if (Equals(sender, MultiplicationButton))
            {
                _selectedOperator = SelectedOperator.Multiplication;
            }
            if (Equals(sender, DivisionButton))
            {
                _selectedOperator = SelectedOperator.Division;
            }
            if (Equals(sender, PlusButton))
            {
                _selectedOperator = SelectedOperator.Adddition;
            }
            if (Equals(sender, MinusButton))
            {
                _selectedOperator = SelectedOperator.Subtracttion;
            }
        }

        /// <summary>
        /// Handles the click event for numeric buttons, updating the displayed result based on the button's content. (0-9)
        /// </summary>
        /// <param name="sender">The button that triggered the event. Must have numeric content to be processed.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if sender is a Button and its Content is not null
            if (sender is not Button button || button.Content == null)
            {
                return;
            }

            // Try to parse the Content as an integer
            if (int.TryParse(button.Content.ToString(), out var selectedValue))
            {
                // Get the current value of the result label (default to "0" if null)
                var current = ResultLabel.Content?.ToString() ?? "0";

                // If the current value is "0", replace it with the selected value
                if (current == "0")
                {
                    ResultLabel.Content = selectedValue.ToString();
                }
                else
                {
                    // Otherwise, append the selected value to the current content
                    ResultLabel.Content = current + selectedValue.ToString();
                }
            }
            else
            {
                // If the button Content is not a number, show a message box
                MessageBox.Show("Button content is not a number.");
            }
        }
    }
}