using sasm;
using System.IO.Enumeration;

class Program
{
    static string ConvertStringToArrayHex(string e)
    {
        string ou = "";
        foreach (var item in e)
        {
            ou += "(CHAR16)" + ((int)(item)).ToString() + ", ";
        }

        ou += "(CHAR16)0";
        return ou;
    }

    static string ConvertFile(string file)
    {
        compiler Compiler = new();

        string outpud = ConvertStringToArrayHex(Compiler.ConvertCode(File.ReadAllText(file)));

        outpud = "CHAR16 " +
            Path.GetFileNameWithoutExtension(file).Replace(" ", "_").Replace("-", "_")
            + "[] = {" + outpud + "};";

        return outpud;
    }

    static void Main(string[] args)
    {
        compiler Compiler = new();
        sasm_debbuger Debugger = new();

        if (
            args.Length == 0
            )
        {
            // recurre to command line

            Console.ForegroundColor = ConsoleColor.Gray;
            // Console.Write("inicialiting S-SUN simulator Envioriment... (necesary for run S-SUN binary programs, this can make mistakes when run some programs)");
            Console.WriteLine();
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("Welcome to ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("S");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("-");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("SUN");

            Console.WriteLine();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("root@local");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("^");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(":#");

                Console.ForegroundColor = ConsoleColor.Gray;
                string asm_file = Console.ReadLine();

                if (
                    asm_file.StartsWith("file ")
                    )
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    string file = asm_file.Substring(5, asm_file.Length - 5);

                    string outpud = Compiler.ConvertCode(File.ReadAllText(file));

                    File.WriteAllText(
                        (Path.GetFullPath(file)).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                        outpud
                    );

                    Console.WriteLine(ConvertFile(file));


                    //Debugger.DebugProgram(File.ReadAllText((Path.GetFullPath(file)).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".bin"));
                }
                if (
                    asm_file.StartsWith("hlvc ")
                    )
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    string file = asm_file.Substring(5, asm_file.Length - 5);

                    string cmpiled = Compiler.TranslateHightLevelToAsm(File.ReadAllText(file));

                    string outpud = Compiler.ConvertCode("EA\n" + cmpiled);

                    File.WriteAllText(
                        (Path.GetFullPath(file)).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                        outpud
                    );

                    Console.WriteLine(ConvertFile(file));

                    //Debugger.DebugProgram(File.ReadAllText((Path.GetFullPath(file)).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".bin"));
                }
                else if (asm_file.StartsWith("decompile "))
                {
                    string file_name = asm_file.Substring(10, asm_file.Length - 10);

                    if (
                        File.Exists(file_name))
                    {
                        Console.WriteLine(Compiler.DecompileCode(File.ReadAllText(file_name)));
                    }
                }
            }
        }

    }
}