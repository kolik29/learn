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
using System.Data.SQLite;
using System.IO;
using System.Collections.ObjectModel;
using System.Data.Common;

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

        public class tree_node //объявление класса
        {
            public string Name { get; set; } //объявление параметров
            public ObservableCollection<tree_node> Nodes { get; set; }
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

            get_userlist();
        }

        private List<List<string>> connect_to_bd(string command, bool read = true)
        {
            List<List<string>> result = new List<List<string>>();
            SQLiteConnection conn = new SQLiteConnection("Data Source=base.db; Version=3;");
            SQLiteCommand sql_command = conn.CreateCommand();
            sql_command.CommandText = command;
            SQLiteDataReader reader = null;
            try
            {
                conn.Open();

                if (!read)
                    return null;

                reader = sql_command.ExecuteReader();
                reader.Read();
                
                foreach(DbDataRecord record in reader)
                {
                    List<string> temple = new List<string>();

                    for (int i = 0; i < record.FieldCount; i++)
                    {
                        temple.Add(record[i].ToString());
                    }

                    result.Add(temple);
                }

                reader.Close();
                conn.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        List<string> grups = new List<string>();
        private void get_userlist()
        {
            List<List<string>> user_list = connect_to_bd("SELECT * FROM USERS");

            grups = new List<string>();
            foreach(List<string> g in user_list)
                grups.Add(g[4]);

            grups = grups.Distinct().ToList();

            ObservableCollection<tree_node> nodes = new ObservableCollection<tree_node>();

            foreach (string g in grups)
            {
                ObservableCollection<tree_node> t_nodes = new ObservableCollection<tree_node>();

                foreach (List<string> u in user_list)
                {
                    if (u[4] == g)
                    {
                        if (u[1] != "")
                            t_nodes.Add(new tree_node() {
                                Name = u[1] + " " + u[2] + " " + u[3] + " (" + u[5] + ")"
                            });
                        else
                            t_nodes.Add(new tree_node()
                            {
                                Name = u[5]
                            });
                    }
                }

                nodes.Add(new tree_node() {
                    Name = g,
                    Nodes = t_nodes
                });
            }
            all_users.ItemsSource = nodes;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            connect_to_bd(@"UPDATE users SET lName = '" +  lName.Text + "', " +
                                "fName = '" + fName.Text + "', " +
                                "mName = '" + mName.Text + "', " +
                                "grup = '" + group.Text + "', " +
                                "login = '" + login.Text + "', " +
                                "pass = '" + pass.Text + "'" + 
                                "WHERE login = '" + login.Text + "';", false);
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            connect_to_bd(@"INSERT INTO users(lName, fName, mName, grup, login, pass)
                            VALUES '" +
                                lName.Text + "', '" +
                                fName.Text + "', '" +
                                mName.Text + "', '" +
                                group.Text + "', '" +
                                login.Text + "', '" +
                                pass.Text + "';", false);
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            connect_to_bd(@"DELETE FROM users WHERE login = '" + login.Text + "';", false);
        }

        private void all_users_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView tv = (TreeView)sender;
            tree_node tn = (tree_node)tv.SelectedItem;

            bool not_grup = true;
            foreach(string g in grups)
            {
                if (g == tn.Name)
                {
                    not_grup = false;
                    break;
                }
            }

            List<List<string>> user_list = new List<List<string>>();
            if (not_grup)
                user_list = connect_to_bd("SELECT * FROM USERS");
            
            foreach (List<string> row in user_list)
            {
                if (row[5] == tn.Name.Split('(')[1].Split(')')[0])
                {
                    lName.Text = row[1];
                    fName.Text = row[2];
                    mName.Text = row[3];
                    group.Text = row[4];
                    login.Text = row[5]; 
                    pass.Text = row[6];
                }
            }
        }

        private void add_theme_Click(object sender, RoutedEventArgs e)
        {
            browser_panel.Visibility = Visibility.Visible;
            select_panel.Visibility = Visibility.Collapsed;
            title_label.Content = "Курсы";
        }

        private void add_user_Click(object sender, RoutedEventArgs e)
        {
            user_panel.Visibility = Visibility.Visible;
            select_panel.Visibility = Visibility.Collapsed;
            title_label.Content = "Пользователи";
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            user_panel.Visibility = Visibility.Collapsed;
            browser_panel.Visibility = Visibility.Collapsed;
            title_label.Content = "Главная";
            select_panel.Visibility = Visibility.Visible;
        }
    }
}
