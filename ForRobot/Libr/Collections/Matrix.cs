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
                    if (i != j && matrix[i, j] != 0)
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
                for (int j = 0; j < matrix.ColumnsCount; j++)
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
                for (int j = 0; j > matrix.ColumnsCount; j++)
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
                for (int j = 0; j < matrix.ColumnsCount; j++)
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
                for (int j = 0; j > matrix.ColumnsCount; j++)
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

        protected Matrix() { }

        public Matrix(int length) : this(length, length) { }

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

        /// <summary>
        /// Определитель квадратной матрицы п-го порядка
        /// </summary>
        /// <returns></returns>
        private double DetN()
        {
            double determinant = 0;

            if (this.IsTriangularMatrix()) // Для треугольной матрицы определитель определяется умножением элементов главной диагонали
            {
                determinant = 1;
                foreach(double val in this.ElementsMainDiagonal())
                    determinant *= val;
            }
            else
            {
                for (int i = 0; i < this.ColumnsCount; i++)
                {
                    determinant += this[0, i] * A_ij(0, i);
                }
            }
            return determinant;
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

        /// <summary>
        /// -A = (-1) * A
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>Матрица противоположная <paramref name="matrix"/></returns>
        public static Matrix operator -(Matrix matrix) => matrix * (-1);

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

            for (int i = 0; i < matrixA.RowsCount; i++)
            {
                for (int j = 0; j < matrixB.ColumnsCount; j++)
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

            if (!matrixA.IsSquareMatrix() || !matrixB.IsSquareMatrix())
                throw new InvalidOperationException("Операция применима только к квадратным матрицам!");

            return matrixA * matrixB.Inverse();
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
            return result;
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

        #region Static

        /// <summary>
        /// Вычисление евклидовой нормы вектора
        /// </summary>
        /// <param name="vector">Входной вектор</param>
        /// <returns>Длина вектора</returns>
        public static double Normalize(double[] vector)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            double sum = 0;

            for (int i = 0; i < vector.Length; i++)
                sum += vector[i] * vector[i];

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Приведение вектора к единичной длине (унитарной норме)
        /// </summary>
        /// <param name="vector">Входной вектор</param>
        /// <param name="norm"></param>
        public static double[] NormLength(double[] vector)
        {
            if (vector == null)
                throw new ArgumentNullException(nameof(vector));

            double norm = Normalize(vector);

            if (norm <= 0)
                throw new ArgumentException($"Невозможно нормализовать вектор: норма = {norm}. Вектор должен иметь положительную длину.", nameof(norm));

            double[] result = new double[vector.Length];

            for (int i = 0; i < vector.Length; i++)
                result[i] = vector[i] / norm;

            return result;
        }

        /// <summary>
        /// Скалярное произведение двух векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Скаляр, т.е. число не зависящее от выбора системы координат</returns>
        public static double Dot(double[] a, double[] b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (a.Length != b.Length)
                throw new InvalidOperationException("Вычисление скалярного произведения векторов разной длины невозвозможно!");

            double norm = 0.0;
            for(int i = 0; i < a.Length; i++)
            {
                norm += a[i] * b[i];
            }
            return norm;
        }

        /// <summary>
        /// Простейшее произведение трёх векторов
        /// <para>
        /// Скалярное произведение двух векторов (a, b), умноженное на третий вектор c
        /// </para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] Dot(double[] a, double[] b, double[] c)
        {
            if (a == null || b == null || c == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (a.Length != b.Length)
                throw new InvalidOperationException("Длины a и b должны совпадать.");

            double scalar = Dot(a, b);
            double[] result = new double[c.Length];

            for (int i = 0; i < c.Length; i++)
                result[i] = scalar * c[i];

            return result;
        }

        /// <summary>
        /// Векторное произведение двух векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Вектор, перпендикулярный двум исходным векторам</returns>
        public static double[] Cross(double[] a, double[] b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            if (a.Length != 3 || b.Length != 3)
                throw new InvalidOperationException("Векторное произведение определено только для векторов размерности 3.");

            // Компоненты вектора a
            double a1 = a[0], a2 = a[1], a3 = a[2];
            // Компоненты вектора b
            double b1 = b[0], b2 = b[1], b3 = b[2];
            
            double c1 = new Matrix(new double[,] { { a2, a3 }, { b2, b3 } }).Det(); // a2*b3 - a3*b2
            double c2 = new Matrix(new double[,] { { a3, a1 }, { b3, b1 } }).Det(); // a3*b1 - a1*b3
            double c3 = new Matrix(new double[,] { { a1, a2 }, { b1, b2 } }).Det(); // a1*b2 - a2*b1

            return new double[] { c1, c2, c3 };
        }

        /// <summary>
        /// Смешанное произведеение
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns>Векторное произведение двух векторов, сколярно умноженное на третий вектор</returns>
        public static double MixCross(double[] a, double[] b, double[] c) => Dot(Cross(a, b), c);

        /// <summary>
        /// Двойное векторное произведение трёх векторов
        /// <para>
        /// Векторное произведение двух векторов умножается векторно на третий вектор.
        /// </para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] Cross(double[] a, double[] b, double[] c) => Cross(Cross(a, b), c);

        /// <summary>
        /// Вычисление угла между двумя векторами
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>Угол в радианах</returns>
        public static double Angel(double[] a, double[] b) => Math.Acos(Dot(a, b));

        /// <summary>
        /// Тензорное (диадное) произведение двух векторов
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Dim(double[] a, double[] b)
        {
            if (a == null || b == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            double[,] result = new double[a.Length, b.Length];
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    result[i, j] = a[i] * b[j];
                }
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Генерация нулевой матрицы
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static Matrix ZeroMatrix(int rowCount, int colCount) => new Matrix(new double[rowCount, colCount]);

        /// <summary>
        /// Генерация единичной матрицы
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <returns></returns>
        public static Matrix IdentityMatrix(int rowCount, int colCount)
        {
            Matrix matrix = ZeroMatrix(rowCount, colCount);
            for (int i = 0; i < Math.Min(rowCount, colCount); i++)
                matrix[i, i] = 1;
            return matrix;
        }

        #endregion Static

        public override bool Equals(object obj)
        {
            if (obj is Matrix other)
                return this == other;
            return false;
        }

        public override int GetHashCode() => Items?.GetHashCode() ?? 0;

        /// <summary>
        /// Вывод матрицы-столбца длиной m, где m - 1
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
        /// <returns>Вектор-столбец, как объект типа <see cref="Matrix"/></returns>
        public Matrix GetColumnMatrix() => this.GetColumnMatrix(0);

        /// <summary>
        /// Вывод матрицы-столбца длиной m
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
        /// <param name="colIndex">Индекс столбца - m</param>
        /// <returns>Вектор-столбец, как объект типа <see cref="Matrix"/></returns>
        public Matrix GetColumnMatrix(int colIndex)
        {
            var col = this.GetColumn(colIndex);

            double[,] result = new double[col.Length, 1];

            for (int j = 0; j < col.Length; j++)
            {
                result[j, 0] = col[j];
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Вывод столбца матрицы
        /// </summary>
        /// <param name="columnIndex">Индекс столбца</param>
        /// <returns>Вектор-столбец</returns>
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
        /// Присвоение значенний элементам столбца
        /// </summary>
        /// <param name="column">Матрица-столбец - источник значений</param>
        /// <param name="colIndex">Индекс столбца</param>
        public void SetColumn(Matrix column, int colIndex)
        {
            for (int i = 0; i < Math.Min(column.RowsCount, this.RowsCount); i++)
            {
                this[i, colIndex] = column[i, 0];
            }
        }

        /// <summary>
        /// Вывод матрицы-строки длиной n, где n - 1
        /// </summary>
        /// <example>
        /// 
        /// Матрица:
        /// 
        /// A = | a_11   a_12   ...    a_1n | = (a_1i), размера 1 x n
        /// 
        /// </example>
        /// <returns>Вектор-строка, как объект типа <see cref="Matrix"/></returns>
        public Matrix GetRowMatrix() => GetRowMatrix(0);

        /// <summary>
        /// Вывод матрицы-строки длиной n
        /// </summary>
        /// <example>
        /// 
        /// Матрица:
        /// 
        /// A = | a_11   a_12   ...    a_1n | = (a_1i), размера 1 x n
        /// 
        /// </example>
        /// <param name="rowIndex">Индекс строки - n</param>
        /// <returns>Вектор-строка, как объект типа <see cref="Matrix"/></returns>
        public Matrix GetRowMatrix(int rowIndex)
        {
            var row = this.GetRow(rowIndex);

            double[,] result = new double[1, row.Length];

            for(int i = 0; i < row.Length; i++)
            {
                result[0, i] = row[i];
            }
            return new Matrix(result);
        }

        /// <summary>
        /// Вывод строки матрицы
        /// </summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <returns>Вектор-строка</returns>
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
        /// Присвоение значенний элементам строки
        /// </summary>
        /// <param name="row">Матрица-строка - источник значений</param>
        /// <param name="rowIndex">Индекс строки</param>
        public void SetRow(Matrix row, int rowIndex)
        {
            for (int j = 0; j < Math.Min(row.ColumnsCount, this.ColumnsCount); j++)
            {
                this[rowIndex, j] = row[0, j];
            }
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
        public double Det()
        {
            if (!this.IsSquareMatrix())
                throw new InvalidOperationException("Вычисление определителя возможно только для квадратной матрицы!");

            int size = this.Items.GetLength(0);

            if (size == 1) return this[0, 0];
            if (size == 2) return this[0, 0] * this[1, 1] - this[0, 1] * this[1, 0];
            if (size == 3) return this[0, 0] * this[1, 1] * this[2, 2] + this[0, 1] * this[1, 2] * this[2, 0] + this[0, 2] * this[1, 0] * this[2, 1] - (this[0, 2] * this[1, 1] * this[2, 0] + this[0, 0] * this[1, 2] * this[2, 1] + this[0, 1] * this[1, 0] * this[2, 2]);
            else return this.DetN();
        }

        /// <summary>
        /// Вычисление минора элемента матрицы.
        /// <para>
        /// Минор - определитель порядка (n-1), полученный из элементов матрицы путем вычеркивания i–строки и j–столбца, на пересечении которых стоит этот элемент
        /// </para>
        /// </summary>
        /// <param name="rowIndex">Индекс строки элемента</param>
        /// <param name="columnIndex">Индекс столбца элемента</param>
        /// <returns></returns>
        public double M_ij(int rowIndex, int columnIndex)
        {
            if (!this.IsSquareMatrix())
                throw new InvalidOperationException("Вычисление минора возможно только для квадратной матрицы!");
            
            double[,] matrix = new double[this.RowsCount - 1, this.ColumnsCount - 1];

            int mi = 0; // Индекс строки минора.
            for(int i = 0; i < this.RowsCount; i++)
            {
                if (i == rowIndex) continue;

                int mj = 0; // Индекс столбца минора.

                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    if (j == columnIndex) continue;
                    matrix[mi, mj] = this[i, j];
                    mj++;
                }
                mi++;
            }
            return new Matrix(matrix).Det();
        }

        /// <summary>
        /// Алгебраическое дополнение элемента
        /// <para>
        /// Это определитель матрицы, полученной вычеркиванием i-ой строки и j-го столбца, в которых находится элемент, с учетом знака (минус - для нечётной сумме индексов; плюс - для чётной).
        /// </para>
        /// </summary>
        /// <param name="rowIndex">Индекс строки элемента</param>
        /// <param name="columnIndex">Индекс столбца элемента</param>
        /// <returns></returns>
        public double A_ij(int rowIndex, int columnIndex) => Math.Pow(-1, rowIndex + columnIndex) * this.M_ij(rowIndex, columnIndex);

        /// <summary>
        /// Бесконечная норма
        /// <para>
        /// Матричная норма на бесконечности, максимальная абсолютная сумма элементов по строкам
        /// </para>
        /// </summary>
        /// <returns></returns>
        public double Normi()
        {
            double[] norm = new double[this.RowsCount];
            for(int i = 0; i < this.RowsCount; i++)
            {
                for(int j = 0; j < this.ColumnsCount; j++)
                {
                    norm[i] += Math.Abs(this[i, j]);
                }
            }
            return norm.Max();
        }

        /// <summary>
        /// Первая норма
        /// <para>
        /// Матричная норма в пространстве L1, максимальная абсолютная сумма элементов по столбцам
        /// </para>
        /// </summary>
        /// <returns></returns>
        public double Norm1()
        {
            double[] norm = new double[this.ColumnsCount];
            for (int j = 0; j < this.ColumnsCount; j++)
            {
                for (int i = 0; i < this.RowsCount; i++)
                {
                    norm[j] += Math.Abs(this[i, j]);
                }
            }
            return norm.Max();
        }

        /// <summary>
        /// Спектральная норма (вторая норма, норма Гильберта)
        /// <para>
        /// Максимальное сингулярное число матрицы, равное квадратному корню из максимального собственного значения матрицы A^T * A
        /// </para>
        /// </summary>
        /// <returns></returns>
        public double Norm2()
        {
            if (this.IsZeroMatrix()) return 0.0;
            
            int n = this.ColumnsCount;

            // Начальный (вектор-столбец) – матрица n×1
            double[,] init = new double[n, 1];
            for (int i = 0; i < n; i++)
                init[i, 0] = 1.0;
            Matrix b = new Matrix(init);   // Текущее приближение собственного вектора

            double eigenvalue = 0.0;
            const double tolerance = 1e-12;
            const int maxIterations = 1000;

            for (int iter = 0; iter < maxIterations; iter++)
            {
                Matrix w = this * b; // (размер m×1)
                Matrix u = this.TransposedMatrix() * w; // (размер n×1)  — это(A ^ T * A) * b

                // Отношение Рэлея: λ = (b·u) / (b·b)
                double bDotU = 0.0, bDotB = 0.0;
                for (int i = 0; i < n; i++)
                {
                    bDotB += b[i, 0] * b[i, 0];
                    bDotU += b[i, 0] * u[i, 0];
                }

                if (bDotB == 0.0)
                    break;

                double lambda = bDotU / bDotB;

                // Проверка сходимости
                if (Math.Abs(lambda - eigenvalue) < tolerance)
                {
                    eigenvalue = lambda;
                    break;
                }
                eigenvalue = lambda;

                // Нормализация вектора u для следующей итерации
                double normU = 0.0;
                for (int i = 0; i < n; i++)
                    normU += u[i, 0] * u[i, 0];
                normU = Math.Sqrt(normU);

                if (normU == 0.0)
                    break;

                // Создание нового нормализованного вектора-столбеца
                double[,] next = new double[n, 1];
                for (int i = 0; i < n; i++)
                    next[i, 0] = u[i, 0] / normU;
                b = new Matrix(next);
            }

            return Math.Sqrt(eigenvalue); // Корень из максимального собственного значения
        }

        /// <summary>
        /// Сферическая норма (норма Фробениуса, евклидова норма)
        /// <para>
        /// Матричная норма в евклидовом пространстве, корень из суммы квадратов всех элементов матрицы
        /// </para>
        /// </summary>
        /// <returns></returns>
        public double Norme()
        {
            double norm = 0;
            for (int i = 0; i < this.RowsCount; i++)
            {
                for (int j = 0; j < this.ColumnsCount; j++)
                {
                    norm += Math.Pow(this[i, j], 2);
                }
            }
            return Math.Sqrt(norm);
        }
               
        ///// <summary>
        ///// Ранг матрицы
        ///// <para>
        ///// Это наибольший из порядков миноров матрицы, отличных от нуля
        ///// </para>
        ///// </summary>
        ///// <returns></returns>
        //public int Rang()
        //{
        //    int maxK = Math.Min(this.ColumnsCount, this.RowsCount);

        //}

        /// <summary>
        /// Является ли представленная матрица обратной к исходной
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool IsInverseMatrix(Matrix matrix) => (this * matrix).IsIdentityMatrix();

        /// <summary>
        /// Присоединённая (союзная, взаимная) матрица
        /// <para>
        /// Матрица из алгебраических дополнений
        /// </para>
        /// </summary>
        /// <returns></returns>
        public Matrix Adj()
        {
            Matrix matrix = new Matrix(this.RowsCount, this.ColumnsCount);
            var t = this.TransposedMatrix();
            for (int i = 0; i < this.RowsCount; i++)
            {
                for(int j = 0; j < this.ColumnsCount; j++)
                {
                    matrix[i, j] = t.A_ij(i, j);
                }
            }
            return matrix;
        }

        /// <summary>
        /// Обратная матрица
        /// </summary>
        /// <returns></returns>
        public Matrix Inverse()
        {
            double det = this.Det();

            if (det == 0)
                throw new InvalidOperationException("Вырождённая матрица не имеет обратной!");

            return this.Adj() / det;
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
