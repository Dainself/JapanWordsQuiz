using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        int dict_len, num_of_ans, addit_num_of_ans, num_of_quest, curr_index, dict_num;
        public MainWindow()
        {
            InitializeComponent();
            RightSideIsActivated(false);
            image.Visibility = Visibility.Hidden;
        }

        string[] true_answers2;
        private void LetNewWord()
        {
            Random r = new Random();
            curr_index = r.Next(dict_len);
            true_answers = (string[])dict.Values.ElementAt(curr_index).line[num_of_ans].Clone();
            if (dict_num == 4)
            {
                true_answers2 = (string[])dict.Values.ElementAt(curr_index).line[addit_num_of_ans].Clone();
            }
            question_text_block.Text = dict.Values.ElementAt(curr_index).line[num_of_quest][0];
            if (dict_num == 1 || dict_num == 2)
            {
                for (int i = 0; i < true_answers.Length; i++)
                {
                    true_answers[i] = true_answers[i].ToUpper().Replace("ё", "е");
                }
                if (dict_num == 1)
                {
                    question_text_block.Text += " " + dict.Values.ElementAt(curr_index).line[1][0];
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // deserialization
            if (!DictTypeTryParse())
            {
                MessageBox.Show("Выберите тип теста");
                return;
            }
            if (!Int32.TryParse(dict_len_textblock.Text, out dict_len) || dict_len == 0)
            {
                MessageBox.Show("В поле 'Количество слов' введите число больше нуля");
                return;
            }
            if (dict_num == 4 && dict_len > 120 || dict_num == 2 && dict_len > 683 || dict_len > 704)
            {
                MessageBox.Show("Слишком большое число слов.");
                return;
            }
            dict = Deserialize(dict_num);
            LeftSideIsActivated(false);
            RightSideIsActivated(true);
            count_text_block.Text = dict_len.ToString();
            LetNewWord();
        }

        bool DictTypeTryParse()
        {
            if ((bool)radioButton.IsChecked)
            {
                dict_num = 1;
                default_size = 3;
                num_of_quest = 0;
                num_of_ans = 2;
                return true;
            }
            else if ((bool)radioButton1.IsChecked)
            {
                dict_num = 2;
                default_size = 3;
                num_of_quest = 1;
                num_of_ans = 2;
                return true;
            }
            else if ((bool)radioButton2.IsChecked)
            {
                dict_num = 3;
                default_size = 3;
                num_of_quest = 2;
                num_of_ans = 0;
                return true;
            }
            else if ((bool)radioButton3.IsChecked)
            {
                dict_num = 4;
                default_size = 4;
                num_of_quest = 0;
                num_of_ans = 1;
                addit_num_of_ans = 2;
                return true;
            }
            else return false;
        }

        private void LeftSideIsActivated(bool status)
        {
            StartButton.IsEnabled = status;
            dict_len_textblock.IsEnabled = status;
            radioButton.IsEnabled = status;
            radioButton1.IsEnabled = status;
            radioButton2.IsEnabled = status;
            radioButton3.IsEnabled = status;
        }
        private void RightSideIsActivated(bool status)
        {
            OK_button.IsEnabled = status;
            back_button.IsEnabled = status;
            answer_text_box.IsEnabled = status;
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

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            dict = null;
            dict_len = 0;
            dict_num = 0;
            addit_num_of_ans = 0;
            num_of_ans = 0;
            num_of_quest = 0;
            LeftSideIsActivated(true);
            RightSideIsActivated(false);
        }

        AboutAutor autor_wind;
        private void autor_Click(object sender, RoutedEventArgs e)
        {
            autor_wind = new AboutAutor();
            autor_wind.Owner = this;
            autor_wind.ShowDialog();
            autor_wind = null;
        }

        AboutApp app_wind;
        private void app_Click(object sender, RoutedEventArgs e)
        {
            app_wind = new AboutApp();
            app_wind.Owner = this;
            app_wind.ShowDialog();
            app_wind = null;
        }

        private void image_switcher_Click(object sender, RoutedEventArgs e)
        {
            if (!image_switcher.IsChecked)
            {
                image.Visibility = Visibility.Hidden;
            }
            else image.Visibility = Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            dict = new Dictionary<int, DictValue>();
            using (StreamReader sr = new StreamReader(@"C:\Users\user\source\repos\JapanWordsQuiz\N5_kanji.tsv"))
            {
                for (int counter = 0; counter < 120; counter++)
                {
                    AnalyseLine(sr.ReadLine(), out int key, out DictValue value);
                    dict[key] = value;
                }
            }
            Serialize();
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
                num_of_ans = 1;
                addit_num_of_ans = 2;
            }
        }

        private async void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (dict_len == 0) return;
            if (answer_text_box.Text == "")
            {
                MessageBox.Show("Введите слово.");
                return;
            }
            if (dict_num == 4)
            {
                string[] user_anss = answer_text_box.Text.Split(new string[] { ", ", "、", "、 ", "，", "， " }, StringSplitOptions.None);
                if (true_answers.Contains(user_anss[0]) && true_answers2.Contains(user_anss[1]))
                {
                    status_text_block.Text = "Правильно!";
                    dict.Remove(dict.Keys.ElementAt(curr_index));
                    dict_len--;
                    count_text_block.Text = dict_len.ToString();
                }
                else status_text_block.Text = "Ошибка.";
            }
            else
            {
                string user_answer = answer_text_box.Text;
                if (dict_num == 1 || dict_num == 2)
                    user_answer = user_answer.ToUpper().Replace("ё", "е");
                if (true_answers.Contains(user_answer))
                {
                    status_text_block.Text = "Правильно!";
                    dict.Remove(dict.Keys.ElementAt(curr_index));
                    dict_len--;
                    count_text_block.Text = dict_len.ToString();
                }
                else status_text_block.Text = "Ошибка.";
            }
            OK_button.IsEnabled = false;
            answer_text_box.Text = "";
            await Task.Run(() => Thread.Sleep(1000));
            question_text_block.Text = "";
            OK_button.IsEnabled = true;
            if (dict_len > 0)
            {
                status_text_block.Text = "...";
                LetNewWord();
            }
            else status_text_block.Text = "Слова закончились.";
        }

        private void Serialize()
        {
            /*StringBuilder sb = new StringBuilder();
            for (int i = 1; i < dict_len; i++)
            {
                sb.Append(dict[i].line[1][0]);
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString());*/
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(@"C:\Users\user\source\repos\JapanWordsQuiz\dict_kanji.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, dict);
            }
            MessageBox.Show("Finish serialize");
        }

        private Dictionary<int, DictValue> Deserialize(int dict_num)
        {
            Dictionary<int, DictValue> new_dict;
            string path = (dict_num == 4) ? @"C:\Users\user\source\repos\JapanWordsQuiz\dict_kanji.dat" :
                @"C:\Users\user\source\repos\JapanWordsQuiz\dict_goi.dat";
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                new_dict = (Dictionary<int, DictValue>)formatter.Deserialize(fs);
            }
            Dictionary<int, DictValue> ret_dict = new Dictionary<int, DictValue>();
            int i = 1;
            while (ret_dict.Count < dict_len)
            {
                if (dict_num == 2)
                {
                    string dbg = new_dict[i].line[1][0];
                    if (dbg != "")
                        ret_dict.Add(i, new_dict[i]);
                }
                else ret_dict.Add(i, new_dict[i]);
                ++i;
            }
            return ret_dict;
        }
    }
}
