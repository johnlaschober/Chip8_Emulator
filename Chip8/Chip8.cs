﻿using System;
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

            pc = 0x0200;     // Program counter starts at 0x200
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
            else // For testing purposes obviously
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
            Console.WriteLine(opcode & 0x00F0);
            Console.WriteLine((opcode & 0x00F0)>>4);
            // Decode Opcode
            switch (opcode & 0xF000) // Here, only check first bit of opcode using bitwise AND
            {
                case 0x0000:
                    // first bit 0 has 3 opcodes (0x0NNN, 0x00E0, 0x00EE)
                    switch (opcode & 0x000F)
                    {
                        // 0x0NNN is a weird case. NNN is some passed in values, so it's not really 'static'
                        // 0x0NNN should come first
                        // 0x0NNN is apparently not implemented? For older machines or something?

                        case 0x0000: // 0x00E0: Clears the screen 
                            Console.WriteLine("Clear screen");
                            break;
                        case 0x000E: // 0x00EE: Returns from a subroutine
                            Console.WriteLine("Return from subroutine");
                            break; 
                        default:
                            Console.WriteLine("Error: No '0x0' case caught opcode!");
                            break;
                    }
                    break;
                case 0x1000: // 0x1NNN: Jump to address NNN
                    pc = (ushort)(opcode & 0x0FFF); // Maybe, needs testing
                    break;
                case 0x2000: // 0x2NNN: Calls subroutine at NNN
                    break;
                case 0x3000: // 0x3XNN: Skip next instruction if VX == NN (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF)){
                        pc += 2;
                    }
                    break;
                case 0x4000: // 0x4XNN: Skip next instruction if VX != KK (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF)){
                        pc += 2;
                    }
                    break;
                case 0x5000: // 0x5XY0: Skip next instruction if VX == VY (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] == V[(opcode & 0x00F0) >> 4]){
                        pc += 2;
                    }
                    break;
                case 0x6000: // 0x6XNN : Put NN into VX
                    break;
                case 0x7000: // 0x7XNN : Adds NN to VX (carry flag not changed according to Wikipedia)
                    break;
                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // 0x8XY0: Sets VX to the value VY
                            break;
                        case 0x0001: // 0x8XY1: Sets VX to VX or VY (Bitwise OR)
                            break;
                        case 0x0002: // 0x8XY2: Sets VX to VX and VY (Bitwise AND)
                            break;
                        case 0x0003: // 0x8XY3: Sets VX to VX xor VY
                            break;
                        case 0x0004: // 0x8XY4: Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't
                            break;
                        case 0x0005: // 0x8XY5: VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't
                            break;
                        case 0x0006: // 0x8XY6: Stores the least significant bit of VX in VF and then shifts VX to the left by 1
                            break;
                        case 0x0007: // 0x8XY7: Sets VX to VY minus VX. VF is set to 0 when there's a borrow nad 1 when there isn't
                            break;
                        case 0x000E: // 0x8XYE: Stores the most significant bit of VX in VF and then shifts VX to the left by 1
                            break;
                        default:
                            Console.WriteLine("Error: No '0x8' case caught opcode!");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine(opcode.ToString(), " Error: No 'main' case caught opcode!");
                    break;
            }

            // Update timers
        }
    }
}
