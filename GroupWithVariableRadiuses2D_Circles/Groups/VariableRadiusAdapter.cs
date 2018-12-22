﻿using System;
using System.Collections.Generic;
using System.Linq;
using hs071_cs.ObjectOptimazation;

namespace hs071_cs
{
    public class VariableRadiusAdapter : IOptimalPoints, IDisposable
    {
        public int _nele_hess = 0;
        private readonly int count = 0;
        private readonly int countFixR = 0;
        private readonly int countVarR = 0;
        private readonly Random _rnd = new Random();
        public readonly int _n;
        public int _m;
        public int _nele_jac;
        public double[] _x_L;
        public double[] _x_U;
        public double[] _g_L;
        public double[] _g_U;
        private readonly Circle2D[] Circles;
        private readonly double[] _ra;
        private readonly double[][] _raSum;
        private readonly int _groupCount;
        private readonly int[] _elemGroupCount;
        private readonly int[][] _indexElemInGroups;

        public double[] X { get; private set; }

        public VariableRadiusAdapter(Circle2D[] c, double[] trueRadiuses)
        {
            if (c.Length != trueRadiuses.Length)
            {
                throw new Exception("Size of arrays didn't matched!");
            }

            _ra = new double[trueRadiuses.Length];
            trueRadiuses.CopyTo(_ra, 0);
            Circles = new Circle2D[c.Length];
            for (int index = 0; index < c.Length; ++index)
            {
                Circles[index] = new Circle2D();
            }

            c.CopyTo(Circles, 0);
            count = c.Length;
            _groupCount = 0;
            for (int index = 0; index < count; ++index)
            {
                if ((uint)c[index].Group > 0U)
                {
                    ++countVarR;
                }

                if (_groupCount < c[index].Group)
                {
                    _groupCount = c[index].Group;
                }
            }
            ++_groupCount;
            _elemGroupCount = new int[_groupCount];
            _indexElemInGroups = new int[_groupCount][];
            for (int index = 0; index < count; ++index)
            {
                ++_elemGroupCount[c[index].Group];
            }

            countFixR = count - countVarR;
            _n = 2 * count + countVarR + 1;
            X = new double[_n];
            for (int index = 0; index < count; ++index)
            {
                X[2 * index] = Circles[index].Coordinate.X;
                X[2 * index + 1] = Circles[index].Coordinate.Y;
            }
            for (int index = 0; index < countVarR; ++index)
            {
                X[2 * count + index] = Circles[index].Radius;
            }

            X[_n - 1] = 20.0;
            _x_L = new double[_n];
            _x_U = new double[_n];
            for (int index = 0; index < count; ++index)
            {
                _x_L[2 * index] = Circles[index].Odz.xL;
                _x_U[2 * index] = Circles[index].Odz.xU;
                _x_L[2 * index + 1] = Circles[index].Odz.yL;
                _x_U[2 * index + 1] = Circles[index].Odz.yU;
            }
            for (int index = 0; index < countVarR; ++index)
            {
                _x_L[2 * count + index] = Circles[index].Odz.rL;
                _x_U[2 * count + index] = Circles[index].Odz.rU;
            }

            _x_L[_n - 1] = ((IEnumerable<double>)trueRadiuses).Max();
            _x_U[_n - 1] = c.Sum(t => t.Radius);

            _nele_jac = 0;
            _m = 0;
            _nele_jac += 3 * count + countVarR;
            _m += count;
            _nele_jac += 2 * count * (count - 1) + countVarR * (countVarR - 1) + countVarR * countFixR;
            _m += count * (count - 1) / 2;
            int num1 = 0;
            for (int index = 1; index < _groupCount; ++index)
            {
                num1 += (int)Math.Pow(2.0, _elemGroupCount[index]);
            }

            _m += num1;
            _nele_jac += CombinatorialHelper.GetOneCountInGroups(_elemGroupCount, _groupCount);
            _g_L = new double[_m];
            _g_U = new double[_m];
            int index1 = 0;
            for (int index2 = 0; index2 < count; ++index2)
            {
                _g_L[index1] = 0.0;
                _g_U[index1++] = 2E+19;
            }
            int num2 = 0;
            for (int index2 = 0; index2 < count - 1; ++index2)
            {
                for (int index3 = index2 + 1; index3 < count; ++index3)
                {
                    num2 += index2 < countVarR || index3 < countVarR ? 0 : 1;
                    _g_L[index1] = index2 < countVarR || index3 < countVarR ? 0.0 : Math.Pow(trueRadiuses[index2] + trueRadiuses[index3], 2.0);
                    _g_U[index1++] = 2E+19;
                }
            }
            double[][] elemInGroup = new double[_groupCount][];
            for (int index2 = 0; index2 < _groupCount; ++index2)
            {
                int num3 = 0;
                elemInGroup[index2] = new double[_elemGroupCount[index2]];
                for (int index3 = 0; index3 < count; ++index3)
                {
                    if (Circles[index3].Group == index2)
                    {
                        elemInGroup[index2][num3++] = trueRadiuses[index3];
                    }
                }
            }
            _raSum = CombinatorialHelper.GetRightPartInCombinatornOgr(elemInGroup);
            for (int index2 = 1; index2 < _groupCount; ++index2)
            {
                double num3 = Math.Pow(2.0, _elemGroupCount[index2]);
                for (int num_value = 1; num_value < num3; ++num_value)
                {
                    string str = CombinatorialHelper.DecToBase(num_value, 2, _elemGroupCount[index2]);
                    int num4 = 0;
                    for (int index3 = 0; index3 < str.Length; ++index3)
                    {
                        if (str[index3] == '1')
                        {
                            ++num4;
                        }
                    }
                    _g_L[index1] = _raSum[index2 - 1][num4 - 1];
                    _g_U[index1++] = num4 == _elemGroupCount[index2] ? _raSum[index2 - 1][_elemGroupCount[index2] - 1] : 2E+19;
                }
                _g_L[index1] = _raSum[index2 - 1][_elemGroupCount[index2]];
                _g_U[index1++] = _raSum[index2 - 1][_elemGroupCount[index2]];
            }
            for (int index2 = 0; index2 < _groupCount; ++index2)
            {
                _indexElemInGroups[index2] = new int[_elemGroupCount[index2]];
            }

            for (int index2 = 0; index2 < _groupCount; ++index2)
            {
                int num3 = 0;
                for (int index3 = 0; index3 < count; ++index3)
                {
                    if (Circles[index3].Group == index2)
                    {
                        _indexElemInGroups[index2][num3++] = index3;
                    }
                }
            }
        }

        public bool eval_f(int n, double[] x, bool new_x, out double obj_value)
        {
            obj_value = x[_n - 1];
            return true;
        }

        public bool eval_grad_f(int n, double[] x, bool new_x, double[] grad_f)
        {
            for (int index = 0; index < _n - 1; ++index)
            {
                grad_f[index] = 0.0;
            }

            grad_f[_n - 1] = 1.0;
            return true;
        }

        public bool eval_g(int n, double[] x, bool new_x, int m, double[] g)
        {
            int index1 = 0;
            for (int index2 = 0; index2 < countVarR; ++index2)
            {
                g[index1++] = Math.Pow(x[_n - 1] - x[2 * count + index2], 2.0) - x[2 * index2] * x[2 * index2] - x[2 * index2 + 1] * x[2 * index2 + 1];
            }

            for (int countVarR = this.countVarR; countVarR < count; ++countVarR)
            {
                g[index1++] = Math.Pow(x[_n - 1] - _ra[countVarR], 2.0) - x[2 * countVarR] * x[2 * countVarR] - x[2 * countVarR + 1] * x[2 * countVarR + 1];
            }

            for (int index2 = 0; index2 < count - 1; ++index2)
            {
                for (int index3 = index2 + 1; index3 < count; ++index3)
                {
                    if (index2 < countVarR && index3 < countVarR)
                    {
                        g[index1] = -Math.Pow(x[2 * count + index2] + x[2 * count + index3], 2.0);
                    }

                    if (index2 >= countVarR && index3 < countVarR)
                    {
                        g[index1] = -Math.Pow(x[2 * count + index3] + _ra[index2], 2.0);
                    }

                    if (index2 < countVarR && index3 >= countVarR)
                    {
                        g[index1] = -Math.Pow(x[2 * count + index2] + _ra[index3], 2.0);
                    }

                    g[index1++] += Math.Pow(x[2 * index2] - x[2 * index3], 2.0) + Math.Pow(x[2 * index2 + 1] - x[2 * index3 + 1], 2.0);
                }
            }
            double num1 = 0.0;
            for (int index2 = 1; index2 < _groupCount; ++index2)
            {
                double num2 = Math.Pow(2.0, _elemGroupCount[index2]);
                for (int num_value = 1; num_value < num2; ++num_value)
                {
                    string str = CombinatorialHelper.DecToBase(num_value, 2, _elemGroupCount[index2]);
                    for (int index3 = 0; index3 < str.Length; ++index3)
                    {
                        if (str[index3] == '1')
                        {
                            num1 += x[2 * count + _indexElemInGroups[index2][index3]];
                        }
                    }
                    g[index1++] = num1;
                    num1 = 0.0;
                }
                for (int index3 = 0; index3 < _elemGroupCount[index2]; ++index3)
                {
                    g[index1] += Math.Pow(x[2 * count + _indexElemInGroups[index2][index3]], 2.0);
                }

                ++index1;
            }
            return true;
        }

        public bool eval_jac_g(int n, double[] x, bool new_x, int m, int nele_jac, int[] iRow, int[] jCol, double[] values)
        {
            if (values == null)
            {
                int index1 = 0;
                int num1;
                for (num1 = 0; num1 < count; ++num1)
                {
                    iRow[index1] = num1;
                    int[] numArray1 = jCol;
                    int index2 = index1;
                    int index3 = index2 + 1;
                    int num2 = 2 * num1;
                    numArray1[index2] = num2;
                    iRow[index3] = num1;
                    int[] numArray2 = jCol;
                    int index4 = index3;
                    int index5 = index4 + 1;
                    int num3 = 2 * num1 + 1;
                    numArray2[index4] = num3;
                    if (num1 < countVarR)
                    {
                        iRow[index5] = num1;
                        jCol[index5++] = 2 * count + num1;
                    }
                    iRow[index5] = num1;
                    int[] numArray3 = jCol;
                    int index6 = index5;
                    index1 = index6 + 1;
                    int num4 = _n - 1;
                    numArray3[index6] = num4;
                }
                int num5 = 0;
                int num6 = 0;
                int num7 = 0;
                for (int index2 = 0; index2 < count - 1; ++index2)
                {
                    for (int index3 = index2 + 1; index3 < count; ++index3)
                    {
                        num7 += 4;
                        iRow[index1] = num1;
                        int[] numArray1 = jCol;
                        int index4 = index1;
                        int index5 = index4 + 1;
                        int num2 = 2 * index2;
                        numArray1[index4] = num2;
                        iRow[index5] = num1;
                        int[] numArray2 = jCol;
                        int index6 = index5;
                        int index7 = index6 + 1;
                        int num3 = 2 * index3;
                        numArray2[index6] = num3;
                        iRow[index7] = num1;
                        int[] numArray3 = jCol;
                        int index8 = index7;
                        int index9 = index8 + 1;
                        int num4 = 2 * index2 + 1;
                        numArray3[index8] = num4;
                        iRow[index9] = num1;
                        int[] numArray4 = jCol;
                        int index10 = index9;
                        index1 = index10 + 1;
                        int num8 = 2 * index3 + 1;
                        numArray4[index10] = num8;
                        if (index2 < countVarR && index3 < countVarR)
                        {
                            num7 += 2;
                            ++num6;
                            iRow[index1] = num1;
                            int[] numArray5 = jCol;
                            int index11 = index1;
                            int index12 = index11 + 1;
                            int num9 = 2 * count + index2;
                            numArray5[index11] = num9;
                            iRow[index12] = num1;
                            int[] numArray6 = jCol;
                            int index13 = index12;
                            index1 = index13 + 1;
                            int num10 = 2 * count + index3;
                            numArray6[index13] = num10;
                        }
                        if (index2 < countVarR && index3 >= countVarR)
                        {
                            ++num7;
                            ++num5;
                            iRow[index1] = num1;
                            jCol[index1++] = 2 * count + index2;
                        }
                        ++num1;
                    }
                }
                for (int index2 = 1; index2 < _groupCount; ++index2)
                {
                    double num2 = Math.Pow(2.0, _elemGroupCount[index2]);
                    for (int num_value = 1; num_value < num2; ++num_value)
                    {
                        string str = CombinatorialHelper.DecToBase(num_value, 2, _elemGroupCount[index2]);
                        for (int index3 = 0; index3 < str.Length; ++index3)
                        {
                            if (str[index3] == '1')
                            {
                                iRow[index1] = num1;
                                jCol[index1++] = 2 * count + _indexElemInGroups[index2][index3];
                            }
                        }
                        ++num1;
                    }
                    for (int index3 = 0; index3 < _elemGroupCount[index2]; ++index3)
                    {
                        iRow[index1] = num1;
                        jCol[index1++] = 2 * count + _indexElemInGroups[index2][index3];
                    }
                    ++num1;
                }
            }
            else
            {
                int num1 = 0;
                for (int index1 = 0; index1 < count; ++index1)
                {
                    double[] numArray1 = values;
                    int index2 = num1;
                    int num2 = index2 + 1;
                    double num3 = -2.0 * x[2 * index1];
                    numArray1[index2] = num3;
                    double[] numArray2 = values;
                    int index3 = num2;
                    int num4 = index3 + 1;
                    double num5 = -2.0 * x[2 * index1 + 1];
                    numArray2[index3] = num5;
                    if (index1 < countVarR)
                    {
                        double[] numArray3 = values;
                        int index4 = num4;
                        int num6 = index4 + 1;
                        double num7 = -2.0 * (x[_n - 1] - x[2 * count + index1]);
                        numArray3[index4] = num7;
                        double[] numArray4 = values;
                        int index5 = num6;
                        num1 = index5 + 1;
                        double num8 = 2.0 * (x[_n - 1] - x[2 * count + index1]);
                        numArray4[index5] = num8;
                    }
                    else
                    {
                        double[] numArray3 = values;
                        int index4 = num4;
                        num1 = index4 + 1;
                        double num6 = 2.0 * (x[_n - 1] - _ra[index1]);
                        numArray3[index4] = num6;
                    }
                }
                for (int index1 = 0; index1 < count - 1; ++index1)
                {
                    for (int index2 = index1 + 1; index2 < count; ++index2)
                    {
                        double[] numArray1 = values;
                        int index3 = num1;
                        int num2 = index3 + 1;
                        double num3 = 2.0 * (x[2 * index1] - x[2 * index2]);
                        numArray1[index3] = num3;
                        double[] numArray2 = values;
                        int index4 = num2;
                        int num4 = index4 + 1;
                        double num5 = -2.0 * (x[2 * index1] - x[2 * index2]);
                        numArray2[index4] = num5;
                        double[] numArray3 = values;
                        int index5 = num4;
                        int num6 = index5 + 1;
                        double num7 = 2.0 * (x[2 * index1 + 1] - x[2 * index2 + 1]);
                        numArray3[index5] = num7;
                        double[] numArray4 = values;
                        int index6 = num6;
                        num1 = index6 + 1;
                        double num8 = -2.0 * (x[2 * index1 + 1] - x[2 * index2 + 1]);
                        numArray4[index6] = num8;
                        if (index1 < countVarR && index2 < countVarR)
                        {
                            double[] numArray5 = values;

                            int index7 = num1;
                            int num9 = index7 + 1;
                            double num10 = -2.0 * (x[2 * count + index1] + x[2 * count + index2]);
                            numArray5[index7] = num10;
                            double[] numArray6 = values;
                            int index8 = num9;
                            num1 = index8 + 1;
                            double num11 = -2.0 * (x[2 * count + index1] + x[2 * count + index2]);
                            numArray6[index8] = num11;
                        }
                        if (index1 < countVarR && index2 >= countVarR)
                        {
                            values[num1++] = -2.0 * (x[2 * count + index1] + _ra[index2]);
                        }
                    }
                }
                for (int index1 = 1; index1 < _groupCount; ++index1)
                {
                    double num2 = Math.Pow(2.0, _elemGroupCount[index1]);
                    for (int num_value = 1; num_value < num2; ++num_value)
                    {
                        foreach (char ch in CombinatorialHelper.DecToBase(num_value, 2, _elemGroupCount[index1]))
                        {
                            if (ch == '1')
                            {
                                values[num1++] = 1.0;
                            }
                        }
                    }
                    for (int index2 = 0; index2 < _elemGroupCount[index1]; ++index2)
                    {
                        values[num1++] = 2.0 * x[2 * count + _indexElemInGroups[index1][index2]];
                    }
                }
            }
            return true;
        }

        public bool eval_h(int n, double[] x, bool new_x, double obj_factor, int m, double[] lambda, bool new_lambda, int nele_hess, int[] iRow, int[] jCol, double[] values)
        {
            return false;
        }

        public void Dispose()
        {
            _m = 0;
            for (int index = 0; index < count; ++index)
            {
                X[2 * index] = 0.0;
                X[2 * index + 1] = 0.0;
                if (index < countVarR)
                {
                    X[2 * count + index] = 0.0;
                }
            }
            X[_n - 1] = 0.0;
        }
    }
}
