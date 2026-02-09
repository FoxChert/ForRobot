using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRobot.Libr.Collections
{
    public class Matrix
    {
        #region Public variables

        /// <summary>
        /// Кол-во строк в матрице
        /// </summary>
        public int RowsCount;
        /// <summary>
        /// Кол-во столбцов в матрице
        /// </summary>
        public int ColumntsCount;

        public Matrix L;
        public Matrix U;

        public double[,] Items;

        /// <summary>
        /// Матрица является квадратной, m = n 
        /// </summary>
        public bool IsSquareMatrix { get => this.Items.GetLength(0) == this.Items.GetLength(1); }
        /// <summary>
        /// Матрица является диагональной, т.е. это квадратная матрица, у которой все элементы вне главной диагонали равны нулю
        /// </summary>
        public bool IsDiagonalMatrix { get => this.ValidateDiagonalMatrix(); }
        /// <summary>
        /// Матрица является нулевой, т.е. все элементы которой равны нулю
        /// </summary>
        public bool IsZeroMatrix { get => this.ValidateZeroMatrix(); }

        #endregion Public variables

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

        #region Private functions

        /// <summary>
        /// Проверка на дагональную матрицу
        /// </summary>
        /// <returns></returns>
        private bool ValidateDiagonalMatrix()
        {
            if (!this.IsSquareMatrix)
                return false;



            return true;
        }

        /// <summary>
        /// Проверка на нулевую матрицу
        /// </summary>
        /// <returns></returns>
        private bool ValidateZeroMatrix()
        {
            for(int m = 0; m < this.ColumntsCount; m++)
            {
                for(int n = 0; n < this.RowsCount; n++)
                {
                    if (this.Items[m, n] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Проверка на единичную матрицу
        /// </summary>
        /// <returns></returns>
        private bool ValidateOneMatrix()
        {
            if (!this.IsDiagonalMatrix)
                return false;

            return this.ElementsMainDiagonal().All(item => item == 1);
        }

        #endregion Private functions

        #region Public functions

        public static bool Equals(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.RowsCount != matrixB.RowsCount || matrixA.ColumntsCount != matrixB.ColumntsCount)
                return false;

            for(int i = 0; )

            return true;
        }

        public bool Equals(Matrix matrixB) => Matrix.Equals(this, matrixB);

        /// <summary>
        /// Вывод матрицы-столбца длиной m, где m - <see cref="RowsCount"/>
        /// </summary>
        /// <example>
        /// 
        /// Матрица:
        /// 
        ///     | a_11 |
        /// A = | a_21 | = (a_j1), размера m x 1
        ///     |  ... |
        ///     | a_m1 |
        ///     
        /// </example>
        /// <returns></returns>
        public double[] GetColumnMatrix() => this.GetColumnMatrix(this.RowsCount);

        /// <summary>
        /// Вывод матрицы-столбца длиной m
        /// </summary>
        /// <param name="m">Выводимое кол-во строк; не может превышать <see cref="RowsCount"/></param>
        /// <example>
        /// 
        /// Матрица:
        /// 
        ///     | a_11 |
        /// A = | a_21 | = (a_j1), размера m x 1
        ///     |  ... |
        ///     | a_m1 |
        ///     
        /// </example>
        /// <returns></returns>
        public double[] GetColumnMatrix(int m)
        {
            if (m > this.RowsCount)
                throw new Exception("Запрашиваемая длина столбца превышает кол-во строк матрицы.");

            double[] elements = new double[this.RowsCount];
            for (int j = 0; j < m; j++)
            {
                elements[j] = this.Items[0, j];
            }
            return elements;
        }

        /// <summary>
        /// Вывод матрицы-строки длиной n, где n - <see cref="ColumntsCount"/>
        /// </summary>
        /// <example>
        /// 
        /// Матрица:
        /// 
        /// A = | a_11   a_12   ...    a_1n | = (a_1i), размера 1 x n
        /// 
        /// </example>
        /// <returns></returns>
        public double[] GetRowMatrix() => this.GetRowMatrix(this.ColumntsCount);

        /// <summary>
        /// Вывод матрицы-строки длиной n
        /// </summary>
        /// <param name="n">Выводимое кол-во столбцов; не может превышать <see cref="ColumntsCount"/></param>
        /// <example>
        /// Матрица:
        /// 
        /// A = | a_11   a_12   ...    a_1n | = (a_1i), размера 1 x n
        /// 
        /// </example>
        /// <returns></returns>
        public double[] GetRowMatrix(int n)
        {
            if (n > this.ColumntsCount)
                throw new Exception("Запрашиваемая длина строки превышает кол-во столбцов матрицы.");

            double[] elements = new double[this.ColumntsCount];
            for (int i = 0; i < n; i++)
            {
                elements[i] = this.Items[i, 0];
            }
            return elements;
        }

        /// <summary>
        /// Элементы главной диагонали, идущей из левого верхнего в правый нижний угол
        /// </summary>
        /// <example>
        /// 
        ///     | a_11   0    ...   0 |
        /// A = |  0    a_22  ...   0 |
        ///     | ...   ...   ...  ...|
        ///     |  0     0    ... a_nn|
        ///     
        /// </example>
        public double[] ElementsMainDiagonal()
        {
            int maxK = Math.Min(this.ColumntsCount, this.RowsCount);
            double[] elements = new double[maxK];
            for(int k = 0; k < maxK; k++)
            {
                elements[0] = this.Items[k, k];
            }
            return elements;
        }

        #endregion Public functions
    }
}
