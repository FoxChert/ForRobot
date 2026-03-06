 using System;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Threading.Tasks;
using System.Collections.Generic;

using HelixToolkit.Wpf;

using ForRobot.Libr.Strategies.ModelingStrategies;
using ForRobot.Models.Detals;
using ForRobot.Models.Settings;

namespace ForRobot.Libr.Modeling
{
    public class SceneSchema
    {
        public IEnumerable<ForRobot.Models.RoboticComplex.Robot> RobotsCollection { get; set; }
        public Detal Detal { get; set; }
    }

    /// <summary>
    /// Класс сервис для сборки 3д сцены
    /// </summary>
    public static class ModelingService
    {
        private static readonly List<IDetalModelingStrategy> _strategies = new List<IDetalModelingStrategy>() {  new PlateModelingStrategy() };
        private static ForRobot.Libr.Services.Providers.IModelProvider _modelProvider = new CacheModel3DProvider(new Model3DProvider()); // Кэшированное хранение моделей.

        ///// <summary>
        ///// Смещение скосов
        ///// </summary>
        //public const double SLOPE_OFF_SET = 10;

        /// <summary>
        /// Коэффициент сужения/расширения для трапеций (от 0.1 до 0.9)
        /// </summary>
        public const double TRAPEZOID_RATIO = 0.2;

        ///// <summary>
        ///// Выгрузка модели из компонентов сборки
        ///// </summary>
        ///// <param name="modelPath"></param>
        ///// <returns></returns>
        //public static Model3DGroup LoadModel(string modelPath)
        //{
        //    Model3DGroup robotModel;
        //    try
        //    {
        //        robotModel = new ModelImporter().Load(modelPath);
        //        if (robotModel == null)
        //        {
        //            throw new System.IO.FileNotFoundException($"Не удалось выгрузить модель робота по пути: {modelPath}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException($"Ошибка выгрузки модели робота: {ex.Message}", ex);
        //    }
        //    return robotModel;
        //}

        /// <summary>
        /// Вывод модели компьютера
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="transform3DGroup"></param>
        /// <returns></returns>
        public static Model3DGroup GetPcModel(double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            //Vector3D pcTranslate = new Vector3D(x, y, z);
            Model3DGroup pcModel = _modelProvider.GetPcModel();

            if (transform3DGroup == null)
                pcModel.Transform = Transform3DBuilder.Create().Translate(x, y, z);
            else if (!transform3DGroup.HasTranslationApplied(out _))
            {
                transform3DGroup.Translate(x, y, z);
                pcModel.Transform = transform3DGroup;
            }
            else
                pcModel.Transform = transform3DGroup;

            return pcModel;
        }

        /// <summary>
        /// Вывод модели робота
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="transform3DGroup"></param>
        /// <returns></returns>
        public static Model3DGroup GetRobotModel(double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            //Vector3D robotTranslate = new Vector3D(x, y, z);
            Model3DGroup robotModel = _modelProvider.GetRobotModel();

            if (transform3DGroup == null)
                robotModel.Transform = Transform3DBuilder.Create().Translate(x, y, z);
            else if (!transform3DGroup.HasTranslationApplied(out _))
            {
                transform3DGroup.Translate(x, y, z);
                robotModel.Transform = transform3DGroup;
            }
            else
                robotModel.Transform = transform3DGroup;

            return robotModel;
        }

        /// <summary>
        /// Вывод модели человека
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="transform3DGroup"></param>
        /// <returns></returns>
        public static Model3DGroup GetMansModel(double x = 0, double y = 0, double z = 0, Transform3DGroup transform3DGroup = null)
        {
            Vector3D manTranslate = new Vector3D(x, y, z);
            Model3DGroup manModel = _modelProvider.GetMansModel();

            if (transform3DGroup == null)
                manModel.Transform = Transform3DBuilder.Create().Translate(x, y, z);
            else if (!transform3DGroup.HasTranslationApplied(out _))
            {
                transform3DGroup.Translate(x, y, z);
                manModel.Transform = transform3DGroup;
            }
            else
                manModel.Transform = transform3DGroup;

            return manModel;
        }

        /// <summary>
        /// Вывод модели детали
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="transform3DGroup"></param>
        /// <returns></returns>
        public static Model3DGroup GetDetalModel(Detal detal)
        {
            IDetalModelingStrategy strategy = _strategies.FirstOrDefault(s => s.CanHandle(detal.DetalType));
            Model3DGroup detalModel3D = strategy?.CreateModel3D(detal) ?? null;
            return detalModel3D;
        }

        public static Model3DGroup Get3DScene(Detal detal)
        {
            Model3DGroup scene = new Model3DGroup();

            return scene;
        }
    }

    /// <summary>
    /// Класс сервис для сборки 3д сцены
    /// </summary>
    //public sealed class ModelingService 
    //{
    //    private const int DEFAULT_DISTANCE = 7;
    //    /// <summary>
    //    /// Масштабный коэффициент
    //    /// </summary>
    //    //private readonly double _scaleFactor;

    //    ///// <summary>
    //    ///// Смещение скоса
    //    ///// </summary>
    //    //public const double SLOPE_OFF_SET = 10;
    //    /// <summary>
    //    /// Коэффициент сужения/расширения для трапеций (от 0.1 до 0.9)
    //    /// </summary>
    //    public const double TRAPEZOID_RATIO = 0.2;

    //    private readonly List<IDetalModelingStrategy> _strategies = new List<IDetalModelingStrategy>()
    //    {
    //        new PlateModelingStrategy()
    //    };

    //    #region Contructor

    //    //public ModelingService()
    //    //{
    //    //    this._strategies = strategies;
    //    //    this._scaleFactor = scaleFactor;
    //    //}

    //    #endregion

    //    #region Private functions

    //    /// <summary>
    //    /// Применяет выбранную конфигурацию расположения дополнительных моделей
    //    /// </summary>
    //    private void ApplySceneConfiguration(Model3DGroup scene, Detal detal, SceneConfiguration configuration)
    //    {
    //        switch (detal.DetalType)
    //        {
    //            case DetalTypes.Plate:
    //                Plate plate = detal as Plate;

    //                if (configuration == SceneConfiguration.FirstCehConfiguration)
    //                    this.PlateFirstCehConfiguration(scene, plate);
    //                else if (configuration == SceneConfiguration.SecondCehConfiguration)
    //                    this.PlateSecondCehConfiguration(scene, plate);
    //                break;
    //        }
    //    }

    //    private void PlateFirstCehConfiguration(Model3DGroup scene, Plate plate)
    //    {
    //        double halfModelPlateLength = (double)plate.PlateLength * this._scaleFactor / 2;
    //        double halfModelPlateWidth = (double)plate.PlateWidth * this._scaleFactor / 2;
    //        double halfModelPlateHeight = (double)plate.PlateThickness * this._scaleFactor / 2;

    //        double offsetDirection = (plate.ScoseType == ScoseTypes.SlopeLeft || plate.ScoseType == ScoseTypes.SlopeRight) ? SLOPE_OFF_SET : 0;

    //        Vector3D robotTranslate = new Vector3D((halfModelPlateLength + offsetDirection) * -1, halfModelPlateWidth + DEFAULT_DISTANCE, 0);
    //        var robotTransform = Transform3DBuilder.Create()
    //                                               .Scale(this._scaleFactor * 10)
    //                                               .Rotate(new System.Windows.Media.Media3D.Vector3D(0, 0, 1), -90);
    //        scene.Children.AddRobot(robotTranslate.X, robotTranslate.Y, robotTranslate.Z, robotTransform);

    //        Vector3D pcTranslate = new Vector3D((halfModelPlateLength + offsetDirection + DEFAULT_DISTANCE) * -1, halfModelPlateWidth + DEFAULT_DISTANCE, 0);
    //        var pcTransform = Transform3DBuilder.Create()
    //                                            .Scale(this._scaleFactor * 200).Translate(pcTranslate)
    //                                            .Rotate(new System.Windows.Media.Media3D.Vector3D(1, 0, 0), 90, new Point3D(pcTranslate.X, pcTranslate.Y, pcTranslate.Z));
    //        scene.Children.AddPC(pcTranslate.X, pcTranslate.Y, pcTranslate.Z, pcTransform);

    //        var manATransform = Transform3DBuilder.Create()
    //                                  .Scale(this._scaleFactor)
    //                                  .Rotate(new System.Windows.Media.Media3D.Vector3D(1, 0, 0), 90);

    //        var manBTransform = manATransform.Clone()
    //                                         .Rotate(new System.Windows.Media.Media3D.Vector3D(0, 0, 1), 90);

    //        scene.Children.AddMan(0, halfModelPlateWidth + 15, 0, manATransform);
    //        scene.Children.AddMan((halfModelPlateLength + DEFAULT_DISTANCE * 1.5 + offsetDirection) * -1, 0, 0, manBTransform);
    //    }

    //    private void PlateSecondCehConfiguration(Model3DGroup scene, Plate plate)
    //    {
    //        double halfModelPlateLength = (double)plate.PlateLength * this._scaleFactor / 2;
    //        double halfModelPlateWidth = (double)plate.PlateWidth * this._scaleFactor / 2;
    //        double halfModelPlateHeight = (double)plate.PlateThickness * this._scaleFactor / 2;

    //        double offsetDirection = (plate.ScoseType == ScoseTypes.SlopeLeft || plate.ScoseType == ScoseTypes.SlopeRight) ? SLOPE_OFF_SET : 0;

    //        Vector3D robotTranslate = new Vector3D(halfModelPlateLength + offsetDirection, -halfModelPlateWidth - DEFAULT_DISTANCE, 0);
    //        var robotTransform = Transform3DBuilder.Create()
    //                                               .Scale(this._scaleFactor * 10)
    //                                               .Translate(robotTranslate)
    //                                               .Rotate(new System.Windows.Media.Media3D.Vector3D(0, 0, 1), 90, new Point3D(robotTranslate.X, robotTranslate.Y, robotTranslate.Z));
    //        scene.Children.AddRobot(robotTranslate.X, robotTranslate.Y, robotTranslate.Z, robotTransform);

    //        Vector3D pcTranslate = new Vector3D(-halfModelPlateLength - offsetDirection, -halfModelPlateWidth - DEFAULT_DISTANCE, 0);
    //        var pcTransform = Transform3DBuilder.Create()
    //                                            .Scale(this._scaleFactor * 200).Translate(pcTranslate)
    //                                            .Rotate(new System.Windows.Media.Media3D.Vector3D(1, 0, 0), 90, new Point3D(pcTranslate.X, pcTranslate.Y, pcTranslate.Z))
    //                                            .Rotate(new System.Windows.Media.Media3D.Vector3D(0, 0, 1), 180, new Point3D(pcTranslate.X, pcTranslate.Y, pcTranslate.Z));
    //        scene.Children.AddPC(pcTranslate.X, pcTranslate.Y, pcTranslate.Z, pcTransform);

    //        var manATransform = Transform3DBuilder.Create()
    //                                              .Scale(this._scaleFactor)
    //                                              .Rotate(new System.Windows.Media.Media3D.Vector3D(1, 0, 0), 90);

    //        var manBTransform = manATransform.Clone()
    //                                         .Rotate(new System.Windows.Media.Media3D.Vector3D(0, 0, 1), -90);

    //        scene.Children.AddMan(0, halfModelPlateWidth + 15, 0, manATransform);
    //        scene.Children.AddMan(halfModelPlateLength + DEFAULT_DISTANCE + offsetDirection, 0, 0, manBTransform);
    //    }

    //    #endregion Private functions

    //    #region Public functions

    //    public Model3DGroup Get3DScene(Detal detal)
    //    {
    //        Model3DGroup scene = new Model3DGroup();

    //        IDetalModelingStrategy strategy = _strategies.FirstOrDefault(s => s.CanHandle(DetalTypes.StringToEnum(detal.DetalType)));
    //        Model3DGroup detalModel3D = strategy?.CreateModel3D(detal) ?? null;

    //        double halfModelPlateLength = (double)detal.PlateLength * this._scaleFactor / 2;
    //        double halfModelPlateWidth = (double)detal.PlateWidth * this._scaleFactor / 2;
    //        double halfModelPlateHeight = (double)detal.PlateThickness * this._scaleFactor / 2;

    //        switch (detal.DetalType)
    //        {
    //            case DetalTypes.Plate:
    //                Plate plate = detal as Plate;
    //                detalModel3D.SetName("Plate");

    //                this.ApplySceneConfiguration(scene, plate, SceneConfiguration.FirstCehConfiguration);
    //                break;

    //            default:
    //                throw new NotSupportedException($"Ошибка построения модели: тип детали {detal.DetalType} не поддерживается!");
    //        }
    //        scene.Children.Add(detalModel3D);
    //        return scene;
    //    }

    //    public async Task<Model3DGroup> Get3DSceneAsync(Detal detal) => await Task.Run(() => this.Get3DScene(detal));

    //    /// <summary>
    //    /// Поворот вершин вокруг оси X на заданный угол (в градусах)
    //    /// </summary>
    //    /// <param name="vertices">Точки вершин</param>
    //    /// <param name="angleDegrees">Угол поворота (в градусах)</param>
    //    public static void RotateVerticesAroundX(Point3D[] vertices, double angleDegrees)
    //    {
    //        double angleRadians = angleDegrees * Math.PI / 180;
    //        double cos = Math.Cos(angleRadians);
    //        double sin = Math.Sin(angleRadians);

    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            double y = vertices[i].Y;
    //            double z = vertices[i].Z;

    //            // Матрица поворота вокруг оси X:
    //            // Y' = Y * cosθ - Z * sinθ
    //            // Z' = Y * sinθ + Z * cosθ
    //            double newY = y * cos - z * sin;
    //            double newZ = y * sin + z * cos;

    //            vertices[i] = new Point3D(
    //                vertices[i].X, // X остаётся без изменений
    //                newY,
    //                newZ
    //            );
    //        }
    //    }

    //    /// <summary>
    //    /// Поворот вершин вокруг оси Y на заданный угол (в градусах)
    //    /// </summary>
    //    /// <param name="vertices">Точки вершин</param>
    //    /// <param name="angleDegrees">Угол поворота (в градусах)</param>
    //    public static void RotateVerticesAroundY(Point3D[] vertices, double angleDegrees)
    //    {
    //        double angleRadians = angleDegrees * Math.PI / 180;
    //        double cos = Math.Cos(angleRadians);
    //        double sin = Math.Sin(angleRadians);

    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            double x = vertices[i].X;
    //            double z = vertices[i].Z;

    //            // Применение матрицы поворота вокруг оси Y:
    //            // X' = X * cosθ - Z * sinθ
    //            // Z' = X * sinθ + Z * cosθ
    //            double newX = x * cos - z * sin;
    //            double newZ = x * sin + z * cos;

    //            vertices[i] = new Point3D(newX, vertices[i].Y, newZ);
    //        }
    //    }

    //    /// <summary>
    //    /// Поворот вершин вокруг оси Z на заданный угол (в градусах)
    //    /// </summary>
    //    /// <param name="vertices">Точки вершин</param>
    //    /// <param name="angleDegrees">Угол поворота (в градусах)</param>
    //    public static void RotateVerticesAroundZ(Point3D[] vertices, double angleDegrees)
    //    {
    //        double angleRadians = angleDegrees * Math.PI / 180;
    //        double cos = Math.Cos(angleRadians);
    //        double sin = Math.Sin(angleRadians);

    //        for (int i = 0; i < vertices.Length; i++)
    //        {
    //            double x = vertices[i].X;
    //            double y = vertices[i].Y;

    //            // Матрица поворота вокруг оси Z:
    //            // X' = X * cosθ - Y * sinθ
    //            // Y' = X * sinθ + Y * cosθ
    //            double newX = x * cos - y * sin;
    //            double newY = x * sin + y * cos;

    //            vertices[i] = new Point3D(
    //                newX,
    //                newY,
    //                vertices[i].Z // Z остаётся без изменений
    //            );
    //        }
    //    }

    //    #endregion Public functions
    //}
}
