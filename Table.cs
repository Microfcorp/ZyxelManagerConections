using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zyxel
{
    class Table
    {
        public const uint Padding = 3;
        public const uint SubPadding = 2;
        public const string PaddingLine = "  ";

        public string Data
        {
            get;
            set;
        }

        private List<List<string>> data = new List<List<string>>();

        private string[] title;

        /// <summary>
        /// Добавить заголовочную строку
        /// </summary>
        public void AddTitle(params string[] data)
        {
            AddLine(data);
            title = data;
        }
        /// <summary>
        /// Добавить пустую строку
        /// </summary>
        public void AddLine()
        {
            data.Add(new List<string>());
        }
        /// <summary>
        /// Добавить строку из списка
        /// </summary>
        /// <param name="line">Список строки</param>
        public void AddLine(List<string> line)
        {
            data.Add(line);
        }
        /// <summary>
        /// Добавляет строку из массива
        /// </summary>
        /// <param name="data">Массив строк</param>
        public void AddLine(params string[] data)
        {
            var tmp = new List<string>();
            foreach (var item in data)
                tmp.Add(item);
            AddLine(tmp);
        }

        /// <summary>
        /// Очищает таблицу, но оставляет заглавие
        /// </summary>
        /// <param name="data">Массив строк</param>
        public void Clear()
        {
            data.Clear();
            AddLine(title);
        }

        /// <summary>
        /// Печать таблицы
        /// </summary>
        public void Print(string[] WP)
        {
            int[] bufer = new int[data[0].Count];
            bufer.Select(s => s = 0);

            for (int i = 0; i < data.Count; i++)
                for (int a = 0; a < data[i].Count; a++)
                    if (data[i][a].Length > bufer[a])
                        bufer[a] = data[i][a].Length;

            Console.Clear();
            Data = "";

            Console.BufferHeight = (data.Count + 5) <= Console.WindowHeight ? Console.WindowHeight + 1 : (data.Count + 5);
            //Console.BufferWidth = data.Max(s => s.Max(a => a.Length)) * bufer.Length;

            int width = Console.WindowWidth;

            for (int i = 0; i < width - Padding; i++) if (i < (int)Padding) Console.Write(' '); else Console.Write('-');
            Console.WriteLine();
            Console.WriteLine();

            for (int i = 0; i < data.Count; i++)
            {
                for (int p = 0; p < Padding + SubPadding; p++) Console.Write(' ');

                for (int ia = 0; ia < data[i].Count; ia++)
                {
                    if (WP.Contains(data[i][3].Split('/')[1]))
                        Console.BackgroundColor = ConsoleColor.Red;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    Console.Write(data[i][ia]);
                    Data += data[i][ia];
                   
                    for (int q = 0; q < (bufer[ia] - data[i][ia].Length) + SubPadding; q++)
                    {                       
                        Console.Write(' ');
                        Data += " ";
                    };

                    Console.BackgroundColor = ConsoleColor.Black;
                }
                Console.WriteLine();
                Data += Environment.NewLine;
            }

            for (int i = 0; i < width - Padding; i++) if (i < (int)Padding) Console.Write(' '); else Console.Write('-');
        }
    }
}
