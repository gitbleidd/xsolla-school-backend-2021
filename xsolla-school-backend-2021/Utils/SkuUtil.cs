using System;
using XsollaSchoolBackend.Models;

namespace XsollaSchoolBackend.Utils
{
    public static class SkuUtil
    {
        // Генерирует CRC для типа, имени и соединяет результат через "-" с id.
        public static string GenerateSku(Item item, int id)
        {
            return GenerateCrc(item.Type) + "-" + GenerateCrc(item.Name) + "-" + id;
        }

        // Генерирует CRC16-CCITT на основе UTF-8 представления строки.
        public static string GenerateCrc(string str)
        {
            var textBuffer = System.Text.Encoding.UTF8.GetBytes(str);

            var textCrc = NullFX.CRC.Crc16.ComputeChecksum(NullFX.CRC.Crc16Algorithm.Ccitt, textBuffer);

            return textCrc.ToString();
        }
    }
}
