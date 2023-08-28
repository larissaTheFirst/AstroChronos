using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Xml.Linq;

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for LocationSelector.xaml
    /// </summary>
    public partial class LocationSelector : Window
    {
        public LocationSelector() {
            InitializeComponent();
            List<string> allLocations = new List<string>();

            void GenerateLocations() {
                using (FileStream stream = File.OpenRead(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "coord_data.csv")))
                using (StreamReader reader = new StreamReader(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "coord_data.csv"), System.Text.Encoding.Default)) {
                    while (!reader.EndOfStream) {
                        string line = reader.ReadLine().TrimEnd(',');
                        string[] values = line.Split(',');
                        allLocations.Add(string.Join(", ", values));
                    }
                    reader.Close();
                }

            }

            GenerateLocations();
            locationsMenu.ItemsSource = allLocations;

        }

        private void ButtonClick(object sender, RoutedEventArgs e) {

            XDocument coord_data = XDocument.Load(Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "Values.xml"));
            string location = locationsMenu.Text;
            string latitude;
            string longitude;
            var shortLocation = location.TrimEnd(',');
            string[] fullArr = location.Split(",");

            if (fullArr.Length == 4) {
                shortLocation = fullArr[0] + ", " + fullArr[3];
            }

            else {
                shortLocation = fullArr[0] + ", " + fullArr[fullArr.Length - 1] + ", " + fullArr[fullArr.Length - 2];
            }
            //Debug.WriteLine("Current location is: "+shortLocation);

            latitude = fullArr[1];
            longitude = fullArr[2];

            coord_data.Root.Element("Latitude").Value = latitude;
            coord_data.Root.Element("Longitude").Value = longitude;
            coord_data.Root.Element("FirstRun").Value = "False";
            coord_data.Root.Element("ShortLocation").Value = shortLocation;
            coord_data.Root.Element("FullLocation").Value = location;

            try {
                coord_data.Save(Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData), "Values.xml"));
                System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
                Application.Current.Shutdown();
            }

            catch (UnauthorizedAccessException) {
                MessageBox.Show("Access denied", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            //Debug.WriteLine("Latitude selected is: " + coord_data.Root.Element("Latitude").Value);
            //Debug.WriteLine("Longitude selected is: " + coord_data.Root.Element("Longitude").Value);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Application.Current.Shutdown();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e) {
            
            if (AboutLocation.isOpen == 0) {
                AboutLocation aboutLocation = new AboutLocation();
                AboutLocation.isOpen = 1;
                aboutLocation.Show();
            }
        }
    }

}
