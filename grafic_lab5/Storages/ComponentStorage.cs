using System;
using System.Collections.Generic;
using System.Linq;
using grafic_lab5.ImagesData;

namespace grafic_lab5.Storages;

/// <summary>
/// Хранилище образов
/// </summary>
public class ComponentStorage
{
    List<ComponentData> components = new List<ComponentData>();

    public void AddComponent(ComponentData component)
    {
        components.Add(component);
    }

    public ComponentData? FindCloserComponent(ulong hash)
    {
        int min_i = -1;
        ulong minDistances = ulong.MaxValue;

        for (int i = 0; i < components.Count; i++)
        {
            var currDistances = PerceptualHash.HammingDistances(components[i].Hash, hash);

            if (currDistances < minDistances)
            {
                minDistances = currDistances;
                min_i = i;
            }
        }

        return min_i != -1 ? components[min_i] : null;
        // return components.MinBy((component) => PerceptualHash.HammingDistances(component.Hash, hash));
    }
}