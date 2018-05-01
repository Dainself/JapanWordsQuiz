/* 
 * Autor: Kakotkin Vyacheslav
 * mailto: MetalSl8@yandex.ru
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace JapanWordsQuiz
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RightSideIsActivated(false);
            rand_len_textbox.IsEnabled = false;
            image.Visibility = Visibility.Hidden;
            ans_for_user.Visibility = Visibility.Visible;
            ans_switcher.IsChecked = true;
            N5.IsChecked = true;
            error_wait_time = 3000;
            //radio buttons
            radioButton1.IsEnabled = false;
            radioButton2.IsEnabled = false;
            radioButton3.IsEnabled = false;
            //dev part
            ser_txtbox.IsEnabled = false;
        }

        string[] true_answers2;
       
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int dict_start_pos = 0; // dict deserial. starts from this index
            // checkes
            if (!DictTypeTryParse())
            {
                MessageBox.Show("Выберите тип теста");
                return;
            }
            if (!N5.IsChecked && !N4.IsChecked)
            {
                MessageBox.Show("Выберите уровень теста");
                return;
            }
            if (!(bool)is_rand_chck_box.IsChecked)
            {
                if (!Int32.TryParse(dict_len_textblock.Text, out dict_len_h) || dict_len_h <= 0)
                {
                    MessageBox.Show("В поле 'Количество слов' введите число больше нуля");
                    return;
                }
                if (!Int32.TryParse(dict_len_start_textblock.Text, out dict_start_pos) || dict_start_pos <= 0)
                {
                    MessageBox.Show("В поле 'Начиная с...' введите число больше нуля");
                    return;
                }
            }
            else
            {
                if (!Int32.TryParse(rand_len_textbox.Text, out dict_len_h) || dict_len_h <= 0)
                {
                    MessageBox.Show("В поле 'Количество' введите число больше нуля");
                    return;
                }
            }
            Dictionary<int, DictValue> big_dict;
            try
            {
                string fname = (N5.IsChecked) ? "N5_goi.dat" : "N4_goi.dat";
                big_dict = Deserialize1(dict_num, fname);
            }
            catch (FileNotFoundException fnfe)
            {
                MessageBox.Show("Указанного файла нет в рабочей директории: " + fnfe.FileName);
                return;
            }
            if (dict_start_pos + dict_len_h > big_dict.Count + 1/*dict_num == 4 && dict_len_l + dict_len_h - 1 > 120 || dict_num == 2 && dict_len_l + dict_len_h - 1 > 683 ||*/)
            {
                MessageBox.Show("Слишком большое число слов.");
                return;
            }
            dict = Deserialize2(big_dict, dict_start_pos, dict_num);
            LeftSideIsActivated(false);
            RightSideIsActivated(true);
            count_text_block.Text = dict_len_h.ToString();
            status_text_block.Text = "...";
            question_text_block.Text = LetNewWord();
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
            is_rand_chck_box.IsEnabled = status;
            if (status)
            {
                if ((bool)is_rand_chck_box.IsChecked)
                {
                    rand_len_textbox.IsEnabled = status;
                }
                else
                {
                    dict_len_textblock.IsEnabled = status;
                    dict_len_start_textblock.IsEnabled = status;
                }
            }
            else
            {
                rand_len_textbox.IsEnabled = status;
                dict_len_textblock.IsEnabled = status;
                dict_len_start_textblock.IsEnabled = status;
            }
            radioButton.IsEnabled = status;
            //radioButton1.IsEnabled = status;
            //radioButton2.IsEnabled = status;
            //radioButton3.IsEnabled = status;
        }
        private void RightSideIsActivated(bool status)
        {
            OK_button.IsEnabled = status;
            back_button.IsEnabled = status;
            answer_text_box.IsEnabled = status;
        }

        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            dict = null;
            dict_len_h = 0;
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

        private void answer_text_box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                OK_button_Click(null, null);
            }
        }

        private void is_rand_chck_box_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)is_rand_chck_box.IsChecked)
            {
                is_rand_chck_box.IsChecked = false;
                rand_len_textbox.IsEnabled = false;
                dict_len_textblock.IsEnabled = true;
                dict_len_start_textblock.IsEnabled = true;
            }
            else
            {
                is_rand_chck_box.IsChecked = true;
                rand_len_textbox.IsEnabled = true;
                dict_len_textblock.IsEnabled = false;
                dict_len_start_textblock.IsEnabled = false;
            }
        }

        private void ser_txtbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (!DictTypeTryParse())
                {
                    MessageBox.Show("Выберите тип теста");
                    return;
                }
                StartSer(ser_txtbox.Text);
                MessageBox.Show("Finish serialize");
            }
        }

        private void N5_Checked(object sender, RoutedEventArgs e)
        {
            if (N4.IsChecked)
            {
                N4.IsChecked = false;
            }
        }

        private void N4_Checked(object sender, RoutedEventArgs e)
        {
            if (N5.IsChecked)
            {
                N5.IsChecked = false;
            }
        }

        private void ans_switcher_Click(object sender, RoutedEventArgs e)
        {
            if (!ans_switcher.IsChecked)
            {
                ans_for_user.Visibility = Visibility.Hidden;
                error_wait_time = 1000;
            }
            else
            {
                ans_for_user.Visibility = Visibility.Visible;
                error_wait_time = 3000;
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
                num_of_ans = 1;
                addit_num_of_ans = 2;
            }
        }

        int error_wait_time;
        private async void OK_button_Click(object sender, RoutedEventArgs e)
        {
            if (dict_len_h == 0) return;
            if (answer_text_box.Text == "")
            {
                MessageBox.Show("Введите слово.");
                return;
            }
            bool cond = IsUserRight(out string ans_str);
            OK_button.IsEnabled = false;
            answer_text_box.Text = "";
            if (cond)
            {
                status_text_block.Text = "Правильно!";
                dict.Remove(dict.Keys.ElementAt(curr_index));
                dict_len_h--;
                count_text_block.Text = dict_len_h.ToString();
                await Task.Run(() => Thread.Sleep(1000));
            }
            else
            {
                status_text_block.Text = "Ошибка.";
                ans_for_user.Text += ans_str;
                await Task.Run(() => Thread.Sleep(error_wait_time));
            }
            //OK_button.IsEnabled = false;
            //answer_text_box.Text = "";
            question_text_block.Text = "";
            ans_for_user.Text = "Ответ: ";
            OK_button.IsEnabled = true;
            if (dict_len_h > 0)
            {
                status_text_block.Text = "...";
                question_text_block.Text = LetNewWord();
            }
            else status_text_block.Text = "Слова закончились.";
        }

    }
}
