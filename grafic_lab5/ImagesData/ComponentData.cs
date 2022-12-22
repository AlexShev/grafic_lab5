using grafic_lab5.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.ImagesData;

/// <summary>
/// Метаданные об объекте
/// </summary>
public class MetaData
{
    public string Name = string.Empty;
}

/// <summary>
/// Расчитыватель перцептивного хэша
/// </summary>
public class PerceptualHash
{
    public static ulong CalcPerceptualHash(BinaryImage binaryImage)
    {
        uint perceptualHash = 0;

        if (binaryImage.Height != 8 || binaryImage.Width != 8)
        {
            binaryImage = binaryImage.Scale(8, 8);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                perceptualHash |= (byte)binaryImage.GetPixel(j, i);
            }

            perceptualHash <<= 1;
        }

        return perceptualHash;
    }

    public static byte HammingDistances(ulong first, ulong second)
    {
        byte distances = 0;

        // Операторы ^ устанавливают в 1 только те биты, которые отличаются
        for (ulong val = first ^ second; val > 0; ++distances)
        {
            // Затем мы считаем бит, установленный в 1, используя Peter Wegnerспособ
            val = val & val - 1; // Установить равным нулю значение младшего порядка val 1
        }

        // Возвращает количество различных битов

        return distances;
    }
}

/// <summary>
/// Класс для хранения данных о образе
/// </summary>
public class ComponentData
{
    // Данные об образе
    public MetaData Data;
    // перцептивный хэш
    public ulong Hash;

    public ComponentData(MetaData data, BinaryImage binaryImage)
    {
        Data = data;
        Hash = PerceptualHash.CalcPerceptualHash(binaryImage);
    }

    public ComponentData()
    {
        Data = new MetaData();
        Hash = 0;
    }
}