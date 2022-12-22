﻿using grafic_lab5.Images;

namespace grafic_lab5.ImagesData;

/// <summary>
/// Метаданные об объекте
/// </summary>
public class MetaData
{
    public string Name;

    public MetaData(string name)
    {
        Name = name;
    }

    public MetaData()
    {
        Name = string.Empty;
    }
}

/// <summary>
/// Расчитыватель перцептивного хэша
/// </summary>
public class PerceptualHash
{
    public static ulong CalcPerceptualHash(BinaryImage binaryImage)
    {
        uint perceptualHash = 0;

        // масштабирующее значение по осям координат
        double scale = Math.Min(8.0 / binaryImage.Height, 8.0 / binaryImage.Width);

        int height = (int)Math.Round(binaryImage.Height * scale);
        int width = (int)Math.Round(binaryImage.Width * scale);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // значение в исходном изображении по масштабируемым значениям нового изображения
                int x = Math.Min((int)Math.Round(j / scale), binaryImage.Width - 1);
                int y = Math.Min((int)Math.Round(i / scale), binaryImage.Height - 1);
                
                perceptualHash |= (byte)binaryImage.GetPixel(x, y);

                perceptualHash <<= 1;
            }

            perceptualHash <<= 8 - width;
        }

        perceptualHash <<= (8 - height) * 8;

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