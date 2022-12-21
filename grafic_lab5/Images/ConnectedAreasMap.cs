﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.Images;

internal class ConnectedAreasMap : BaseImage<int>
{
    public int MaxComponentCount { get; private set; }

    private ConnectedAreasMap(BinaryImage image) : base(image.Width, image.Height)
    {
        for (int y = 0; y < image.Height; y++)
        {
            for (int x = 0; x < image.Width; x++)
            {
                if (image.GetPixel(x, y) == Bit.one)
                {
                    SetPixel(x, y, 1);
                }
            }
        }
    }

    public void ReplaseIndex(int oldIndex, int newIndex)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (GetPixel(x, y) == oldIndex)
                {
                    SetPixel(x, y, newIndex);
                }
            }
        }
    }


    public static ConnectedAreasMap Create(BinaryImage image)
    {
        ConnectedAreasMap result = new ConnectedAreasMap(image);
        int componentCounter = 0;

        for (int y = 0; y < result.Height; y++)
        {
            for (int x = 0; x < result.Width; x++)
            {
                int a = result.GetPixel(x, y);
                if (a == 0)
                    continue;

                int b = 0, c = 0;

                int temp_j = x - 1;

                if (temp_j > 0)
                {
                    b = result.GetPixel(temp_j, y);
                }

                int temp_i = y - 1;
                
                if (temp_i > 0)
                {
                    c = result.GetPixel(x, temp_i);
                }

                if (b == 0 && c == 0)
                {
                    ++componentCounter;

                    result.SetPixel(x, y, componentCounter);
                }
                else
                {
                    if (b != 0 && c == 0)
                    {
                        result.SetPixel(x, y, b);
                    }
                    else if (b == 0 && c != 0)
                    {
                        result.SetPixel(x, y, c);
                    }
                    else if (b == c)
                    {
                        result.SetPixel(x, y, b);
                    }
                    else
                    {
                        result.SetPixel(x, y, c);
                        result.ReplaseIndex(b, c);
                    }
                }
            }
        }

        result.MaxComponentCount = componentCounter;

        return result;
    }
}
