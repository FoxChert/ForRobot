using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRobot.Libr.Collections
{
    public class Matrix
    {
        public int RowsCount;
        public int ColumntsCount;

        public Matrix L;
        public Matrix U;

        public double[,] Items;

        public class MatException : Exception
        {
            public MatException(string Message) : base(Message) { }
        }

        #region Constructors

        public Matrix(int rows, int cols)
        {
            this.RowsCount = rows;
            this.ColumntsCount = cols;
            this.Items = new double[rows, cols];
        }

        public Matrix(double[,] pointArray)
        {
            this.ColumntsCount = pointArray.GetLength(0);
            this.RowsCount = pointArray.GetLength(1);
            
            this.Items = new double[this.RowsCount, this.ColumntsCount];
            for (int i = 0; i < this.ColumntsCount; i++)
            {
                for (int y = 0; y < this.RowsCount; y++)
                {
                    this.Items[y, i] = pointArray[i, y];
                }
            }
        }

        #endregion Constructors
    }
}
