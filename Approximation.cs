using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Double;


namespace Projekt4_Aproksymacja
{
    class Approximation
    {
        public static List<Double> solveApproximation(int order, List<Double> domain, List<Double> range)
        {
            if (domain.Count != range.Count)
                throw new ArgumentException("Domain should have as many values as range");
            switch (order)
            {
                case 1:
                    {
                        List<Double> result = new List<Double>();
                        int domainCount = domain.Count;

                        double[] domainSecondPower = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                            domainSecondPower[i] = domain[i] * domain[i];
                        double[] domainTimesRange = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                            domainTimesRange[i] = domain[i] * range[i];

                        double denominator = domainCount * domainSecondPower.Sum() - (domain.Sum() * domain.Sum());
                        result.Add((domainSecondPower.Sum() * range.Sum() - (domain.Sum() * domainTimesRange.Sum())) / denominator);
                        result.Add((domainCount * domainTimesRange.Sum() - (domain.Sum() * range.Sum())) / denominator);

                        return result;
                    }
                case 2:
                    {
                        int domainCount = domain.Count;

                        double[] domainSecondPower = new double[domainCount];
                        double[] domainThirdPower = new double[domainCount];
                        double[] domainFourthPower = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                        {
                            domainSecondPower[i] = domain[i] * domain[i];
                            domainThirdPower[i] = domainSecondPower[i] * domain[i];
                            domainFourthPower[i] = domainThirdPower[i] * domain[i];
                        }
                        double[] domainTimesRange = new double[domainCount];
                        double[] domainTimesRangeSecondPower = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                        {
                            domainTimesRange[i] = domain[i] * range[i];
                            domainTimesRangeSecondPower[i] = domain[i] * domainTimesRange[i];
                        }

                        var A = Matrix.Build.DenseOfArray(new double[,] {
                            { domainCount, domain.Sum(), domainSecondPower.Sum() },
                            { domain.Sum(), domainSecondPower.Sum(), domainThirdPower.Sum() },
                            { domainSecondPower.Sum() ,domainThirdPower.Sum(), domainFourthPower.Sum() }
                        });
                        var b = Vector.Build.Dense(new double[] { range.Sum(), domainTimesRange.Sum(), domainTimesRangeSecondPower.Sum() });
                        var x = A.Solve(b);

                        return new List<Double>(x);
                    }
                case 3:
                    {
                        int domainCount = domain.Count;

                        double[] domainSecondPower = new double[domainCount];
                        double[] domainThirdPower = new double[domainCount];
                        double[] domainFourthPower = new double[domainCount];
                        double[] domainFifthPower = new double[domainCount];
                        double[] domainSixthPower = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                        {
                            domainSecondPower[i] = domain[i] * domain[i];
                            domainThirdPower[i] = domainSecondPower[i] * domain[i];
                            domainFourthPower[i] = domainThirdPower[i] * domain[i];
                            domainFifthPower[i] = domainFourthPower[i] * domain[i];
                            domainSixthPower[i] = domainFifthPower[i] * domain[i];
                        }
                        double[] domainTimesRange = new double[domainCount];
                        double[] domainTimesRangeSecondPower = new double[domainCount];
                        double[] domainTimesRangeThirdPower = new double[domainCount];
                        for (int i = 0; i < domainCount; i++)
                        {
                            domainTimesRange[i] = domain[i] * range[i];
                            domainTimesRangeSecondPower[i] = domain[i] * domainTimesRange[i];
                            domainTimesRangeThirdPower[i] = domain[i] * domainTimesRangeSecondPower[i];
                        }

                        var A = Matrix.Build.DenseOfArray(new double[,] {
                            { domainCount, domain.Sum(), domainSecondPower.Sum(), domainThirdPower.Sum() },
                            { domain.Sum(), domainSecondPower.Sum(), domainThirdPower.Sum(), domainFourthPower.Sum() },
                            { domainSecondPower.Sum(), domainThirdPower.Sum(), domainFourthPower.Sum(), domainFifthPower.Sum() } ,
                            { domainThirdPower.Sum() ,domainFourthPower.Sum(), domainFifthPower.Sum(), domainSixthPower.Sum() }
                        });
                        var b = Vector.Build.Dense(new double[] { range.Sum(), domainTimesRange.Sum(), domainTimesRangeSecondPower.Sum(), domainTimesRangeThirdPower.Sum() });
                        var x = A.Solve(b);

                        return new List<Double>(x);
                    }
                default:
                    return null;
            }
        }
    }
}
