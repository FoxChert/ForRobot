using System;
using System.Configuration;

namespace ForRobot.Libr.Configuration.ConfigurationProperties
{
    /// <summary>
    /// Класс для вывода стандартных свойств детали типа <see cref="ForRobot.Models.Detals.DetalTypes.Plita"/> из app.config
    /// </summary>
    public class PlateConfigurationSection : BaseConfigurationSection
    {
        [ConfigurationProperty("plate_script_name", IsRequired = false)]
        public string PlateScriptName => GetValue<string>("plate_script_name");
        [ConfigurationProperty("plate_program_name", IsRequired = false)]
        public string PlateProgramName => GetValue<string>("plate_program_name");

        [ConfigurationProperty("detail_reverse_deflection", IsRequired = false)]
        public decimal ReverseDeflection => GetValue<decimal>("detail_reverse_deflection");
        [ConfigurationProperty("base_width", IsRequired = false)]
        public decimal PlateWidth => GetValue<decimal>("base_width");
        [ConfigurationProperty("base_length", IsRequired = false)]
        public decimal PlateLength => GetValue<decimal>("base_length");
        [ConfigurationProperty("base_thickness", IsRequired = false)]
        public decimal PlateThickness => GetValue<decimal>("base_thickness");
        [ConfigurationProperty("base_bevel_left", IsRequired = false)]
        public decimal PlateBevelToLeft => GetValue<decimal>("base_bevel_left");
        [ConfigurationProperty("base_bevel_right", IsRequired = false)]
        public decimal PlateBevelToRight => GetValue<decimal>("base_bevel_right");

        [ConfigurationProperty("wall_height", IsRequired = false)]
        public decimal RibsHeight => GetValue<decimal>("wall_height");
        [ConfigurationProperty("wall_thickness", IsRequired = false)]
        public decimal RibsThickness => GetValue<decimal>("wall_thickness");
        [ConfigurationProperty("wall_count", IsRequired = false)]
        public int RibsCount => GetValue<int>("wall_count");
        [ConfigurationProperty("weld_first_dist", IsRequired = false)]
        public decimal DistanceToFirstRib => GetValue<decimal>("weld_first_dist");
        [ConfigurationProperty("weld_between_dist", IsRequired = false)]
        public decimal DistanceBetweenRibs => GetValue<decimal>("weld_between_dist");
        [ConfigurationProperty("wall_long_dist_left", IsRequired = false)]
        public decimal RibsIdentToLeft => GetValue<decimal>("wall_long_dist_left");
        [ConfigurationProperty("wall_long_dist_right", IsRequired = false)]
        public decimal RibsIdentToRight => GetValue<decimal>("wall_long_dist_right");
        [ConfigurationProperty("weld_offset_left", IsRequired = false)]
        public decimal WeldsDissolutionLeft => GetValue<decimal>("weld_offset_left");
        [ConfigurationProperty("weld_offset_right", IsRequired = false)]
        public decimal WeldsDissolutionRight => GetValue<decimal>("weld_offset_right");

        [ConfigurationProperty("search_offset_start", IsRequired = false)]
        public decimal SearchOffsetStart => GetValue<decimal>("search_offset_start");
        [ConfigurationProperty("search_offset_end", IsRequired = false)]
        public decimal SearchOffsetEnd => GetValue<decimal>("search_offset_end");
        [ConfigurationProperty("weld_tech_offset_start", IsRequired = false)]
        public decimal TechOffsetSeamStart => GetValue<decimal>("weld_tech_offset_start");
        [ConfigurationProperty("weld_tech_offset_end", IsRequired = false)]
        public decimal TechOffsetSeamEnd => GetValue<decimal>("weld_tech_offset_end");
        [ConfigurationProperty("weld_overlap", IsRequired = false)]
        public decimal SeamsOverlap => GetValue<decimal>("weld_overlap");
        [ConfigurationProperty("weld_job", IsRequired = false)]
        public decimal ProgramNom => GetValue<decimal>("weld_job");
        [ConfigurationProperty("weld_velocity", IsRequired = false)]
        public decimal WeldingSpead => GetValue<decimal>("weld_velocity");
        [ConfigurationProperty("gantry_radius_weld", IsRequired = false)]
        public decimal DistanceForWelding => GetValue<decimal>("gantry_radius_weld");
        [ConfigurationProperty("gantry_radius_search", IsRequired = false)]
        public decimal DistanceForSearch => GetValue<decimal>("gantry_radius_search");
    }
}
