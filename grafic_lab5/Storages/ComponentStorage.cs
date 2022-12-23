using grafic_lab5.ImagesData;

namespace grafic_lab5.Storages;

/// <summary>
/// Хранилище образов
/// </summary>
public class ComponentStorage
{
    /// <summary>
    /// Массив компонент
    /// </summary>
    private List<ComponentData> _components = new List<ComponentData>();

    /// <summary>
    /// Добавить образ
    /// </summary>
    /// <param name="component">образ</param>
    public void AddComponent(ComponentData component)
    {
        _components.Add(component);
    }

    /// <summary>
    /// Добавить образ
    /// </summary>
    /// <param name="components">список образов</param>
    public void AddComponent(List<ComponentData> components)
    {
        components.AddRange(components);
    }

    /// <summary>
    /// Найти наиболее подходящий образ
    /// </summary>
    /// <param name="hash">хэш изображения, которому нужно подобрать образ</param>
    /// <returns></returns>
    public ComponentData? FindCloserComponent(ulong hash)
    {
        //int min_i = -1;
        //ulong minDistances = ulong.MaxValue;

        //for (int i = 0; i < _components.Count; i++)
        //{
        //    var currDistances = PerceptualHash.HammingDistances(_components[i].Hash, hash);

        //    if (currDistances < minDistances)
        //    {
        //        minDistances = currDistances;
        //        min_i = i;
        //    }
        //}

        //return min_i != -1 ? _components[min_i] : null;
        return _components.MinBy((component) => PerceptualHash.HammingDistances(component.Hash, hash));
    }
}