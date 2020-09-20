using System;
using System.Collections.Generic;
using System.Text;

namespace MegaScrypt
{
    interface IFunction
    {
        string Name { get; }

        List<string> ParameterNames { get; }

        object Invoke(List<object> parameters, InvocationContext ctx = null);
    }
}
