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

    /**
     * SolveDependences
     * 
     * solves the dependences in a specific code using a path work as a reference
     */
    static string SolveDependences(string code, string pathwork)
    {
        string outpud = code.Replace("\r", "");

        string[] lines = outpud.Split("\n");

        for (int i = 0; i < lines.Length; i++)
        {
            if (
                lines[i].Trim() == "%include"
                )
            {
                outpud = outpud.Replace("%include\n" + lines[i + 1], (File.Exists(Path.Join(pathwork, lines[i + 1].Trim())) ? SolveDependences(File.ReadAllText(Path.Join(pathwork, lines[i + 1].Trim())), pathwork) : ""));
            }
        }

        return outpud;
    }

    /**
     * Main
     * 
     * el punto de entrada del ensamblador
     */
    static void Main(string[] args)
    {


        ///
        /// configurar variables
        ///
        compiler Compiler = new();
        sasm_debbuger Debugger = new();

        ///
        /// no hay parametros o se llamo a la ayuda
        ///

        if (args.Length == 0 ? true : (args[0] == "--help"))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("E: not outpud files\n");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Usage? OK");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--hlvc");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   compile a file with the HightLevel compiler that is like C but not the same\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--o");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("      compile a file with the normal compiler that is like assembly but more easy\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--uno");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("    decompile a file\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--mkl");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("    automatize the compilation of varius files with a .mkl\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("--help");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("  shows this help message\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Welcome to the prompt");

            string current_dir = Environment.CurrentDirectory;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string line = Console.ReadLine();

                ///
                /// --hlvc archivo
                ///
                if (line == "--hlvc")
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("       ");

                    ///
                    /// el archivo a convertir
                    ///
                    string file = Console.ReadLine();

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        ///
                        /// convertir el archivo
                        ///
                        string outpud = Compiler.TranslateHightLevelToAsm(SolveDependences(File.ReadAllText(file),current_dir));

                        ///
                        /// hacer un intermedio
                        ///
                        if (!File.Exists(file + ".easm"))
                        { File.Create(file + ".easm"); }
                        
                        File.WriteAllText(file + ".easm", outpud);

                        ///
                        /// convertirlo ahora si a .ebf
                        ///

                        File.WriteAllText(
                            Path.GetFullPath(file).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                            Compiler.ConvertCode(File.ReadAllText(file + ".easm"))
                        );
                    }

                }

                ///
                /// --o archivo
                ///
                else if (line == "--o")
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("    ");

                    ///
                    /// el archivo a convertir
                    ///
                    string file = Console.ReadLine();

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        ///
                        /// convertirlo a .ebf
                        ///

                        File.WriteAllText(
                            Path.GetFullPath(file).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                            Compiler.ConvertCode(SolveDependences(File.ReadAllText(file),current_dir))
                        );
                    }

                }

                else if (line == "--uno")
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("      ");

                    ///
                    /// el archivo a convertir
                    ///
                    string file = Console.ReadLine();

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        ///
                        /// convertirlo a .ebf
                        ///

                        Console.WriteLine(Compiler.DecompileCode(File.ReadAllText(file)));
                    }
                }

                else if (line == "--dbg")
                {

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("      ");

                    ///
                    /// el archivo a convertir
                    ///
                    string file = Console.ReadLine();

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        Debugger.DebugProgram(File.ReadAllText(file));
                    }
                }

                else if (line.StartsWith("--cd "))
                {
                    string path = line.Substring(5);

                    current_dir = path;
                }

                else if (line == "--exit")
                {
                    return;
                }
            }
        }
        else
        {
            for (global::System.Int32 i = 0; i < args.Length; i++)
            {
                if (
                    args[i] == "--hlvc"
                    )
                {

                    ///
                    /// el archivo a convertir
                    ///
                    string file = args[i + 1];

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        ///
                        /// convertir el archivo
                        ///
                        string outpud = Compiler.TranslateHightLevelToAsm(File.ReadAllText(file));

                        ///
                        /// hacer un intermedio
                        ///
                        if (!File.Exists(file + ".easm"))
                        { File.Create(file + ".easm"); }

                        File.WriteAllText(file + ".easm", outpud);

                        ///
                        /// convertirlo ahora si a .ebf
                        ///

                        File.WriteAllText(
                            Path.GetFullPath(file).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                            Compiler.ConvertCode(File.ReadAllText(file + ".easm"))
                        );
                    }
                }
                else if (
                    args[i] == "--o"
                    )
                {
                    ///
                    /// el archivo a convertir
                    ///
                    string file = args[i + 1];

                    ///
                    /// si existe
                    ///
                    if (File.Exists(file))
                    {
                        ///
                        /// convertirlo a .ebf
                        ///

                        File.WriteAllText(
                            Path.GetFullPath(file).Replace(Path.GetFileName(file), Path.GetFileNameWithoutExtension(file)) + ".ebf",
                            Compiler.ConvertCode(File.ReadAllText(file))
                        );
                    }
                }
                else if (
                    args[i] == "--uno"
                    )
                {
                    string file = args[i + 1];

                    File.WriteAllText(
                        file + ".easm",
                        Compiler.DecompileCode(
                                File.ReadAllText(file)
                            ));
                }
                else if (
                    args[i] == "--mkl"
                    )
                {
                    string file = args[i + 1];

                    string popcwd = Environment.CurrentDirectory;

                    Environment.CurrentDirectory = Directory.GetParent(file).FullName;

                    string[] lines = File.ReadAllText(file).Replace("\r", "").Split("\n");

                    for (global::System.Int32 j = 0; j < lines.Length; j++)
                    {
                        string line_trimed = lines[j].Trim();

                        if (
                            line_trimed.StartsWith("package ")
                            )
                        {
                            string[] argumments = line_trimed.Substring(8, line_trimed.Length - 8).Split(" -> ");

                            Main(argumments);
                        }
                    }

                    Environment.CurrentDirectory = popcwd;
                }
            }
        }
    }
    
    /*static void Main(string[] args)
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

    }*/
}