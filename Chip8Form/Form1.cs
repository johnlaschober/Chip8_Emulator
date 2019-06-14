using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chip8Form
{

    public partial class Form1 : Form
    {
        static Chip8 chip = new Chip8();
        public Form1()
        {
            InitializeComponent();
            chip.Initailize();
            this.Width = 640;
            this.Height = 320;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            Brush whiteBrush = new SolidBrush(Color.White);
            Brush blackBrush = new SolidBrush(Color.Black);

            chip.EmulateCycle();

            if (chip.drawFlag)
            {
                int x = 0, y = 0;
                for (int i = 0; i < chip.gfx.Length; i++)
                {
                    x = i % 64;
                    y = i / 64;
                    if (chip.gfx[i] == 1)
                    {
                        g.FillRectangle(whiteBrush, x * 10, y * 10, 1 * 10, 1 * 10);
                    }
                    else
                    {
                        g.FillRectangle(blackBrush, x * 10, y * 10, 1 * 10, 1 * 10);
                    }
                }
            }
            this.Invalidate();
        }
    }
}
