using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WPF_CrazyBit_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ESC누르면 서버 종료");
            Data.Sock = new s_sock(7500);
            

            Control.update_start();

            ConsoleKeyInfo cki;
            do
            {
                cki = Console.ReadKey(true);
                for(int i=0;i<Data._Game.Map_data.Count;i++)
                {
                    Console.Write(Data._Game.Map_data[i]);
                    if (i % 15 == 0)
                        Console.WriteLine();
                }
            }while (cki.Key != ConsoleKey.Escape);
            Console.WriteLine("서버 종료");
        }
    }
}
