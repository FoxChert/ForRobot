using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRobot.Libr.Collections
{
    public static class MatrixExtensions
    {
        /// <summary>
        /// Матрица является квадратной, m = n 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsSquareMatrix(this Matrix matrix) => matrix?.RowsCount == matrix?.ColumnsCount;

        /// <summary>
        /// Матрица является диагональной, т.е. это квадратная матрица, у которой все элементы вне главной диагонали равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsDiagonalMatrix(this Matrix matrix)
        {
            if (matrix == null || !matrix.IsSquareMatrix())
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    if (i == j && matrix[i, j] == 0)
                        return false;
                    else if (i != j && matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Матрица является единичной, т.е. диагональной, у которой все элементы главной диагонали равны 1
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsIdentityMatrix(this Matrix matrix)
        {
            if (matrix == null || !matrix.IsDiagonalMatrix())
                return false;

            return matrix.ElementsMainDiagonal().All(item => item == 1);
        }

        /// <summary>
        /// Матрица является (верхне или нижне) треугольной
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsTriangularMatrix(this Matrix matrix) => matrix.IsUpperTriangularMatrix() || matrix.IsLowerTriangularMatrix();

        /// <summary>
        /// Матрица является верхне треугольной, т.е. квадратной, все элементы которой, расположенные ниже главной диагонали, равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsUpperTriangularMatrix(this Matrix matrix)
        {
            if (matrix == null || !matrix.IsSquareMatrix())
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Матрица является нижне треугольной, т.е. квадратной, все элементы которой, расположенные выше главной диагонали, равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsLowerTriangularMatrix(this Matrix matrix)
        {
            if (matrix == null || !matrix.IsSquareMatrix())
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j > i; j++)
                {
                    if (matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Матрица является (верхне или нижне) трапециевидной
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsTrapezoidMatrix(this Matrix matrix) => matrix.IsUpperTrapezoidMatrix() || matrix.IsLowerTrapezoidMatrix();

        /// <summary>
        /// Матрица является верхне трапециевидной, т.е. в которой элементы ниже главной диагонали равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsUpperTrapezoidMatrix(this Matrix matrix)
        {
            if (matrix == null)
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Матрица является нижне трапециевидной, т.е. в которой элементы выше главной диагонали равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsLowerTrapezoidMatrix(this Matrix matrix)
        {
            if (matrix == null)
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j > i; j++)
                {
                    if (matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Матрица является нулевой, т.е. все элементы которой равны нулю
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsZeroMatrix(this Matrix matrix)
        {
            if (matrix == null)
                return false;

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    if (matrix[i, j] != 0)
                        return false;
                }
            }
            return true;
        }

        public static double Minor(this Matrix matrix, int rowIndex, int columnIndex)
        {
            if (!matrix.IsSquareMatrix() && matrix.RowsCount != 3)
                throw new InvalidOperationException("");
        }

        /// <summary>
        /// Транспонированная матрица
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix TransposedMatrix(this Matrix matrix)
        {
            double[,] transposed = new double[matrix.ColumnsCount, matrix.RowsCount];

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    transposed[j, i] = matrix[i, j];
                }
            }
            return new Matrix(transposed);
        }
    }

    public class Matrix
    {
        #region Public variables

        /// <summary>
        /// Кол-во строк в матрице
        /// </summary>
        public int RowsCount { get; }
        /// <summary>
        /// Кол-во столбцов в матрице
        /// </summary>
        public int ColumnsCount { get; }

        public double[,] Items { get; }

        public double this[int indexA, int indexB = 0] { get => this.Items[indexA, indexB]; set => this.Items[indexA, indexB] = value; }

        #endregion Public variables

        public class MatException : Exception
        {
            public MatException(string Message) : base(Message) { }
        }

        #region Constructors

        public Matrix(int rows, int cols)
        {
            this.RowsCount = rows;
            this.ColumnsCount = cols;
            this.Items = new double[rows, cols];
        }

        public Matrix(double[,] pointArray)
        {
            if (pointArray == null)
                throw new ArgumentNullException(nameof(pointArray));

            this.RowsCount = pointArray.GetLength(0);
            this.ColumnsCount = pointArray.GetLength(1);
            
            this.Items = new double[this.RowsCount, this.ColumnsCount];
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    this[i, j] = pointArray[i, j];
                }
            }
        }

        #endregion Constructors

        #region Private functions

        private double Determinant2() => this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];

        private double Determinant3()
        {

        }

        private double DeterminantN()
        {

        }

        #endregion Private functions

        #region Public functions

        #region Operators

        public static Matrix operator +(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA == null || matrixB == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (matrixA.RowsCount != matrixB.RowsCount || matrixA.ColumnsCount != matrixB.ColumnsCount)
                throw new InvalidOperationException("Для матриц с разным размером сложение невозможно!");

            double[,] matrixC = new double[matrixA.RowsCount, matrixB.ColumnsCount];

            for(int i = 0; i < matrixC.GetLength(0); i++)
            {
                for(int j = 0; j < matrixC.GetLength(1); j++)
                {
                    matrixC[i, j] = matrixA[i, j] + matrixB[i, j];
                }
            }
            return new Matrix(matrixC);
        }

        public static Matrix operator -(Matrix matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            Matrix result = new Matrix(matrix.RowsCount, matrix.ColumnsCount);

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    result[i, j] = -matrix[i, j];
                }
            }
            return result;
        }

        public static Matrix operator -(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA == null || matrixB == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (matrixA.RowsCount != matrixB.RowsCount || matrixA.ColumnsCount != matrixB.ColumnsCount)
                throw new InvalidOperationException("Для матриц с разным размером вычитание невозможно!");

            double[,] matrixC = new double[matrixA.RowsCount, matrixB.ColumnsCount];

            for (int i = 0; i < matrixC.GetLength(0); i++)
            {
                for (int j = 0; j < matrixC.GetLength(1); j++)
                {
                    matrixC[i, j] = matrixA[i, j] - matrixB[i, j];
                }
            }
            return new Matrix(matrixC);
        }

        public static Matrix operator *(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA == null || matrixB == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (matrixA.ColumnsCount != matrixB.RowsCount)
                throw new InvalidOperationException("Количество столбцов первой матрицы должно совпадать с количеством строк второй.");

            double[,] matrixC = new double[matrixA.RowsCount, matrixB.ColumnsCount];

            for (int i = 0; i < matrixC.GetLength(0); i++)
            {
                for (int j = 0; j < matrixC.GetLength(1); j++)
                {
                    for (int k = 0; k < matrixA.ColumnsCount; k++)
                    {
                        matrixC[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }
            return new Matrix(matrixC);
        }

        public static Matrix operator *(Matrix matrix, double scalar)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            var result = new Matrix(matrix.RowsCount, matrix.ColumnsCount);

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    result[i, j] =  matrix[i, j] * scalar;
                }
            }
            return result;
        }

        public static Matrix operator /(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA == null || matrixB == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (matrixA.RowsCount != matrixB.RowsCount || matrixA.ColumnsCount != matrixB.ColumnsCount)
                throw new InvalidOperationException("Для матриц с разным размером деление невозможно!");

            double[,] matrixC = new double[matrixA.RowsCount, matrixB.ColumnsCount];

            //for (int i = 0; i < matrixC.GetLength(0); i++)
            //{
            //    for (int j = 0; j < matrixC.GetLength(1); j++)
            //    {
            //        matrixC[i, j] = matrixA[i, j] / matrixB[i, j];
            //    }
            //}
            return new Matrix(matrixC);
        }

        public static Matrix operator /(Matrix matrix, double divisor)
        {
            if (matrix == null)
                throw new ArgumentNullException(nameof(matrix));

            if (divisor == 0)
                throw new DivideByZeroException("Деление на ноль невозможно");

            Matrix result = new Matrix(matrix.RowsCount, matrix.ColumnsCount);

            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    result[i, j] = matrix[i, j] / divisor;
                }
            }
            return matrix;
        }
        
        public static bool operator ==(Matrix matrixA, Matrix matrixB)
        {
            if (ReferenceEquals(matrixA, matrixB)) return true;
            if (matrixA is null || matrixB is null) return false;
            if (matrixA.RowsCount != matrixB.RowsCount || matrixA.ColumnsCount != matrixB.ColumnsCount)
                return false;

            for (int i = 0; i < matrixA.RowsCount; i++)
            {
                for (int j = 0; j < matrixA.ColumnsCount; j++)
                {
                    if (matrixA[i, j] != matrixB[i, j])
                        return false;
                }
            }
            return true;
        }

        public static bool operator !=(Matrix matrixA, Matrix matrixB) => !(matrixA == matrixB);

        #endregion Operators

        public override bool Equals(object obj)
        {
            if (obj is Matrix other)
                return this == other;
            return false;
        }

        public override int GetHashCode() => Items?.GetHashCode() ?? 0;

        //public static double Normalize(double[] vector)
        //{
        //    double x = 0;
        //    for(int i = 0; i < vector.Length; i++)
        //    {
        //        x += vector[i] * vector[i];
        //    }
        //    return Math.Sqrt(x);
        //}

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
        public double[] GetColumnMatrix() => this.GetColumn(this.RowsCount);

        public double[] GetColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= this.ColumnsCount)
                throw new ArgumentOutOfRangeException(nameof(columnIndex));

            double[] column = new double[this.RowsCount];
            for (int i = 0; i < this.RowsCount; i++)
            {
                column[i] = this[i, columnIndex];
            }
            return column;
        }

        /// <summary>
        /// Вывод матрицы-строки длиной n, где n - <see cref="ColumnsCount"/>
        /// </summary>
        /// <example>
        /// 
        /// Матрица:
        /// 
        /// A = | a_11   a_12   ...    a_1n | = (a_1i), размера 1 x n
        /// 
        /// </example>
        /// <returns></returns>
        public double[] GetRowMatrix() => this.GetRow(this.ColumnsCount);

        public double[] GetRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.RowsCount)
                throw new ArgumentOutOfRangeException(nameof(rowIndex));

            double[] row = new double[this.ColumnsCount];
            for (int i = 0; i < this.ColumnsCount; i++)
            {
                row[i] = this[rowIndex, i];
            }
            return row;
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
            int maxK = Math.Min(this.ColumnsCount, this.RowsCount);
            double[] elements = new double[maxK];
            for(int k = 0; k < maxK; k++)
            {
                elements[k] = this[k, k];
            }
            return elements;
        }

        /// <summary>
        /// Вычисление определителя матрицы
        /// </summary>
        /// <returns></returns>
        public double Determinant()
        {
            if (!this.IsSquareMatrix())
                throw new InvalidOperationException("Вычисление определителя возможно только для квадратной матрицы!");

            if (this.RowsCount == 2)
                return this.Determinant2();
            else if (this.RowsCount == 3)
                return this.Determinant3();
            else
                return this.DeterminantN();
        }

        public Matrix Clone()
        {
            Matrix clone = new Matrix(this.RowsCount, this.ColumnsCount);
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    clone[i, j] = this[i, j];
                }
            }
            return clone;
        }

        #endregion Public functions
    }
}
