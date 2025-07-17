using DesktopContactApp.Classes;
using SQLite;
using System.Windows;

namespace DesktopContactApp
{
    /// <summary>
    /// Interaction logic for ContactDetailsWindow.xaml
    /// </summary>
    public partial class ContactDetailsWindow : Window
    {
        Contact contact;

        public ContactDetailsWindow(Contact contact)
        {
            InitializeComponent();

            // Location
            Owner = Application.Current.MainWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            this.contact = contact;
            NameTextBox.Text = contact.Name;
            EmailTextBox.Text = contact.Email;
            PhoneTextBox.Text = contact.Phone;
        }

        /// <summary>
        /// Handles the click event of the Update button, updating the contact information and saving changes to the
        /// database.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Update button.</param>
        /// <param name="e">The event data associated with the button click.</param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Update the contact object with the new values from the text boxes
            contact.Name = NameTextBox.Text;
            contact.Email = EmailTextBox.Text;
            contact.Phone = PhoneTextBox.Text;

            // Update the contact in the database
            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
            {
                connection.CreateTable<Contact>();
                connection.Update(contact);
            }

            this.Close();
        }

        /// <summary>
        /// Handles the click event of the Delete button to delete a contact from the database.
        /// </summary>
        /// <param name="sender">The source of the event, typically the Delete button.</param>
        /// <param name="e">The event data associated with the click action.</param>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Delete the contact from the database
            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
            {
                connection.CreateTable<Contact>();
                connection.Delete(contact);
            }

            this.Close();
        }
    }
}
