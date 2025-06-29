using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        SelectedOperator selectedOperator;

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
            acButton.Click += AcButton_Click;
            negativeButton.Click += NegativeButton_Click;
            percentageButton.Click += PercentageButton_Click;
            equalButton.Click += EqualButton_Click;
        }

        /// <summary>
        /// Handles the click event for the AC button, resetting the displayed result to zero.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void AcButton_Click(object sender, RoutedEventArgs e)
        {
            resultLabel.Content = "0";
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
            if (double.TryParse(resultLabel.Content.ToString(), out double newNumber))
            {
                switch (selectedOperator)
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
                    resultLabel.Content = "Error"; // Handle division by zero
                }
                else
                {
                    resultLabel.Content = _result.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the click event for the negative button, converting the displayed number to its negative value.
        /// </summary>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(resultLabel.Content.ToString(), out _lastNumber))
            {
                _result = -_lastNumber;
                resultLabel.Content = _result.ToString();
            }
            else
            {
                resultLabel.Content = "Error";
            }
        }

        /// <summary>
        /// Handles the click event for the percentage button.
        /// </summary>
        /// <param name="sender">The source of the event, typically the percentage button.</param>
        /// <param name="e">The event data associated with the click event.</param>
        private void PercentageButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(resultLabel.Content.ToString(), out double tempNumber))
            {
                // Calculate the percentage of the last number
                tempNumber /= 100;

                // If there was a previous number, multiply the percentage by it
                if (_lastNumber != 0)
                {
                    // If there was a previous number, multiply the percentage by it
                    tempNumber *= _lastNumber;
                }
                resultLabel.Content = tempNumber.ToString();
            }
            else
            {
                resultLabel.Content = "Error"; // If parsing fails, show an error
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
            string current = resultLabel.Content?.ToString() ?? "0";

            // If the current content already contains a decimal point, do nothing
            if (current.Contains('.'))
            {
                return;
            }

            // If the current content is empty or "0", start with "0."
            if (string.IsNullOrEmpty(current) || current == "0")
            {
                resultLabel.Content = "0.";
            }
            else
            {
                // Otherwise, append a decimal point to the current content
                resultLabel.Content = current + ".";
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
                if (double.TryParse(resultLabel.Content.ToString(), out _lastNumber))
                {
                    resultLabel.Content = "0";
                }
                else
                {
                    resultLabel.Content = "Error";
                }
            }

            // Determine which operator button was clicked and set the selectedOperator accordingly
            if (sender == multiplicationButton)
            {
                selectedOperator = SelectedOperator.Multiplication;
            }
            if (sender == divisionButton)
            {
                selectedOperator = SelectedOperator.Division;
            }
            if (sender == plusButton)
            {
                selectedOperator = SelectedOperator.Adddition;
            }
            if (sender == minusButton)
            {
                selectedOperator = SelectedOperator.Subtracttion;
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
            if (sender is Button button && button.Content != null)
            {
                // Try to parse the Content as an integer
                if (int.TryParse(button.Content.ToString(), out int selectedValue))
                {
                    // Get the current value of the result label (default to "0" if null)
                    string current = resultLabel.Content?.ToString() ?? "0";

                    // If the current value is "0", replace it with the selected value
                    if (current == "0")
                    {
                        resultLabel.Content = selectedValue.ToString();
                    }
                    else
                    {
                        // Otherwise, append the selected value to the current content
                        resultLabel.Content = current + selectedValue.ToString();
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
}