using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Projekt4_Aproksymacja
{
    class Program
    {
        static void Main(string[] args)
        {
            Random random = new Random();
            System.IO.Directory.CreateDirectory("wynikiApro");
            LinearEquation le1;
            System.Diagnostics.Stopwatch watch;
            long buildTime, workTime;

            List<Double> gaussDomain = new List<Double>();
            List<Double> optimizedGaussDomain = new List<Double>();
            List<Double> gaussSeidelDomain = new List<Double>();
            List<Double> mathNetDomain = new List<Double>();
            List<Double> gaussRange = new List<Double>();
            List<Double> optimizedGaussRange = new List<Double>();
            List<Double> gaussSeidelRange = new List<Double>();
            List<Double> mathNetRange = new List<Double>();

            #region Zad 1
            StringBuilder building = new StringBuilder(), working = new StringBuilder();
            building.Append("Agenci;Metoda;Czas budowania\n");
            working.Append("Agenci;Metoda;Czas rozwiazywania\n");

            int start = 2;
            int maxAgents = 30;

            for (int i = start; i <= maxAgents; i++)
            {
                le1 = new LinearEquation(i);
                double precision = 0.0000000001;
                Console.WriteLine("Agenci: " + i);

                // Partial Gauss
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.ElapsedMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.SolveEquationPartialPivotingAlgorithm(false);
                watch.Stop();
                workTime = watch.ElapsedMilliseconds;
                if (i > start)
                {
                    gaussDomain.Add(i);
                    gaussRange.Add(workTime);
                }
                building.Append(i + ";Gauss z częsciowym wyborem;" + buildTime + "\n");
                working.Append(i + ";Gauss z częsciowym wyborem;" + workTime + "\n");

                // Zopymalizowany Gauss
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.ElapsedMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.SolveEquationPartialPivotingAlgorithm(true);
                watch.Stop();
                workTime = watch.ElapsedMilliseconds;
                if (i > start)
                {
                    optimizedGaussDomain.Add(i);
                    optimizedGaussRange.Add(workTime);
                }

                building.Append(i + ";Zoptymalizowany Gauss z częsciowym wyborem;" + buildTime + "\n");
                working.Append(i + ";Zoptymalizowany Gauss z częsciowym wyborem;" + workTime + "\n");

                // Gauss-Seidel
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.ElapsedMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.GaussSeidelIterationAlgorithm(precision);
                watch.Stop();
                workTime = watch.ElapsedMilliseconds;
                if (i > start)
                {
                    gaussSeidelDomain.Add(i);
                    gaussSeidelRange.Add(workTime);
                }

                building.Append(i + ";Gauss-Seidel;" + buildTime + "\n");
                working.Append(i + ";Gauss-Seidel;" + workTime + "\n");

                // Biblioteka MathNet
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.ElapsedMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                var A = SparseMatrix.Build.DenseOfArray(le1.Matrix.Matrix);
                var b = Vector.Build.Dense(le1.Vector.Values);
                var x = A.Solve(b);
                watch.Stop();
                workTime = watch.ElapsedMilliseconds;
                if (i > start)
                {
                    mathNetDomain.Add(i);
                    mathNetRange.Add(workTime);
                }

                building.Append(i + ";Metoda z biblioteki MathNet;" + buildTime + "\n");
                working.Append(i + ";Metoda z biblioteki MathNet;" + workTime + "\n");
            }

            File.Delete("wynikiApro/czas_budowania.csv");
            File.AppendAllText("wynikiApro/czas_budowania.csv", building.ToString());
            File.Delete("wynikiApro/czas_rozwiazania.csv");
            File.AppendAllText("wynikiApro/czas_rozwiazania.csv", working.ToString());

            #endregion
            #region Zad 2

            StringBuilder coefficients = new StringBuilder();
            coefficients.Append("Metoda;Wspolczynniki\n");

            List<Double> gaussApproximation = Approximation.solveApproximation(3, gaussDomain, gaussRange);
            coefficients.Append("Gauss;" + gaussApproximation.ElementAt(3) + " * x^3;" + gaussApproximation.ElementAt(2) + " * x^2;" + gaussApproximation.ElementAt(1) + " * x;" + gaussApproximation.ElementAt(0) + "\n");

            List<Double> oGaussApproximation = Approximation.solveApproximation(2, optimizedGaussDomain, optimizedGaussRange);
            coefficients.Append("Gauss zoptymalizowany;" + oGaussApproximation.ElementAt(2) + " * x^2;" + oGaussApproximation.ElementAt(1) + " * x;" + oGaussApproximation.ElementAt(0) + "\n");

            List<Double> gaussSeidelApproximation = Approximation.solveApproximation(2, gaussSeidelDomain, gaussSeidelRange);
            coefficients.Append("Gauss-Seidel;" + gaussSeidelApproximation.ElementAt(2) + " * x^2;" + gaussSeidelApproximation.ElementAt(1) + " * x;" + gaussSeidelApproximation.ElementAt(0) + "\n");

            List<Double> mathNetApproximation = Approximation.solveApproximation(1, mathNetDomain, mathNetRange);
            coefficients.Append("Biblioteka MathNet;" + mathNetApproximation.ElementAt(1) + " * x;" + mathNetApproximation.ElementAt(0) + "\n");

            File.Delete("wynikiApro/wspolczynniki.csv");
            File.AppendAllText("wynikiApro/wspolczynniki.csv", coefficients.ToString());

            #endregion
            #region Zad 4 
            StringBuilder prediction = new StringBuilder();
            int size = 100000;
            int agents = 0, tempSize = 1;
            //Liczenie ilości agentów na podstawie rozmiaru
            for (int i = 0; i < size; i++)
            {
                if (tempSize < size)
                {
                    tempSize += i + 2;
                    agents++;
                }
                else
                {
                    break;
                }
            }
            prediction.Append("Metoda;Czas;Ilosc agentow: " + agents + "\n");
            double resultGauss, resultOptimizedGauss, resultGaussSeidel, resultMathNet;
            resultGauss = (gaussApproximation.ElementAt(3) * Math.Pow(agents, 3)) +
                (gaussApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussApproximation.ElementAt(1) * agents) +
                gaussApproximation.ElementAt(0);
            prediction.Append("Gauss;" + resultGauss + "\n");

            resultOptimizedGauss = (oGaussApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (oGaussApproximation.ElementAt(1) * agents) +
                oGaussApproximation.ElementAt(0);
            prediction.Append("Gauss zoptymalizowany;" + resultOptimizedGauss + "\n");

            resultGaussSeidel = (gaussSeidelApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussSeidelApproximation.ElementAt(1) * agents) +
                gaussSeidelApproximation.ElementAt(0);
            prediction.Append("Gauss-Seidel;" + resultGaussSeidel + "\n");

            resultMathNet = (mathNetApproximation.ElementAt(1) * agents) +
                mathNetApproximation.ElementAt(0);
            prediction.Append("Biblioteka MathNet;" + resultMathNet + "\n");

            File.Delete("wynikiApro/100kPrediction.csv");
            File.AppendAllText("wynikiApro/100kPrediction.csv", prediction.ToString());
            #endregion
            #region Zad 5 - not working

            StringBuilder work100k = new StringBuilder();
            work100k.Append("Metoda;Czas;Czas przewidywany\n");

            //Liczenie ilości agentów na podstawie rozmiaru
            agents = maxAgents;
                le1 = new LinearEquation(agents);

            watch = System.Diagnostics.Stopwatch.StartNew();
            le1.SolveEquationPartialPivotingAlgorithm(true);
            watch.Stop();
            workTime = watch.ElapsedMilliseconds;

            resultOptimizedGauss = (oGaussApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (oGaussApproximation.ElementAt(1) * agents) +
                oGaussApproximation.ElementAt(0);

            work100k.Append("Gauss zoptymalizowany;" + workTime + ";" + resultOptimizedGauss + "\n");

            watch = System.Diagnostics.Stopwatch.StartNew();
            var C = SparseMatrix.Build.DenseOfArray(le1.Matrix.Matrix);
            var d = Vector.Build.Dense(le1.Vector.Values);
            var y = C.Solve(d);
            watch.Stop();
            workTime = watch.ElapsedMilliseconds;

            resultMathNet = (mathNetApproximation.ElementAt(1) * agents) +
                mathNetApproximation.ElementAt(0);

            work100k.Append("Biblioteka MathNet;" + workTime + ";" + resultMathNet + "\n");

            File.Delete("wynikiApro/100kWork.csv");
            File.AppendAllText("wynikiApro/100kWork.csv", work100k.ToString());
            #endregion
            Console.WriteLine("END");
            Console.Read();
        }
    }
}
