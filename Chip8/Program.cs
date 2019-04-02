using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chip8;

namespace Chip8
{
    class Program
    {
        static void Main(string[] args)
        {
            Chip8 chippy = new Chip8();
            chippy.Initailize();
            chippy.EmulateCycle();
            Console.ReadLine();
        }
    }
}
