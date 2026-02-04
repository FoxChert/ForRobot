using System;
using System.Text.Json;

namespace ForRobot.Models.RoboticComplex
{
    public static class RobotExtensions
    {
        public static string Serialize(this Robot robot, JsonSerializerOptions jsonSerializerOptions = null)
        {
            if (robot == null)
                throw new ArgumentNullException(nameof(robot));

            var options = jsonSerializerOptions ?? new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                WriteIndented = true
            };

            return JsonSerializer.Serialize<Robot>(robot, jsonSerializerOptions);
        }
    }
}
