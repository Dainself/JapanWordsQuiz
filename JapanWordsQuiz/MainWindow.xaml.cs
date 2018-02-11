using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JapanWordsQuiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [Serializable]
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
        int default_size;
        Dictionary<int, DictValue> dict = new Dictionary<int, DictValue>();
        string[] true_answers;
        int dict_len, num_of_ans, num_of_quest, curr_index, dict_num;
        public MainWindow()
        {
            InitializeComponent();
            radioButton3.IsEnabled = false;
            back_button.IsEnabled = false;
            OK_button.IsEnabled = false;
        }
    
        private void LetNewWord()
        {
            Random r = new Random();
            curr_index = r.Next(1, dict_len);
            true_answers = (string[])dict.Values.ElementAt(curr_index).line[num_of_ans].Clone();//(string[])dict[curr_index].line[num_of_ans].Clone();
            if (num_of_ans == 2)
            {
                for (int i = 0; i < true_answers.Length; i++)
                {
                    true_answers[i] = true_answers[i].ToUpper().Replace("ё", "е");
                }
            }
            question_text_block.Text = dict.Values.ElementAt(curr_index).line[0][0];
            question_text_block.Text += " " + dict.Values.ElementAt(curr_index).line[1][0];
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LetNewWord();
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            // deserialization
            if (dict_num == 0)
            {
                MessageBox.Show("Выберите тип теста");
                return;
            }
            if (!Int32.TryParse(dict_len_textblock.Text, out dict_len) & dict_len == 0)
            {
                MessageBox.Show("В поле 'Количество слов' введите число больше нуля");
                return;
            }
            dict = Deserialize(dict_num);
            StartButton.IsEnabled = false;
            dict_len_textblock.IsEnabled = false;
            OK_button.IsEnabled = true;
            back_button.IsEnabled = true;
            count_text_block.Text = dict_len.ToString();
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
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (((RadioButton)sender).Name == "radioButton")
            {
                dict_num = 1;
                default_size = 3;
                num_of_quest = 0;
                num_of_ans = 2;
            }
            else if (((RadioButton)sender).Name == "radioButton1")
            {
                dict_num = 2;
                default_size = 3;
                num_of_quest = 1;
                num_of_ans = 2;
            }
            else if (((RadioButton)sender).Name == "radioButton2")
            {
                dict_num = 3;
                default_size = 3;
                num_of_quest = 2;
                num_of_ans = 0;
            }
            else if (((RadioButton)sender).Name == "radioButton3")
            {
                dict_num = 4;
                default_size = 4;
                num_of_quest = 0;
                //num_of_ans = 1, 2;
            }
        }

        private async void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (dict.Count == 0) return;
            string user_answer = answer_text_box.Text;
            if (user_answer == "")
            {
                MessageBox.Show("Введите слово.");
                return;
            }
            if (num_of_ans == 2)
                user_answer = user_answer.ToUpper().Replace("ё", "е");
            if (true_answers.Contains(user_answer))
            {
                status_text_block.Text = "Правильно!";
                dict.Remove(dict.Keys.ElementAt(curr_index));
                dict_len--;
                textBox.Text = dict_len.ToString();
            }
            else status_text_block.Text = "Ошибка.";
            button1.IsEnabled = false;
            answer_text_box.Text = "";
            await Task.Run(() => Thread.Sleep(1000));
            button1.IsEnabled = true;
            if (dict_len > 0)
            {
                status_text_block.Text = "...";
                LetNewWord();
            }
            else status_text_block.Text = "Слова закончились.";
        }

        private void Serialize()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(@"C:\Users\user\source\repos\JapanWordsQuiz\dict_goi.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, dict);
            }
        }

        private Dictionary<int, DictValue> Deserialize(int dict_num)
        {
            Dictionary<int, DictValue> new_dict;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(@"C:\Users\user\source\repos\JapanWordsQuiz\dict_goi.dat", FileMode.Open))
            {
                new_dict = (Dictionary<int, DictValue>)formatter.Deserialize(fs);
            }
            return new_dict;
        }
    }
}
