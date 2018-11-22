using System;
using BooleanConfiguration;

namespace hs071_cs
{
    // TODO: TO REALISE IPOPT_adapter
    internal class IPOPTAdapter : BaseAdaptor, IDisposable
    {
        public IPOPTAdapter(Data data)
        {

        }

        /*
        public double[] X { get; } //массив х (входят все координаты и радиусы)

        //public FixedRadius3dAdaptor(Data data)
        //{
        //    AllIteration = new List<double[]>();

        //    countCircles = data.ballCount; // задаём количетво кругов
        //    _n = countCircles * 3 + 1; // количество переменных в векторе
        //    radius = new double[countCircles];
        //    unsortedRadius = new double[countCircles];
        //    //вспомогательные счетчики
        //    double sumR = 0;
        //    int it = 0;

        //    foreach (var item in data.ball)
        //    {
        //        radius[it++] = item.R;
        //        sumR += item.R;
        //    }
        //    radius = radius.OrderBy(a => a).ToArray();
        //    X = new double[_n];
        //    try
        //    {
        //        if (data.ball != null && data != null && data.R >= 0)
        //        {
        //            for (int i = 0; i < countCircles; i++)
        //            {
        //                X[3 * i] = data.ball[i].X;
        //                X[3 * i + 1] = data.ball[i].Y;
        //                X[3 * i + 2] = data.ball[i].Z;
        //            }
        //            X[_n - 1] = data.R;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        new PrintErrorMessageDel(OutPut.ErrorMessage)(ex.Message);
        //    }

        //    /*    Ограничения переменных
        //    * *************************************************************************************/

        //    if (countCircles <= 10)
        //        K1 = 1;
        //    else
        //        K1 = 0.4;

        //    _x_L = new double[_n];
        //    _x_U = new double[_n];

        //    double max = radius[countCircles - 1];

        //    // обнуляем необходимые счетчики
        //    int countXYZR = 0,
        //        countCoordinate = 0;
        //    for (int i = 0; i < countCircles; i++)
        //    {
        //        //if (data.ball[i].ObjectType == (ObjectType)2)
        //        //{
        //        //    _x_L[3 * i] = data.ball[i].X;
        //        //    _x_U[3 * i] = data.ball[i].X;

        //        //    _x_L[3 * i + 1] = data.ball[i].Y;
        //        //    _x_U[3 * i + 1] = data.ball[i].Y;

        //        //    _x_L[3 * i + 2] = data.ball[i].Z;
        //        //    _x_U[3 * i + 2] = data.ball[i].Z;
        //        //}
        //        //else
        //        //{
        //        _x_L[3 * i] = Ipopt.NegativeInfinity;// -K1 * DiametrSum(radius) + radius[countCoordinate];
        //        _x_U[3 * i] = Ipopt.PositiveInfinity;// K1 * DiametrSum(radius) - radius[countCoordinate];

        //        _x_L[3 * i + 1] = Ipopt.NegativeInfinity; // - K1 * DiametrSum(radius) + radius[countCoordinate];
        //        _x_U[3 * i + 1] = Ipopt.PositiveInfinity;// K1 * DiametrSum(radius) - radius[countCoordinate];

        //        _x_L[3 * i + 2] = Ipopt.NegativeInfinity;// - K1 * DiametrSum(radius) + radius[countCoordinate];
        //        _x_U[3 * i + 2] = Ipopt.PositiveInfinity; // K1 * DiametrSum(radius) - radius[countCoordinate];
        //        //}
        //        if (max < radius[countCoordinate])
        //            max = radius[countCoordinate];
        //        countCoordinate++;
        //        countXYZR++;
        //    }

        //    _x_L[_n - 1] = max;
        //    _x_U[_n - 1] = sumR * K1;

        //    /*    Огрaничения
        //     **************************************************************************************/
        //    _nele_jac = 0;
        //    _m = 0;

        //    // (R-r[i])^2-x[i]^2-y[i]^2 -z[i]^2 >= 0
        //    _nele_jac += 4 * countCircles; // x, y, z , R
        //    _m += countCircles;

        //    // (x[i]-x[j])^2 + (y[i]-y[j])^2 + (z[i]-z[j])^2 - (r[i]-r[j])^2 >=0
        //    int v = 3 * countCircles * (countCircles - 1);
        //    _nele_jac += v;  // 2*3 два ограничения по 3 ненулевых частных производных
        //    int v1 = countCircles * (countCircles - 1) / 2;
        //    _m += v1;

        //    //m[i]*x[i] + count
        //    // _nele_jac += 3 * countCircles;
        //    // _m += countCircles;

        //    _g_L = new double[_m];
        //    _g_U = new double[_m];
        //    int op = 0;
        //    for (int j = 0; j < countCircles; j++) // радиусы от 0 до MAX
        //    {
        //        _g_L[op] = 0;
        //        _g_U[op++] = Ipopt.PositiveInfinity;
        //    }
        //    for (int i = 0; i < countCircles - 1; i++)
        //    {
        //        for (int j = i + 1; j < countCircles; j++)
        //        {
        //            _g_L[op] = Math.Pow((radius[i] + radius[j]), 2);
        //            _g_U[op++] = Ipopt.PositiveInfinity;
        //        }
        //    }

        //    double eps = Ipopt.PositiveInfinity;

        //    //Constraints of variebles For  Weight[i] * X[i] + Weight[i] * Y[i] + Weight[i] * Z[i]
        //    //for (int i = 0; i < countCircles; i++)
        //    //{
        //    //    _g_L[op] = 0;
        //    //    _g_U[op++] = eps;
        //    //}

        //    _nele_hess = 0;

        //} // End_Конструктор 

        public override bool Eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            // R -> min
            obj_value = 1;// K2 * x[_n - 1] + K2 * C_Multiply_by_Length(x);
            return true;
        }

        public override bool Eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            ///grad_f[_n - 1] = K1 * 1;
            return true;
        }

        public override bool Eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            return true;
        }

        public override bool Eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            return true;
        }

        public override bool Eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        public void Dispose()
        {
            _m = 0;
        }
    }
}
