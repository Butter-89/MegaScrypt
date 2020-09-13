using System;
using System.Collections.Generic;
using MegaScrypt;

namespace MegaScrypt
{
    class Program
    {
        static object Print(List<object> parameters)
        {
            foreach(object o in parameters)
            {
                Console.WriteLine(o.ToString());
            }
            return null;
        }

        static object Abs(List<object> parameters)
        {
            int i = (int)parameters[0];
            return Math.Abs(i);
        }

        static void Main(string[] args)
        {
            Machine machine = new Machine();
            machine.Declare(Print);
            machine.Declare(Abs, new string[] { "i" });

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
                        //PrintVariables(machine);
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
