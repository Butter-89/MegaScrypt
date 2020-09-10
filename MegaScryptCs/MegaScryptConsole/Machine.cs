﻿using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace MegaScryptConsole
{
    class Machine
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
            AntlrInputStream input = new AntlrInputStream(expression );
            MegaScryptLexer lexer = new MegaScryptLexer(input);
            CommonTokenStream token = new CommonTokenStream(lexer);
            MegaScryptParser parser = new MegaScryptParser(token);

            MegaScryptParser.ExpressionContext root = parser.expression();
            object result = root.Accept(processor);
            return result;
        }
    }
}
