using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace OnTheFly
{
    public class Repl
    {
        private VirtualMachine vm;
        public Repl()
        {
        }

        public void Run()
        {
            while (true)
            {
                Console.Write(">>> ");
                var input = Console.ReadLine();
                if(input == null)
                    continue;
                var items = FlyCode.Parse(input);
                if(vm == null)
                    vm = new VirtualMachine(items.Instructions, items.Contexts);
                else
                    vm.AddCode(items.Instructions, items.Contexts);
                try
                {
                    vm.Run();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
