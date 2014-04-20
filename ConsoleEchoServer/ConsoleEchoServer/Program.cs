using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 3000;
            Console.WriteLine("Now listening on port " + port);
            new Server(port);
        }


    }
}
