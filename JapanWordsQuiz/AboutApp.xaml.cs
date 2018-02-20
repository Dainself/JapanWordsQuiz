using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JapanWordsQuiz
{
    /// <summary>
    /// Логика взаимодействия для AboutApp.xaml
    /// </summary>
    public partial class AboutApp : Window
    {
        public AboutApp()
        {
            InitializeComponent();
        }

        private void krakozyabr_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(krakozyabr.NavigateUri.ToString());
        }

        private void readme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(readme.NavigateUri.ToString());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Не удается найти указанный файл: ReadMe.txt.\n" +
                    "Убедитесь, что файл находится в директории программы.");
                return;
            }
        }
    }
}
