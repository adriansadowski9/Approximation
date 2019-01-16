using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Double.Solvers;

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
            double buildTime, workTime;

            List<Double> gaussDomain = new List<Double>();
            List<Double> optimizedGaussDomain = new List<Double>();
            List<Double> gaussSeidelDomain = new List<Double>();
            List<Double> mathNetDomain = new List<Double>();
            List<Double> gaussBuildDomain = new List<Double>();
            List<Double> optimizedGaussBuildDomain = new List<Double>();
            List<Double> gaussSeidelBuildDomain = new List<Double>();
            List<Double> mathNetBuildDomain = new List<Double>();
            List<Double> gaussRange = new List<Double>();
            List<Double> optimizedGaussRange = new List<Double>();
            List<Double> gaussSeidelRange = new List<Double>();
            List<Double> mathNetRange = new List<Double>();
            List<Double> gaussBuildRange = new List<Double>();
            List<Double> optimizedGaussBuildRange = new List<Double>();
            List<Double> gaussSeidelBuildRange = new List<Double>();
            List<Double> mathNetBuildRange = new List<Double>();

            #region Zad 1

            Console.WriteLine("Wykonuje zadanie 1");
            StringBuilder building = new StringBuilder(), working = new StringBuilder();
            building.Append("Agenci;Metoda;Czas budowania\n");
            working.Append("Agenci;Metoda;Czas rozwiazywania\n");

            int start = 15;
            int maxAgents = 61;

            double[,] gaussTimes = new Double[maxAgents + 1, 2];
            double[,] optimizedGaussTimes = new Double[maxAgents + 1, 2];
            double[,] gaussSeidelTimes = new Double[maxAgents + 1, 2];
            double[,] mathNetTimes = new Double[maxAgents + 1, 2];

            for (int i = start; i <= maxAgents; i++)
            {
                le1 = new LinearEquation(i);
                double precision = 0.0000000001;
                Console.WriteLine("Agenci: " + i);

                // Partial Gauss
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.Elapsed.TotalMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.SolveEquationPartialPivotingAlgorithm(false);
                watch.Stop();
                workTime = watch.Elapsed.TotalMilliseconds;
                gaussBuildDomain.Add(i);
                gaussBuildRange.Add(buildTime);
                gaussDomain.Add(i);
                gaussRange.Add(workTime);
                gaussTimes[i, 0] = buildTime;
                gaussTimes[i, 1] = workTime;
                building.Append(i + ";Gauss z częsciowym wyborem;" + buildTime + "\n");
                working.Append(i + ";Gauss z częsciowym wyborem;" + workTime + "\n");

                // Zopymalizowany Gauss
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.Elapsed.TotalMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.SolveEquationPartialPivotingAlgorithm(true);
                watch.Stop();
                workTime = watch.Elapsed.TotalMilliseconds;
                optimizedGaussBuildDomain.Add(i);
                optimizedGaussBuildRange.Add(buildTime);
                optimizedGaussDomain.Add(i);
                optimizedGaussRange.Add(workTime);
                optimizedGaussTimes[i, 0] = buildTime;
                optimizedGaussTimes[i, 1] = workTime;
          
                building.Append(i + ";Zoptymalizowany Gauss z częsciowym wyborem;" + buildTime + "\n");
                working.Append(i + ";Zoptymalizowany Gauss z częsciowym wyborem;" + workTime + "\n");

                // Gauss-Seidel
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                watch.Stop();
                buildTime = watch.Elapsed.TotalMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                le1.GaussSeidelIterationAlgorithm(precision);
                watch.Stop();
                workTime = watch.Elapsed.TotalMilliseconds;

                gaussSeidelBuildDomain.Add(i);
                gaussSeidelBuildRange.Add(buildTime);
                gaussSeidelDomain.Add(i);
                gaussSeidelRange.Add(workTime);
                gaussSeidelTimes[i, 0] = buildTime;
                gaussSeidelTimes[i, 1] = workTime;

                building.Append(i + ";Gauss-Seidel;" + buildTime + "\n");
                working.Append(i + ";Gauss-Seidel;" + workTime + "\n");

                // Biblioteka MathNet
                watch = System.Diagnostics.Stopwatch.StartNew();
                le1 = new LinearEquation(i);
                var A = SparseMatrix.Build.SparseOfArray(le1.Matrix.Matrix);
                var b = SparseVector.Build.SparseOfEnumerable(le1.Vector.Values);
                watch.Stop();
                buildTime = watch.Elapsed.TotalMilliseconds;

                watch = System.Diagnostics.Stopwatch.StartNew();
                var x = A.Solve(b);
                watch.Stop();
                workTime = watch.Elapsed.TotalMilliseconds;

                mathNetBuildDomain.Add(i);
                mathNetBuildRange.Add(buildTime);
                mathNetDomain.Add(i);
                mathNetRange.Add(workTime);
                mathNetTimes[i, 0] = buildTime;
                mathNetTimes[i, 1] = workTime;

                building.Append(i + ";Metoda z biblioteki MathNet;" + buildTime + "\n");
                working.Append(i + ";Metoda z biblioteki MathNet;" + workTime + "\n");
            }

            File.Delete("wynikiApro/czas_budowania.csv");
            File.AppendAllText("wynikiApro/czas_budowania.csv", building.ToString());
            File.Delete("wynikiApro/czas_rozwiazania.csv");
            File.AppendAllText("wynikiApro/czas_rozwiazania.csv", working.ToString());

            #endregion
            #region Zad 2

            Console.WriteLine("Wykonuje zadanie 2");
            StringBuilder coefficients = new StringBuilder();
            coefficients.Append("Metoda;Wspolczynniki\n");

            List<Double> gaussBuildApproximation = Approximation.solveApproximation(3, gaussBuildDomain, gaussBuildRange);
            coefficients.Append("Gauss budowanie;" + gaussBuildApproximation.ElementAt(3) + " * x^3;" + gaussBuildApproximation.ElementAt(2) + " * x^2;" + gaussBuildApproximation.ElementAt(1) + " * x;" + gaussBuildApproximation.ElementAt(0) + "\n");
            List<Double> gaussApproximation = Approximation.solveApproximation(3, gaussDomain, gaussRange);
            coefficients.Append("Gauss rozwiazywanie;" + gaussApproximation.ElementAt(3) + " * x^3;" + gaussApproximation.ElementAt(2) + " * x^2;" + gaussApproximation.ElementAt(1) + " * x;" + gaussApproximation.ElementAt(0) + "\n");

            List<Double> oGaussBuildApproximation = Approximation.solveApproximation(2, optimizedGaussBuildDomain, optimizedGaussBuildRange);
            coefficients.Append("Gauss zoptymalizowany budowanie;" + oGaussBuildApproximation.ElementAt(2) + " * x^2;" + oGaussBuildApproximation.ElementAt(1) + " * x;" + oGaussBuildApproximation.ElementAt(0) + "\n");
            List<Double> oGaussApproximation = Approximation.solveApproximation(2, optimizedGaussDomain, optimizedGaussRange);
            coefficients.Append("Gauss zoptymalizowany rozwiazywanie;" + oGaussApproximation.ElementAt(2) + " * x^2;" + oGaussApproximation.ElementAt(1) + " * x;" + oGaussApproximation.ElementAt(0) + "\n");

            List<Double> gaussSeideBuildApproximation = Approximation.solveApproximation(2, gaussSeidelBuildDomain, gaussSeidelBuildRange);
            coefficients.Append("Gauss-Seidel budowanie;" + gaussSeideBuildApproximation.ElementAt(2) + " * x^2;" + gaussSeideBuildApproximation.ElementAt(1) + " * x;" + gaussSeideBuildApproximation.ElementAt(0) + "\n");
            List<Double> gaussSeidelApproximation = Approximation.solveApproximation(2, gaussSeidelDomain, gaussSeidelRange);
            coefficients.Append("Gauss-Seidel rozwiazywanie;" + gaussSeidelApproximation.ElementAt(2) + " * x^2;" + gaussSeidelApproximation.ElementAt(1) + " * x;" + gaussSeidelApproximation.ElementAt(0) + "\n");

            List<Double> mathNetBuildApproximation = Approximation.solveApproximation(1, mathNetBuildDomain, mathNetBuildRange);
            coefficients.Append("Biblioteka MathNet budowanie;" + mathNetBuildApproximation.ElementAt(1) + " * x;" + mathNetBuildApproximation.ElementAt(0) + "\n");
            List<Double> mathNetApproximation = Approximation.solveApproximation(1, mathNetDomain, mathNetRange);
            coefficients.Append("Biblioteka MathNet rozwiazywanie;" + mathNetApproximation.ElementAt(1) + " * x;" + mathNetApproximation.ElementAt(0) + "\n");

            File.Delete("wynikiApro/wspolczynniki.csv");
            File.AppendAllText("wynikiApro/wspolczynniki.csv", coefficients.ToString());

            #endregion
            #region Zad 3

            Console.WriteLine("Wykonuje zadanie 3");
            double resultBuildGauss, resultGauss, resultBuildOptimizedGauss, resultOptimizedGauss, resultBuildGaussSeidel, resultGaussSeidel, resultBuildMathNet, resultMathNet, appBuildError, appError;
            double gaussBuildError = 0, gaussError = 0, optimizedGaussBuildError = 0, optimizedGaussError = 0, gaussSeidelBuildError = 0, gaussSeidelError = 0, mathNetBuildError = 0, mathNetError = 0;

            StringBuilder approximationError = new StringBuilder();
            approximationError.Append("Agenci;Metoda;Czas budowania - przyblizony;Czas budowania - rzeczywisty;Blad budowania;Czas rozwiazania - przyblizony;Czas rozwiazania - rzeczywisty;Blad rozwiazania\n");

            for (int i = start; i <= maxAgents; i++)
            {
                resultBuildGauss = (gaussBuildApproximation.ElementAt(3) * Math.Pow(i, 3)) +
                    (gaussBuildApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (gaussBuildApproximation.ElementAt(1) * i) +
                    gaussBuildApproximation.ElementAt(0);
                resultGauss = (gaussApproximation.ElementAt(3) * Math.Pow(i, 3)) +
                    (gaussApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (gaussApproximation.ElementAt(1) * i) +
                    gaussApproximation.ElementAt(0);
                appBuildError = Math.Abs((resultBuildGauss - gaussTimes[i, 0])) / Math.Abs(resultBuildGauss);
                appError = Math.Abs((resultGauss - gaussTimes[i, 1])) / Math.Abs(resultGauss);
                gaussBuildError += appBuildError; gaussError += appError;
                approximationError.Append(i + ";Gauss;" + resultBuildGauss + ";" + gaussTimes[i, 0] + ";" + appBuildError + ";" + resultGauss + ";" + gaussTimes[i, 1] + ";" + appError + "\n");

                resultBuildOptimizedGauss = (oGaussBuildApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (oGaussBuildApproximation.ElementAt(1) * i) +
                oGaussBuildApproximation.ElementAt(0);
                resultOptimizedGauss = (oGaussApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (oGaussApproximation.ElementAt(1) * i) +
                    oGaussApproximation.ElementAt(0);
                appBuildError = Math.Abs((resultBuildOptimizedGauss - optimizedGaussTimes[i, 0])) / Math.Abs(resultBuildOptimizedGauss);
                appError = Math.Abs((resultOptimizedGauss - optimizedGaussTimes[i, 1])) / Math.Abs(resultOptimizedGauss);
                optimizedGaussBuildError += appBuildError; optimizedGaussError += appError;
                approximationError.Append(i + ";Gauss zoptymalizowany;" + resultBuildOptimizedGauss + ";" + optimizedGaussTimes[i, 0] + ";" + appBuildError + ";" + resultOptimizedGauss + ";" + optimizedGaussTimes[i, 1] + ";" + appError + "\n");

                resultBuildGaussSeidel = (gaussSeideBuildApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (gaussSeideBuildApproximation.ElementAt(1) * i) +
                    gaussSeideBuildApproximation.ElementAt(0);
                resultGaussSeidel = (gaussSeidelApproximation.ElementAt(2) * Math.Pow(i, 2)) +
                    (gaussSeidelApproximation.ElementAt(1) * i) +
                    gaussSeidelApproximation.ElementAt(0);
                appBuildError = Math.Abs((resultBuildGaussSeidel - gaussSeidelTimes[i, 0])) / Math.Abs(resultBuildGaussSeidel);
                appError = Math.Abs((resultGaussSeidel - gaussSeidelTimes[i, 1])) / Math.Abs(resultGaussSeidel);
                gaussSeidelBuildError += appBuildError; gaussSeidelError += appError;
                approximationError.Append(i + ";Gauss-Seidel;" + resultBuildGaussSeidel + ";" + gaussSeidelTimes[i, 0] + ";" + appBuildError + ";" + resultGaussSeidel + ";" + gaussSeidelTimes[i, 1] + ";" + appError + "\n");

                resultBuildMathNet = (mathNetBuildApproximation.ElementAt(1) * i) +
                    mathNetBuildApproximation.ElementAt(0);
                resultMathNet = (mathNetApproximation.ElementAt(1) * i) +
                    mathNetApproximation.ElementAt(0);
                appBuildError = Math.Abs((resultBuildMathNet - mathNetTimes[i, 0])) / Math.Abs(resultBuildMathNet);
                appError = Math.Abs((resultMathNet - mathNetTimes[i, 1])) / Math.Abs(resultMathNet);
                mathNetBuildError += appBuildError; mathNetError += appError;
                approximationError.Append(i + ";Biblioteka MathNet;" + resultBuildMathNet + ";" + mathNetTimes[i, 0] + ";" + appBuildError + ";" + resultMathNet + ";" + mathNetTimes[i, 1] + ";" + appError + "\n");
            }
            int amount = maxAgents - start;

            approximationError.Append("Metoda;Sredni blad budowania [s]; Sredni blad rozwiazywania [s]\n");
            approximationError.Append("Gauss;" + gaussBuildError / (amount * 1000) + ";" + gaussError / (amount * 1000) + "\n");
            approximationError.Append("Gauss zoptymalizowany;" + optimizedGaussBuildError / (amount * 1000) + ";" + optimizedGaussError / (amount * 1000) + "\n");
            approximationError.Append("Gauss-Seidel;" + gaussSeidelBuildError / (amount * 1000) + ";" + gaussSeidelError / (amount * 1000) + "\n");
            approximationError.Append("Biblioteka MathNet;" + mathNetBuildError / (amount * 1000) + ";" + mathNetError / (amount * 1000) + "\n");

            File.Delete("wynikiApro/approximationError.csv");
            File.AppendAllText("wynikiApro/approximationError.csv", approximationError.ToString());

            #endregion
            #region Zad 4 

            Console.WriteLine("Wykonuje zadanie 4");
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
            resultBuildGauss = (gaussBuildApproximation.ElementAt(3) * Math.Pow(agents, 3)) +
                (gaussBuildApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussBuildApproximation.ElementAt(1) * agents) +
                gaussBuildApproximation.ElementAt(0);
            resultGauss = (gaussApproximation.ElementAt(3) * Math.Pow(agents, 3)) +
                (gaussApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussApproximation.ElementAt(1) * agents) +
                gaussApproximation.ElementAt(0) + resultBuildGauss;
            prediction.Append("Gauss;" + resultGauss + "\n");

            resultBuildOptimizedGauss = (oGaussBuildApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (oGaussBuildApproximation.ElementAt(1) * agents) +
                oGaussBuildApproximation.ElementAt(0);
            resultOptimizedGauss = (oGaussApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (oGaussApproximation.ElementAt(1) * agents) +
                oGaussApproximation.ElementAt(0) + resultBuildOptimizedGauss;
            prediction.Append("Gauss zoptymalizowany;" + resultOptimizedGauss + "\n");

            resultBuildGaussSeidel = (gaussSeideBuildApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussSeideBuildApproximation.ElementAt(1) * agents) +
                gaussSeideBuildApproximation.ElementAt(0);
            resultGaussSeidel = (gaussSeidelApproximation.ElementAt(2) * Math.Pow(agents, 2)) +
                (gaussSeidelApproximation.ElementAt(1) * agents) +
                gaussSeidelApproximation.ElementAt(0) + resultBuildGaussSeidel;
            prediction.Append("Gauss-Seidel;" + resultGaussSeidel + "\n");

            resultBuildMathNet = (mathNetBuildApproximation.ElementAt(1) * agents) +
                mathNetBuildApproximation.ElementAt(0);
            resultMathNet = (mathNetApproximation.ElementAt(1) * agents) +
                mathNetApproximation.ElementAt(0) + resultBuildMathNet;
            prediction.Append("Biblioteka MathNet;" + resultMathNet + "\n");

            File.Delete("wynikiApro/100kPrediction.csv");
            File.AppendAllText("wynikiApro/100kPrediction.csv", prediction.ToString());

            #endregion
            #region Zad 5
            Console.WriteLine("Wykonuje zadanie 5");
            StringBuilder maxWork = new StringBuilder();
            maxWork.Append("Metoda;Czas budowania;Czas rozwiazania;Przewidywany czas budowania;Przewidywany czas rozwiazania\n");

            agents = 155;

            watch = System.Diagnostics.Stopwatch.StartNew();
            le1 = new LinearEquation(agents);
            var C = SparseMatrix.Build.SparseOfArray(le1.Matrix.Matrix);
            var d = SparseVector.Build.SparseOfEnumerable(le1.Vector.Values);
            watch.Stop();
            buildTime = watch.Elapsed.TotalMilliseconds;

            watch = System.Diagnostics.Stopwatch.StartNew();
            var y = C.Solve(d);
            watch.Stop();
            workTime = watch.Elapsed.TotalMilliseconds;

            resultBuildMathNet = (mathNetBuildApproximation.ElementAt(1) * agents) +
                mathNetBuildApproximation.ElementAt(0);
            resultMathNet = (mathNetApproximation.ElementAt(1) * agents) +
                mathNetApproximation.ElementAt(0);
            maxWork.Append("Biblioteka MathNet;" + buildTime + ";" + workTime + ";" + resultBuildMathNet + ";" + resultMathNet + "\n");

            File.Delete("wynikiApro/maxWork.csv");
            File.AppendAllText("wynikiApro/maxWork.csv", maxWork.ToString());

            #endregion

            Console.WriteLine("END");
            Console.Read();
        }
    }
}
