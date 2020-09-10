using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace MegaScryptConsole
{
    class Calculator: MegaScryptBaseVisitor<object>
    {
        public float Evaluate(string expression)
        {
            AntlrInputStream input = new AntlrInputStream(expression);
            MegaScryptLexer lexer = new MegaScryptLexer(input);
            CommonTokenStream token = new CommonTokenStream(lexer);
            MegaScryptParser parser = new MegaScryptParser(token);

            MegaScryptParser.ExpressionContext root = parser.expression();
            object obj_result = root.Accept(this);
            float f_result = Convert.ToSingle(obj_result);
            return f_result;
        }

        //public override object VisitNumber([NotNull] MegaScryptParser.NumberContext context)
        //{
        //    if(context.Dot()!=null)
        //    {
        //        float f = float.Parse(context.GetText());
        //        return f;
        //    }
        //    else
        //    {
        //        int i = int.Parse(context.GetText());
        //        return i;
        //    } 
        //}

        //public override object VisitExpression([NotNull] MegaScryptParser.ExpressionContext context)
        //{
        //    if(context.number() != null)
        //    {
        //        object result = context.number().Accept(this);
        //        return result;
        //    }

        //    MegaScryptParser.ExpressionContext[] exprs = context.expression();
        //    if(exprs.Length == 1)
        //    {
        //        object result = exprs[0].Accept(this);
        //        if(context.Minus() != null)
        //        {
        //            if (result is int)
        //                result = -Convert.ToInt32(result);
        //            else
        //                result = -Convert.ToSingle(result);
        //        }
        //        return result;
        //    }

        //    if(exprs.Length == 2)
        //    {
        //        object a = exprs[0].Accept(this);
        //        object b = exprs[1].Accept(this);
        //        ITerminalNode operatorNode = context.children[1] as ITerminalNode;
        //        if(a is int && b is int)
        //        {
        //            return IntegerBinaryOperation(a, b, operatorNode.Symbol.Type);
        //        }
        //        else
        //        {
        //            return FloatBinaryOperation(a, b, operatorNode.Symbol.Type);
        //        }
        //    }
        //    throw new InvalidOperationException();
            
        //}

        protected int IntegerBinaryOperation(object oa, object ob, int op)
        {
            int a = Convert.ToInt32(oa);
            int b = Convert.ToInt32(ob);
            switch (op)
            {
                case MegaScryptParser.Plus: return a + b;
                case MegaScryptParser.Minus: return a - b;
                case MegaScryptParser.Multiply: return a * b;
                case MegaScryptParser.Divide: return a / b;
            }

            throw new InvalidOperationException();
        }

        protected float FloatBinaryOperation(object oa, object ob, int op)
        {
            float a = Convert.ToSingle(oa);
            float b = Convert.ToSingle(ob);
            switch (op)
            {
                case MegaScryptParser.Plus: return a + b;
                case MegaScryptParser.Minus: return a - b;
                case MegaScryptParser.Multiply: return a * b;
                case MegaScryptParser.Divide: return a / b;
            }

            throw new InvalidOperationException();
        }
    }
}
