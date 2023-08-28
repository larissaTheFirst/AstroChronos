using System;
using System.Windows;

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public static byte checkIfOpen = 0;
        public About() {
            InitializeComponent();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            checkIfOpen = 1;
            Owner.Activate();
            GC.Collect();
        }
    }
}
