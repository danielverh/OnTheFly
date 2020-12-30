using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OnTheFly.Vm.Runtime.Exceptions;

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
        public int StackCount { get; }
        public FBlock Previous { get; }
        public FObject this[string key]
        {
            get
            {
                if (_vars.ContainsKey(key))
                    return _vars[key];
                if (Previous != null)
                {
                    return Previous[key];
                }
                throw new RuntimeException($"Variable {key} not found in current scope.");
            }
            set
            {
                if(!_vars.ContainsKey(key))
                    removeAfterClose.Add(key);
                _vars[key] = value;
            }
        }

        public bool Contains(string key)
        {
            return _vars.ContainsKey(key);
        }
        public FBlock(Dictionary<string, FObject> vars, int stackCount, FBlock previous, bool loop = false, int endPos =0)
        {
            _vars = vars;
            Loop = loop;
            EndPos = endPos;
            StackCount = stackCount;
            Previous = previous;
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