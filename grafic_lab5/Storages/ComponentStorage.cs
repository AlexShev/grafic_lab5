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
        return components.MinBy((component) => PerceptualHash.HammingDistances(component.Hash, hash));
    }
}