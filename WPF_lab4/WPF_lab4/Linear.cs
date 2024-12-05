using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_lab4
{
    internal class Linear : IFunction
    {
        public double calc(double x, double y)
        {
            return (-0.3 * x + 0.1 * y);
        }
    }
}
