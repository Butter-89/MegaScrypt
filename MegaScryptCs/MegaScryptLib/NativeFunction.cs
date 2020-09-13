using System;
using System.Collections.Generic;
using System.Text;

namespace MegaScrypt
{
    public class NativeFunction : IFunction
    {
        private string name;
        public string Name => name;

        private List<string> parameterNames;
        public List<string> ParameterNames => parameterNames;

        public delegate object Callback(List<object> parameters);
        private Callback callback;

        public NativeFunction(Callback callback, IEnumerable<string> parameters)
        {
            this.callback = callback;
            this.parameterNames = parameters != null ? new List<string>(parameters) : null;
            this.name = callback.Method.Name;
        }

        public object Invoke(List<object> parameters)
        {
            object ret = callback.Invoke(parameters);
            return ret;
        }
    }
}
