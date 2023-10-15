using SQLite.Net;
using StartFinance.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        int globalID;
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            // Creating table
            Reload();
        }

        //METHOD, load data from the database in to the appointment list. 
        public void Reload()
        {
            // Creating table
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            AppointmentList.ItemsSource = query.ToList();
        }


        //BUTTON, Delete appointment click
        private async void DeleteAppointmentClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectionForDeletion = ((Appointments)AppointmentList.SelectedItem).EventName;
                //Replace any ' with '' for the escape character that SQL needs. 
                selectionForDeletion = selectionForDeletion.Replace("'", "''");
                //IF nothing is selected for deletion, show error. 
                if (selectionForDeletion == "")
                {
                    MessageDialog dialog = new MessageDialog("Please select an event for deletion");
                    await dialog.ShowAsync();
                }
                //ELSE, delete the selected item. 
                else
                {
                    conn.CreateTable<Appointments>();
                    var queryRead = conn.Table<Appointments>();
                    var queryDelete = conn.Query<Appointments>("DELETE FROM Appointments WHERE EventName = '" + selectionForDeletion + "'");
                    AppointmentList.ItemsSource = queryRead.ToList();
                }

            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("No appointment selected for deletion. \nPlease select an appointment before pressing delete. ");
                await dialog.ShowAsync();
            }
        }
        //EVENT on listbox
        private void Appointments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var appointment = AppointmentList.SelectedItem as Appointments;

            if (appointment != null)
            {
                //Frame.Navigate(typeof(views.DetailsPage), product.productId);
                globalID = appointment.ID;
                var queryRead = conn.Table<Appointments>();
                string eventName = appointment.EventName;
                string location = appointment.Location;
                string startDateTemp = appointment.StartDate;
                string startTimeTemp = appointment.StartTime;
                string endTimeTemp = appointment.EndTime;

                DateTime startDate = Convert.ToDateTime(startDateTemp);
                TimeSpan startTime = TimeSpan.Parse(startTimeTemp);
                TimeSpan endTime = TimeSpan.Parse(endTimeTemp);

                EventName.Text = eventName;
                Location.Text = location;
                StartDate.Date = startDate;
                StartTime.Time = startTime;
                EndTime.Time = endTime;
            }
            AppointmentList.SelectedIndex = -1;
        }

        private async void SaveUpdateAppointmentClick(object sender, RoutedEventArgs e)
        {
            var appointment = AppointmentList.SelectedItem as Appointments;

            try
            {
                string eventName = EventName.Text;
                string location = Location.Text;

                string startDay = StartDate.Date.Day.ToString();
                string startMonth = StartDate.Date.Month.ToString();
                string startYear = StartDate.Date.Year.ToString();

                string finalDate = "" + startMonth + "/" + startDay + "/" + startYear;



                string startHour = StartTime.Time.Hours.ToString();
                string startMinute = StartTime.Time.Minutes.ToString();

                string finalStartTime = "" + startHour + ":" + startMinute;


                string endHour = EndTime.Time.Hours.ToString();
                string endMinute = EndTime.Time.Minutes.ToString();
                string finalEndTime = "" + endHour + ":" + endMinute;


                //This varible is the number of hours the appointment will run for. 
                int durationHours = StartTime.Time.Hours - EndTime.Time.Hours;
                //This variable is the number of minutes the appointment will run for. 
                int durationMinutes = StartTime.Time.Minutes - EndTime.Time.Minutes;

                //IF appointment time is 0 hours and 0 minutes or less, show an error.  
                if (durationHours >= 0 && durationMinutes >= 0)
                {
                    MessageDialog messageDialog1 = new MessageDialog("Start Time can't be less than End Time");
                    await messageDialog1.ShowAsync();

                    //IF there is no event name, show an error. 
                    if (eventName == "")
                    {
                        MessageDialog messageDialog2 = new MessageDialog("Please enter a unique Event Name. ");
                        await messageDialog2.ShowAsync();
                        EventName.Focus(FocusState.Programmatic);

                        //IF there is no location, show an error. 
                        if (eventName == "")
                        {
                            MessageDialog messageDialog3 = new MessageDialog("Please enter a Location for the appointment. ");
                            await messageDialog3.ShowAsync();
                            Location.Focus(FocusState.Programmatic);
                            return;
                        }
                    }
                }

                // ELSE insert data
                else
                {
                    var updateStatment = conn.Query<Task>("UPDATE Appointments SET EventName = '" + eventName + "' , Location = '" + location + "' , StartDate = '" + finalDate + "' , StartTime = '" + finalStartTime + "' ,  EndTime = '" + finalEndTime + "' WHERE ID = " + globalID);
                    MessageDialog AddedConfirmed = new MessageDialog("Appointment changed. ");
                    await AddedConfirmed.ShowAsync();
                    //Reload the data into the list view. 
                    Reload();
                    ClearForm();

                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Please check all fields have data, then press Add.");
                    await dialog.ShowAsync();
                }

            }
        }

        //BUTTON, add an appointment. 
        private async void AddAppointmentClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string eventName = EventName.Text;
                string location = Location.Text;

                string startDay = StartDate.Date.Day.ToString();
                string startMonth = StartDate.Date.Month.ToString();
                string startYear = StartDate.Date.Year.ToString();

                string finalDate = "" + startMonth + "/" + startDay + "/" + startYear;



                string startHour = StartTime.Time.Hours.ToString();
                string startMinute = StartTime.Time.Minutes.ToString();

                string finalStartTime = "" + startHour + ":" + startMinute;


                string endHour = EndTime.Time.Hours.ToString();
                string endMinute = EndTime.Time.Minutes.ToString();
                string finalEndTime = "" + endHour + ":" + endMinute;


                //This varible is the number of hours the appointment will run for. 
                int durationHours = StartTime.Time.Hours - EndTime.Time.Hours;
                //This variable is the number of minutes the appointment will run for. 
                int durationMinutes = StartTime.Time.Minutes - EndTime.Time.Minutes;

                //IF appointment time is 0 hours and 0 minutes or less, show an error.  
                if (durationHours >= 0 && durationMinutes >= 0)
                {
                    MessageDialog messageDialog1 = new MessageDialog("Start Time can't be less than End Time");
                    await messageDialog1.ShowAsync();

                    //IF there is no event name, show an error. 
                    if (eventName == "")
                    {
                        MessageDialog messageDialog2 = new MessageDialog("Please enter a unique Event Name. ");
                        await messageDialog2.ShowAsync();
                        EventName.Focus(FocusState.Programmatic);

                        //IF there is no location, show an error. 
                        if (eventName == "")
                        {
                            MessageDialog messageDialog3 = new MessageDialog("Please enter a Location for the appointment. ");
                            await messageDialog3.ShowAsync();
                            Location.Focus(FocusState.Programmatic);
                            return;
                        }
                    }
                }

                // ELSE insert data
                else
                {
                    conn.Insert(new Appointments()
                    {
                        EventName = eventName,
                        Location = location,
                        StartDate = finalDate,
                        StartTime = finalStartTime,
                        EndTime = finalEndTime

                    });
                    MessageDialog AddedConfirmed = new MessageDialog("Appointment added");
                    await AddedConfirmed.ShowAsync();
                    //Reload the data into the list view. 
                    Reload();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("Please check all fields have data, then press Add.");
                    await dialog.ShowAsync();
                }

            }
        }


        //METHOD, reset values to default (or as close to default as possible). 
        private void ClearForm()
        {
            EventName.Text = "";
            Location.Text = "";
            StartDate.Date = DateTime.Now.Date;
            StartTime.Time = DateTime.Now.TimeOfDay;
            EndTime.Time = DateTime.Now.TimeOfDay;
        }


    }
}
