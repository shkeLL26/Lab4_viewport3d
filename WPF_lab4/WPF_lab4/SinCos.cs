using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_lab4
{
    internal class SinCos : IFunction
    {
        public double calc(double x, double y)
        {
            return Math.Sin(x) + Math.Cos(y);
        }
    }
}
