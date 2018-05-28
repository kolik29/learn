﻿using System;
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
            ObservableCollection<tree_node> nodes = new ObservableCollection<tree_node>();
            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = Environment.CurrentDirectory +  @"\include\index.html";
            chromiumWebBrowser.Margin = new Thickness(5, 30, 5, 40);
            chromiumWebBrowser.Width = double.NaN;
            chromiumWebBrowser.Height = double.NaN;

            Grid.SetRow(chromiumWebBrowser, 1);
            browser_panel.Children.Add(chromiumWebBrowser);

            SQLiteConnection conn = new SQLiteConnection("Data Source=base.db; Version=3;");
            try
            {
                conn.Open();
                SQLiteCommand sql_command = conn.CreateCommand();
                sql_command.CommandText = "SELECT grup FROM users";

                SQLiteDataReader reader = sql_command.ExecuteReader();
                reader.Read();

                List<string> groups = new List<string>();

                while (reader.Read())
                    groups.Add(reader[0].ToString());

                groups = groups.Distinct().ToList();

                foreach (string gr in groups)
                    nodes.Add(new tree_node { Name = gr });

                for (int i = 0; i < groups.Count; i++)
                {
                    sql_command.CommandText = "SELECT lName, fName, mName FROM users WHERE grup='" + groups[i].ToString() + "'";
                    while (reader.Read())
                    {
                        nodes[i].Nodes.Add(new tree_node { Name = reader[0].ToString() });
                    }
                }
            }

            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }

            all_users.ItemsSource = nodes;
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=base.db; Version=3;");
            try
            {
                conn.Open();
                SQLiteCommand sql_command = conn.CreateCommand();
                sql_command.CommandText = "SELECT * FROM users WHERE login='" + login.Text + "'";

                SQLiteDataReader reader = sql_command.ExecuteReader();
                reader.Read();

                if (reader.StepCount == 0)
                {
                    reader.Close();
                    sql_command = conn.CreateCommand();
                    sql_command.CommandText = @"INSERT INTO users(lName, fName, mName, grup, login, pass) VALUES('" + 
                                                lName.Text + "', '" + 
                                                fName.Text + "', '" + 
                                                mName.Text + "', '" + 
                                                group.Text + "', '" +
                                                login.Text + "', '" +
                                                pass.Text + "')";

                    sql_command.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
