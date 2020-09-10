using System;
using System.Collections.Generic;
using MegaScrypt;

namespace MegaScryptConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Machine machine = new Machine();
            string script = "";
            string line;
            while(true)
            {
                line = Console.ReadLine();
                if (line == "exit")
                    break;

                if(line != null)
                {
                    script += line + "\n";
                }
                else
                {
                    try
                    {
                        machine.Execute(script);
                        PrintVariables(machine);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    script = "";
                }

                
                //line = Console.ReadLine();
                //if (line == null)
                //{
                //    try
                //    {
                //        machine.Execute(script);
                //    }
                //    catch(Exception ex)
                //    {
                //        Console.WriteLine(ex.Message);
                //    }
                //    script = "";
                //}
                //else
                //{
                //    script += line + "\n";
                //}
            }
        }

        static void PrintVariables(Machine machine)
        {
            List<string> varNames = machine.Target.VariableNames;
            foreach(string varName in varNames)
            {
                Console.WriteLine($"{varName} = {machine.Target.Get(varName)}");
            }
        }
    }
}
