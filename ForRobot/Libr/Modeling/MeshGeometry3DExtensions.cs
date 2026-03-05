using System;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace ForRobot.Libr.Modeling
{
    /// <summary>
    /// Класс расширяющий тип <see cref="MeshGeometry3D"/>
    /// </summary>
    public static class MeshGeometry3DExtensions
    {
        /// <summary>
        /// Получение уникальных вершин
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HashSet<Point3D> GetPoints(this MeshGeometry3D mesh) => new HashSet<Point3D>(mesh.Positions);

        /// <summary>
        /// Поиск всех рёбер
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static HashSet<Tuple<int, int>> GetEdges(this MeshGeometry3D mesh)
        {
            var edges = new HashSet<Tuple<int, int>>();

            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int[] indices = new int[3] {
                                            mesh.TriangleIndices[i],
                                            mesh.TriangleIndices[i+1],
                                            mesh.TriangleIndices[i+2]
                };

                for (int j = 0; j < 3; j++)
                {
                    int a = Math.Min(indices[j], indices[(j + 1) % 3]);
                    int b = Math.Max(indices[j], indices[(j + 1) % 3]);
                    edges.Add(Tuple.Create(a, b));
                }
            }
            return edges;
        }

        /// <summary>
        /// Расчёт площади поверхности
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static double CalculateSurfaceArea(this MeshGeometry3D mesh)
        {
            double area = 0;
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                var p0 = mesh.Positions[mesh.TriangleIndices[i]];
                var p1 = mesh.Positions[mesh.TriangleIndices[i + 1]];
                var p2 = mesh.Positions[mesh.TriangleIndices[i + 2]];

                System.Windows.Media.Media3D.Vector3D v1 = p1 - p0;
                System.Windows.Media.Media3D.Vector3D v2 = p2 - p0;
                area += System.Windows.Media.Media3D.Vector3D.CrossProduct(v1, v2).Length * 0.5;
            }
            return area;
        }
    }
}
