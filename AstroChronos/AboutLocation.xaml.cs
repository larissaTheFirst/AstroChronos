using System;
using System.Collections.Generic;
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

namespace AstroChronos
{
    /// <summary>
    /// Interaction logic for AboutLocation.xaml
    /// </summary>
    public partial class AboutLocation : Window
    {
        public static byte isOpen = 0;
        public AboutLocation() {
            InitializeComponent();
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            isOpen = 0;
        }

        private void ButtonClick(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
