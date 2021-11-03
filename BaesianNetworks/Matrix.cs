using System;
using System.Collections.Generic;

namespace BaesianNetworks
{
    public class Matrix
    {
        // Variables to hold the dimensions of this matrix
        int m;
        int n;

        LinkedList<Cell> matrix = new LinkedList<Cell>();

        /// <summary>
        /// Creates an m (rows) by n (columns) matrix
        /// </summary>
        /// <param name="_m"></param>
        /// <param name="_n"></param>
        public Matrix(int _m, int _n)
        {
            //Rows
            m = _m;

            //Columns
            n = _n;
        }

        public int GetRows()
        {
            return m;
        }

        public int GetColumns()
        {
            return n;
        }

        public LinkedList<Cell> GetMatrix()
        {
            return matrix;
        }

        public void SetCellValue(int _target, string _value)
        {
            Cell head = matrix.First.Value;
            for (int i = 1; i < _target; i++)
            {
                head = matrix.First.Next.Value;
            }
            head.SetValue(_value);
        }

        public string GetCellValue(int _target)
        {
            Cell head = matrix.First.Value;
            for (int i = 1; i < _target; i++)
            {
                head = matrix.First.Next.Value;
            }
            return head.GetValue();
        }

        public override string ToString()
        {
            string output = "TODO:";
            return output;
        }
    }
}
