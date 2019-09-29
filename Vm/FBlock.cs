using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFly.Vm
{
    internal class FBlock
    {
        private readonly Dictionary<string, FObject> _vars;
        private List<string> removeAfterClose = new List<string>();

        public FObject this[string key]
        {
            get => _vars[key];
            set
            {
                if(!_vars.ContainsKey(key))
                    removeAfterClose.Add(key);
                _vars[key] = value;
            }
        }

        public FBlock(Dictionary<string, FObject> vars)
        {
            _vars = vars;
        }

        public void Close()
        {
            foreach (var key in removeAfterClose)
            {
                _vars.Remove(key);
            }
        }
    }
}