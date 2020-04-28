using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPER.System_of_differential_equations
{
    public static class Integral
    {
        public static double RectangleMethod(Dictionary<double,double> items, double intervalBegin, double intervalEnd,int iterations)
        {
            double integral = 0;
            double step;

            step = (intervalEnd - intervalBegin) / iterations;
            for (int i = 1; i <= iterations; ++i)
            {
                integral += step * items[(intervalBegin + (i - 1) * step)];
            }
            return integral;
        }

        public static double TrapeziumMethod(Dictionary<double, double> items, double intervalBegin, double intervalEnd, int iterations)
        {
            double integral = 0;
            double step;

            step = (intervalEnd - intervalBegin) / iterations;
            integral = 0.5 * (items[(intervalBegin)] + items[(intervalEnd)]);
            for (int i = 1; i < iterations; ++i)
            {
                integral += items[(intervalBegin + step * i)];
            }
            integral *= step;


            return integral;
        }
    }
}
