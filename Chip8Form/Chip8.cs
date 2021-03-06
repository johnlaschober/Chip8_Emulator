﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; // Allows for File library

namespace Chip8Form
{
    class Chip8
    {
        ushort opcode; // unsigned 16-bit integer 0 to 65,535
                       // Chip 8 has 35 opcodes, each 2 bytes long

        byte[] memory = new byte[4096]; // 4 kilobytes memory

        byte[] V = new byte[16]; // Chip 8 has 15 8-bit general purpose registers named V0, V1, up to VE
                                 // 16th register is the carry flag

        ushort I; // Index register
        ushort pc; // Program counter

        public byte[] gfx = new byte[64 * 32];

        ushort[] stack = new ushort[16]; // Anytime you perform a jump or call a subroutine, store the pc in the stack before proceeding
        ushort sp; // stack pointer

        byte delay_timer;
        byte sound_timer;

        byte[] key = new byte[16]; // Stores states of key presses

        byte[] chip8_fontset = {
          0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
          0x20, 0x60, 0x20, 0x20, 0x70, // 1
          0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
          0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
          0x90, 0x90, 0xF0, 0x10, 0x10, // 4
          0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
          0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
          0xF0, 0x10, 0x20, 0x40, 0x40, // 7
          0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
          0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
          0xF0, 0x90, 0xF0, 0x90, 0x90, // A
          0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
          0xF0, 0x80, 0x80, 0x80, 0xF0, // C
          0xE0, 0x90, 0x90, 0x90, 0xE0, // D
          0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
          0xF0, 0x80, 0xF0, 0x80, 0x80  // F
        };

        public Boolean drawFlag = false;

        Random rnd;

        public void Initailize()
        {
            // Iniitalize registers and memory once.

            Random rnd = new Random();

            pc = 0x0200;     // Program counter starts at 0x200
            opcode = 0;     // Reset current opcode
            I = 0;          // Reset index register
            sp = 0;         // Reset stack pointer

            Array.Clear(gfx, 0, gfx.Length); // Clear display
            Array.Clear(stack, 0, stack.Length);  // Clear stack
            Array.Clear(V, 0, V.Length); // Clear registers V0-Vf
            Array.Clear(memory, 0, memory.Length); // Clear memory

            for (int i = 0; i < 80; ++i)
            {
                memory[i] = chip8_fontset[i];
            }
            bool readFromFile = true;
            if (readFromFile)
            {
                byte[] fileBytes = File.ReadAllBytes("E:\\Downloads\\Breakout.ch8");
                // 00 E0 64     -> How it looks in memory
                // 0  112  100  -> How it looks as an opcode

                for (int i = 0; i < fileBytes.Length; i++) // Whether this is loading in the right bytes or not is anyone's guess
                {
                    string test = string.Format("{0:x}", fileBytes[i]); // Right
                    memory[i + 512] = Convert.ToByte(Convert.ToUInt32(test, 16)); // Iffy but sure
                }
            }
            else // For testing purposes obviously
            {
                memory[512] = Convert.ToByte(Convert.ToUInt32("00", 16)); // Iffy but sure
                memory[513] = Convert.ToByte(Convert.ToUInt32("E0", 16)); // Iffy but sure
            }

            Console.WriteLine("Chip8 initialized");
        }

        public void EmulateCycle()
        {
            // Fetch Opcode
            opcode = (ushort)(memory[pc] << 8 | memory[pc + 1]); // working
            Console.WriteLine(opcode.ToString("X"));
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
                            Array.Clear(gfx, 0, gfx.Length);
                            pc += 2;
                            Console.WriteLine("Clear screen");
                            break;
                        case 0x000E: // 0x00EE: Returns from a subroutine
                            pc = stack[sp];
                            --sp;
                            Console.WriteLine("Return from subroutine");
                            break;
                        default:
                            Console.WriteLine("Error: No '0x0' case caught opcode!");
                            pc += 2; // Move to next opcode?
                            // This might actually catch the 0x0NNN case
                            break;
                    }
                    break;
                case 0x1000: // 0x1NNN: Jump to address NNN
                    pc = (ushort)(opcode & 0x0FFF); // Maybe, needs testing
                    break;
                case 0x2000: // 0x2NNN: Calls subroutine at NNN
                    stack[sp] = pc;
                    ++sp;
                    pc = (ushort)(opcode & 0x0FFF);
                    break;
                case 0x3000: // 0x3XNN: Skip next instruction if VX == NN (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] == (opcode & 0x00FF))
                    {
                        pc += 2;
                    }
                    pc += 2;
                    break;
                case 0x4000: // 0x4XNN: Skip next instruction if VX != KK (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] != (opcode & 0x00FF))
                    {
                        pc += 2;
                    }
                    pc += 2;
                    break;
                case 0x5000: // 0x5XY0: Skip next instruction if VX == VY (increment PC by 2)
                    if (V[(opcode & 0x0F00) >> 8] == V[(opcode & 0x00F0) >> 4])
                    {
                        pc += 2;
                    }
                    pc += 2;
                    break;
                case 0x6000: // 0x6XNN : Put NN into VX
                    V[(opcode & 0x0F00) >> 8] = (byte)(opcode & 0x00FF);
                    pc += 2;
                    break;
                case 0x7000: // 0x7XNN : Adds NN to VX (carry flag not changed according to Wikipedia)
                    V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] + (opcode & 0x00FF));
                    pc += 2;
                    break;
                case 0x8000:
                    switch (opcode & 0x000F)
                    {
                        case 0x0000: // 0x8XY0: Sets VX to the value VY
                            V[(opcode & 0x0F00) >> 8] = V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;
                        case 0x0001: // 0x8XY1: Sets VX to VX or VY (Bitwise OR)
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] | V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0002: // 0x8XY2: Sets VX to VX and VY (Bitwise AND)
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] & V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0003: // 0x8XY3: Sets VX to VX xor VY
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] ^ V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;
                        case 0x0004: // 0x8XY4: Adds VY to VX. VF is set to 1 when there's a carry, and to 0 when there isn't
                            if (V[(opcode & 0x0F00) >> 8] + V[opcode & 0x00F0 >> 4] > 255)
                            {
                                V[0xF] = 1; // Carry
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[(opcode & 0x0F00) >> 8] += V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;
                        case 0x0005: // 0x8XY5: VY is subtracted from VX. VF is set to 0 when there's a borrow, and 1 when there isn't
                            if (V[(opcode & 0x0F00) >> 8] > V[opcode & 0x00F0 >> 4])
                            {
                                V[0xF] = 1; // Borrow
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[(opcode & 0x0F00) >> 8] -= V[(opcode & 0x00F0) >> 4];
                            pc += 2;
                            break;
                        case 0x0006: // 0x8XY6: Stores the least significant bit of VX in VF and then shifts VX to the left by 1
                            V[0xF] = (byte)(0x000F & V[(opcode & 0x0F00) >> 8]);
                            V[(opcode & 0x0F00)] = (byte)(V[(opcode & 0x0F00)] << 1);
                            pc += 2;
                            break;
                        case 0x0007: // 0x8XY7: Sets VX to VY minus VX. VF is set to 0 when there's a borrow and 1 when there isn't
                            if (V[(opcode & 0x00F0) >> 4] > V[(opcode & 0x0F00) >> 8])
                            {
                                V[0xF] = 1;
                            }
                            else
                            {
                                V[0xF] = 0;
                            }
                            V[(opcode & 0x00F0) >> 4] = (byte)(V[(opcode & 0x0F00) >> 8] - V[(opcode & 0x00F0) >> 4]);
                            pc += 2;
                            break;
                        case 0x000E: // 0x8XYE: Stores the most significant bit of VX in VF and then shifts VX to the left by 1
                            V[0xF] = (byte)(0xF000 & V[(opcode & 0x0F00) >> 8] >> 12); // Might have done this wrong...
                            V[(opcode & 0x0F00) >> 8] = (byte)(V[(opcode & 0x0F00) >> 8] << 1);
                            pc += 2;
                            break;
                        default:
                            Console.WriteLine("Error: No '0x8' case caught opcode!");
                            break;
                    }
                    break;
                case 0x9000: // 0x9XY0: Skips the next instruction if VX doesn't equal VY
                    if (V[(opcode & 0x0F00) >> 8] != V[(opcode & 0x00F0) >> 4])
                    {
                        pc += 2;
                    }
                    pc += 2;
                    break;
                case 0xA000: // 0xANNN: Sets I to the address NNN
                    I = (ushort)(opcode & 0x0FFF);
                    pc += 2;
                    break;
                case 0xB000: // 0xBNNN: Jumps to the address NNN plus V0
                    pc = (ushort)((ushort)(opcode & 0x0FFF) + V[0]); // Again, needs testing
                    break;
                case 0xC000: // 0xCXNN: Sets VX to the result of a bitwise AND operation on a random number (typically 0 to 255) and NN
                    ushort rand = (ushort)rnd.Next(0, 255);
                    V[(ushort)(opcode & 0x0F00) >> 8] = (byte)(rand & (ushort)(opcode & 0x00FF));
                    pc += 2;
                    break;
                case 0xD000: // Sprite drawing
                             // 0xDXYN draws sprite at X,Y with height N (reads sprite data from
                             // memory at I and goes I + N times)
                             // Also does things with carry flag and draw flag
                    ushort x = V[(opcode & 0x0F00) >> 8];
                    ushort y = V[(opcode & 0x00F0) >> 4];
                    ushort height = (ushort)(opcode & 0x000F);
                    ushort pixel;

                    V[0xF] = 0;
                    for (int yline = 0; yline < height; yline++)
                    {
                        pixel = memory[I + yline]; // more like a row of a sprite than a pixel
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0) // does bit == 1?
                            {
                                if (gfx[(x + xline + ((y + yline) * 64))] == 1)
                                {
                                    V[0xF] = 1; // Did we flip on a pixel?
                                }
                                gfx[x + xline + ((y + yline) * 64)] ^= 1; // XOR pixels onto screen
                            }
                        }
                    }

                    drawFlag = true;
                    pc += 2;
                    break;
                case 0xE000:
                    switch (opcode & 0x000F)
                    {
                        case 0x000E: // 0xEX9E: Skips next instruction if key stored in VX is pressed
                            if (key[V[(ushort)(opcode & 0x0F00) >> 8]] != 0)
                            {
                                pc += 4;
                            }
                            else
                            {
                                pc += 2;
                            }
                            break;
                        case 0x0001: // 0xEXA1: Skips next instruction if key stored in VX isn't pressed
                            if (key[V[(ushort)(opcode & 0x0F00) >> 8]] == 0)
                            {
                                pc += 4;
                            }
                            else
                            {
                                pc += 2;
                            }
                            break;
                        default:
                            Console.WriteLine("Error: No '0xE' case caught opcode!");
                            break;
                    }
                    break;
                case 0xF000:
                    switch (opcode & 0x00FF)
                    {
                        case 0x0007: // 0xFX07: Sets VX to the value of the DT
                            V[(ushort)(opcode & 0x0F00) >> 8] = delay_timer;
                            pc += 2;
                            break;
                        case 0x000A: // 0xFX0A: Wait for a key press, store the value in VX
                            bool keyPressed = false;

                            for (int i = 0; i < key.Length; i++)
                            {
                                if (key[i] != 0)
                                {
                                    V[(ushort)(opcode & 0x0F00) >> 8] = (byte)i;
                                    keyPressed = true;
                                }

                                if (!keyPressed)
                                {
                                    return;
                                }
                                pc += 2;
                            }
                            break;
                        case 0x0015: // 0xFX15: Delay timer (DT) = VX
                            delay_timer = V[(ushort)(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x0018: // 0xFX18: Sound timer (ST) = VX
                            sound_timer = V[(ushort)(opcode & 0x0F00) >> 8];
                            pc += 2;
                            break;
                        case 0x001E: // 0xFX1E: I = I + VX
                            I = (byte)(I + V[(ushort)(opcode & 0x0F00) >> 8]);
                            pc += 2;
                            break;
                        case 0x0029:
                            I = (byte)(V[(ushort)(opcode & 0x0F00) >> 8] * 0x5);
                            break;
                        case 0x0033:
                            memory[I] = (byte)(V[(opcode & 0x0F00) >> 8] / 100);
                            memory[I + 1] = (byte)((V[(opcode & 0x0F00) >> 8] / 10) % 10);
                            memory[I + 2] = (byte)((V[(opcode & 0x0F00) >> 8] % 100) % 10);
                            pc += 2;
                            break;
                        case 0x0055:
                            for (int i = 0; i <= ((opcode & 0x0F00) >> 8); ++i)
                            {
                                memory[I + i] = V[i];
                            }

                            I += (ushort)(((opcode & 0x0F00) >> 8) + 1);
                            pc += 2;
                            break;
                        case 0x0065:
                            for (int i = 0; i <= ((opcode & 0x0F00) >> 8); ++i)
                            {
                                V[i] = memory[I + i];
                            }

                            I += (ushort)(((opcode & 0x0F00) >> 8) + 1);
                            pc += 2;
                            break;
                        default:
                            Console.WriteLine("Error: No '0xF' case caught opcode!");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine(opcode.ToString(), " Error: No 'main' case caught opcode!");
                    break;
            }

            // Update timers
        }

        /*
        Keypad                   Keyboard
        +-+-+-+-+                +-+-+-+-+
        |1|2|3|C|                |1|2|3|4|
        +-+-+-+-+                +-+-+-+-+
        |4|5|6|D|                |Q|W|E|R|
        +-+-+-+-+       =>       +-+-+-+-+
        |7|8|9|E|                |A|S|D|F|
        +-+-+-+-+                +-+-+-+-+
        |A|0|B|F|                |Z|X|C|V|
        +-+-+-+-+                +-+-+-+-+
        */
    }
}
