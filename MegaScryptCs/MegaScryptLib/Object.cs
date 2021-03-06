﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MegaScrypt
{
    public class Object
    {
        private Object parent = null;
        public Object Parent => parent;

        private Dictionary<string, object> variables = new Dictionary<string, object>();
        public List<string> VariableNames => new List<string>(variables.Keys);

        public Object(Object parent = null)
        {
            this.parent = parent;
        }


        public void Declare(string varName, object value = null)
        {
            if(varName == "prototype")
            {
                parent = value as Object;
                return;
            }

            if (variables.ContainsKey(varName))
                throw new InvalidOperationException($"Variable \"{varName}\" is already declared.");
            variables.Add(varName, value);
        }

        public object Get(string varName, bool allowParentChaining = true)
        {
            if (varName == "prototype")
            {
                return parent;
            }

            if (variables.ContainsKey(varName))
            {
                return variables[varName];
            }

            if (allowParentChaining && parent != null)
            {
                return parent.Get(varName, true);
            }
            throw new InvalidOperationException($"Variable \"{varName}\" is not declared - get.");
        }

        public void Set(string varName, object value, bool allowParentChaining = true)
        {
            if (varName == "prototype")
            {
                parent = value as Object;
                return;
            }
            if (variables.ContainsKey(varName))
            {
                variables[varName] = value;
                return;
            }

            if (allowParentChaining && parent != null)
            {
                parent.Set(varName, value, true);
                return;
            }

            throw new InvalidOperationException($"Variable \"{varName}\" is not declared - set.");
        }

        public bool Has(string varName, bool allowParentChaining = true)
        {
            if (varName == "prototype")
            {
                return parent != null;
            }
            bool has = variables.ContainsKey(varName);
            if (!has && allowParentChaining && parent != null)
            {
                return parent.Has(varName, true);
            }
            return has;
        }
    }
}
