using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.SymbolStore;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using MegaScrypt;

namespace MegaScrypt
{
    class Processor : MegaScryptBaseVisitor<object>
    {
        //public float Evaluate(string expression)
        //{

        //}
        private Object target;
        public Object Target
        {
            get { return target; }
            set { target = value; }
        }

        #region Basic

        public override object VisitDeclaration([NotNull] MegaScryptParser.DeclarationContext context)
        {
            string varName = context.Id().GetText();
            object value = null;
            if (context.expression() != null)
            {
                value = context.expression().Accept(this);
            }
            else if (context.compoundIdentifier() != null)
            {
                value = context.compoundIdentifier().Accept(this);
            }
            target.Declare(varName, value);
            return value;
        }

        public override object VisitBlock([NotNull] MegaScryptParser.BlockContext context)
        {
            Object pre_target = target;
            target = new Object(pre_target);

            object result = base.VisitBlock(context);

            target = pre_target;

            return result;
        }

        public override object VisitIfStmt([NotNull] MegaScryptParser.IfStmtContext context)
        {
            MegaScryptParser.ExpressionContext[] expressions = context.expression();
            MegaScryptParser.BlockContext[] blocks = context.block();
            object ret = null;
            for (int i = 0; i < expressions.Length; i++)
            {
                object result = expressions[i].Accept(this);
                bool test = ToBoolean(result);
                if (test)
                {
                    ret = blocks[i].Accept(this);
                    return test;
                }
            }

            if (context.elseStmt() != null)
            {
                ret = context.elseStmt().block().Accept(this);
                return ret;
            }

            return null;
        }

        public override object VisitElseStmt([NotNull] MegaScryptParser.ElseStmtContext context)
        {
            if (context.block() != null)
            {
                object ret = context.block().Accept(this);
                return ret;
            }

            throw new InvalidOperationException("There is not statement in else!");
        }

        public override object VisitAssignment([NotNull] MegaScryptParser.AssignmentContext context)
        {
            object result = null;
            string varName = context.Id().GetText();
            //string varName = context.id().Accept(this) as string;
            if (context.compoundIdentifier() != null)
            {
                result = context.compoundIdentifier().Accept(this);
                //return result;
            }
            else if (context.expression() != null)
            {
                object exprValue = context.expression().Accept(this);
                if (!target.Has(varName))
                    throw new InvalidOperationException($"The variable {varName} is not declared");

                object varValue = target.Get(varName);

                if (context.PlusEql() != null)
                {
                    result = BinaryOperation(varValue, exprValue, MegaScryptLexer.Plus);
                }
                else if (context.MinusEql() != null)
                    result = BinaryOperation(varValue, exprValue, MegaScryptLexer.Minus);
                else if (context.TimesEql() != null)
                    result = BinaryOperation(varValue, exprValue, MegaScryptLexer.Multiply);
                else if (context.DivideEql() != null)
                    result = BinaryOperation(varValue, exprValue, MegaScryptLexer.Divide);
            }


            if (result == null)
                throw new InvalidOperationException();

            target.Set(varName, result);
            return result;
        }

        public override object VisitIncrement([NotNull] MegaScryptParser.IncrementContext context)
        {
            string varName = context.Id().GetText();
            if (target.Has(varName))
            {
                object value = target.Get(varName);
                if (target.Get(varName) is int)
                {
                    int result = Convert.ToInt32(value) + 1;
                    target.Set(varName, result);
                }
                else if (target.Get(varName) is float)
                {
                    float result = Convert.ToSingle(value) + 1;
                    target.Set(varName, result);
                }
            }
            return base.VisitIncrement(context);
        }

        public override object VisitDecrement([NotNull] MegaScryptParser.DecrementContext context)
        {
            string varName = context.Id().GetText();
            if (target.Has(varName))
            {
                object value = target.Get(varName);
                if (target.Get(varName) is int)
                {
                    int result = Convert.ToInt32(value) - 1;
                    target.Set(varName, result);
                }
                else if (target.Get(varName) is float)
                {
                    float result = Convert.ToSingle(value) - 1;
                    target.Set(varName, result);
                }
            }
            return base.VisitDecrement(context);
        }

        public string ParseString(ITerminalNode node)
        {
            string result = node.GetText();
            result = result.Substring(1, result.Length - 2);

            return result;
        }

        public override object VisitTerminal(ITerminalNode node)
        {
            switch (node.Symbol.Type)
            {
                case MegaScryptParser.True: return true;
                case MegaScryptParser.False: return false;
                case MegaScryptParser.Id: return node.GetText();
                case MegaScryptParser.Null: return null;
                case MegaScryptParser.String: return ParseString(node);
                case MegaScryptParser.Number:
                    {
                        string s = node.GetText();
                        if (s.Contains("."))
                        {
                            float f = float.Parse(s);
                            return f;
                        }
                        else
                        {
                            int i = int.Parse(s);
                            return i;
                        }
                    }

            }
            return base.VisitTerminal(node);
        }

        public override object VisitInstantiation([NotNull] MegaScryptParser.InstantiationContext context)
        {
            string objName = context.Id().GetText();
            Object obj = new Object();
            MegaScryptParser.KeyValuePairContext[] pairs = context.keyValuePairs().keyValuePair();
            foreach (MegaScryptParser.KeyValuePairContext data in pairs)
            {
                string varName = data.Id().GetText();
                object varValue = data.expression().Accept(this);
                obj.Declare(varName, varValue);
            }

            target.Declare(objName, obj);
            return base.VisitInstantiation(context);
        }

        public override object VisitCompoundIdentifier([NotNull] MegaScryptParser.CompoundIdentifierContext context)
        {
            Object currentObj = target;
            ITerminalNode[] ids = context.Id();
            for (int i = 0; i < ids.Length - 1; i++)
            {
                string objName = ids[i].GetText();
                if (currentObj.Has(objName))
                {
                    object result = currentObj.Get(objName);
                    if (result is Object)
                    {
                        currentObj = (Object)result;
                    }
                }
            }

            return currentObj.Get(ids[ids.Length - 1].GetText());
        }


        protected object GetValue([NotNull] ITerminalNode context)
        {
            string varName = context.GetText();
            object value = target.Get(varName);
            return value;
        }

        public override object VisitExpression([NotNull] MegaScryptParser.ExpressionContext context)
        {
            if (context.children.Count == 1)
            {
                if (context.Id() != null)
                    return GetValue(context.Id());

                object result = context.children[0].Accept(this);
                return result;
            }

            MegaScryptParser.ExpressionContext[] exprs = context.expression();
            if (exprs.Length == 1)
            {
                object result = exprs[0].Accept(this);
                if (context.Minus() != null)
                {
                    if (result is int)
                        result = -Convert.ToInt32(result);
                    else
                        result = -Convert.ToSingle(result);
                }
                else if (context.Exclamation() != null)
                {
                    result = !ToBoolean(result);
                }
                return result;
            }

            if (exprs.Length == 2)
            {
                object a = exprs[0].Accept(this);
                object b = exprs[1].Accept(this);
                ITerminalNode operatorNode = context.children[1] as ITerminalNode;
                return BinaryOperation(a, b, operatorNode.Symbol.Type);
            }
            throw new InvalidOperationException();

        }

        protected object BinaryOperation(object a, object b, int op)
        {
            if (a is int && b is int)
            {
                return IntegerBinaryOperation(a, b, op);
            }
            else if (a is bool && b is bool)
                return BooleanBinaryOperation(a, b, op);
            else if (a is string && b is string)
                return StringBinaryOperation(a, b, op);
            else
            {
                return FloatBinaryOperation(a, b, op);
            }
            throw new InvalidOperationException();
        }
        public static bool ToBoolean(object o)
        {
            if (o is bool)
                return (bool)o;

            throw new InvalidOperationException($"Unable to cast \"{o}\" as a boolean");
        }

        protected object StringBinaryOperation(object oa, object ob, int op)
        {
            string a = oa.ToString();
            string b = ob.ToString();
            switch (op)
            {
                case MegaScryptParser.Plus: return a + b;
                case MegaScryptParser.DoubleEquals: return a == b;
                case MegaScryptParser.NotEquals: return a != b;
            }

            throw new InvalidOperationException();
        }

        protected object BooleanBinaryOperation(object oa, object ob, int op)
        {
            bool a = ToBoolean(oa);
            bool b = ToBoolean(ob);
            switch (op)
            {
                case MegaScryptParser.DoubleAmp: return a && b;
                case MegaScryptParser.DoubleVertical: return a || b;
                case MegaScryptParser.DoubleEquals: return a == b;
                case MegaScryptParser.NotEquals: return a != b;
            }

            throw new InvalidOperationException();
        }

        protected object IntegerBinaryOperation(object oa, object ob, int op)
        {
            int a = Convert.ToInt32(oa);
            int b = Convert.ToInt32(ob);
            switch (op)
            {
                case MegaScryptParser.Plus: return a + b;
                case MegaScryptParser.Minus: return a - b;
                case MegaScryptParser.Multiply: return a * b;
                case MegaScryptParser.Divide: return a / b;
                case MegaScryptParser.Mod: return a % b;
                case MegaScryptParser.Smaller: return a < b;
                case MegaScryptParser.Greater: return a > b;
                case MegaScryptParser.SmallerEql: return a <= b;
                case MegaScryptParser.GreaterEql: return a >= b;
                case MegaScryptParser.DoubleEquals: return a == b;
                case MegaScryptParser.NotEquals: return a != b;
            }

            throw new InvalidOperationException();
        }

        protected object FloatBinaryOperation(object oa, object ob, int op)
        {
            float a = Convert.ToSingle(oa);
            float b = Convert.ToSingle(ob);
            switch (op)
            {
                case MegaScryptParser.Plus: return a + b;
                case MegaScryptParser.Minus: return a - b;
                case MegaScryptParser.Multiply: return a * b;
                case MegaScryptParser.Divide: return a / b;
                case MegaScryptParser.DoubleEquals: return a == b;
                case MegaScryptParser.NotEquals: return a != b;
            }

            throw new InvalidOperationException();
        }
        #endregion

        #region Functions

        public override object VisitInvocation([NotNull] MegaScryptParser.InvocationContext context)
        {
            object obj = GetValue(context.Id());

            return base.VisitInvocation(context);
        }

        public override object VisitParamList([NotNull] MegaScryptParser.ParamListContext context)
        {
            return base.VisitParamList(context);
        }


        #endregion
    }
}
