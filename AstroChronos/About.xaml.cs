using System;
using System.Windows;

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public static bool checkIfOpen;
        public About() {
            InitializeComponent();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            checkIfOpen = false;
            Owner.Activate();
            GC.Collect();
        }
    }
}
