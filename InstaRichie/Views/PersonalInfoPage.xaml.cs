using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            Results();
        }

        // Results 
        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();

            /// Refresh Data
            var query = conn.Table<PersonalInfo>();
            PersonalInfoListView.ItemsSource = query.ToList();
        }

        // Add Personal Info
        private async void AddInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (FirstNameText.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("No Account Name entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = FirstNameText.Text,
                        LastName = LastNameText.Text,
                        DOB = DOBText.Date.ToString("MM-dd-yyyy"),
                        Gender = GenderText.SelectedItem.ToString(),
                        EmailAddress = EmailText.Text,
                        MobilePhone = Convert.ToInt32(MobileText.Text)
                    });
                    Results();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A similar Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
            }
        }

        //Edit the Personal Information
        private async void EditInfo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                int AccSelection = ((PersonalInfo)PersonalInfoListView.SelectedItem).ID;
                if (AccSelection == null)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                   /* conn.Update(new PersonalInfo()
                    {
                        FirstName = FirstNameText.Text,
                        LastName = LastNameText.Text,
                        DOB = DOBText.Date.ToString("MM-dd-yyyy"),
                        Gender = GenderText.SelectedItem.ToString(),
                        EmailAddress = EmailText.Text,
                        MobilePhone = Convert.ToInt32(MobileText.Text)
                    });
                    Results();*/

                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("UPDATE PersonalInfo SET FirstName ='" + AccSelection + "'");
                    PersonalInfoListView.ItemsSource = query1.ToList();
                }
            }
           catch (Exception ex)
           {
                if (ex is NullReferenceException)
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Value or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("A similar Name already exists, Try a different name", "Oops..!");
                    await dialog.ShowAsync();
                }
                
            }
            
        }

        // Resets everything to default 
        private async void ClearFields_Click(object sender, RoutedEventArgs e)
        {
            FirstNameText.Text = string.Empty;
            LastNameText.Text = string.Empty;
            DOBText.SelectedDate = null;
            GenderText.Text = string.Empty;
            EmailText.Text = string.Empty;
            MobileText.Text = string.Empty;

            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        // Deletes 
        private async void DeleteInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string AccSelection = ((PersonalInfo)PersonalInfoListView.SelectedItem).FirstName;
                if (AccSelection == "")
                {
                    MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE FirstName ='" + AccSelection + "'");
                    PersonalInfoListView.ItemsSource = query1.ToList();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("Not selected the Item", "Oops..!");
                await dialog.ShowAsync();
            }
        }


        /* private async void PersonalInfoListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             try
             {
                 string AccSelection = ((PersonalInfo)PersonalInfoListView.SelectedItem).FirstName;

                 conn.CreateTable<PersonalInfo>();
                 var query1 = conn.Table<PersonalInfo>();
                 var query3 = conn.Query<PersonalInfo>("SELECT * FROM PersonalInfo WHERE FirstName ='" + AccSelection + "'");
                 PersonalInfoListView.ItemsSource = query1.ToList();

             }
             catch (NullReferenceException)
             {
                 MessageDialog dialog = new MessageDialog("This means it didn't work", "Oops..!");
                 await dialog.ShowAsync();
             }

         }*/
    }
}
