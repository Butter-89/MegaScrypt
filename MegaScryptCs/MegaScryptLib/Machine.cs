using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace MegaScrypt
{
    public class Machine
    {
        private Processor processor;
        private Object target;
        public Object Target => target;

        public Machine()
        {
            processor = new Processor();
            target = new Object();
            processor.Target = target;
        }
        public object Execute(string script)
        {
            AntlrInputStream input = new AntlrInputStream(script);
            MegaScryptLexer lexer = new MegaScryptLexer(input);
            CommonTokenStream token = new CommonTokenStream(lexer);
            MegaScryptParser parser = new MegaScryptParser(token);

            MegaScryptParser.ProgramContext root = parser.program();
            object result = root.Accept(processor);
            return result;
        }

        public object Evaluate(string expression)
        {
            AntlrInputStream input = new AntlrInputStream(expression);
            MegaScryptLexer lexer = new MegaScryptLexer(input);
            CommonTokenStream token = new CommonTokenStream(lexer);
            MegaScryptParser parser = new MegaScryptParser(token);

            MegaScryptParser.ExpressionContext root = parser.expression();
            object result = root.Accept(processor);
            return result;
        }

        public void Declare(NativeFunction funtion)
        {
            target.Declare(funtion.Name, funtion);
        }

        public void Declare(NativeFunction.Callback callback, IEnumerable<string> parameterNames = null)
        {
            NativeFunction function = new NativeFunction(callback, parameterNames);
            target.Declare(function.Name, function);
        }
    }
}
