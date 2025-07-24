using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace sasm
{
    //
    // S-SUN assembly debbuger
    //
    class sasm_debbuger
    {
        // the registers
        public Dictionary<int,int> memory_acces = new Dictionary<int,int>();

        public string condraw = "";

        public int
            IMemo
            (
                int item
            )
        {
            if (memory_acces.ContainsKey(item)) return memory_acces[item];

            return 0;
        }

        public int
        AllocateMemory
        (
            int size
        )
        {
            //
            // check if not trolling
            //

            if (
                size == 0
                )
                return 0;

            //
            // set the buffer variables
            //

            int Buffer = -1;

            //
            // search space in the memory
            //

            for (int i = 2001; i < 4000; i++)
            {
                if (
                    !memory_acces.ContainsKey(i)
                    )
                {
                    Buffer = i;
                    break;
                }
            }

            //
            // check if the buffer is not nulling
            //

            if (
                Buffer != -1
                )
            {
                //
                // configure vars
                //

                int fill_end = Buffer + 1 + size;
                int filling_i = Buffer + 1;

                //Print(L"Try to allocate memory starting in based of Header at %d and ends in %d\n", filling_i, fill_end);

                //
                // initialize buffer
                //

                while (filling_i != fill_end)
                {
                    memory_acces[filling_i] = 1;
                    filling_i++;
                }

                memory_acces[Buffer] = size;

                // Print(L"Pool starting at: %d\n",(INTN)Buffer);

                return Buffer;
            }

            return 0;
        }

        public string
            LocateMemory
            (
                int StartingAt
            )
        {
            int size = memory_acces[StartingAt];

            string Located = "";

            int reader = StartingAt + 1;

            for (int i = 0; i < ((int)size); i++)
            {
                Located += memory_acces.ContainsKey(reader) ? ((char)memory_acces[reader]).ToString() : "";
                reader++;
            }

            return Located;
        }

        public void
        SetMemoryPool
        (
            int Pool,
            int Index,
            int Value
        )
        {
            //Print(L"the size of the pool is : %d\n", memory_acces[Pool]);

            memory_acces[
                Pool + 1 + Index
            ] = Value;
        }
        
        public int
        GetMemoryPool
        (
            int Pool,
            int Index
        )
        {
            return memory_acces[(Pool + 1) + Index];
        }

        public int
        AllocateStringMemory
        (
            string Str
        )
        {
            //
            // configure vars
            //
            int len = Str.Length;
            int BufferDir;

            //
            // allocate the direction
            //
            BufferDir = AllocateMemory(len) + 1;

            int index = 0;

            for (int i = BufferDir; i < BufferDir + (len); i++)
            {
                memory_acces[i] = (int)Str[index];
                index++;
            }

            //Console.Write(BufferDir.ToString());            
            
            return BufferDir - 1;
        }


        public void
        FillMemorySpaces
        (
            int From,
            int To,
            int NewValue
        )
        {
            for (int i = 0; i < (To + 1); i++) memory_acces[i] = NewValue;
        }

        public void
            DebugProgram
            (
                string program
            )
        {
            /**
            * stack_pop
            * 
            * for the differents ret
            */
            Dictionary<int,int> stack_pop = new();

            /**
            * curr_popback
            * 
            * the current index for write or delete
            */
            int curr_popback = 0;
            compiler instructtions = new compiler();

            memory_acces[10] = Convert.ToChar(3);

            char[] p = program.ToCharArray();

            ConsoleKeyInfo KeyPressed = new();

            while (memory_acces[10] < p.Length)
            {
                char ch = p[memory_acces[10]];

                int p1 = '0';
                int p2 = '0';

                try
                {
                    p1 = Convert.ToChar((int)(p[memory_acces[10] + 1]) - (int)(instructtions.safetynow_for_up));
                    p2 = Convert.ToChar((int)(p[memory_acces[10] + 2]) - (int)(instructtions.safetynow_for_up));
                }
                catch {
                    p2 = '?';
                }

                int p1r = '.';
                int p2r = '.';

                if (
                    memory_acces.ContainsKey(p1)
                    )
                {
                    p1r = memory_acces[p1];
                }
                else
                {
                    p1r = '0';
                }

                if (
                    memory_acces.ContainsKey(p2)
                    )
                {
                    p2r = memory_acces[p2];
                }
                else
                {
                    p2r = '0';
                }

                if (
                    ch == instructtions.mov_instruction // mov
                    )
                {
                    memory_acces[p1] = p2;
                }
                if (
                    ch == instructtions.add_instruction // add
                    )
                {
                    memory_acces[p1] += p2r;
                }
                if (
                    ch == instructtions.sub_instruction // sub
                    )
                {
                    memory_acces[p1] -= p2r;
                }
                if (
                    ch == instructtions.div_instruction // div
                    )
                {
                    memory_acces[p1] /= p2r;
                }
                if (
                    ch == instructtions.imul_instruction // multiplicate
                    )
                {
                    memory_acces[p1] *= p2r;
                }
                if (
                    ch == instructtions.incr_instruction // incrementase
                    )
                {
                    memory_acces[p1]++;
                }
                if (
                    ch == instructtions.decr_instruction // decrementase
                    )
                {
                    memory_acces[p1]--;
                }

                if (
                    ch == instructtions.jump_instruction // jump
                    )
                {
                    int search_s = 3;

                    while (
                        (p[search_s] != instructtions.section_instruction) &&
                        (p[search_s + 1] != p1) 
                        )
                        search_s++
                        ;
                    memory_acces[10] = search_s;
                }

                if (
                    ch == instructtions.interruption_instruction
                    )
                {
                    if (
                        p1 == 1
                        )
                    {
                        Console.Write((char)p2r);
                    }
                    else if (
                        p1 == 2
                        )
                    {
                        Console.Clear();
                    }

                    if (
                        p1 == 3 || p1 == 4
                        )
                    {
                        ConsoleColor e;
                        switch ((int)p2r)
                        {
                            case 1:
                                e = ConsoleColor.Black;
                                break;
                            case 2:
                                e = ConsoleColor.DarkGray;
                                break;
                            case 3:
                                e = ConsoleColor.DarkGray;
                                break;
                            case 4:
                                e = ConsoleColor.Gray;
                                break;
                            case 5:
                                e = ConsoleColor.White;
                                break;
                            case 6:
                                e = ConsoleColor.White;
                                break;

                            case 7:
                                e = ConsoleColor.DarkRed;
                                break;
                            case 8:
                                e = ConsoleColor.Red;
                                break;
                            case 9:
                                e = ConsoleColor.Red;
                                break;

                            case 10:
                                e = ConsoleColor.DarkYellow;
                                break;
                            case 11:
                                e = ConsoleColor.Yellow;
                                break;
                            case 12:
                                e = ConsoleColor.Yellow;
                                break;

                            case 13:
                                e = ConsoleColor.DarkYellow;
                                break;
                            case 14:
                                e = ConsoleColor.Yellow;
                                break;
                            case 15:
                                e = ConsoleColor.Yellow;
                                break;

                            case 16:
                                e = ConsoleColor.DarkGreen;
                                break;
                            case 17:
                                e = ConsoleColor.Green;
                                break;
                            case 18:
                                e = ConsoleColor.Green;
                                break;

                            case 19:
                                e = ConsoleColor.DarkCyan;
                                break;
                            case 20:
                                e = ConsoleColor.Cyan;
                                break;
                            case 21:
                                e = ConsoleColor.Cyan;
                                break;

                            case 22:
                                e = ConsoleColor.DarkCyan;
                                break;
                            case 23:
                                e = ConsoleColor.Cyan;
                                break;
                            case 24:
                                e = ConsoleColor.Cyan;
                                break;

                            case 25:
                                e = ConsoleColor.DarkBlue;
                                break;
                            case 26:
                                e = ConsoleColor.Blue;
                                break;
                            case 27:
                                e = ConsoleColor.Blue;
                                break;
                            default:
                                e = ConsoleColor.Gray;
                                break;
                        }
                        if (
                            p1 == 3
                            )
                            Console.ForegroundColor = e;
                        if (
                            p1 == 4
                            )
                            Console.BackgroundColor = e;
                    }
                    if (
                        p1 == 5
                        )
                    {
                        Console.CursorLeft = p2r;
                    }
                    if (
                        p1 == 7
                        )
                    {
                        Console.CursorTop = p2r;
                    }
                    if ( 
                        p1 == 8
                        )
                    {
                        condraw += ((char)memory_acces[p2]).ToString();
                    }
                    if (
                        p1 == 9
                        )
                    {
                        Console.Write(condraw);

                        condraw = "";
                    }
                    if (
                        p1 == 10
                        )
                    {
                        Task.Delay(p2r / 10);
                    }
                    if (
                        p1 == 11
                        )
                    {
                        KeyPressed = Console.ReadKey(true);
                    }
                    if (
                        p1 == 12
                        )
                    {
                        memory_acces[p2] = (char)KeyPressed.Key;
                        memory_acces[p2 + 1] = KeyPressed.KeyChar;
                    }
                    if (
                        p1 == 13
                        )
                    {

                        memory_acces[p2] = (char)DateTime.Now.Second;
                        memory_acces[p2 + 1] = (char)DateTime.Now.Minute;
                        memory_acces[p2 + 2] = (char)DateTime.Now.Hour;

                        memory_acces[p2 + 3] = (char)DateTime.Now.Day;
                        memory_acces[p2 + 4] = (char)DateTime.Now.Month;
                        memory_acces[p2 + 5] = (char)DateTime.Now.Year;
                    }
                    if (
                        p1 == 14
                        )
                    {
                        char size = (char)memory_acces[p2 + 1];

                        memory_acces[p2] = AllocateMemory(size);
                    }
                    if (
                        p1 == 15
                        )
                    {
                        SetMemoryPool(memory_acces[p2], memory_acces[p2 + 1], memory_acces[p2 + 2]);
                    }
                    if (
                        p1 == 16
                        )
                    {
                        SetMemoryPool(memory_acces[p2], memory_acces[p2 + 1], memory_acces[p2 + 2]);
                    }
                    if (
                        p1 == 17
                        )
                    {
                        int BufferToManipule = memory_acces[p2];
                        int BufferItem = memory_acces[p2 + 1];
                        int RedirectTo = p2 + 2;

                        memory_acces[RedirectTo] = memory_acces[(BufferToManipule + 1) + BufferItem];
                    }
                    if (
                        p1 == 18
                        )
                    {
                        Console.WriteLine(p2r.ToString());
                    }
                    if (
                        p1 == 19
                        )
                    {
                        Console.WriteLine(p2r.ToString("X"));

                    }
                    if (
                        p1 == 20
                        )
                    {
                        int Buffer = p2r;

                        int lenght = IMemo(Buffer);

                        int MemFree = Buffer;

                        for (int i = 0; i < lenght; i++)
                        {
                            memory_acces[MemFree] = 0;
                            MemFree++;
                        }
                    }

                    if (
                        p1 == 22
                        )
                    {
                        Console.WriteLine(LocateMemory(memory_acces[p2]));
                    }

                    if (
                        p1 == 23
                        )
                    {
                        string Line = Console.ReadLine();

                        memory_acces[p2] = AllocateStringMemory(Line);
                    }

                    if (
                        p1 == 24
                        )
                    {
                        memory_acces[256] = Convert.ToInt32(LocateMemory(memory_acces[p2]) == LocateMemory(memory_acces[p2 + 1]));
                    }

                    if (
                        p1 == 25
                        )
                    {
                        memory_acces[256] = Convert.ToInt32(memory_acces[p2] == memory_acces[p2 + 1]);
                    }
                    if (
                    p1 == 26
                    )
                    {
                        memory_acces[256] = Convert.ToInt32(memory_acces[p2] == memory_acces[p2 + 1]);
                    }

                }
                else if (
                ch == instructtions.jq_instruction
                )
                {
                    if (
                        memory_acces[256] != 0
                        )
                    {

                        /**
                        * search_s
                        *
                        * represents the search in program function
                        */
                        int search_s = 0;

                        //
                        // loops
                        //
                        while (
                            (
                                //
                                // you found a function?
                                //
                                (p[search_s] != instructtions.section_instruction) &&

                                //
                                // are function founded
                                //
                                (p[search_s + 1] != p1)
                                )
                            )
                            search_s++
                            ;

                        //
                        // save the before stack
                        //

                        stack_pop[curr_popback] = memory_acces[10] + 1;
                        curr_popback++; // next position

                        //
                        // set the stack position
                        //
                        memory_acces[10] = search_s;
                    }
                }

                memory_acces[10]++;
            }
        }
    }
}
