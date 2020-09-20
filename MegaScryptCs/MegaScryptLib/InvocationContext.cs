using System;
using System.Collections.Generic;
using System.Text;

namespace MegaScrypt
{
    public class InvocationContext
    {
        private Object container;
        public Object Container => container;

        public InvocationContext(Object container)
        {
            this.container = container;
        }
    }
}
