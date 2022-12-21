using grafic_lab5.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.ImageWorkers;

public class MorphologicalFilter
{
    public BinaryImage ErosionFilter(BinaryImage image)
    {
        BinaryImage res = new BinaryImage(image.Width, image.Height);

        int height = image.Height - 1;
        int width = image.Width - 1;

        for (int y = 1; y < height; y++)
        {
            for (int x = 1; x < width; x++)
            {
                if (image.GetPixel(x - 1, y - 1) == Bit.one
                    || image.GetPixel(x - 1, y) == Bit.one
                    || image.GetPixel(x - 1, y + 1) == Bit.one
                    || image.GetPixel(x, y - 1) == Bit.one
                    || image.GetPixel(x, y) == Bit.one
                    || image.GetPixel(x, y + 1) == Bit.one
                    || image.GetPixel(x + 1, y - 1) == Bit.one
                    || image.GetPixel(x + 1, y) == Bit.one
                    || image.GetPixel(x + 1, y + 1) == Bit.one)
                {
                    res.SetPixel(x, y, Bit.one);
                }
            }
        }

        return res;
    }

    public BinaryImage DilatationFilter(BinaryImage image)
    {
        BinaryImage res = new BinaryImage(image.Width, image.Height);

        int height = image.Height - 1;
        int width = image.Width - 1;

        for (int y = 1; y < height; y++)
        {
            for (int x = 1; x < width; x++)
            {
                if (image.GetPixel(x, y) == Bit.one)
                {
                    res.SetPixel(x - 1, y - 1, Bit.one);
                    res.SetPixel(x - 1, y, Bit.one);
                    res.SetPixel(x - 1, y + 1, Bit.one);
                    res.SetPixel(x, y - 1, Bit.one);
                    res.SetPixel(x, y, Bit.one);
                    res.SetPixel(x, y + 1, Bit.one);
                    res.SetPixel(x + 1, y - 1, Bit.one);
                    res.SetPixel(x + 1, y, Bit.one);
                    res.SetPixel(x + 1, y + 1, Bit.one);
                }
            }
        }

        return res;
    }

}
