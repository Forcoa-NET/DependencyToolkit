using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedNetwork
{
   public class SparseMatrix<T>
    {
        public readonly Dictionary<int, Dictionary<int, T>> Matrix;
        private T defaultvalue;

        private object matrixLock = new object();

        public SparseMatrix(T defaultValue)
        {
            Matrix = new Dictionary<int, Dictionary<int, T>>();
            this.defaultvalue = defaultValue;
        }

        public bool ContainsValue(int i, int j)
        {
            Dictionary<int, T> row;
            T x;
            if (Matrix.TryGetValue(i, out row))
            {
                if (row.TryGetValue(j, out x))
                {
                    return true;
                }
            }

            return false;
        }

        public T GetValue(int i, int j)
        {
            if (this.ContainsValue(i, j))
            {
                return Matrix[i][j];
            }

            return this.defaultvalue;
        }

        public bool TryGetValue(int i, int j, out T value)
        {
            Dictionary<int, T> row;
            T x;
            if (Matrix.TryGetValue(i, out row))
            {
                if (row.TryGetValue(j, out x))
                {
                    value = x;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        public void SetValue(int i, int j, T value)
        {
            lock (matrixLock)
            {
                Dictionary<int, T> iRow;
                T x;
                if (!Matrix.TryGetValue(i, out iRow))
                {
                    iRow = new Dictionary<int, T>();
                    iRow.Add(j, value);
                    this.Matrix.Add(i, iRow);
                }

                if (!iRow.TryGetValue(j, out x))
                {
                    iRow.Add(j, value);
                }
                else
                {
                    iRow[j] = value;
                }
            }
        }

    }

}
