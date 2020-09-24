using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.SymbolStore;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
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

        private Object container; // To temporarilyy store the current Object it's in
        private string idText;  // To temporarilly store the content of Id
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
            else if (context.instantiation() != null)
            {
                Object obj = DeclareStruct(context.instantiation());
                target.Declare(varName, obj);
                return base.VisitInstantiation(context.instantiation());
            }
            target.Declare(varName, value);
            return value;
        }

        private Object DeclareStruct(MegaScryptParser.InstantiationContext ctx)
        {
            Object obj = new Object();
            MegaScryptParser.KeyValuePairContext[] pairs = ctx.keyValuePairs().keyValuePair();
            foreach (MegaScryptParser.KeyValuePairContext data in pairs)
            {
                string dataName = data.Id().GetText();
                object dataValue = null;
                if (data.expression() != null)
                    dataValue = data.expression().Accept(this);
                else if (data.instantiation() != null)
                {
                    dataValue = DeclareStruct(data.instantiation());
                }

                obj.Declare(dataName, dataValue);
            }

            return obj;
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
            string varName = null;    // Todo: switch to adapt CompoundId
            Object prevTarget = target;
            //string varName = context.id().Accept(this) as string;
            if (context.compoundIdentifier() != null)
            {
                result = context.compoundIdentifier().Accept(this);
                varName = idText;
                target = container;
                //return result;
            }
            
            if (context.instantiation() != null)
            {
                result = DeclareStruct(context.instantiation());
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
                else if (context.Equals() != null)
                    result = exprValue;
            }


            if (result == null)
                throw new InvalidOperationException();

            target.Set(varName, result);
            target = prevTarget;
            return result;
        }

        public override object VisitIncrement([NotNull] MegaScryptParser.IncrementContext context)
        {
            object varValue = context.compoundIdentifier().Accept(this);
            string varName = idText;
            Object prevTarget = target;
            target = container;

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
            target = prevTarget;
            return base.VisitIncrement(context);
        }

        public override object VisitDecrement([NotNull] MegaScryptParser.DecrementContext context)
        {
            object varValue = context.compoundIdentifier().Accept(this);
            string varName = idText;
            Object prevTarget = target;
            target = container;
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
            target = prevTarget;
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

        //public override object VisitInstantiation([NotNull] MegaScryptParser.InstantiationContext context)
        //{
        //    string objName = context.Id().GetText();
        //    Object obj = new Object();
        //    MegaScryptParser.KeyValuePairContext[] pairs = context.keyValuePairs().keyValuePair();
        //    foreach (MegaScryptParser.KeyValuePairContext data in pairs)
        //    {
        //        string varName = data.Id().GetText();
        //        object varValue = data.expression().Accept(this);
        //        obj.Declare(varName, varValue);
        //    }

        //    target.Declare(objName, obj);
        //    return base.VisitInstantiation(context);
        //}

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

            container = currentObj;
            idText = ids[ids.Length - 1].GetText();
            return currentObj.Get(idText);
        }

        // Pass in an Id and retrieve its value
        protected object GetValue([NotNull] ITerminalNode context)
        {
            string varName = context.GetText();
            object value = target.Get(varName);
            return value;
        }

        public override object VisitExpression([NotNull] MegaScryptParser.ExpressionContext context)
        {
            if(context.compoundIdentifier() != null)
            {
                return context.compoundIdentifier().Accept(this);
            }

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
            float f_a = 0f;
            float f_b = 0f;
            if (a is float && b is int)
                f_b = Convert.ToSingle(b);
            else if (a is int && b is float)
                f_a = Convert.ToSingle(a);

            if (a is int && b is int)
            {
                return IntegerBinaryOperation(a, b, op);
            }
            else if (a is float && b is float)
                return FloatBinaryOperation(a, b, op);
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
                case MegaScryptParser.Smaller: return a < b;
                case MegaScryptParser.Greater: return a > b;
                case MegaScryptParser.SmallerEql: return a <= b;
                case MegaScryptParser.GreaterEql: return a >= b;
                case MegaScryptParser.DoubleEquals: return a == b;
                case MegaScryptParser.NotEquals: return a != b;
            }

            throw new InvalidOperationException();
        }
        #endregion

        #region Functions

        public override object VisitStatement([NotNull] MegaScryptParser.StatementContext context)
        {
            if (returned)
                return lastReturnedValue;
            return base.VisitStatement(context);
        }

        public override object VisitFuncDeclaration([NotNull] MegaScryptParser.FuncDeclarationContext context)
        {
            ScriptFunction function = new ScriptFunction(this, Invoke, context);
            return function;
        }

        public object Invoke(ScriptFunction function, List<object> parameters, InvocationContext ctx)
        {
            // Scope
            Object prevTarget = target;
            Object parentScope = ctx != null && ctx.Container != null ? ctx.Container : prevTarget;
            target = new Object(parentScope);

            // Declare variables
            if(parameters != null)
            {
                if (function.ParameterNames != null && function.ParameterNames.Count != parameters.Count)
                    throw new InvalidOperationException($"Function {function.Name} expected {function.ParameterNames.Count} parameters but received {parameters.Count}.");

                for(int i = 0; i < function.ParameterNames.Count; i++)
                {
                    target.Declare(function.ParameterNames[i], parameters[i]);
                }
            }

            lastReturnedValue = null;
            returned = false;

            // execute body
            base.VisitFuncDeclaration(function.DeclarationContext);

            // Pop scope
            target = prevTarget;

            //return value
            object ret = lastReturnedValue;
            lastReturnedValue = null;
            returned = false;

            return ret;
        }

        public override object VisitInvocation([NotNull] MegaScryptParser.InvocationContext context)
        {
            object obj = context.compoundIdentifier().Accept(this);
            IFunction function = obj as IFunction;
            if(function == null)
            {
                throw new Exception("Invalid function call!");
            }

            List<object> parameters;
            if (context.paramList() != null)
            {
                parameters = context.paramList().Accept(this) as List<object>;
            }
            else
                parameters = new List<object>();

            InvocationContext invCtx = new InvocationContext(container);
            

            object ret = function.Invoke(parameters, invCtx);
            return ret;
        }

        public override object VisitParamList([NotNull] MegaScryptParser.ParamListContext context)
        {
            List<object> parameters = new List<object>();
            MegaScryptParser.ExpressionContext[] exprs = context.expression();
            foreach(MegaScryptParser.ExpressionContext expr in exprs)
            {
                object result = expr.Accept(this);
                parameters.Add(result);
            }

            return parameters;
        }

        public override object VisitVarList([NotNull] MegaScryptParser.VarListContext context)
        {
            List<string> varList = new List<string>();

            ITerminalNode[] ids = context.Id();
            foreach(ITerminalNode id in ids)
            {
                string result = id.Accept(this) as string;
                varList.Add(result);
            }

            return varList;
        }

        private object lastReturnedValue;
        private bool returned = false;
        public override object VisitReturnStmt([NotNull] MegaScryptParser.ReturnStmtContext context)
        {
            if (context.expression() != null)
                lastReturnedValue = context.expression().Accept(this);
            else
                lastReturnedValue = null;

            returned = true;
            return lastReturnedValue;
        }
        #endregion
    }
}
