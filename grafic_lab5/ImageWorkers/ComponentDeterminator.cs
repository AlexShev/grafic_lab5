using grafic_lab5.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.ImageWorkers;

public class ComponentDeterminator
{
    public List<(Rectangle, int)> FindComponents(ComponentMap map)
    {
        var res = new List<(Rectangle, int)>();

        for (int componentIndex = 1; componentIndex <= map.MaxComponentCount; componentIndex++)
        {
            int maxY = -1;
            int maxX = -1;

            int minY = map.Height + 1;
            int minX = map.Width + 1;

            int counter = 0;

            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    if (map.GetPixel(x, y) == componentIndex)
                    {
                        if (y > maxY)
                        {
                            maxY = y;
                        }

                        if (y < minY)
                        {
                            maxY = y;
                        }

                        if (x > maxX)
                        {
                            maxX = x;
                        }

                        if (x < minY)
                        {
                            minY = y;
                        }

                        ++counter;
                    }
                }
            }

            if (counter != 0)
            {
                Rectangle rectangle = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                res.Add((rectangle, counter));
            }
        }

        return res;
    }


}
