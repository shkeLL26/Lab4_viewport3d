using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_lab4
{
    internal class Arctan : IFunction
    {
        public double calc(double x, double y)
        {
            return Math.Atan(x - y);
        }
    }
}
