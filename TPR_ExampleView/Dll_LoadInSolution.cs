using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPR_ExampleView
{
    public class Dll_LoadInSolution : List<String>
    {
        public static Dll_LoadInSolution List { get; } = new Dll_LoadInSolution
        {
            "TestLibrary",
        };
    }
}
