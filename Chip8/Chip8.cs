using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // Allows for File library

namespace Chip8
{
    class Chip8
    {
        ushort opcode; // unsigned 16-bit integer 0 to 65,535
                       // Chip 8 has 35 opcodes, each 2 bytes long

        char[] memory = new char[4096]; // 4 kilobytes memory, supposed to be unsigned?

        char[] V = new char[16]; // Chip 8 has 15 8-bit general purpose registers named V0, V1, up to VE
                                 // 16th register is the carry flag

        ushort I; // Index register
        ushort pc; // Program counter

        char[,] gfx = new char[64,32];

        ushort[] stack = new ushort[16]; // Anytime you perform a jump or call a subroutine, store the pc in the stack before proceeding
        ushort sp; // stack pointer

        public void Initailize()
        {
            // Iniitalize registers and memory once.

            pc = 0x200;     // Program counter starts at 0x200
            opcode = 0;     // Reset current opcode
            I = 0;          // Reset index register
            sp = 0;         // Reset stack pointer

            // Rather than new, we might want to clear in the future?
            gfx = new char[64, 32]; // Clear display
            stack = new ushort[16]; // Clear stack
            V = new char[16]; // Clear registers V0-Vf
            memory = new char[4096]; // Clear memory

            bool readFromFile = false;
            if (readFromFile)
            {
                byte[] fileBytes = File.ReadAllBytes("E:\\Downloads\\Cave.ch8");
                // 00 E0 64     -> How it looks in memory
                // 0  112  100  -> How it looks as an opcode

                for (int i = 0; i < fileBytes.Length; i++) // Whether this is loading in the right bytes or not is anyone's guess
                {
                    string test = string.Format("{0:x}", fileBytes[i]); // Right
                    memory[i + 512] = Convert.ToChar(Convert.ToUInt32(test, 16)); // Iffy but sure
                }
            }
            else
            {
                memory[512] = Convert.ToChar(Convert.ToUInt32("00", 16)); // Iffy but sure
                memory[513] = Convert.ToChar(Convert.ToUInt32("E0", 16)); // Iffy but sure
            }

            Console.WriteLine("Chip8 initialized");
        }

        public void EmulateCycle()
        {
            // Fetch Opcode
            opcode = (ushort)(memory[pc] << 8 | memory[pc + 1]); // working
            Console.WriteLine(memory[512]);
            Console.WriteLine(memory[513]);
            Console.WriteLine(opcode);
            // Decode Opcode
            switch (opcode & 0xF000) // Here, only check first bit of opcode using bitwise AND
            {
                case 0x0000:
                    // first bit 0 has 3 opcodes (0x0NNN, 0x00E0, 0x00EE)
                    switch (opcode & 0x000F)
                    {
                        // 0x0NNN is a weird case. NNN is some passed in values, so it's not really 'static'
                        // 0x0NNN should come first
                        case 0x0000: // 0x00E0: Clears the screen 
                            Console.WriteLine("Clear screen");
                            break;
                        case 0x000E: // 0x00EE: Returns from a subroutine
                            Console.WriteLine("Return from subroutine");
                            break; 
                        default:
                            Console.WriteLine("Error: No '0x000' case caught opcode!");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Error: No 'main' case caught opcode!");
                    break;
            }

            // Update timers
        }
    }
}
