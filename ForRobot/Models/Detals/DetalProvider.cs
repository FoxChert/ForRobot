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
        /// Создает деталь типа <see cref="DetalType.Plita"/> с параметрами из конфигурации
        /// </summary>
        /// <returns>Новая деталь типа Plita</returns>
        public Plita CreatePlita()
        {
            var plateConfig = this._configurationProvider.GetPlateConfig();

            if (plateConfig == null)
                throw new InvalidOperationException($"Конфигурация для детали {DetalType.Plita} не найдена");

            return new Plita()
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
                WeldsDissolutionLeft = plateConfig.WeldsDissolutionLeft,
                WeldsDissolutionRight = plateConfig.WeldsDissolutionRight,

                WeldingProperties = new WeldingProperties()
                {
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

        public PlitaStringer CreatePlitaStringer()
        {
            throw new NotImplementedException();
        }

        public PlitaTreygolnik CreatePlitaTreygolnik()
        {
            throw new NotImplementedException();
        }
    }
}
