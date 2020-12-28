using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheFly.Vm
{
    /// <summary>
    /// The FBlock class keeps track of the context required for function and variable scopes
    /// </summary>
    internal class FBlock
    {
        private readonly Dictionary<string, FObject> _vars;
        private List<string> removeAfterClose = new List<string>();
        public bool Loop;
        public int EndPos = 0;
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

        public FBlock(Dictionary<string, FObject> vars, bool loop = false, int endPos =0)
        {
            _vars = vars;
            Loop = loop;
            EndPos = endPos;
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