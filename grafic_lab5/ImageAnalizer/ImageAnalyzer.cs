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

public struct AnalyzerResult : IComparable<AnalyzerResult>
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

    public int CompareTo(AnalyzerResult other)
    {
        int result = Location.X.CompareTo(other.Location.X);

        if (result == 0)
        {
            result = Location.Y.CompareTo(other.Location.Y);
        }

        return result;
    }
}

public class ImageAnalyzer
{
    /// <summary>
    /// База образов
    /// </summary>
    private ComponentStorage _storage;

    /// <summary>
    /// Линейный фильтр
    /// </summary>
    private LinearFilter? _linearFilter;

    /// <summary>
    /// минимальный процент совпадения (от 0 до 1)
    /// </summary>
    private const double MinimumMatchPercentage = 0.7;

    /// <summary>
    /// Барьер бинаризации число от 0 до 255
    /// </summary>
    private double _binarizationBarrier;

    /// <summary>
    /// Степень бинаризации - число от 0 до 1
    /// </summary>
    public double BinarizationMeasure 
    {
        set => _binarizationBarrier = value * byte.MaxValue;
        get => _binarizationBarrier / byte.MaxValue;
    }

    public bool IsInteractiv { set; get; }

    public ImageAnalyzer(ComponentStorage storage, bool isInteractiv)
    {
        _storage = storage;
        BinarizationMeasure = 0;
        IsInteractiv = isInteractiv;
    }

    public void SetLinearFilter(LinearFilter linearFilter)
    {
        _linearFilter = linearFilter;
    }

    /// <summary>
    /// Получить список образов, которые известны базе
    /// </summary>
    /// <param name="image">изображение для анализа</param>
    /// <returns>список характеристик образов</returns>
    public List<AnalyzerResult> AnalyzeImage(Bitmap image, bool isBlackBackground)
    {
        BinaryImage toAnalysis = PrepareForAnalysis(image, isBlackBackground);

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

            var matchPercentage = 1 - PerceptualHash.HammingDistances(hash, response.Hash) / 64.0;

            if (response != null && MinimumMatchPercentage <= matchPercentage)
            {
                result.Add(new AnalyzerResult(response.Data.Name, component.Item1));
            }
            else if (matchPercentage <= 0.5)
            {

            }
        }

        return result;
    }

    // подготовка изображения
    public BinaryImage PrepareForAnalysis(Bitmap bitmap, bool isBlackBackground)
    {
        GrayImage image = GrayImage.Create(bitmap);

        if (_linearFilter == null)
        {
            MatrixParser parser = new MatrixParser("..\\..\\..\\ImageFilter\\filter.txt");

            _linearFilter = new LinearFilter(parser.Matrix, parser.Coefficient);

        }

        image = _linearFilter.Filter(image, isBlackBackground);

        double binarizationBarrier = BinaryImage.CuclBinarizationBarrier(image);

        if (IsInteractiv)
        {
            if (Math.Abs(binarizationBarrier - _binarizationBarrier) >= 0.1)
            {
                var result = MessageBox.Show(
                    $"Программа считает, что порог бинаризации выгоднее принять за {binarizationBarrier}, чем {_binarizationBarrier}",
                    "Вы хотите поменять на рекомендуемую велечину?", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                {
                    binarizationBarrier = _binarizationBarrier;
                }
            }
        }

        BinaryImage binaryImage = BinaryImage.Create(image, binarizationBarrier, isBlackBackground);

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
                AddComponent(new MetaData(nameAndImagePath[0]), new Bitmap(nameAndImagePath[2]), nameAndImagePath[1] == "1");
            }
            catch (Exception) { }
        }
    }

    public void AddComponent(MetaData metaData, Bitmap bitmap, bool isBlackBackground)
    {
        BinaryImage toAdd = PrepareForAnalysis(bitmap, isBlackBackground);

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
