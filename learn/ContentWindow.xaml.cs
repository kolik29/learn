using System;
using System.Collections.Generic; //обяхеление пространства имен для типов List
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; //для работы с компонентами wpf
using System.Windows.Controls; //аналогично
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel; //объектаная модель для List
using CefSharp; //для работы с встроенным браузером на основе Chromium
using CefSharp.Wpf; //для работы с Chromium в wpf
using System.IO; //для работы с файлами
using img = System.Drawing; //для работы с картинками

namespace learn //объявление пространства имен learns
{
    /// <summary>
    /// Логика взаимодействия для ContentWindow.xaml
    /// </summary>
    public partial class ContentWindow : Window //объявление класса ContentWindow наследуемого от windows
    {
        public ContentWindow() //первая функция, выполняемая при октрытии окна
        {
            InitializeComponent(); //инициализация кмпонентов
        }

        class table_row //объявление класса
        {
            public table_row(string subj, string tests, string average) //объявление метода с параметрами
            {
                this.subj = subj; //сами параметры
                this.tests = tests;
                this.average = average;
            }
            public string subj { get; set; }
            public string tests { get; set; }
            public string average { get; set; }
        }

        public class tree_node //объявление класса
        {
            public string Name { get; set; } //объявление параметров
            public ObservableCollection<tree_node> Nodes { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fam.Content += "Орджоникидзе";
            f_name.Content += "Валерий";
            l_name.Content += "Стефанович";
            group.Content += "АПР-123";

            List<table_row> result = new List<table_row>(3);
            result.Add(new table_row("Русский", "4 из 6", "4,9"));
            result.Add(new table_row("Менеджмент", "12 из 19", "4,1"));
            result.Add(new table_row("Информатика", "4 из 9", "3,9"));
            main_table.ItemsSource = result;

            foreach(DataGridColumn col in main_table.Columns)
            {
                col.MinWidth = col.ActualWidth;
                col.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            }

            main_table.Columns[0].Header = "Предмет";
            main_table.Columns[1].Header = "Пройденные тесты";
            main_table.Columns[2].Header = "Средняя оценка";

            ObservableCollection<tree_node> nodes = new ObservableCollection<tree_node>();
            nodes.Add(new tree_node
            {
                Name = "Предмет 1",
                Nodes = new ObservableCollection<tree_node>
                    {
                        new tree_node { Name = "Тема 1" },
                        new tree_node { Name = "Тема 2" },
                        new tree_node
                        {
                            Name ="Глава 1",
                            Nodes = new ObservableCollection<tree_node>
                            {
                                new tree_node { Name = "Глава 1.1" },
                                new tree_node { Name = "Глава 1.2" },
                                new tree_node { Name = "Глава 1.3" },
                                new tree_node { Name = "Глава 1.4" },
                            }
                        }
                    }
            });
            nodes.Add(new tree_node
            {
                Name = "Предемет 2",
                Nodes = new ObservableCollection<tree_node>
                {
                    new tree_node { Name = "Тема 1" },
                    new tree_node { Name = "Тема 2" }
                }
            });

            content.Visibility = Visibility.Collapsed;

            _tree.ItemsSource = nodes;

            chromiumWebBrowser = new ChromiumWebBrowser();
            chromiumWebBrowser.Address = Environment.CurrentDirectory +  @"\include\index.html";
            chromiumWebBrowser.Margin = new Thickness(5, 0, 5, 5);
            chromiumWebBrowser.Width = double.NaN;
            chromiumWebBrowser.Height = double.NaN;

            Grid.SetRow(chromiumWebBrowser, 1);
            grid.Children.Add(chromiumWebBrowser);
        }

        ChromiumWebBrowser chromiumWebBrowser;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string text = chromiumWebBrowser.EvaluateScriptAsync(@"window.frames[0].document.body.innerHTML").Result.Result.ToString();
            int strStart = text.IndexOf("<img src=\"") + 10,
                len = text.IndexOf("\">", strStart) - 10;

            string path = text.Substring(strStart, len),
                base64String = string.Empty;

            using (img.Image image = img.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    
                    base64String = Convert.ToBase64String(imageBytes);
                }
            }

            MessageBox.Show(text.Replace(text.Substring(strStart, len), base64String));
        }
    }
}
