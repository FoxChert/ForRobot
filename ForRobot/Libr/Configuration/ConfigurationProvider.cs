using System;
using System.Configuration;

using ForRobot.Libr.Configuration.ConfigurationProperties;

namespace ForRobot.Libr.Configuration
{
    /// <summary>
    /// Провайдер конфигурации для получения настроек различных компонентов системы
    /// </summary>
    public class ConfigurationProvider : ForRobot.Libr.Services.Providers.IConfigurationProvider
    {
        private const string PlateConfigName = "plate";
        private const string RobotConfigName = "robot";

        /// <summary>
        /// Получение конфигурации узла plate
        /// </summary>
        /// <returns></returns>
        public PlateConfigurationSection GetPlitaConfig() => this.SelectConfig(PlateConfigName) as PlateConfigurationSection ?? throw new ConfigurationErrorsException(this.GetConfigErrorMessage(PlateConfigName));

        /// <summary>
        /// Получение конфигурации узла robot
        /// </summary>
        /// <returns></returns>
        public RobotConfigurationSection GetRobotConfig() => this.SelectConfig(RobotConfigName) as RobotConfigurationSection ?? throw new ConfigurationErrorsException(this.GetConfigErrorMessage(RobotConfigName));

        /// <summary>
        /// Выборка узла конфигурации по имени
        /// </summary>
        /// <param name="configName">Имя узла конфигурации</param>
        /// <returns>Объект узла конфигурации</returns>
        /// <exception cref="ConfigurationErrorsException">Выбрасывается, если конфигурация не найдена</exception>
        /// <exception cref="ArgumentNullException">Выбрасывается, если configName равен null</exception>
        private ConfigurationSection SelectConfig(string configName)
        {
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentNullException(nameof(configName), "Имя конфигурации не может быть null или пустым");

            var config = ConfigurationManager.GetSection(configName);

            if (config == null)
                throw new ConfigurationErrorsException(string.Format("Конфигурация для '{0}' не найдена", configName));

            ConfigurationSection configSection = config as System.Configuration.ConfigurationSection;
            if (configSection == null)
            {
                throw new ConfigurationErrorsException(string.Format("Конфигурация для '{0}' не является ConfigurationSection", configName));
            }

            return configSection;
        }

        private string GetConfigErrorMessage(string configName) => string.Format("Конфигурация для '{0}' имеет неверный тип", configName);
    }
}
