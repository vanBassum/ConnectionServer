using STDLib.Commands;
using System;
using System.Linq;

namespace ConnectionServer
{
    class Program
    {



        static void Main(string[] args)
        {
            ConnectionServer server = new ConnectionServer();

            BaseCommand.Do();


            
        }

        
    }

}
