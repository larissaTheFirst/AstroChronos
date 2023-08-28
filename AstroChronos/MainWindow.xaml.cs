using CoordinateSharp;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //get coordinates and time format
        static XDocument coords = XDocument.Load(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Values.xml"));
        public static string timeFormat = coords.Root.Element("TimeFormat").Value;
        static string get_lat = coords.Root.Element("Latitude").Value;
        static string get_longi = coords.Root.Element("Longitude").Value;
        string firstRun = coords.Root.Element("FirstRun").Value;
        public static double lat = double.Parse(get_lat, System.Globalization.CultureInfo.InvariantCulture);
        public static double longi = double.Parse(get_longi, System.Globalization.CultureInfo.InvariantCulture);
        DateTime date = DateTime.Now;
        List<double> times = new(); //holds the values of planetary hours, starting from sunrise=0, incremented by duration of planetary hour each
        char[] glyphs = new char[] { '☉', '☾', '♂', '☿', '♃', '♀', '♄' };
        byte[] days = new byte[] { 0, 3, 6, 2, 5, 1, 4 }; //value of index at this array will be used as starting index for hoursChaldean array. 
        string[] hoursChaldean = new string[] { "the Sun", "Venus", "Mercury", "the Moon", "Saturn", "Jupiter", "Mars", "the Sun", "Venus", "Mercury", "the Moon", "Saturn", "Jupiter", "Mars", "the Sun", "Venus", "Mercury", "the Moon", "Saturn", "Jupiter", "Mars", "the Sun", "Venus", "Mercury", "the Moon", "Saturn", "Jupiter", "Mars", "the Sun", "Venus", "Mercury", "the Moon" };

        public MainWindow() {

            InitializeComponent();

            //if app is running for the first time, makes sure location selector is show before everything else
            if (firstRun == "True") {

                LocationSelector locationSelector = new LocationSelector();
                this.Hide();
                locationSelector.Show();
            }

            else {
                SetPlanetaryHour();
            }

            //makes sure information is up to date after system sleep
            void OnPowerChange(object o, PowerModeChangedEventArgs e) {
                if (e.Mode == PowerModes.Resume) {
                    SetPlanetaryHour();
                }
            }

            SystemEvents.PowerModeChanged += OnPowerChange;

        }

        async void SetPlanetaryHour() {
            try {
                byte day = (byte)date.DayOfWeek;
                DateTime dateNow = DateTime.Now;
                Coordinate c = new Coordinate(lat, longi, date);
                Coordinate cTomorrow = new Coordinate(lat, longi, date.AddDays(1));
                DateTime getSunriseToday = ((DateTime)c.CelestialInfo.SunRise).ToLocalTime();
                DateTime getSunriseTomorrow = ((DateTime)cTomorrow.CelestialInfo.SunRise).ToLocalTime();
                DateTime getSunset = ((DateTime)c.CelestialInfo.SunSet).ToLocalTime();
                var getMoonSign = c.CelestialInfo.AstrologicalSigns.EMoonSign;
                double getMoonPhase = (double)c.CelestialInfo.MoonIllum.Phase;
                string getMoonPhaseName = c.CelestialInfo.MoonIllum.PhaseName;
                string sunrise = getSunriseToday.ToString(timeFormat);
                string sunset = getSunset.ToString(timeFormat);
                string sunriseTomorrow = getSunriseTomorrow.ToString();
                TimeSpan differenceNight = getSunriseTomorrow - getSunset;
                double nightHourDuration = (double)differenceNight.TotalMinutes / 12;
                TimeSpan getDifferenceNow = dateNow - getSunriseToday;
                double differenceNow = (double)getDifferenceNow.TotalMinutes;
                TimeSpan differenceHours = getSunset - getSunriseToday;
                double hourDuration = (double)differenceHours.TotalMinutes / 12;
                times.Clear();

                double num = 0;
                for (int i = 0; i <= 11; i++) {
                    times.Add(num);
                    //Debug.WriteLine("Hour added: " + num);
                    num += hourDuration;
                }

                for (int i = 0; i < 13; i++) {
                    times.Add(num);
                    //Debug.WriteLine("Night hour added: " + num);
                    num += nightHourDuration;
                }


                for (int i = 0; i < times.Count - 1; i++) {
                    if (times[i] <= differenceNow && times[i + 1] > differenceNow) {
                        Debug.WriteLine("Today's day is: " + day);
                        planetaryHour.Text = "Hour of " + hoursChaldean[i + days[day]];
                        hourStarted.Text = "(Started: " + (getSunriseToday.AddMinutes(times[i])).ToString(timeFormat) + ")";
                        planetaryDay.Text = "Day of " + hoursChaldean[days[day]];
                        planetarySymbol.Text = glyphs[day].ToString();
                        nextHour.Text = "Next hour: " + hoursChaldean[i + days[day] + 1];
                        nextHourStarts.Text = "(Starts: " + (getSunriseToday.AddMinutes(times[i + 1])).ToString(timeFormat) + ")";
                        sunriseTime.Text = sunrise;
                        sunsetTime.Text = sunset;
                        moonPhase.Text = getMoonPhaseName;
                        moonSign.Text = "(In " + getMoonSign + ")";
                        new ToastContentBuilder()
                        .AddText("Day of " + hoursChaldean[days[day]])
                        .AddText("Hour of " + hoursChaldean[i + days[day]])
                        .Show();
                        int sleepAmount = (int)Math.Ceiling((times[i + 1] - differenceNow) * 60000);
                        GC.Collect();
                        times.Clear();
                        await Task.Delay(sleepAmount);
                        SetPlanetaryHour();
                    }

                    else if (differenceNow < 0) {
                        date = DateTime.Now.AddDays(-1);
                        day = (byte)date.DayOfWeek;
                        SetPlanetaryHour();
                    }

                    else {
                        if (differenceNow > times[24]) {
                            StartNewDay();
                        }
                    }
                }
            }

            catch(Exception ex) {
                MessageBox.Show("Invalid location (likely cause: polar day or night.) Press 'OK to select another location.'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SettingsWindow settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            }

        }



        void StartNewDay() {
            date = DateTime.Now;
            SetPlanetaryHour();
        }

        private void SeeAllHours(object sender, RoutedEventArgs e)
        {

            if (HoursView.checkIfOpen == 0) {
                HoursView hoursView = new HoursView();//no, I am not going to simplify "new()", new what?
                hoursView.Owner = this;
                //Debug.WriteLine(HoursView.checkIfOpen);
                hoursView.Show();
                HoursView.checkIfOpen = 1;
            }

        }


    }
}

