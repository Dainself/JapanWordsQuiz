using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;

// проверка слов целиком
// ё
// большие-маленькие буквы
namespace JapanWordsQuiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    internal class ListElem
    {
        public List<String[]> elems;
        public ListElem()
        {
            elems = new List<string[]>();
        }
    }
    public partial class MainWindow : Window
    {
        List<ListElem> dict_list = new List<ListElem>();
        EventWaitHandle handle = new AutoResetEvent(false);
        const int SHIFT = 1;
        int dict_len, split_constr, num_of_ans, curr_index;
        //int[] key_indexes, value_indexes;
        //string curr_hiragana, curr_kanzi;
        public MainWindow()
        {
            InitializeComponent();
            split_constr = 4;
            num_of_ans = 2;
            //key_indexes = new int[] { 1, 2 };
            //value_indexes = new int[] { 3 };
        }
    
        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Tabulated text files (*.tsv)|*.tsv"; // it's a lie
            if (fileDialog.ShowDialog() != null)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(fileDialog.FileName))
                    {
                        if (!Int32.TryParse(textBox.Text, out dict_len))
                        {
                            throw new ArgumentException("Количество строк должно быть числом.");
                        }
                        else if (dict_len == 0) throw new ArgumentException("Число не может быть равно нулю.");
                        for (int i = 0; i < SHIFT; sr.ReadLine(), i++) ;
                        for (int counter = SHIFT; counter <= dict_len; counter++)
                        {
                            ListElem elem = AnalyseLine(sr.ReadLine());
                            dict_list.Add(elem);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Возникла ошибка: " + ex.Message);
                }
                MessageBox.Show(dict_list.Count.ToString());
                handle.Set();
            }
        }

        private void LetNewWord()
        {
            Random r = new Random();
            curr_index = r.Next(dict_len);
            textBlock1.Text = dict_list[curr_index].elems[0]/*column in table*/[0]/*num of word*/;
        }

        private ListElem AnalyseLine(string line)
        {
            line = line.TrimStart().TrimEnd();
            string[] str_arr = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            ListElem temp_elem = new ListElem();
            for (int i = 1; i < split_constr; i++)
            {
                string[] str_arr2 = str_arr[i].Split(new string[] { ", " }, StringSplitOptions.None);
                temp_elem.elems.Add(str_arr2);
            }
            return temp_elem;
            /*try
            {

                var temp = from i in key_indexes select str_arr[i];
                key = temp.ToArray();
                temp = from i in value_indexes select str_arr[i];
                value = temp.ToArray();
                //foreach (var elem in key)
                //{
                //    MessageBox.Show(elem);
                //}
            }
            catch (NullReferenceException)
            {
                throw;
            }*/
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run( () => handle.WaitOne());
            LetNewWord();
        }

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            string[] true_answer = dict_list[curr_index].elems[num_of_ans];
            if (true_answer.Contains(textBox1.Text))
            {
                textBlock2.Text = "Правильно!";
                dict_list.Remove(dict_list[curr_index]);
                dict_len--;
                textBox.Text = dict_len.ToString();
            }
            else textBlock2.Text = "Ошибка.";
            button1.IsEnabled = false;
            textBox1.Text = "";
            await Task.Run(() => Thread.Sleep(1000));
            button1.IsEnabled = true;
            if (dict_len > 0)
            {
                textBlock2.Text = "...";
                LetNewWord();
            }
            else textBlock2.Text = "Слова закончились.";
        }
    }
}
