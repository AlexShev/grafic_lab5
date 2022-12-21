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

    private BinaryImage(Bit[,] pixels) : base(pixels)
    {
    }

    public static BinaryImage Create(GrayImage image, double binarizationThreshold)
    {
        BinaryImage result = new BinaryImage(image.Width, image.Height);

        double binarizationBarrier = byte.MaxValue * binarizationThreshold;

        for (int y = 0; y < image.Height; ++y)
        {
            for (int x = 0; x < image.Width; ++x)
            {
                if (image.GetPixel(x, y) >= binarizationBarrier)
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

    public BinaryImage Scale(int newWidth, int newHeight)
    {
        return new BinaryImage(ScaleMatrix(newWidth, newHeight));
    }
}
