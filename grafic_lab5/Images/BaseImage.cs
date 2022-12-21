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

    public virtual void SetPixel(int x, int y, T pixel)
    {
        _pixels[y, x] = pixel;
    }

    public virtual T GetPixel(int x, int y)
    {
        return _pixels[y, x];
    }
}
