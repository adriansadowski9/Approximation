using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt4_Aproksymacja
{
    public enum Algorithm { PartialGauss, OptimizedPartialGauss, GaussSeidel }
    public enum Agent { Y, N, U }
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            Point other = (Point)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode() => X * Y + Y;
    }
    class LinearEquation
    {
        public MyMatrix Matrix { get; }
        public MyVector Vector { get; }
        public int Agents { get; }
        public int Size { get; }

        public LinearEquation(MyMatrix matrix, MyVector vector)
        {
            if (matrix.Rows != vector.Length)
                throw new System.Exception(String.Format("Matrix has {0} rows, but the vector has {1} values", matrix.Rows, vector.Length));

            Matrix = (MyMatrix)matrix.Clone();
            Vector = (MyVector)vector.Clone();
        }

        public LinearEquation(int agents)
        {
            if (agents < 2)
                throw new System.ArgumentException("Too few agents");

            int size = 1;
            for (int i = 0; i < agents; i++)
                size += i + 2;

            Size = size;
            Agents = agents;

            double[,] matrix = new double[size, size];
            double[] vector = new double[size];

            int row = 0;
            double combinationProbability = 2.0 / (Agents * (Agents - 1));
            for (int x = 0; x <= Agents; x++)
            {
                for (int y = 0; y <= Agents - x; y++)
                {
                    matrix[row, row] = 1;
                    if (!((x == 0 && y == 0) || (x == 0 && y == Agents) || (x == Agents && y == 0)))
                    {
                        Agent[] arr = GenerateArrayOfAgents(x, y);
                        int index;
                        for (int ind1 = 0; ind1 < Agents - 1; ind1++)
                        {
                            for (int ind2 = ind1 + 1; ind2 < Agents; ind2++)
                            {
                                if (arr[ind1] == Agent.Y && arr[ind2] == Agent.U)
                                {
                                    index = TranslateCoordinates(new Point(x + 1, y));
                                    matrix[row, index] -= combinationProbability;
                                }
                                else if (arr[ind1] == Agent.Y && arr[ind2] == Agent.N)
                                {
                                    index = TranslateCoordinates(new Point(x - 1, y - 1));
                                    matrix[row, index] -= combinationProbability;
                                }
                                else if (arr[ind1] == Agent.N && arr[ind2] == Agent.U)
                                {
                                    index = TranslateCoordinates(new Point(x, y + 1));
                                    matrix[row, index] -= combinationProbability;
                                }
                                else
                                {
                                    index = TranslateCoordinates(new Point(x, y));
                                    matrix[row, index] -= combinationProbability;
                                }
                            }
                        }

                    }
                    else if (x == Agents && y == 0)
                        vector[row] = 1;

                    row++;
                }
            }

            Matrix = new MyMatrix(matrix);
            Vector = new MyVector(vector);
        }
        public Agent[] GenerateArrayOfAgents(int positive, int negative)
        {
            Agent[] arr = new Agent[Agents];
            int index = 0;
            for (int i = 0; i < positive; i++, index++)
                arr[index] = Agent.Y;
            for (int i = 0; i < negative; i++, index++)
                arr[index] = Agent.N;
            while (index < Agents)
                arr[index++] = Agent.U;
            return arr;
        }

        public int TranslateCoordinates(Point p)
        {
            if (p.X + p.Y > Agents || (p.X < 0 || p.X > Agents) || (p.Y < 0 || p.X > Agents))
                throw new ArgumentException("Invalid point!");
            int sum = p.Y;
            for (int i = 0; i < p.X; i++)
                sum += Agents - i + 1;
            return sum;
        }
        public Point TranslateIndex(int index)
        {
            if (index > Size - 1)
                throw new ArgumentException("Index is out of range!");
            int x = 0, y = 0;
            if (index > Agents)
            {
                int ind = index;
                for (int i = 0; ind >= 0; i++)
                {
                    ind -= Agents - i + 1;
                    x++;
                }
                x--;
            }
            for (int i = 0; i < x; i++)
                index -= Agents - i + 1;
            y = index;
            return new Point(x, y);
        }
        public MyVector CalculateResultVector()
        {
            MyVector vec = new MyVector(Vector.Length);
            for (int i = Vector.Length - 1; i >= 0; i--)
            {
                double sum = 0;
                for (int j = i + 1; j < Matrix.Columns; j++)
                    sum += (Matrix.Matrix[i, j]) * vec.Values[j] * (-1);
                vec.Values[i] = (Vector.Values[i] + sum) / (Matrix.Matrix[i, i]);
            }
            return vec;
        }
        public void RowReductionOptimized(int currentRow, int col)
        {
            for (int row = currentRow + 1; row < Matrix.Rows; row++)
                if (Matrix.Matrix[row, col] != 0 as dynamic)
                {
                    Vector.Values[row] += Vector.Values[currentRow] * (Matrix.Matrix[row, col] as dynamic / Matrix.Matrix[currentRow, col] as dynamic) * (-1);
                    Matrix.AddRow(row, currentRow, (Matrix.Matrix[row, col] as dynamic / Matrix.Matrix[currentRow, col] as dynamic) * (-1));
                }
        }

        public void RowReduction(int currentRow, int col)
        {
            for (int row = currentRow + 1; row < Matrix.Rows; row++)
            {
                Vector.Values[row] += Vector.Values[currentRow] * (Matrix.Matrix[row, col] as dynamic / Matrix.Matrix[currentRow, col] as dynamic) * (-1);
                Matrix.AddRow(row, currentRow, (Matrix.Matrix[row, col] as dynamic / Matrix.Matrix[currentRow, col] as dynamic) * (-1));
            }
        }

        public MyVector SolveEquationPartialPivotingAlgorithm(bool optimize)
        {
            PartialPivotingGaussianElimination(optimize);
            return CalculateResultVector();
        }
        public void PartialPivotingGaussianElimination(bool optimize)
        {
            for (int col = 0, currentRow = 0; col < Matrix.Columns || currentRow == Matrix.Rows - 1; col++, currentRow++)
            {
                if (currentRow == Matrix.Rows - 1)
                    break;
                int res = Matrix.FindRowWithTheHighestValue(currentRow, col);
                if (res != col)
                {
                    Matrix.SwapRows(currentRow, res);
                    Vector.SwapValues(col, res);
                }
                if (optimize)
                    RowReductionOptimized(currentRow, col);
                else
                    RowReduction(currentRow, col);
            }
        }

        public double[] JacobiIterationAlgorithm(double precision)
        {
            int iterationCounter = 0;
            double[] result = new double[Matrix.Rows];
            double[] previous = new double[Matrix.Rows];
            double value = 0;

            for (int i = 0; i < Matrix.Rows; i++)
            {
                result[i] = 0;
                previous[i] = 0;
            }

            while (true)
            {
                for (int i = 0; i < Matrix.Rows; i++)
                {
                    double sum = Vector.Values[i] as dynamic;

                    for (int j = 0; j < Matrix.Rows; j++)
                    {
                        if (j != i)
                        {
                            value = Matrix.Matrix[i, j] as dynamic;
                            value *= previous[j];
                            sum -= value;
                        }
                    }
                    value = Matrix.Matrix[i, i] as dynamic;
                    result[i] = 1 / value * sum;
                }
                iterationCounter++;

                bool stop = true;
                for (int i = 0; i < Matrix.Rows; i++)
                    if (Math.Abs(result[i] - previous[i]) > precision)
                        stop = false;

                if (stop)
                    break;

                for (int i = 0; i < Matrix.Rows; i++)
                    previous[i] = result[i];
            }
            return result;
        }

        public double[] GaussSeidelIterationAlgorithm(double precision)
        {
            int iterationCounter = 0;
            double[] result = new double[Matrix.Rows];
            double[] previous = new double[Matrix.Rows];
            double value = 0;

            for (int i = 0; i < Matrix.Rows; i++)
            {
                result[i] = 0;
                previous[i] = 0;
            }

            while (true)
            {
                for (int i = 0; i < Matrix.Rows; i++)
                {
                    double sum = Vector.Values[i];

                    for (int j = 0; j < Matrix.Rows; j++)
                    {
                        if (j != i)
                        {
                            value = Matrix.Matrix[i, j];
                            value *= result[j];
                            sum -= value;
                        }
                    }
                    value = Matrix.Matrix[i, i];
                    result[i] = 1 / value * sum;
                }
                iterationCounter++;

                bool stop = true;
                for (int i = 0; i < Matrix.Rows; i++)
                    if (Math.Abs(result[i] - previous[i]) > precision)
                        stop = false;

                if (stop)
                    break;

                for (int i = 0; i < Matrix.Rows; i++)
                    previous[i] = result[i];
            }
            return result;
        }

        public double[] GaussSeidelSparseIterationAlgorithm(double precision)
        {
            int iterationCounter = 0;
            double[] result = new double[Matrix.Rows];
            double[] previous = new double[Matrix.Rows];
            double value = 0;

            for (int i = 0; i < Matrix.Rows; i++)
            {
                result[i] = 0;
                previous[i] = 0;
            }

            while (true)
            {
                for (int i = 0; i < Matrix.Rows; i++)
                {
                    double sum = Vector.Values[i];

                    for (int j = 0; j < Matrix.Rows; j++)
                    {
                        if (j != i)
                        {
                            value = Matrix.Matrix[i, j];
                            value *= result[j];
                            sum -= value;
                        }
                    }
                    value = Matrix.Matrix[i, i];
                    result[i] = 1 / value * sum;
                }
                iterationCounter++;

                bool stop = true;
                for (int i = 0; i < Matrix.Rows; i++)
                    if (Math.Abs(result[i] - previous[i]) > precision)
                        stop = false;

                if (stop)
                    break;

                for (int i = 0; i < Matrix.Rows; i++)
                    previous[i] = result[i];
            }
            return result;
        }

        public static double CalculatePrecision(MyMatrix matrix, MyVector vector, Algorithm algo)
        {
            LinearEquation le = new LinearEquation(matrix, vector);
            MyVector x;
            switch (algo)
            {
                case Algorithm.PartialGauss:
                    x = le.SolveEquationPartialPivotingAlgorithm(false);
                    break;
                case Algorithm.OptimizedPartialGauss:
                    x = le.SolveEquationPartialPivotingAlgorithm(true);
                    break;
                case Algorithm.GaussSeidel:
                    x = new MyVector(le.GaussSeidelIterationAlgorithm(0));
                    break;
                default:
                    x = le.SolveEquationPartialPivotingAlgorithm(true);
                    break;
            }
            MyVector b = matrix * x;
            return MyVector.VectorNorm(vector - b);
        }

        public override string ToString() => $"Macierz\n{Matrix}Wektor\n{Vector}";
    }
}