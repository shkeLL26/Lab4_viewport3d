using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_lab4
{
    internal class Abs : IFunction
    {
        public double calc(double x, double y)
        {
            return -Math.Abs((x+y)/4);
            //return Math.Log10(x) + Math.Log10(y);
        }
    }
}
