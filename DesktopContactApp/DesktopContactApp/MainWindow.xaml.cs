using DesktopContactApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SQLite;

namespace DesktopContactApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Field to store the list of contacts retrieved from the database
        private List<Contact> contacts;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the contacts list
            contacts = new List<Contact>();

            // Read contacts from the database upon application startup
            ReadDatabase();
        }

        // Event handler for the 'New Contact' button click
        private void NewContactButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the new contact entry window and wait for it to close
            NewContactWindow newContactWindow = new NewContactWindow();
            newContactWindow.ShowDialog();

            // Read the database again to update the UI with the new contact
            ReadDatabase();
        }

        /// <summary>
        /// Method to read contacts from the database
        /// </summary>
        private void ReadDatabase()
        {
            // Establish a database connection using 'using' to ensure it is closed automatically.
            using (SQLiteConnection connection = new SQLiteConnection(App.dataBasePath))
            {
                // Create the Contact table if it doesn't already exist.
                connection.CreateTable<Contact>();

                // Retrieve all contacts from the database, convert to a list, and order by Name.
                contacts = connection.Table<Contact>().ToList().OrderBy(c => c.Name).ToList();
            }

            // Bind the data to the UI's ListView
            if (contacts != null && contacts.Count > 0)
            {
                ContactsListView.ItemsSource = contacts;
            }
            else
            {
                // If the list is empty, clear the ListView.
                ContactsListView.ItemsSource = null;
            }
        }

        /// <summary>
        /// Event handler for the search TextBox content changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Cast the sender object to a TextBox to access its Text property
            TextBox searchTextBox = sender as TextBox;

            // Filter the existing 'contacts' list based on the search text (case-insensitive)
            var filterList = contacts.Where(contact => contact.Name.ToLower().Contains(searchTextBox.Text.ToLower())).ToList();

            // Display the filtered list in the ListView
            ContactsListView.ItemsSource = filterList;
        }

        /// <summary>
        /// Handles the selection change event for the ContactsListView.
        /// </summary>
        /// <param name="sender">The source of the event, typically the ContactsListView control.</param>
        /// <param name="e">The event data containing information about the selection change.</param>
        private void ContactsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Cast the selected item to a Contact object
            Contact selectedContact = (Contact)ContactsListView.SelectedItem;

            // Check if an item is selected in the ListView
            if (selectedContact != null)
            {
                // Open the NewContactWindow with the selected contact for editing
                ContactDetailsWindow contractDetailWindow = new ContactDetailsWindow(selectedContact);
                contractDetailWindow.ShowDialog();

                // Refresh the contacts list after editing
                ReadDatabase();
            }
        }
    }
}