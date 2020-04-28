using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*namespace VIPER.System_of_differential_equations
{

    //   public static List<List<double>> RungeKuttaMethods(int equationCount, int step, int maxTime, double[,] coefficients,double [] initialValues)
    public class Runge
    {
        public double[] initialValues { get; set; }
        public double[] equationValues { get; set; }

        public int equationCount { get; set; }

        public Runge(int _equationCount)
        {
            this.equationCount = _equationCount;
            initialValues = new double[_equationCount];
            equationValues = new double[_equationCount];
        }

        public double[] getStepResult(int x, int dx, double[] y)
        {
            double[] res = new double[_equationCount];
            double[] tmp = new double[_equationCount];

            var y1 = _eqs(x, y);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y1[i] * (dx / 2.0);
            }

            var y2 = _eqs(x + dx / 2.0, tmp);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y2[i] * (dx / 2.0);
            }

            var y3 = _eqs(x + dx / 2.0, tmp);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y3[i] * dx;
            }

            var y4 = _eqs(x + dx, tmp);
            for (let i = 0; i < y.length; i++)
            {
                res[i] = y[i] + dx / 6.0 * (y1[i] + 2.0 * y2[i] + 2.0 * y3[i] + y4[i]);
            }

            return res;
        }
    }

    /*

function RungeKutta()
    {
        var _y;
        var _eqs;

        var getStepResult = function(x, dx, y) {
            var res = new Array(_y.length);
            var tmp = new Array(_y.length);

            var y1 = _eqs(x, y);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y1[i] * (dx / 2.0);
            }

            var y2 = _eqs(x + dx / 2.0, tmp);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y2[i] * (dx / 2.0);
            }

            var y3 = _eqs(x + dx / 2.0, tmp);
            for (let i = 0; i < y.length; i++)
            {
                tmp[i] = y[i] + y3[i] * dx;
            }

            var y4 = _eqs(x + dx, tmp);
            for (let i = 0; i < y.length; i++)
            {
                res[i] = y[i] + dx / 6.0 * (y1[i] + 2.0 * y2[i] + 2.0 * y3[i] + y4[i]);
            }

            return res;
        };

        this.init = function(y0, eqs) {
            _y = y0;
            _eqs = eqs;
        };

        this.getSolution = function(x0, xMax, dx) {
            var res = [];
            var y = _y;
            for (let x = x0; x <= xMax; x += dx)
            {
                y = getStepResult(x, dx, y);
                res.push({ x: x, data: y});
        }

        return res;
    };*/

