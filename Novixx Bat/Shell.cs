using System.Diagnostics;

namespace Novixx_Bat
{
    internal class Shell
    {
        Dictionary<string, string> variables = new Dictionary<string, string>();
        Dictionary<string, int> labels = new Dictionary<string, int>();
        string code = "";
        public Shell(string file)
        {
            Execute(File.ReadAllText(file));
        }

        public Shell()
        {
            while (true)
            {
                Console.Write(">>> ");
                Execute(Console.ReadLine());
            }
        }

        public void Execute(string command)
        {
            // For each line in the command
            string[] lines = command.Split('\n');
            foreach (string line in lines)
            {
                code += line.Trim() + "\n";
                string cmd = line.Trim().Split(' ')[0].ToLower();
                string[] args = line.Trim().Split(' ').Skip(1).ToArray();

                // If the command is a built-in command
                if (cmd == "echo")
                {
                    Console.WriteLine(ParseVars(string.Join(" ", args)));
                }
                else if (cmd.StartsWith(":") && !cmd.StartsWith("::"))
                {
                    labels[cmd.Substring(1)] = code.Split('\n').Length - 1;
                }
                else if (cmd == "goto")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: goto <label>");
                    }
                    else
                    {
                        if (labels.ContainsKey(args[0]))
                        {
                            code = code.Split('\n')[labels[args[0]]] + "\n";
                        }
                        else
                        {
                            Console.WriteLine("Label not found");
                        }
                    }
                }
                else if (cmd == "if")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: if <condition> <command>");
                    }
                    else
                    {
                        if (args[0] == "exist")
                        {
                            if (args.Length == 1)
                            {
                                Console.WriteLine("Usage: if exist <file> <command>");
                            }
                            else
                            {
                                if (File.Exists(args[1]))
                                {
                                    Execute(string.Join(" ", args.Skip(2)));
                                }
                            }
                        }
                        else if (args[0] == "not")
                        {
                            if (args.Length == 1)
                            {
                                Console.WriteLine("Usage: if not <condition> <command>");
                            }
                            else
                            {
                                if (args[1] == "exist")
                                {
                                    if (args.Length == 2)
                                    {
                                        Console.WriteLine("Usage: if not exist <file> <command>");
                                    }
                                    else
                                    {
                                        if (!File.Exists(args[2]))
                                        {
                                            Execute(string.Join(" ", args.Skip(3)));
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (args[1] == "equ")
                            {
                                if (ParseVars(args[0]) == ParseVars(args[2]))
                                {
                                    // Execute the command
                                    Execute(string.Join(" ", args.Skip(3)));
                                }
                            }
                        }
                    }
                }
                else if (cmd == "call")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: call <file>");
                    }
                    else
                    {
                        Execute(File.ReadAllText(args[0]));
                    }
                }
                else if (cmd == "pause")
                {
                    Console.ReadKey();
                }
                else if (cmd == "rem")
                {
                    // Do nothing
                }
                else if (cmd == "start")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: start <file>");
                    }
                    else
                    {
                        Process.Start(args[0]);
                    }
                }
                else if (cmd == "exit")
                {
                    Environment.Exit(0);
                }
                else if (cmd == "cd")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine(ParseVars(Environment.CurrentDirectory));
                    }
                    else
                    {
                        Environment.CurrentDirectory = args[0];
                    }
                }
                else if (cmd == "dir")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine(string.Join("\n", Directory.GetFiles(Environment.CurrentDirectory)));
                    }
                    else
                    {
                        Console.WriteLine(ParseVars(string.Join("\n", Directory.GetFiles(args[0]))));
                    }
                }
                else if (cmd == "type")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: type <file>");
                    }
                    else
                    {
                        Console.WriteLine(ParseVars(File.ReadAllText(args[0])));
                    }
                }
                else if (cmd == "del")
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Usage: del <file>");
                    }
                    else
                    {
                        File.Delete(args[0]);
                    }
                }
                else if (cmd == "cls")
                {
                    Console.Clear();
                }
                else if (cmd == "set")
                {
                    if (args.Length > 0)
                    {
                        if (args[0].Contains("="))
                        {
                            string[] parts = args[0].Split('=');
                            variables[parts[0]] = parts[1];
                        }
                        else if (args[0] == "/p" || args[0] == "/P")
                        {
                            if (args.Length == 1)
                            {
                                Console.WriteLine("Usage: set /p <variable>");
                            }
                            else
                            {
                                Console.Write(args[1] + "=");
                                variables[args[1]] = Console.ReadLine();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Usage: set <variable>=<value>");
                        }
                    }
                }
            }
        }

        private string ParseVars(string v)
        {
            string[] parts = v.Split('%');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (variables.ContainsKey(parts[i]))
                    {
                        parts[i] = variables[parts[i]];
                    }
                    else
                    {
                        parts[i] = "%" + parts[i] + "%";
                    }
                }
            }
            return string.Join("", parts);
        }
    }
}