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

            if (chippy.drawFlag) {
                DrawGraphics(chippy.gfx);
            }

            // Call setkeys, find a way to read multiple keys pressed
            chippy.SetKeys();

            Console.ReadLine(); // used for debug
            // right now we are emulating 1 frame only
        }

        static void DrawGraphics(byte[,] gfx) {
            // draw graphics here using a form
        }
    }
}
