using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.Images;

/// <summary>
/// Базовый класс изображения
/// </summary>
/// <typeparam name="T">тип пикселя</typeparam>
public abstract class BaseImage <T> where T : struct
{
    // высота
    public int Height { get; private set; }
    // ширина
    public int Width { get; private set; }

    // матрица пикселей
    private T[,] _pixels;

    // коструктор - все ячейки нулевые
    public BaseImage(int width, int height)
    {
        Width = width;
        Height = height;

        _pixels = new T[height, width];
    }

    // конструктор
    public BaseImage(T[,] pixels)
    {
        _pixels = pixels;

        Height = pixels.GetLength(0);
        Width = pixels.GetLength(1);
    }

    /// <summary>
    /// установить значение пикселю
    /// </summary>
    /// <param name="x">номер столбца</param>
    /// <param name="y">номер строки</param>
    /// <param name="pixel">значение</param>
    public void SetPixel(int x, int y, T pixel)
    {
        _pixels[y, x] = pixel;
    }

    /// <summary>
    /// Получить значение пикселя
    /// </summary>
    /// <param name="x">номер столбца</param>
    /// <param name="y">номер строки</param>
    /// <returns>значение</returns>
    public T GetPixel(int x, int y)
    {
        return _pixels[y, x];
    }

    /// <summary>
    /// Масштабировать матрицу пикселей
    /// </summary>
    /// <param name="newWidth">новая ширина</param>
    /// <param name="newHeight">новая высота</param>
    /// <returns></returns>
    protected T[,] ScaleMatrix(int newWidth, int newHeight)
    {
        var res = new T[newWidth, newHeight];
        // масштабирующее значение по осям координат
        double dX = Width / newWidth;
        double dY = Height / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                // значение в исходном изображении по масштабируемым значениям нового изображения
                res[y, x] = _pixels[(int)Math.Round(y * dY), (int)Math.Round(x * dX)];
            }
        }            
  
        return res;
    }
}
