using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace JapanWordsQuiz
{
    static class Utils
    {
        public static bool IsNumberic(this Key key, int limit)
        {
            int[] res = new int[10];
            if (limit > -1)
                if (key != Key.D0 && key != Key.NumPad0) res[0]++;
            if (limit > 0)
                if (key != Key.D1 && key != Key.NumPad1) res[1]++;
            if (limit > 1)
                if (key != Key.D2 && key != Key.NumPad2) res[2]++;
            if (limit > 2)
                if (key != Key.D3 && key != Key.NumPad3) res[3]++;
            if (limit > 3)
                if (key != Key.D4 && key != Key.NumPad4) res[4]++;
            if (limit > 4)
                if (key != Key.D5 && key != Key.NumPad5) res[5]++;
            if (limit > 5)
                if (key != Key.D6 && key != Key.NumPad6) res[6]++;
            if (limit > 6)
                if (key != Key.D7 && key != Key.NumPad7) res[7]++;
            if (limit > 7)
                if (key != Key.D8 && key != Key.NumPad8) res[8]++;
            if (limit > 8)
                if (key != Key.D9 && key != Key.NumPad9) res[9]++;
            return (res.SumCount() == limit + 1) ? true : false;
        }

        public static int SumCount(this int[] arr)
        {
            int result = 0;
            foreach (int elem in arr)
                result += elem;
            return result;
        }
    }
}
