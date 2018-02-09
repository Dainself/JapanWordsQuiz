using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace JapanWordsQuiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    /*internal class ListElem
    {
        public List<String[]> elems;
        public ListElem()
        {
            elems = new List<string[]>();
        }
    }*/
    internal class DictValue
    {
        public string[][] line;
        public DictValue(int default_size)
        {
            line = new string[default_size][];
        }
    }
    public partial class MainWindow : Window
    {
        //List<ListElem> dict_list = new List<ListElem>();
        // if kana -> rus mode, then
        int default_size = 3;
        Dictionary<int, DictValue> dict = new Dictionary<int, DictValue>();
        string[] true_answers;
        const int SHIFT = 1;
        int dict_len, num_of_ans, num_of_quest, curr_index;
        //int[] key_indexes, value_indexes;
        //string curr_hiragana, curr_kanzi;
        public MainWindow()
        {
            InitializeComponent();
            button2.IsEnabled = false;
            num_of_quest = 0;
            num_of_ans = 2;
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
                            AnalyseLine(sr.ReadLine(), out int key, out DictValue value);
                            dict[key] = value;
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Возникла ошибка: " + ex.Message);
                    return;
                }
                button2.IsEnabled = true;
                textBlock1.Text = "";
                textBlock2.Text = "";
            }
        }

        private void LetNewWord()
        {
            Random r = new Random();
            curr_index = r.Next(1, dict_len);
            true_answers = (string[])dict[curr_index].line[num_of_ans].Clone();//(string[])dict[curr_index].line[num_of_ans].Clone();
            if (num_of_ans == 2)
            {
                for (int i = 0; i < true_answers.Length; i++)
                {
                    true_answers[i] = true_answers[i].ToUpper().Replace("ё", "е");
                }
            }
            textBlock1.Text = dict[curr_index].line[0][0]; //dict_list[curr_index].elems[0]/*column in table*/[0]/*num of word*/;
            textBlock1.Text += " " + dict[curr_index].line[1][0];
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            button2.IsEnabled = false;
            LetNewWord();
        }

        private void AnalyseLine(string line, out int key, out DictValue value)
        {
            line = line.TrimStart('\"').TrimEnd('\"');
            string[] str_arr = line.Split(new string[] { "\",\"" }, StringSplitOptions.None);
            key = Int32.Parse(str_arr[0]);
            value = new DictValue(default_size);
            for (int i = 1; i <= default_size; i++)
            {
                string[] str_arr2 = str_arr[i].Split(new string[] { ", " }, StringSplitOptions.None);
                value.line[i - 1] = str_arr2;
            }
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

        private async void button1_Click(object sender, RoutedEventArgs e)
        {
            if (dict.Count == 0) return;
            string user_answer = textBox1.Text;
            if (user_answer == "")
            {
                MessageBox.Show("Введите слово.");
                return;
            }
            if (num_of_ans == 2)
                user_answer = user_answer.ToUpper().Replace("ё", "е");
            if (true_answers.Contains(user_answer))
            {
                textBlock2.Text = "Правильно!";
                dict.Remove(curr_index);
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
