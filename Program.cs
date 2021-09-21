using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FInalKursovaCsharp
{
    [Serializable]
    class Word_Book
    {
        public Dictionary<string, string[]> Book { get; set; }
        public string Name_Dicthionary { get; set; }
        public Word_Book(string _name)
        {
            Book = new Dictionary<string, string[]>();
            Name_Dicthionary = _name;
        }
        public Word_Book()
        {
            Book = new Dictionary<string, string[]>();
            Name_Dicthionary = default;
        }
        public void Add(string _word, params string[] _trans)
        {
            Book.Add(_word, _trans);
        }
        public bool Search_Word(string _search_word)
        {
            return Book.ContainsKey(_search_word);
        }
        public bool Search_Translete(string _search_word, string _search_translete)
        {
            string[] Tmp = Book[_search_word];
            for (int i = 0; i < Book[_search_word].Length; i++)
            {
                if (Tmp[i] == _search_translete)
                    return true;
            }
            return false;
        }
        public override string ToString()
        {
            string Show = default;
            foreach (KeyValuePair<string, string[]> _Show_Book in Book)
            {
                Show += _Show_Book.Key + "\t\t- ";
                for (int i = 0; i < _Show_Book.Value.Length; i++)
                    Show += (" <" + _Show_Book.Value[i] + "> ");
                Show += "\n";
            } 
            return Show;
        }
        public void Change_World(string _search_word, string _new_word)
        {
            string[] Tmp = Book[_search_word];
            Book.Remove(_search_word);
            Book.Add(_new_word, Tmp);
        }
        public void Change_Translete(string _search_word, params string[] _new_translete)
        {
            Book[_search_word] = _new_translete;
        }
        public void Delete_World(string _search_word)
        {
            Book.Remove(_search_word);
        }
        public void Delete_Translete(string _search_word, string _delete_translete_world)
        {
            if (Book[_search_word].Length == 1)
                Console.WriteLine("В даного слова тільки один варіант переводу");
            else
            {
                string[] Tmp = Book[_search_word];
                string[] New_Tmp = new string[Tmp.Length - 1];
                for (int i = 0; i < Book[_search_word].Length; i++)
                {
                    if (Tmp[i] != _delete_translete_world)
                        New_Tmp[i] = Tmp[i];
                }
                Book[_search_word] = New_Tmp;
            }
        }
    }

    [Serializable]
    class Menu
    {
        private List<Word_Book> List_Book;
        private Word_Book Page;
        private int Index;
        private int Keyboard_Iter;
        public Menu()
        {
            Page = new Word_Book();
            List_Book = new List<Word_Book>();
            Index = 0;
            ReadFile();
        }
        private void Rewrite()
        {
            List_Book.RemoveAt(Index - 1);
            List_Book.Insert(Index - 1, Page);
            WriteFile();
        }
        private void Couter()
        {
            BinaryFormatter New_BinFormat = new BinaryFormatter();
            using (Stream fStream = File.Create("C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\SaveCount.bin"))
                New_BinFormat.Serialize(fStream, List_Book.Count);
        }
        private int StartCounter()
        {
            BinaryFormatter New_BinFormat = new BinaryFormatter();
            int Num = default;
            string Path = "C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\SaveCount.bin";
            FileInfo File_Inf = new FileInfo(Path);
            if (File_Inf.Exists)
            {
                using (Stream fStream = File.OpenRead("C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\SaveCount.bin"))
                    Num = (int)New_BinFormat.Deserialize(fStream);
            }
            return Num;
        }
        private void WriteFile()
        {
            Couter();
            BinaryFormatter New_BinFormat = new BinaryFormatter();
            using (Stream fStream = File.Create("C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\Save.bin"))
            {
                for (int i = 0; i < List_Book.Count; i++)
                {
                    New_BinFormat.Serialize(fStream, List_Book.ElementAt(i));
                }
            }
        }
        private void ReadFile()
        {
            int size = StartCounter();
            Word_Book Page_Tmp = new Word_Book();
            BinaryFormatter New_BinFormat = new BinaryFormatter();
            using (Stream fStream = File.OpenRead("C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\Save.bin"))
            {
                for (int i = 0; i < size; i++)
                {
                    Page_Tmp = (Word_Book)New_BinFormat.Deserialize(fStream);
                    List_Book.Add(Page_Tmp);
                }
            }
        }
        public string[] Read(string _path)
        {
            string[] Push_Str = new string[19];
            using (StreamReader sr = new StreamReader(_path, System.Text.Encoding.Default))
            {
                string Tmp = default;
                int i = 0;
                while ((Tmp = sr.ReadLine()) != null)
                    Push_Str[i++] += Tmp;
            }
            return Push_Str;
        }
        public void Management_Keyboard_Menu(string _path)
        {
            Console.Clear();
            string[] Helo_Open = Read(_path);
            char[] Stroka = new char[256];
            for (int i = 0; i < Helo_Open.Length; i++)
            {
                Stroka = Helo_Open[i].ToCharArray();
                if (i == Keyboard_Iter)
                    Stroka[5] = '>';
                else
                    Stroka[5] = ' ';
                if (i == 17)
                {
                    char[] Name = Page.Name_Dicthionary.ToCharArray();
                    for (int j = 28, l = 0; j < Name.Length + 28; j++, l++)
                    {
                        Stroka[j] = Name[l];
                    }
                }
                Helo_Open[i] = new string(Stroka);
                Console.WriteLine(Helo_Open[i]);
            }
        }

        public void Select_Dictionary()
        {
            do
            {
                Console.Clear();
                Show_Dictionary();
                Console.Write("\n\nEnter number Dicthionary:\t\t");
                Index = int.Parse(Console.ReadLine());
            } while (List_Book.Count < Index && Index > 0);
            Page = List_Book.ElementAt(Index - 1);
        }
        public void Add_Dictionary()
        {
            Console.Write("\n\n\n\n\n\n\t\t\t\t\tENTER A DICTIONARY NAME\n\n\t\t\t\t\t");
            string Name = Console.ReadLine();
            Word_Book Word = new Word_Book(Name);
            List_Book.Add(Word);
            WriteFile();
        }
        public void Show_Dictionary()
        {
            int iter = 1;
            Console.WriteLine("\n\n\n\t\t\t");
            foreach (var item in List_Book)
            {
                Console.WriteLine("\n\t\t" + iter++ + "].\t<" + item.Name_Dicthionary+">");
            }
        }
        public void Delete_Dictionary()
        {
            if (List_Book.Count == 1)
                return;
            else
            {
                List_Book.RemoveAt(List_Book.LastIndexOf(Page));
                Select_Dictionary();
                WriteFile();
            }
        }
        public void Keyboard_Start_Menu()
        {
            string path = "C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\MenuStart.txt";
            bool exit = true;
            if (List_Book.Count > 0)
                Select_Dictionary();
            else
            {
                Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                Console.ReadLine();
            }
            Keyboard_Iter = 11;
            Management_Keyboard_Menu(path);
            int Max_Pointer = 17, Min_Pointer = 11;
            while (exit)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (Keyboard_Iter > Min_Pointer)
                            Keyboard_Iter--;
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (Keyboard_Iter < Max_Pointer)
                            Keyboard_Iter++;
                    }
                    Management_Keyboard_Menu(path);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (Keyboard_Iter >= Min_Pointer && Keyboard_Iter <= Max_Pointer)
                        {
                            switch (Keyboard_Iter)
                            {
                                case 11:
                                    Console.Clear();
                                    if (List_Book.Count > 0)
                                        Select_Dictionary();
                                    else
                                    {
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                        Console.ReadLine();
                                    }
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 12:
                                    Console.Clear();
                                    Add_Dictionary();
                                    Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tDICTIONARY ADDED");
                                    Console.ReadLine();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 13:
                                    Console.Clear();
                                    if (List_Book.Count > 0)
                                        Show_Dictionary();
                                    else
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                    Console.ReadLine();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 14:
                                    Console.Clear();
                                    if (List_Book.Count > 0)
                                        Delete_Dictionary();
                                    else
                                    {
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                        Console.ReadLine();
                                    }
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 15:
                                    Console.Clear();
                                    Dicthionary_Manegement_Menu();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 17:
                                    exit = false;
                                    Console.Write("\n\t\t\t\t");
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void Add_Words_And_Translete()
        {
            string word = default, tmp = default;
            string[] translete = new string[10], push = default;
            Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO ADD TO THE DICTIONARY\n\n\n\n\t\t\t");
            word = Console.ReadLine();
            Console.Clear();
            Console.Write("\n\n\n\t\t\tENTER TRANSLETE YOU WANT TO ADD TO THE DICTIONARY\n\t\t\tTO Exit ENTER [0]\n\n");
            for (int i = 0; i < translete.Length; i++)
            {
                Console.Write("\t\t\t" + (i + 1) + "].\t");
                tmp = Console.ReadLine();
                if (tmp == "0")
                {
                    push = new string[i];
                    i = translete.Length;
                }
                else
                    translete[i] = tmp;
            }
            for (int i = 0; i < push.Length; i++)
                push[i] = translete[i];
            Page.Add(word, push);
            Rewrite();
        }
        public void Delete_Word()
        {
            string tmp = default;
            do
            {
                Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO DELETE THE DICTIONARY\n\n\n\n\t\t\t");
                tmp = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Word(tmp));
            Page.Delete_World(tmp);
            Rewrite();
        }
        public void Delete_Translete()
        {
            string tmp = default, delete = default;
            do
            {
                Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO DELETE TRANSLETE IN THIS DICTIONARY\n\n\n\n\t\t\t");
                tmp = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Word(tmp));
            string str = default;
            string[] temp = Page.Book[tmp];
            str = string.Join(", ", temp);
            do
            {
                Console.Write("\n\n\n\t\t\tENTER TRANSLETE YOU WANT TO DELETE IN THIS DICTIONARY\n\t\t\t" + str + "\n\n\n\n\t\t\t");
                delete = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Translete(tmp, delete));
            Page.Delete_Translete(tmp,delete);
            Rewrite();
        }
        public void Show_Worlds()
        {
            Console.WriteLine(Page.ToString());
        }
        public void Change_Word()
        {
            string tmp = default;
            do
            {
                Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO SEARCH OF DICTIONARY\n\n\n\n\t\t\t");
                tmp = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Word(tmp));
            Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO CHANGE OF DICTIONARY\n\n\n\n\t\t\t");
            string New_Word = Console.ReadLine();
            Page.Change_World(tmp, New_Word);
            Rewrite();
        }
        public void Change_Translete()
        {
            string Tmp = default, Change = default, New_Word = default;
            do
            {
                Console.Write("\n\n\n\t\t\tENTER WORD YOU WANT TO CHANGE TRANSLETE IN THIS DICTIONARY\n\n\n\n\t\t\t");
                Tmp = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Word(Tmp));
            string Str = default;
            string[] Temp = Page.Book[Tmp];
            Str = string.Join(", ", Temp);
            do
            {
                Console.Write("\n\n\n\t\t\tENTER TRANSLETE YOU WANT TO CHANGE IN THIS DICTIONARY\n\t\t\t" + Str + "\n\n\n\n\t\t\t");
                Change = Console.ReadLine();
                Console.Clear();
            } while (!Page.Search_Translete(Tmp, Change));
            Console.Write("\n\n\n\t\t\tENTER NEW TRANSLETE YOU WANT TO CHANGE IN THIS DICTIONARY" + "\n\n\n\n\t\t\t");
            New_Word = Console.ReadLine();
            for (int i = 0; i < Temp.Length; i++)
            {
                if (Temp[i] == Change)
                    Temp[i] = New_Word;
            }
            Page.Change_Translete(Tmp, Temp);
            Rewrite();
        }
        public void Dicthionary_Manegement_Menu()
        {
            string path = "C:\\Users\\user\\source\\repos\\FInalKursovaCsharp\\MenuMenegement.txt";
            bool exit = true;
            Keyboard_Iter = 11;
            Management_Keyboard_Menu(path);
            int Max_Pointer = 17, Min_Pointer = 11;
            while (exit)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (Keyboard_Iter > Min_Pointer)
                            Keyboard_Iter--;
                    }
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (Keyboard_Iter < Max_Pointer)
                            Keyboard_Iter++;
                    }
                    
                    Management_Keyboard_Menu(path);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (Keyboard_Iter >= Min_Pointer && Keyboard_Iter <= Max_Pointer)
                        {
                            switch (Keyboard_Iter)
                            {
                                case 11:
                                    Console.Clear();
                                    Add_Words_And_Translete();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 12:
                                    Console.Clear();
                                    if (Page.Book.Count > 0)
                                        Delete_Word();
                                    else
                                    {
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                        Console.ReadLine();
                                    }
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 13:
                                    Console.Clear();
                                    Delete_Translete();
                                    Console.ReadLine();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 14:
                                    Console.Clear();
                                    if (Page.Book.Count > 0)
                                        Change_Word();
                                    else
                                    {
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                        Console.ReadLine();
                                    }
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 15:
                                    Console.Clear();
                                    if (Page.Book.Count > 0)
                                        Change_Translete();
                                    else
                                    {
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                        Console.ReadLine();
                                    }
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 16:
                                    Console.Clear();
                                    if (Page.Book.Count > 0)
                                        Show_Worlds();
                                    else
                                        Console.WriteLine("\n\n\n\n\n\n\t\t\t\t\tNO DICTIONARIES AVAILABLE");
                                    Console.ReadLine();
                                    Management_Keyboard_Menu(path);
                                    break;
                                case 17:
                                    exit = false;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.Keyboard_Start_Menu();
        }
    }
}
