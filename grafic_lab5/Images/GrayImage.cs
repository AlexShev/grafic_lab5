using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.Images;

public class GrayImage : BaseImage<byte>
{
    public GrayImage(int width, int height) : base(width, height)
    {
    }

    public static GrayImage Create(Bitmap bitmap)
    {
        GrayImage result = new GrayImage(bitmap.Width, bitmap.Height);

        for (int y = 0; y < bitmap.Height; ++y)
        {
            for (int x = 0; x < bitmap.Width; ++x)
            {
                result.SetPixel(x, y, ToGray(bitmap.GetPixel(x, y)));
            }
        }

        return result;
    }

    private static byte ToGray(Color color)
    {
        return (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
    }
}
