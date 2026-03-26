using System;
using System.Collections.Generic;

namespace ForRobot.Libr.Configuration.ConfigurationProperties
{
    /// <summary>
    /// Класс для выбора стандартных свойств <see cref="ForRobot.Models.Robot"/> из app.config
    /// </summary>
    public class RobotConfigurationSection : BaseConfigurationSection
    {
        public string ControlFolderPath => GetValue<string>("control_folder_path");
        public string PathFolderGeneration => GetValue<string>("path_folder_gen");
        public string SystemFolders => GetValue<string>("system_folders");
    }
}
