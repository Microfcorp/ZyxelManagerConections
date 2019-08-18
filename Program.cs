using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Zyxel.ZyxelAPI;

namespace Zyxel
{
    class Program
    {
        public enum TypeR : byte
        {
            AllMy,
            NotMy,
            Input,
            Output,
            All,
        }

        public static Table t = new Table();
        
        static void Main(string[] args)
        {
            Parser p = new Parser(args);
            if (p.FindParams("--help"))
            {
                Console.WriteLine("Zyxel. Менеджер текущий соеденений");
                Console.WriteLine("Ключи запуска:");
                Console.WriteLine("-P Показывать ли исходящий порт (не обязательно)");
                Console.WriteLine("-d Адрес до роутера");
                Console.WriteLine("-l Логин");
                Console.WriteLine("-p Пароль");
                Console.WriteLine("-t Время обновления (в миллисекундах)");
                Console.WriteLine("-i Локальный IP адрес (не обязательно)");               
                Console.WriteLine("-w Опасные порты, через запятую (не обязательно)");
                Console.WriteLine("Комманды в работе:");
                Console.WriteLine("E Выход");
                Console.WriteLine("I Просмотр входящих подключений");
                Console.WriteLine("A Просмотр всех подключений");
                Console.WriteLine("P Пауза обновлений");
                Console.WriteLine("S Сохранить лог");
            }
            else
            {
                bool Correct = false;
                bool Running = true;
                bool Pause =! true;
                bool SP = p.FindParams("-P");

                string[] WP = p.FindParamsAndArgs("-w", out Correct).Split(',');

                TypeR type = TypeR.All;

                ZyxelAPI.ZyxelAPI z = new ZyxelAPI.ZyxelAPI(p.FindParamsAndArgs("-d", out Correct), p.FindParamsAndArgs("-l", out Correct), p.FindParamsAndArgs("-p", out Correct));

                t.AddTitle("#", "Источник", "Назначение", "Сервис/порт", "Размер");
                Console.WriteLine("Соеденение с {0}. Логин {1} Пароль {2}.", z.URL, z.Login, z.Password);                                

                while (!Console.TreatControlCAsInput & Running)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey().Key;

                        if (key == ConsoleKey.E)
                            Running = false;
                        else if (key == ConsoleKey.I)
                            type = TypeR.Input;
                        else if (key == ConsoleKey.O)
                            type = TypeR.Output;
                        else if (key == ConsoleKey.A)
                            type = TypeR.All;
                        else if (key == ConsoleKey.P)
                            Pause = !Pause;
                        else if (key == ConsoleKey.S)
                            System.IO.File.WriteAllText(Environment.CurrentDirectory + "//" + DateTime.Now.ToString().Replace(':', '-') + ".txt", t.Data);
                        Console.Beep(637, 1000);
                    }

                    if (!Pause)
                    {
                        XmlNode xml = z.GetConnection();
                        t.Clear();
                        for (int i = 0; i < xml.ChildNodes.Count; i++)
                        {                            
                            if (type == TypeR.All)
                                t.AddLine(i.ToString(),
                                    xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText,
                                    xml.ChildNodes.Item(i).SelectSingleNode("dst").InnerText,
                                    xml.ChildNodes.Item(i).SelectSingleNode("protocol").InnerText + "/" + (SP ? xml.ChildNodes.Item(i).SelectSingleNode("sport").InnerText + "-" : "") + xml.ChildNodes.Item(i).SelectSingleNode("dport").InnerText,
                                    FileSize.Calculate(xml.ChildNodes.Item(i).SelectSingleNode("bytes").InnerText));
                            else if (type == TypeR.Input)
                                if (!IPManager.IsLANIP(xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText) & xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText != p.FindParamsAndArgs("-i", out Correct))
                                    t.AddLine(i.ToString(),
                                        xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText,
                                        xml.ChildNodes.Item(i).SelectSingleNode("dst").InnerText,
                                        xml.ChildNodes.Item(i).SelectSingleNode("protocol").InnerText + "/" + (SP ? xml.ChildNodes.Item(i).SelectSingleNode("sport").InnerText + "-" : "") + xml.ChildNodes.Item(i).SelectSingleNode("dport").InnerText,
                                        FileSize.Calculate(xml.ChildNodes.Item(i).SelectSingleNode("bytes").InnerText));
                            else if (type == TypeR.Output)
                                if (IPManager.IsLANIP(xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText) || xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText == p.FindParamsAndArgs("-i", out Correct))
                                    t.AddLine(i.ToString(),
                                        xml.ChildNodes.Item(i).SelectSingleNode("src").InnerText,
                                        xml.ChildNodes.Item(i).SelectSingleNode("dst").InnerText,
                                        xml.ChildNodes.Item(i).SelectSingleNode("protocol").InnerText + "/" + (SP ? xml.ChildNodes.Item(i).SelectSingleNode("sport").InnerText + "-" : "") + xml.ChildNodes.Item(i).SelectSingleNode("dport").InnerText,
                                        FileSize.Calculate(xml.ChildNodes.Item(i).SelectSingleNode("bytes").InnerText));
                        }
                        t.Print(WP);
                    }                                       

                    if (!Pause)
                    {
                        System.Threading.Thread.Sleep(int.Parse(p.FindParamsAndArgs("-t", out Correct)));
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                    }
                }
            }
        }
    }
}
