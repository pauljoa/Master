using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.Utility
{
    static class DllLoader
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dll);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr module, string procName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr module);
    }

}
