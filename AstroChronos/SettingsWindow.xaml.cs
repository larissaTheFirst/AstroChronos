using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public static byte checkIfOpen = 0;
        List<String> timeFormat = new List<string>();
        List<string> allLocations = new List<string>();
        static XDocument value_data = XDocument.Load(System.IO.Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "Values.xml"));
        string locationValue = value_data.Root.Element("FullLocation").Value;
        string timeFormatNameValue = value_data.Root.Element("TimeFormatName").Value;

        public SettingsWindow() {

            InitializeComponent();

            SettingsHandler();
            selectLocation.ItemsSource = allLocations;
            selectLocation.SelectedIndex = allLocations.IndexOf(locationValue.ToString());
            selectTimeFormat.ItemsSource = timeFormat;
            selectTimeFormat.SelectedIndex = timeFormat.IndexOf(timeFormatNameValue);
        }

        public void SettingsHandler() {
            timeFormat.Add("24 hour");
            timeFormat.Add("12 hour");


                using (FileStream stream = File.OpenRead(System.IO.Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "coord_data.csv")))
                using (StreamReader reader = new StreamReader(System.IO.Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "coord_data.csv"))) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine();
                        string[] values = line.Split(',');

                        allLocations.Add(string.Join(", ", values));
                    }
                    reader.Close();
                }






        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            checkIfOpen = 0;
            Owner.Activate();
            GC.Collect();
        }

        private void ButtonClick(object sender, RoutedEventArgs e) {
            if(selectTimeFormat.SelectedValue.ToString()=="24 hour") {
                value_data.Root.Element("TimeFormat").Value = "HH:mm";
                value_data.Root.Element("TimeFormatName").Value =selectTimeFormat.SelectedValue.ToString();
                value_data.Save(System.IO.Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "Values.xml"));
                Debug.WriteLine("Selected value is: " + selectTimeFormat.SelectedValue);
            }
            else{
                value_data.Root.Element("TimeFormat").Value = "h:mm tt";
                value_data.Root.Element("TimeFormatName").Value = "12 hour";
                value_data.Save(System.IO.Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "Values.xml"));
                Debug.WriteLine("Selected value is: "+selectTimeFormat.SelectedValue);
            }
            string[] fullLocation = selectLocation.SelectedValue.ToString().Split(',');
            value_data.Root.Element("Latitude").Value = fullLocation[1];
            value_data.Root.Element("Longitude").Value = fullLocation[2];
            value_data.Root.Element("FullLocation").Value = selectLocation.SelectedValue.ToString();
            value_data.Save(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Values.xml"));

            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Application.Current.Shutdown();
        }

    }
}
