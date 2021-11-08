using System;
using System.Collections.Generic;

namespace BaesianNetworks
{
    public class Matrix {
        private List<String> factors;
        
        public Matrix(int rows, int columns, List<String> factors) {
            this.factors = new List<string>(factors);
        }

        public Matrix() {
            
        }
        
        //     // Variables to hold the dimensions of this matrix
        //
        //     /// <summary>
        //     /// Creates an m (rows) by n (columns) matrix
        //     /// </summary>
        //     /// <param name="_m"></param>
        //     /// <param name="_n"></param>
        //     public Matrix(int _m, int _n)
        //     {
        //         //Rows
        //         m = _m;
        //
        //         //Columns
        //         n = _n;
        //
        //         size = m * n;
        //
        //         Fill();
        //     }
        //
        //     public int GetRows()
        //     {
        //         return m;
        //     }
        //
        //     public int GetColumns()
        //     {
        //         return n;
        //     }
        //
        //     public void SetCellValue(int _target, double _value)
        //     {
        //         // TODO:
        //     }
        //
        //     public double GetCellValue(int _target)
        //     {
        //         // TODO:
        //         return -1;
        //     }
        //
        //     /// <summary>
        //     /// Converts the matrix into string format and returns the string
        //     /// </summary>
        //     /// <returns></returns>
        //     public override string ToString()
        //     {
        //         string output = "";
        //         LinkedListNode<double> currentCell = matrix.First;
        //         for (int x = 0; x < m; x++)
        //         {
        //             for (int y = 0; y < n; y++)
        //             {
        //                 output += currentCell.Value + " ";
        //                 currentCell = currentCell.Next;
        //             }
        //             output += "\n";
        //         }
        //         return output;
        //     }
        //
        //     /// <summary>
        //     /// Fills the matrix with 0's creating the 0 matrix
        //     /// </summary>
        //     void Fill()
        //     {
        //         LinkedListNode<double> currentCellToAdd;
        //         for (int i = 0; i < size; i++)
        //         {
        //             currentCellToAdd = new LinkedListNode<double>(0);
        //             matrix.AddFirst(currentCellToAdd);
        //         }
        //     }
        // }
    }
}