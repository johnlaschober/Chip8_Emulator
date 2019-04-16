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
            Brush blackBrush = Brushes.Black;
            Graphics g = chipForm.CreateGraphics();

            g.Clear(Color.Black);
            chippy.Initailize();


            for (; ;)
            {
                chippy.EmulateCycle();

                if (chippy.drawFlag)
                {
                    DrawGraphics(chippy.gfx, g, whiteBrush, blackBrush);
                }

                chippy.SetKeys();
                Console.ReadLine();
            }
        }

        static void DrawGraphics(byte[] gfx, Graphics g, Brush wb, Brush bb) {
            // draw graphics here using a form
            // 64 x 32

            int x = 0, y = 0;
            for (int i = 0; i < gfx.Length; i++)
            {
                x = i % 64;
                y = i / 64;
                if (gfx[i] == 1)
                {
                    g.FillRectangle(wb, x * 10, y * 10, 1 * 10, 1 * 10);
                }
                else
                {
                    g.FillRectangle(bb, x * 10, y * 10, 1 * 10, 1 * 10);
                }

            }

        }
    }
}
