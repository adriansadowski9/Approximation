using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt4_Aproksymacja
{
    class MyVector : ICloneable
    {
        public double[] Values { get; private set; }
        public int Length { get; private set; }

        public MyVector(double[] values)
        {
            Values = values;
            Length = values.Length;
        }
        public MyVector(int size)
        {
            Values = new double[size];
            Length = size;
        }
        public MyVector(int size, double val)
        {
            Values = new double[size];
            Length = size;
            for (int i = 0; i < size; i++)
                Values[i] = val;
        }
        public void SwapValues(int index1, int index2)
        {
            double temp = Values[index1];
            Values[index1] = Values[index2];
            Values[index2] = temp;
        }
        public static MyVector operator -(MyVector vec1, MyVector vec2)
        {
            if (vec1.Length != vec2.Length)
                throw new Exception("Vectors should have the same length");
            MyVector result = new MyVector(vec1.Length);
            for (int i = 0; i < vec1.Length; i++)
                result.Values[i] = vec1.Values[i] as dynamic - vec2.Values[i];
            return result;
        }
        public object Clone()
        {
            MyVector newVector = new MyVector(Length);
            for (int i = 0; i < Length; i++)
                newVector.Values[i] = Values[i];
            return newVector;
        }
        public static double VectorNorm(MyVector vec)
        {
            double maxValue = (vec.Values[0] < 0) ? (vec.Values[0] * -1) : vec.Values[0];
            for (int i = 1; i < vec.Length; i++)
            {
                double currentVal = (vec.Values[i] < 0) ? (vec.Values[i] * -1) : vec.Values[i];
                if (currentVal > maxValue)
                    maxValue = currentVal;
            }
            return maxValue;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[ ");
            for (int i = 0; i < Values.Length; i++)
            {
                sb.Append(Values[i]);
                if (i != Values.Length - 1)
                    sb.Append(" ");
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}
