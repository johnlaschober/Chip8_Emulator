using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chip8;

using System.Windows.Forms;
using System.Drawing;

namespace Chip8
{
    class Program
    {
        static void Main(string[] args)
        {
            Chip8 chippy = new Chip8();
            Form chipForm = new Form();
            chipForm.Width = 640;
            chipForm.Height = 320;
            chipForm.Show();
            Brush whiteBrush = Brushes.White;
            Graphics g = chipForm.CreateGraphics();
            g.Clear(Color.Black);

            

            chippy.Initailize();
            chippy.EmulateCycle();

            if (chippy.drawFlag) {
                DrawGraphics(chippy.gfx, g, whiteBrush);
            }

            chippy.SetKeys();

            Console.ReadLine(); // used for debug
            // right now we are emulating 1 frame only
        }

        static void DrawGraphics(byte[] gfx, Graphics g, Brush b) {
            // draw graphics here using a form
            // 64 x 32

            int x = 0, y = 0;
            for (int i = 0; i < gfx.Length; i++)
            {
                x = i % 64;
                y = i / 64;
                g.FillRectangle(b, x*10, y*10, 1*10, 1*10);
            }

        }
    }
}
