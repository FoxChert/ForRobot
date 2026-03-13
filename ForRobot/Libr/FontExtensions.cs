using System;
using System.Drawing;

namespace ForRobot.Libr
{
    /// <summary>
    /// Класс расширяющий тип <see cref="Font"/>
    /// </summary>
    public static class FontExtensions
    {
        /// <summary>
        /// Маштабирование шрифта
        /// </summary>
        /// <param name="font">Маштабируемый шрифт</param>
        /// <param name="context">Графический контекст</param>
        /// <param name="str">строка, которую нужно разместить</param>
        /// <param name="room">Область, под которую маштабируется шрифт</param>
        /// <returns></returns>
        public static Font Scale(this Font font, System.Drawing.Graphics context, string str, Size room)
        {
            SizeF RealSize = context.MeasureString(str, font);
            float HeightScaleRatio = room.Height / RealSize.Height;
            float WidthScaleRatio = room.Width / RealSize.Width;

            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;

            float ScaleFontSize = font.Size * ScaleRatio;

            return new Font(font.FontFamily, ScaleFontSize);
        }
    }
}
