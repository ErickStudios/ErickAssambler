using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sasm
{
    //
    // S-SUN assembly debbuger
    //
    class sasm_debbuger
    {
        // the registers
        Dictionary<int,char> memory_acces = new Dictionary<int,char>();

        public void
            DebugProgram
            (
                string program
            )
        {
            compiler instructtions = new compiler();

            memory_acces[10] = Convert.ToChar(3);

            char[] p = program.ToCharArray();

            for (; memory_acces[10] < p.Length; memory_acces[10]++)
            {
                char ch = p[memory_acces[10]];

                char p1 = '0';
                char p2 = '0';

                try
                {
                    p1 = Convert.ToChar(Convert.ToInt16(p[memory_acces[10] + 1]) - Convert.ToInt16(instructtions.safetynow_for_up));
                    p2 = Convert.ToChar(Convert.ToInt16(p[memory_acces[10] + 2]) - Convert.ToInt16(instructtions.safetynow_for_up));

                }
                catch {
                    p2 = '?';
                }

                char p1r;
                char p2r;

                if (
                    memory_acces.ContainsKey(Convert.ToInt16(p1))
                    )
                {
                    p1r = memory_acces[Convert.ToInt16(p1)];
                } 
                else
                {
                    p1r = '0';
                }

                if (
                    memory_acces.ContainsKey(Convert.ToInt16(p2))
                    )
                {
                    p2r = memory_acces[Convert.ToInt16(p2)];
                }
                else
                {
                    p2r = '0';
                }

                if (
                    ch == instructtions.mov_instruction // mov
                    )
                {
                    if (
                        !memory_acces.ContainsKey(p1)
                        )
                    {
                        memory_acces.Add(p1,p2);
                    }
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
                   int search_s = 0;

                    while (
                        (p[search_s] == instructtions.section_instruction) &&
                        (p[search_s + 1] == p1) &&
                        (p[search_s] != '\0') // Asegura que sea una comparación válida
                        )
                        search_s++
                        ;
                    memory_acces[10] = Convert.ToChar(search_s);
                }

                if (
                    ch == instructtions.interruption_instruction
                    )
                {
                    if (
                        p1 == 1
                        )
                    {
                        Console.Write(p2r);
                    }
                }
            }
        }
    }
}
