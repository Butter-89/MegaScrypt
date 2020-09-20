using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Antlr4.Runtime.Tree;

namespace MegaScrypt
{
    class ScriptFunction : IFunction
    {
        private string name;
        public string Name => name;

        private List<string> parameterNames;
        public List<string> ParameterNames => parameterNames;

        private MegaScryptParser.FuncDeclarationContext declContext;
        public MegaScryptParser.FuncDeclarationContext DeclarationContext => declContext;

        public delegate object Invocation(ScriptFunction function, List<object> parameters, InvocationContext ctx);
        Invocation invocation;

        public ScriptFunction(Processor processor, Invocation invocation, MegaScryptParser.FuncDeclarationContext declContext)
        {
            this.declContext = declContext;
            this.name = TryFindName(processor);
            // Parameters
            if (declContext.varList() != null)
            {
                this.parameterNames = declContext.varList().Accept(processor) as List<string>;
            }
            else
                this.parameterNames = new List<string>();

            this.invocation = invocation;
        }

        private string TryFindName(Processor processor)
        {
            IRuleNode node = declContext.Parent;
            while(node != null)
            {
                MegaScryptParser.DeclarationContext varContext = node as MegaScryptParser.DeclarationContext;
                if(varContext != null)
                {
                    return varContext.Id().Accept(processor) as string;
                }

                node = node.Parent;
            }
            return null;
        }

        public object Invoke(List<object> parameters, InvocationContext ctx = null)
        {
            return invocation.Invoke(this, parameters, ctx);
        }
    }
}
