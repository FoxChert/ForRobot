using System;
using System.Text;
using System.Security.Cryptography;

namespace ForRobot.Libr.Cryptography
{
    public class Hashing
    {
        /// <summary>
        /// Алгоритм хэширования MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string MD5(string input)
        {
            using (MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Алгоритм хэширования Sha256
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Sha256(string input)
        {
            using (var hash = SHA256.Create())
            {
                byte[] hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        ///// <summary>
        ///// Ввод и сравнение пин-кодов
        ///// </summary>
        ///// <returns>Верный ли введенный пользователем пин-код</returns>
        //public static bool Equals()
        //{
        //    string pin = new ForRobot.Libr.Services.WindowsAppService().InputWindowShow();
        //    return !string.IsNullOrEmpty(pin) && ForRobot.Libr.Cryptography.Hashing.Sha256(pin) == ForRobot.Properties.Settings.Default.PinCode;
        //}
    }
}
