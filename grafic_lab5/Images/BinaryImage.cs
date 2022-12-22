namespace grafic_lab5.Images;

public enum Bit : byte
{
    zero,
    one
}

public class BinaryImage : BaseImage<Bit>
{
    public BinaryImage(int width, int height) : base(width, height)
    {
    }

    public BinaryImage(Bit[,] pixels) : base(pixels)
    {
    }

    public static BinaryImage Create(GrayImage image, double binarizationBarrier, bool isBlackBackground)
    {
        BinaryImage result = new BinaryImage(image.Width, image.Height);

        Func<double, double, bool> comporator = isBlackBackground ? ((double a, double b) => a >= b) : ((double a, double b) => a < b);

        for (int y = 0; y < image.Height; ++y)
        {
            for (int x = 0; x < image.Width; ++x)
            {
                if (comporator(image.GetPixel(x, y), binarizationBarrier))
                {
                    result.SetPixel(x, y, Bit.one);
                }
                else
                {
                    result.SetPixel(x, y, Bit.zero);
                }
            }
        }

        return result;
    }

    public static double CuclBinarizationBarrier(GrayImage image)
    {
        double sum = 0;

        for (int y = 0; y < image.Height; ++y)
        {
            for (int x = 0; x < image.Width; ++x)
            {
                sum += image.GetPixel(x, y);
            }
        }

        return sum / (image.Height * image.Width);
    }

    public BinaryImage Scale(int newWidth, int newHeight)
    {
        return new BinaryImage(ScaleMatrix(newWidth, newHeight));
    }
}
