/* 
 * Autor: Kakotkin Vyacheslav
 * mailto: MetalSl8@yandex.ru
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JapanWordsQuiz
{
    partial class MainWindow
    {
        [Serializable]
        internal class DictValue
        {
            public string[][] line;
            public DictValue(int default_size)
            {
                line = new string[default_size][];
            }
        }

        int default_size;
        Dictionary<int, DictValue> dict = new Dictionary<int, DictValue>();
        string[] true_answers;
        int dict_len_h; // current len of dict
        int num_of_ans, addit_num_of_ans, num_of_quest, curr_index, dict_num;

        private string LetNewWord()
        {
            string question = "";
            Random r = new Random();
            curr_index = r.Next(dict_len_h);
            true_answers = (string[])dict.Values.ElementAt(curr_index).line[num_of_ans].Clone();
            if (dict_num == 4)
            {
                true_answers2 = (string[])dict.Values.ElementAt(curr_index).line[addit_num_of_ans].Clone();
            }
            question += dict.Values.ElementAt(curr_index).line[num_of_quest][0];
            if (dict_num == 1 || dict_num == 2)
            {
                for (int i = 0; i < true_answers.Length; i++)
                {
                    true_answers[i] = true_answers[i].ToUpper().Replace("ё", "е");
                }
                if (dict_num == 1)
                {
                    question += " " + dict.Values.ElementAt(curr_index).line[1][0];
                }
            }
            return question;
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

        private void StartSer(string path)
        {
            dict = new Dictionary<int, DictValue>();
            using (StreamReader sr = new StreamReader(".//" + path + ".tsv"))
            {
                while (!sr.EndOfStream)//for (int counter = 0; counter < 120; counter++)
                {
                    AnalyseLine(sr.ReadLine(), out int key, out DictValue value);
                    dict[key] = value;
                }
            }
            Serialize(path);
        }

        private bool IsUserRight(out string ans_str)
        {
            bool cond;
            if (dict_num == 4)
            {
                string[] user_anss = answer_text_box.Text.Split(new string[] { ", ", "、", "、 ", "，", "， " }, StringSplitOptions.None);
                ans_str = dict.Values.ElementAt(curr_index).line[num_of_ans][0] + ", " +
                    dict.Values.ElementAt(curr_index).line[addit_num_of_ans][0];
                cond = true_answers.Contains(user_anss[0]) && true_answers2.Contains(user_anss[1]);
            }
            else
            {
                string user_answer = answer_text_box.Text;
                ans_str = dict.Values.ElementAt(curr_index).line[num_of_ans][0];
                if (dict_num == 1 || dict_num == 2)
                    user_answer = user_answer.ToUpper().Replace("ё", "е");
                cond = true_answers.Contains(user_answer);
            }
            return cond;
        }
        private void Serialize(string path)
        {
            /*StringBuilder sb = new StringBuilder();
            for (int i = 1; i < dict_len; i++)
            {
                sb.Append(dict[i].line[1][0]);
                sb.AppendLine();
            }
            MessageBox.Show(sb.ToString());*/
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(".\\" + path + ".dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, dict);
            }
        }

        private Dictionary<int, DictValue> Deserialize1(int dict_num, string fname)
        {
            Dictionary<int, DictValue> new_dict;
            string path = (dict_num == 4) ? ".\\dict_kanji.dat" :
                ".\\" + fname;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                new_dict = (Dictionary<int, DictValue>)formatter.Deserialize(fs);
            }
            return new_dict;
        }

        private Dictionary<int, DictValue> Deserialize2(Dictionary<int, DictValue> big_dict, int start_from, int dict_num)
        {
            Dictionary<int, DictValue> ret_dict = new Dictionary<int, DictValue>();
            if (!(bool)is_rand_chck_box.IsChecked)
            {
                while (ret_dict.Count < dict_len_h)
                {
                    if (dict_num == 2)
                    {
                        string dbg = big_dict[start_from].line[1][0];
                        if (dbg != "")
                            ret_dict.Add(start_from, big_dict[start_from]);
                    }
                    else ret_dict.Add(start_from, big_dict[start_from]);
                    start_from++;
                }
            }
            else
            {
                int i;
                List<int> added_indexes = new List<int>();
                Random r = new Random();
                while (ret_dict.Count < dict_len_h)
                {
                    do
                    {
                        i = r.Next(1, big_dict.Count);
                    }
                    while (added_indexes.Contains(i));
                    added_indexes.Add(i);
                    if (dict_num == 2)
                    {
                        string dbg = big_dict[i].line[1][0];
                        if (dbg != "")
                            ret_dict.Add(i, big_dict[i]);
                    }
                    else ret_dict.Add(i, big_dict[i]);
                }
            }
            return ret_dict;
        }
    }
}
