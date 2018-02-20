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
    /// Логика взаимодействия для AboutAutor.xaml
    /// </summary>
    public partial class AboutAutor : Window
    {
        public AboutAutor()
        {
            InitializeComponent();
        }

        private void vk_Click(object sender, RoutedEventArgs e)
        {
            string url = vk.NavigateUri.ToString();
            Process.Start(url);
        }
    }
}
