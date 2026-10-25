using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DIRECTORY directory = new DIRECTORY();
            directory.FileWrite();
            System system = new System();
            system.Run();
        }
    }
}