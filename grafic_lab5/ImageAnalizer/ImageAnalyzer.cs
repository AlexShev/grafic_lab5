using grafic_lab5.ImageFilter;
using grafic_lab5.Images;
using grafic_lab5.ImagesData;
using grafic_lab5.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grafic_lab5.ImageAnalizer;

public struct AnalyzerResult
{
    // Название
    public string Name;
    // Положение
    public Rectangle Location;

    public AnalyzerResult(string name, Rectangle location)
    {
        Name = name;
        Location = location;
    }
}

public class ImageAnalyzer
{
    /// <summary>
    /// База образов
    /// </summary>
    private ComponentStorage _storage;

    /// <summary>
    /// минимальный процент совпадения (от 0 до 1)
    /// </summary>
    private double _minimumMatchPercentage;

    public double MinimumMatchPercentage 
    { 
        get => _minimumMatchPercentage;
        set
        {
            if (value > 1)
            {
                _minimumMatchPercentage = 1;
            }
            else if (value < 0)
            {
                _minimumMatchPercentage = 0;
            }
            else
            {
                _minimumMatchPercentage = value;
            }
        }
    }

    public ImageAnalyzer(ComponentStorage storage)
    {
        _storage = storage;
    }

    /// <summary>
    /// Получить список образов, которые известны базе
    /// </summary>
    /// <param name="image">изображение для анализа</param>
    /// <returns>список характеристик образов</returns>
    public List<AnalyzerResult> AnalyzeImage(Bitmap image)
    {
        BinaryImage toAnalysis = PrepareForAnalysis(image);

        // нахождение карты компонент связности
        ComponentMap componentMap = ComponentMap.Create(toAnalysis);
        // нахождение компонент связности по карте
        var components = ComponentDeterminator.FindComponents(componentMap);

        List<AnalyzerResult> result = new List<AnalyzerResult>();

        foreach (var component in components)
        {
            BinaryImage componentToFind = componentMap.ClipComponent(component.Item1, component.Item2);

            var hash = PerceptualHash.CalcPerceptualHash(componentToFind);

            var response = _storage.FindCloserComponent(hash);

            if (response != null && MinimumMatchPercentage <= 1 - PerceptualHash.HammingDistances(hash, response.Hash) / 64)
            {
                result.Add(new AnalyzerResult(response.Data.Name, component.Item1));
            }
        }

        return result;
    }

    // подготовка изображения
    public BinaryImage PrepareForAnalysis(Bitmap bitmap)
    {
        GrayImage image = GrayImage.Create(bitmap);

        MatrixParser parser = new MatrixParser("C:\\Users\\ALEX\\Desktop\\ComputerGraphics\\code\\grafic_lab5\\grafic_lab5\\ImageFilter\\filter.txt");

        image = new LinearFilter(parser.Matrix, parser.Coefficient).Filter(image);

        double binarizationBarrier = BinaryImage.CuclBinarizationBarrier(image);

        BinaryImage binaryImage = BinaryImage.Create(image, binarizationBarrier, true);

        binaryImage = MorphologicalFilter.OpeningFilter(binaryImage);

        return binaryImage;
    }

    public void FillComponentStorage(string path)
    {
        string[] ComponentNameAndImage = File.ReadAllLines(path);

        for (int i = 0; i < ComponentNameAndImage.Length; i++)
        {
            try
            {
                string[] nameAndImagePath = ComponentNameAndImage[i].Trim().Split();
                AddComponent(new MetaData(nameAndImagePath[0]), new Bitmap(nameAndImagePath[1]));
            }
            catch (Exception) { }
        }
    }

    public void AddComponent(MetaData metaData, Bitmap bitmap)
    {
        BinaryImage toAdd = PrepareForAnalysis(bitmap);

        // нахождение карты компонент связности
        ComponentMap componentMap = ComponentMap.Create(toAdd);
        // нахождение компонент связности по карте
        var components = ComponentDeterminator.FindComponents(componentMap);

        if (components.Count == 1)
        {
            BinaryImage componentToAdd = componentMap.ClipComponent(components[0].Item1, components[0].Item2);

            _storage.AddComponent(new ComponentData(metaData, componentToAdd));
        }
    }
}
