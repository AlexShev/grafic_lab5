using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.Images;

public class BaseImage <T> where T : struct
{
    public int Height { get; private set; }
    public int Width { get; private set; }

    private T[,] _pixels;

    public BaseImage(int width, int height)
    {
        Width = width;
        Height = height;

        _pixels = new T[height, width];
    }

    protected BaseImage(T[,] pixels)
    {
        _pixels = pixels;

        Height = pixels.GetLength(0);
        Width = pixels.GetLength(1);
    }

    public virtual void SetPixel(int x, int y, T pixel)
    {
        _pixels[y, x] = pixel;
    }

    public virtual T GetPixel(int x, int y)
    {
        return _pixels[y, x];
    }

    protected T[,] ScaleMatrix(int newWidth, int newHeight)
    {
        var res = new T[newWidth, newHeight];

        double dX = Width / newWidth;
        double dY = Height / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                res[y, x] = _pixels[(int)Math.Round(y * dY), (int)Math.Round(x * dX)];
            }
        }            
  
        return res;
    }
}
