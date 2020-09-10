using System;
using System.Collections.Generic;

namespace MegaScrypt
{
    class Object
    {
        private Object parent = null;

        public Object(Object parent = null)
        {
            this.parent = parent;
        }

        public void Declare(string varName, object value = null)
        {
            throw new NotImplementedException();
        }

        public object Get(string varName, bool allowParentChaining = true)
        {
            throw new NotImplementedException();
        }

        public void Set(string varName, object value, bool allowParentChaining = true)
        {
            throw new NotImplementedException();
        }

        public bool Has(string varName, bool allowParentChaining = true)
        {
            throw new NotImplementedException();
        }
    }
}
