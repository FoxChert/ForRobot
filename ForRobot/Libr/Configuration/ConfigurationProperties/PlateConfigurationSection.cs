using System;
using System.Configuration;

namespace ForRobot.Libr.Configuration.ConfigurationProperties
{
    /// <summary>
    /// Класс для вывода стандартных свойств детали типа <see cref="ForRobot.Models.Detals.DetalTypes.Plate"/> из app.config
    /// </summary>
    public class PlateConfigurationSection : BaseConfigurationSection
    {
        public string PlateScriptName => GetValue<string>("plate_script_name");
        public string PlateProgramName => GetValue<string>("plate_program_name");
        
        public decimal ReverseDeflection => GetValue<decimal>("detail_reverse_deflection");
        public decimal PlateWidth => GetValue<decimal>("base_width");
        public decimal PlateLength => GetValue<decimal>("base_length");
        public decimal PlateThickness => GetValue<decimal>("base_thickness");
        public decimal PlateBevelToLeft => GetValue<decimal>("base_bevel_left");
        public decimal PlateBevelToRight => GetValue<decimal>("base_bevel_right");
        
        public decimal RibsHeight => GetValue<decimal>("wall_height");
        public decimal RibsThickness => GetValue<decimal>("wall_thickness");
        public int RibsCount => GetValue<int>("wall_count");
        public decimal DistanceToFirstRib => GetValue<decimal>("weld_first_dist");
        public decimal DistanceBetweenRibs => GetValue<decimal>("weld_between_dist");
        public decimal RibsIdentToLeft => GetValue<decimal>("wall_long_dist_left");
        public decimal RibsIdentToRight => GetValue<decimal>("wall_long_dist_right");
        public decimal WeldsDissolutionLeft => GetValue<decimal>("weld_offset_left");
        public decimal WeldsDissolutionRight => GetValue<decimal>("weld_offset_right");
        
        public decimal SearchOffsetStart => GetValue<decimal>("search_offset_start");
        public decimal SearchOffsetEnd => GetValue<decimal>("search_offset_end");
        public decimal TechOffsetSeamStart => GetValue<decimal>("weld_tech_offset_start");
        public decimal TechOffsetSeamEnd => GetValue<decimal>("weld_tech_offset_end");
        public decimal SeamsOverlap => GetValue<decimal>("weld_overlap");
        public int ProgramNom => GetValue<int>("weld_job");
        public int WeldingSpead => GetValue<int>("weld_velocity");
        public decimal DistanceForWelding => GetValue<decimal>("gantry_radius_weld");
        public decimal DistanceForSearch => GetValue<decimal>("gantry_radius_search");
    }
}
