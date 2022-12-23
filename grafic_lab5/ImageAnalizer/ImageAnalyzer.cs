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

/// <summary>
/// класс - результат анализа
/// </summary>
public class AnalyzerResult
{
    /// <summary>
    /// найденные образы
    /// </summary>
    public List<ResultFindedItem> ResultFindedItems;
    /// <summary>
    /// Ненайденные образы
    /// </summary>
    public List<ResultNewComponentItem> ResultNewComponentItems;

    public AnalyzerResult()
    {
        ResultFindedItems = new List<ResultFindedItem>();
        ResultNewComponentItems = new List<ResultNewComponentItem>();
    }
}

/// <summary>
/// Найденный образ
/// </summary>
public class ResultFindedItem : IComparable<ResultFindedItem>
{
    /// <summary>
    /// Методанные
    /// </summary>
    public MetaData MetaData;
    
    /// <summary>
    /// Положение
    /// </summary>
    public Rectangle Location;

    public ResultFindedItem(MetaData metaData, Rectangle location)
    {
        MetaData = metaData;
        Location = location;
    }

    /// <summary>
    /// Чтобы можно было сортировать по местуположению
    /// </summary>
    /// <param name="other">Другой объект</param>
    public int CompareTo(ResultFindedItem? other)
    {
        if (other == null)
            return 1;

        int result = Location.X.CompareTo(other.Location.X);

        if (result == 0)
        {
            result = Location.Y.CompareTo(other.Location.Y);
        }

        return result;
    }
}

/// <summary>
/// Новый образ
/// </summary>
public class ResultNewComponentItem
{
    /// <summary>
    /// Положение
    /// </summary>
    public Rectangle Location;

    /// <summary>
    /// Хэш изображения
    /// </summary>
    public ulong Hash;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="location">положение</param>
    /// <param name="hash">хэш изображения</param>
    public ResultNewComponentItem(Rectangle location, ulong hash)
    {
        Location = location;
        Hash = hash;
    }
}

/// <summary>
/// Анализатор изображения
/// </summary>
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
    private const double MinimumMatchPercentage = 0.9;

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

    /// <summary>
    /// Режим работы с пользователем
    /// </summary>
    public bool IsInteractive { set; get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="storage">База образов</param>
    /// <param name="isInteractiv">Режим работы с пользователем</param>
    public ImageAnalyzer(ComponentStorage storage, bool isInteractiv)
    {
        _storage = storage;
        BinarizationMeasure = 0;
        IsInteractive = isInteractiv;
    }

    /// <summary>
    /// Установить линейный фильтр
    /// </summary>
    /// <param name="linearFilter">Линейный фильтр</param>
    public void SetLinearFilter(LinearFilter linearFilter)
    {
        _linearFilter = linearFilter;
    }

    /// <summary>
    /// Получить список образов, которые известны и не известны базе образов
    /// </summary>
    /// <param name="image">изображение для анализа</param>
    /// <returns>список характеристик образов</returns>
    public AnalyzerResult AnalyzeImage(Bitmap image, bool isBlackBackground)
    {
        BinaryImage toAnalysis = PrepareForAnalysis(image, isBlackBackground);

        // нахождение карты компонент связности
        ComponentMap componentMap = ComponentMap.Create(toAnalysis);
        // нахождение компонент связности по карте
        var components = ComponentDeterminator.FindComponents(componentMap);

        AnalyzerResult result = new AnalyzerResult();

        foreach (var component in components)
        {
            BinaryImage componentToFind = componentMap.ClipComponent(component.Item1, component.Item2);

            var hash = PerceptualHash.CalcPerceptualHash(componentToFind);

            var response = _storage.FindCloserComponent(hash);

            var matchPercentage = 1 - PerceptualHash.HammingDistances(hash, response.Hash) / 64.0;

            if (response != null && MinimumMatchPercentage <= matchPercentage)
            {
                result.ResultFindedItems.Add(new ResultFindedItem(response.Data, component.Item1));
            }
            else
            {
                result.ResultNewComponentItems.Add(new ResultNewComponentItem(component.Item1, hash));
            }
        }

        return result;
    }

    /// <summary>
    /// Подготовка изображения
    /// </summary>
    /// <param name="bitmap">Изображения</param>
    /// <param name="isBlackBackground">Чёрный ли фон</param>
    /// <returns></returns>
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

        if (IsInteractive)
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

    /// <summary>
    /// Заполнить базу образов из файла
    /// </summary>
    /// <param name="path">путь к списку образов</param>
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

    /// <summary>
    /// Добавление образа - считается, что он один
    /// </summary>
    /// <param name="metaData">Методанные</param>
    /// <param name="bitmap">Изображение с образом</param>
    /// <param name="isBlackBackground">Чёрный ли фон</param>
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
