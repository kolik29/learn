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
using CefSharp.Wpf; //для работы с Chromium в wpf

namespace learn
{
    /// <summary>
    /// Логика взаимодействия для adminWindow.xaml
    /// </summary>
    public partial class adminWindow : Window
    {
        public adminWindow()
        {
            InitializeComponent();
        }

        private void generate_pass_Click(object sender, RoutedEventArgs e)
        {
            pass.Text = string.Empty;
            List<string> pass_char = new List<string> { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "a", "s", "d", "f", "g", "h", "j", "k", "l", "z", "x", "c", "v", "b", "n", "m", "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            int r = new int();
            Random rand = new Random();
            r = rand.Next(7, 12);

            for (int i = 0; i < r; i++)
                pass.Text += pass_char[rand.Next(0, pass_char.Count)];
        }

        ChromiumWebBrowser chromiumWebBrowser;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = Environment.CurrentDirectory +  @"\include\index.html";
            chromiumWebBrowser.Margin = new Thickness(5, 30, 5, 40);
            chromiumWebBrowser.Width = double.NaN;
            chromiumWebBrowser.Height = double.NaN;

            Grid.SetRow(chromiumWebBrowser, 1);
            browser_panel.Children.Add(chromiumWebBrowser);
        }
    }
}
