using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForRobot.Libr.Collections
{
    /// <summary>
    /// Гомогенная (однородная) матрица координат
    /// </summary>
    public class HomogeneousMatrix : Matrix
    {
        #region Constructors

        public HomogeneousMatrix(int rowCount, int colCount) : base(rowCount, colCount)
        {
            this.ValidateHomogeneousMatrix();
        }

        public HomogeneousMatrix(double[,] pointArray) : base(pointArray)
        {
            this.ValidateHomogeneousMatrix();
        }

        public HomogeneousMatrix(Matrix matrix) : base(matrix.RowsCount, matrix.ColumnsCount)
        {
            for (int i = 0; i < matrix.RowsCount; i++)
            {
                for (int j = 0; j < matrix.ColumnsCount; j++)
                {
                    this[i, j] = matrix[i, j];
                }
            }
            this.ValidateHomogeneousMatrix();
        }

        #endregion Constructors

        #region Prifate functions

        /// <summary>
        /// Проверка однородной матрицы на соответствие требованиям
        /// </summary>
        /// <returns></returns>
        private bool ValidateHomogeneousMatrix()
        {
            if (!this.IsSquareMatrix() || (this.RowsCount != 3 && this.RowsCount != 4))
                throw new InvalidOperationException("Неверная размерность матрицы. Однородная матрица должна быть 3x3 (в 2D) или 4x4 (в 3D)!");
            return true;
        }

        #endregion Prifate functions

        #region Public functions

        #region Operators

        public static HomogeneousMatrix operator *(HomogeneousMatrix matrixA, HomogeneousMatrix matrixB)
        {
            if (matrixA == null || matrixB == null)
                throw new ArgumentNullException("Один из аргументов равен null");

            Matrix result = matrixA * matrixB;
            return new HomogeneousMatrix(result);
        }

        #endregion Operators

        #region Static

        /// <summary>
        /// Создание единичной гомогенной матрицы 3x3
        /// </summary>
        /// <returns></returns>
        public static HomogeneousMatrix Identity3x3() => new HomogeneousMatrix(Matrix.IdentityMatrix(3, 3).Items);

        /// <summary>
        /// Создание единичной гомогенной матрицы 4x4
        /// </summary>
        /// <returns></returns>
        public static HomogeneousMatrix Identity4x4() => new HomogeneousMatrix(Matrix.IdentityMatrix(4, 4).Items);

        /// <summary>
        /// Создание матрицы преобразования 2D перемещения
        /// </summary>
        /// <example>
        ///            |  1   0   tx |
        ///  T(x, y) = |  0   1   ty |
        ///            |  0   0    1 |
        /// </example>
        /// <param name="tx">Перемещение по X</param>
        /// <param name="ty">Перемещение по Y</param>
        /// <returns></returns>
        public static HomogeneousMatrix Translation2D(double tx, double ty)
        {
            var matrix = Identity3x3();
            matrix[0, 2] = tx;
            matrix[1, 2] = ty;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования 3D перемещения
        /// </summary>
        /// <example>
        ///               |  1   0   0   tx |
        ///  T(x, y, z) = |  0   1   0   ty |
        ///               |  0   0   1   tz |
        ///               |  0   0   0    1 |
        /// </example>
        /// <param name="tx">Перемещение по X</param>
        /// <param name="ty">Перемещение по Y</param>
        /// <param name="tz">Перемещение по Z</param>
        /// <returns></returns>
        public static HomogeneousMatrix Translation3D(double tx, double ty, double tz)
        {
            var matrix = Identity4x4();
            matrix[0, 3] = tx;
            matrix[1, 3] = ty;
            matrix[2, 3] = tz;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования 2D поворота
        /// </summary>
        /// <example>
        ///            |  soc   -sin    0 |
        ///  T(x, y) = |  sin    cos    0 |
        ///            |   0      0     1 |
        /// </example>
        /// <param name="angle">Угол поворота в радианах</param>
        /// <returns></returns>
        public static HomogeneousMatrix Rotation2D(double angle)
        {
            var matrix = Identity3x3();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования 3D поворота вокруг оси X
        /// </summary>
        /// <example>
        ///               |  1      0       0       tx |
        ///  T(x, y, z) = |  0    cos(x)  -sin(x)   ty |
        ///               |  0    sin(x)   cos(x)   tz |
        ///               |  0      0       0        1 |
        /// </example>
        /// <param name="angle">Угол поворота в радианах</param>
        /// <returns></returns>
        public static HomogeneousMatrix Rotation3DAxisX(double angle)
        {
            var matrix = Identity4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[3, 2] = sin;
            matrix[3, 3] = cos;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования 3D поворота вокруг оси Y
        /// </summary>
        /// <example>
        ///               | cos(x)  0   sin(x)   tx |
        ///  T(x, y, z) = |  0      1    0       ty |
        ///               |-sin(x)  0   cos(x)   tz |
        ///               |  0      0    0        1 |
        /// </example>
        /// <param name="angle">Угол поворота в радианах</param>
        /// <returns></returns>
        public static HomogeneousMatrix Rotation3DAxisY(double angle)
        {
            var matrix = Identity4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 2] = sin;
            matrix[2, 0] = -sin;
            matrix[2, 2] = cos;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования 3D поворота вокруг оси Z
        /// </summary>
        /// <example>
        ///               | cos(x)  -sin(x)    0   tx |
        ///  T(x, y, z) = | sin(x)   cos(x)    0   ty |
        ///               |  0        0        1   tz |
        ///               |  0        0        0    1 |
        /// </example>
        /// <param name="angle">Угол поворота в радианах</param>
        /// <returns></returns>
        public static HomogeneousMatrix Rotation3DAxisZ(double angle)
        {
            var matrix = Identity4x4();
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования масштабирования 2D
        /// </summary>
        /// <param name="sx">Масштаб по X</param>
        /// <param name="sy">Масштаб по Y</param>
        /// <returns></returns>
        public static HomogeneousMatrix Scale2D(double sx, double sy)
        {
            var matrix = Identity3x3();
            matrix[0, 0] = sx;
            matrix[1, 1] = sy;
            return matrix;
        }

        /// <summary>
        /// Создание матрицы преобразования масштабирования 3D
        /// </summary>
        /// <param name="sx">Масштаб по X</param>
        /// <param name="sy">Масштаб по Y</param>
        /// <param name="sz">Масштаб по Z</param>
        /// <returns></returns>
        public static HomogeneousMatrix Scale3D(double sx, double sy, double sz)
        {
            var matrix = Identity4x4();
            matrix[0, 0] = sx;
            matrix[1, 1] = sy;
            matrix[2, 2] = sz;
            return matrix;
        }

        /// <summary>
        /// Преобразование точки 2D с помощью гомогенной матрицы
        /// </summary>
        /// <param name="point">Точка [x, y]</param>
        /// <returns>Преобразованная точка</returns>
        public double[] TransformPoint2D(double[] point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            if (point.Length != 2)
                throw new ArgumentException("Точка должна содержать 2 координаты", nameof(point));

            if (this.RowsCount != 3)
                throw new InvalidOperationException("Для 2D преобразования требуется 3x3 матрица");

            // Преобразование точки в однородные координаты [x, y, 1]
            double[] homogeneousPoint = new double[] { point[0], point[1], 1 };

            // Умножение матрицы на вектор.
            double[] result = new double[3];
            for (int i = 0; i < 3; i++)
            {
                result[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    result[i] += this[i, j] * homogeneousPoint[j];
                }
            }
            
            return new double[] { result[0] / result[2], result[1] / result[2] };
        }

        /// <summary>
        /// Преобразование точки 3D с помощью гомогенной матрицы
        /// </summary>
        /// <param name="point">Точка [x, y, z]</param>
        /// <returns>Преобразованная точка</returns>
        public double[] TransformPoint3D(double[] point)
        {
            if (point == null)
                throw new ArgumentNullException(nameof(point));

            if (point.Length != 3)
                throw new ArgumentException("Точка должна содержать 3 координаты", nameof(point));

            if (this.RowsCount != 4)
                throw new InvalidOperationException("Для 3D преобразования требуется 4x4 матрица");

            // Преобразование точки в однородные координаты [x, y, z, 1]
            double[] homogeneousPoint = new double[] { point[0], point[1], point[2], 1 };

            // Умножение матрицы на вектор
            double[] result = new double[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = 0;
                for (int j = 0; j < 4; j++)
                {
                    result[i] += this[i, j] * homogeneousPoint[j];
                }
            }

            return new double[] { result[0] / result[3], result[1] / result[3], result[2] / result[3] };
        }

        #endregion Static

        public new HomogeneousMatrix Clone() => base.Clone() as HomogeneousMatrix;

        #endregion Public functions
    }
}
