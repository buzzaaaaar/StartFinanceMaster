using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SQLite.Net;
using StartFinance.Models;
using Windows.UI.Popups;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetails : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetails()
        {

            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table
            Results();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Contacts>();
            var query = conn.Table<Contacts>();
            ContactListView.ItemsSource = query.ToList();
        }


        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // checks if account name is null
                if (FirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (CompanyName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (MobilePhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new Contacts()
                    {
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        CompanyName = CompanyName.Text,
                        MobilePhones = MobilePhone.Text,

                    });
                    Results();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Account Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Account will delete all transactions of this account", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {
                    int ContactID = ((Contacts)ContactListView.SelectedItem).ContactID;
                    var qu = conn.Query<Contacts>("DELETE FROM Contacts WHERE ContactID='" + ContactID + "'");
                    Results();

                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private async void EditItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure you want to update the contact detail", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Edit")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;

            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                FirstName.Text = ((Contacts)ContactListView.SelectedItem).FirstName;
                LastName.Text = ((Contacts)ContactListView.SelectedItem).LastName;
                CompanyName.Text = ((Contacts)ContactListView.SelectedItem).CompanyName;
                MobilePhone.Text = ((Contacts)ContactListView.SelectedItem).MobilePhones;
                AddButton.IsEnabled = false;
                saveButton.IsEnabled = true;
                DeleteButton.IsEnabled = false;
                EditButton.IsEnabled = false;
                cancelButton.IsEnabled = true;
                AddButton.Visibility = Visibility.Collapsed;
                saveButton.Visibility = Visibility.Visible;
                cancelButton.Visibility = Visibility.Visible;
                DeleteButton.Visibility = Visibility.Collapsed;
                EditButton.Visibility = Visibility.Collapsed;
            }

            else
            {
                //
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // checks if account name is null
                if (FirstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (LastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (CompanyName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Company Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (MobilePhone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Mobile phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    String FirstName1 = FirstName.Text;
                    String LastName1 = LastName.Text;
                    String CompanyName1 = CompanyName.Text;
                    String MobilePhones1 = MobilePhone.Text;
                    int ContactID = ((Contacts)ContactListView.SelectedItem).ContactID;
                    var querydel = conn.Query<Contacts>("UPDATE Contacts SET FirstName ='" + FirstName1 + "', LastName ='" + LastName1 + "', CompanyName ='" + CompanyName1 + "', MobilePhones ='" + MobilePhones1 + "'  WHERE ContactID='" + ContactID + "'");
                    MessageDialog dialog = new MessageDialog("Succesfully Edited");
                    await dialog.ShowAsync();
                    FirstName.Text = "";
                    LastName.Text = "";
                    CompanyName.Text = "";
                    MobilePhone.Text = "";
                    AddButton.IsEnabled = true;
                    saveButton.IsEnabled = false;
                    DeleteButton.IsEnabled = true;
                    EditButton.IsEnabled = true;
                    cancelButton.IsEnabled = true;
                    AddButton.Visibility = Visibility.Visible;
                    saveButton.Visibility = Visibility.Collapsed;
                    cancelButton.Visibility = Visibility.Collapsed;
                    DeleteButton.Visibility = Visibility.Visible;
                    EditButton.Visibility = Visibility.Visible;
                    Results();
                }
            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Amount or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated

                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog(ex.ToString());
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FirstName.Text = "";
            LastName.Text = "";
            CompanyName.Text = "";
            MobilePhone.Text = "";
            AddButton.IsEnabled = true;
            saveButton.IsEnabled = false;
            DeleteButton.IsEnabled = true;
            EditButton.IsEnabled = true;
            AddButton.Visibility = Visibility.Visible;
            saveButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
            DeleteButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Visible;
            Results();

        }
    }
}
