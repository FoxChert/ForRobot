using System;

using ForRobot.Libr.Services.Providers;
using ForRobot.Models.Welding;

namespace ForRobot.Models.Detals
{
    public class DetalProvider : IDetalProvider
    {
        private IConfigurationProvider _configurationProvider;

        public DetalProvider(IConfigurationProvider configurationProvider)
        {
            this._configurationProvider = configurationProvider;
        }

        /// <summary>
        /// Создает деталь типа <see cref="DetalType.Plate"/> с параметрами из конфигурации
        /// </summary>
        /// <returns>Новая деталь типа Plate</returns>
        public Plate CreatePlita()
        {
            var plateConfig = this._configurationProvider.GetPlateConfig();

            if (plateConfig == null)
                throw new InvalidOperationException($"Конфигурация для детали {DetalType.Plate} не найдена");
            
            return new Plate()
            {
                ReverseDeflection = plateConfig.ReverseDeflection,
                PlateWidth = plateConfig.PlateWidth,
                PlateLength = plateConfig.PlateLength,
                PlateThickness = plateConfig.PlateThickness,
                PlateBevelToLeft = plateConfig.PlateBevelToLeft,
                PlateBevelToRight = plateConfig.PlateBevelToRight,

                RibsHeight = plateConfig.RibsHeight,
                RibsThickness = plateConfig.RibsThickness,
                RibsCount = plateConfig.RibsCount,
                DistanceToFirstRib = plateConfig.DistanceToFirstRib,
                DistanceBetweenRibs = plateConfig.DistanceBetweenRibs,
                RibsIdentToLeft = plateConfig.RibsIdentToLeft,
                RibsIdentToRight = plateConfig.RibsIdentToRight,

                WeldingProperties =
                {
                    WeldsDissolutionLeft = plateConfig.WeldsDissolutionLeft,
                    WeldsDissolutionRight = plateConfig.WeldsDissolutionRight,
                    SearchOffsetStart = plateConfig.SearchOffsetStart,
                    SearchOffsetEnd = plateConfig.SearchOffsetEnd,
                    TechOffsetSeamStart = plateConfig.TechOffsetSeamStart,
                    TechOffsetSeamEnd = plateConfig.TechOffsetSeamEnd,
                    SeamsOverlap = plateConfig.SeamsOverlap,
                    ProgramNom = plateConfig.ProgramNom,
                    WeldingSpead = plateConfig.WeldingSpead,
                    DistanceForWelding = plateConfig.DistanceForWelding,
                    DistanceForSearch = plateConfig.DistanceForSearch,
                    SelectedWeldingSchema = WeldingSchemaTypes.LeftEvenOdd_RightEvenOdd
                }
            };
        }

        public PlateStringer CreatePlitaStringer()
        {
            throw new NotImplementedException();
        }

        public PlateTreygolnik CreatePlitaTreygolnik()
        {
            throw new NotImplementedException();
        }
    }
}
