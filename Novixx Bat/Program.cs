namespace Novixx_Bat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // If no file is specified, run the interactive shell
            if (!args.Contains("-f"))
            {
                Shell shell = new Shell();
                while (true)
                {
                    Console.Write(">>> ");
                    shell.Execute(Console.ReadLine());
                }
            }
            else
            {
                Shell shell = new Shell(args[args.ToList().IndexOf("-f") + 1]);
            }
        }
    }
}