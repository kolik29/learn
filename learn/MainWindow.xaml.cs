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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace learn
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=base.db; Version=3;");
            try
            {
                conn.Open();
                SQLiteCommand sql_command = conn.CreateCommand();
                sql_command.CommandText = "SELECT grup FROM users WHERE login='" + login.Text + "' AND pass='" + pass.Text + "'";

                SQLiteDataReader reader = sql_command.ExecuteReader();
                reader.Read();

                if (reader.StepCount == 0)
                {
                    MessageBox.Show("Авторизация не удалась.");
                }
                else
                {
                    if (reader[0].ToString() == "Администраторы")
                    {
                        adminWindow w = new adminWindow();
                        w.Show();
                        this.Close();
                    }
                    else
                    {
                        ContentWindow w = new ContentWindow();
                        w.Show();
                        this.Close();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
