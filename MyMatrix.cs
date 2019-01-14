using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt4_Aproksymacja
{
    class MyMatrix : ICloneable
    {
        public double[,] Matrix { get; private set; }
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        public MyMatrix(int rows, int columns)
        {
            Random random = new Random();
            int randomValue;
            Matrix = new double[rows, columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                {
                    randomValue = random.Next(-65536, 65535);
                    Matrix[i, j] = randomValue / 65536.0;
                }
            Rows = rows;
            Columns = columns;
        }
        public object Clone() => new MyMatrix(Matrix);
        public MyMatrix(Double[,] values)
        {
            Rows = values.GetLength(0);
            Columns = values.GetLength(1);
            Matrix = new double[Rows, Columns];
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Columns; j++)
                    Matrix[i, j] = values[i, j];
        }

        public int FindRowWithNonZeroValueAtColumn(int row, int col)
        {
            for (int i = row + 1; i < Rows; i++)
                if (Matrix[i, col] != 0)
                    return i;
            return -1;
        }
        public int FindRowWithTheHighestValue(int row, int col)
        {
            double highestValue = Matrix[row, col];
            int index = row;
            for (int i = row + 1; i < Rows; i++)
            {
                double currentVal = Matrix[i, col];
                if ((currentVal) < 0)
                    currentVal *= (-1);
                if (currentVal > highestValue)
                {
                    highestValue = currentVal;
                    index = i;
                }
            }
            return index;
        }
        public int[] FindHighestValue(int row, int col)
        {
            double highestValue = Matrix[row, col];
            int indexRow = row, indexCol = col;

            for (int i = row; i < Rows; i++)
                for (int j = col; j < Columns; j++)
                    if (Matrix[i, j] > highestValue)
                    {
                        highestValue = Matrix[i, j];
                        indexRow = i;
                        indexCol = j;
                    }
            return new int[2] { indexRow, indexCol };
        }

        public void SwapRows(int row1, int row2)
        {
            if (row1 > Rows - 1 || row2 > Rows - 1)
                throw new ArgumentException(String.Format("Can't swap rows {0} and {1} because the matrix has {2} rows.", row1, row2, Rows));
            else if (row1 < 0 || row2 < 0)
                throw new ArgumentException(String.Format("Row numbers should have positive value."));
            for (int i = 0; i < Rows; i++)
            {
                double temp = Matrix[row2, i];
                Matrix[row2, i] = Matrix[row1, i];
                Matrix[row1, i] = temp;
            }
        }
        public void SwapColumns(int col1, int col2)
        {
            if (col1 > Columns - 1 || col2 > Columns - 1)
                throw new ArgumentException(String.Format("Can't swap columns {0} and {1} because the matrix has {2} columns.", col1, col2, Columns));
            else if (col1 < 0 || col2 < 0)
                throw new ArgumentException(String.Format("Column numbers should have positive value."));
            for (int i = 0; i < Rows; i++)
            {
                double temp = Matrix[i, col2];
                Matrix[i, col2] = Matrix[i, col1];
                Matrix[i, col1] = temp;
            }
        }
        public void MultiplyRow(int row, dynamic value)
        {
            if (row > Rows - 1 || row < 0)
                throw new ArgumentException(String.Format("Can't multiply row {0} because it doesn't exists.", row));
            for (int i = 0; i < Columns; i++)
                Matrix[row, i] *= value;
        }
        public void MultiplyColumn(int col, dynamic value)
        {
            if (col > Columns - 1 || col < 0)
                throw new ArgumentException(String.Format("Can't multiply row {0} because it doesn't exists.", col));
            for (int i = 0; i < Rows; i++)
                Matrix[i, col] *= value;
        }
        public void AddRow(int destRow, int sourceRow, dynamic times)
        {
            if (destRow > Rows - 1 || sourceRow > Rows - 1)
                throw new ArgumentException(String.Format("Can't add row {0} to {1} because the matrix has {2} rows.", sourceRow, destRow, Rows));
            else if (destRow < 0 || sourceRow < 0)
                throw new ArgumentException(String.Format("Row numbers should have positive value."));
            for (int i = 0; i < Columns; i++)
                Matrix[destRow, i] += Matrix[sourceRow, i] * times;
        }
        public void AddColumn(int destCol, int sourceCol, dynamic times)
        {
            if (destCol > Columns - 1 || sourceCol > Columns - 1)
                throw new ArgumentException(String.Format("Can't add column {0} to {1} because the matrix has {2} columns.", sourceCol, destCol, Rows));
            else if (destCol < 0 || sourceCol < 0)
                throw new ArgumentException(String.Format("Row numbers should have positive value."));
            for (int i = 0; i < Rows; i++)
                Matrix[i, destCol] += Matrix[i, sourceCol] * times;
        }
        public double[] GetRow(int number)
        {
            double[] row = new double[Columns];
            for (int i = 0; i < Rows; i++)
                row[i] = Matrix[number, i];
            return row;
        }
        public double[] GetColumn(int number)
        {
           double[] column = new double[Rows];
            for (int i = 0; i < Rows; i++)
                column[i] = Matrix[i, number];
            return column;
        }
        public static MyVector operator *(MyMatrix matrix, MyVector vector)
        {
            if (matrix.Columns != vector.Length)
                throw new Exception("The number of columns of the 1st matrix must equal the number of rows of the 2nd matrix");
            MyVector newVector = new MyVector(matrix.Rows);
            double sum;
            for (int i = 0; i < matrix.Rows; i++)
            {
                    sum = 0;
                for (int j = 0; j < matrix.Columns; j++)
                    sum += (matrix.Matrix[i, j]) * vector.Values[j];
                newVector.Values[i] = sum;
            }
            return newVector;
        }
        override public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                sb.Append("[ ");
                for (int j = 0; j < Columns; j++)
                    sb.Append(Matrix[i, j] + " ");
                sb.Append("]\n");
            }
            return sb.ToString();
        }
    }
}
