using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VIPER.System_of_differential_equations
{
    public static class RungeKuttaMethod
    {
        public static double Func(double[] arr, int i, int n, float[,] func)
        {
            double res = 0;
           
            for (int j = 0; j < n; j++)
                res += arr[j] * func[i, j];
            return res;
        }
        public static List<List<double>> RungeKuttaMethods(int equationCount, int step, int maxTime, float[,] coefficients,double [] initialValues)
        {
            var result = new List<List<double>>();

            double[] k0 = new double[maxTime * equationCount];
            double[] k1 = new double[maxTime * equationCount];
            double[] k1_help = new double[equationCount];
            double[] k2 = new double[maxTime * equationCount];
            double[] k2_help = new double[equationCount];
            double[] k3 = new double[maxTime * equationCount];
            double[] k3_help = new double[equationCount];
            double[] res = new double[maxTime * equationCount];

           var k = 0;
            for (; k < maxTime; k+=step)
            {
                var resultTmpl = new List<double>();

                for (int i = 0; i < equationCount; i++)
                {
                    k0[i + k * equationCount] = Func(initialValues, i, equationCount, coefficients) * step;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k1_help[i] = initialValues[i] + k0[i + k * equationCount] / 2;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k1[i + k * equationCount] = Func(k1_help, i, equationCount, coefficients) * step;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k2_help[i] = initialValues[i] + k1[i + k * equationCount] / 2;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k2[i + k * equationCount] = Func(k2_help, i, equationCount, coefficients) * step;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k3_help[i] = initialValues[i] + k2[i + k * equationCount];
                }

                for (int i = 0; i < equationCount; i++)
                {
                    k3[i + k * equationCount] = Func(k3_help, i, equationCount, coefficients) * step;
                }

                for (int i = 0; i < equationCount; i++)
                {
                    res[i + k * equationCount] = initialValues[i] + 1.0 / 6.0 * (k0[i + k * equationCount] + 2 * k1[i + k * equationCount] + 2 * k2[i + k * equationCount] + k3[i + k * equationCount]);
                }

                for (int i = 0; i < equationCount; i++)
                {
                    initialValues[i] = res[i + k * equationCount];
                    resultTmpl.Add(res[i + k * equationCount]);
                }

                result.Add(resultTmpl);
            }
            return result;
        }
        
    
    }
}
