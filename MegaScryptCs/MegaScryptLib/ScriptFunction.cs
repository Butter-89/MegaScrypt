using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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

        public ScriptFunction(MegaScryptParser.FuncDeclarationContext declContext)
        {

        }
    }
}
