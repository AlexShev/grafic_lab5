using grafic_lab5.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.ImageWorkers;

public class LinearFilter
{
    private int[,] _matrix;
    private double _coefficient;

    int _matrixCenterY;
    int _matrixCenterX;

    public LinearFilter(int[,] matrix, double coefficient)
    {
        _matrix = matrix;
        _coefficient = coefficient;

        _matrixCenterY = _matrix.GetLength(0) / 2;
        _matrixCenterX = _matrix.GetLength(1) / 2;
    }

    public GrayImage Filter(GrayImage image)
    {
        GrayImage res = new GrayImage(image.Width, image.Height);

        int height = image.Height - _matrixCenterY;
        int width = image.Width - _matrixCenterX;

        for (int y = _matrixCenterY; y < height; y++)
        {
            for (int x = _matrixCenterX; x < width; x++)
            {
                res.SetPixel(x, y, Filter(x, y, image));
            }
        }

        return res;
    }

    private byte Filter(int x, int y, GrayImage image)
    {
        double result = 0;

        for (int p = 0; p < _matrix.GetLength(0); p++)
        {
            for (int q = 0; q < _matrix.GetLength(1); q++)
            {
                var i = y - _matrixCenterY + p;
                var j = x - _matrixCenterX + q;

                result += image.GetPixel(j, i) * image.GetPixel(p, q);
            }
        }

        return (byte)(result * _coefficient);
    }
}
