using grafic_lab5.ImageAnalizer;
using grafic_lab5.ImageFilter;
using grafic_lab5.Storages;

namespace grafic_lab5;

/// <summary>
/// Основная форма
/// </summary>
public partial class Form1 : Form
{
    /// <summary>
    /// Анализатор образов
    /// </summary>
    private ImageAnalyzer _analyzer;
    /// <summary>
    /// Хранилище образов
    /// </summary>
    private ComponentStorage _storage;
    /// <summary>
    /// Изображение, которое анализируется
    /// </summary>
    private Bitmap _bitmap;

    public Form1()
    {
        InitializeComponent();

        _storage = new ComponentStorage();
        _analyzer = new ImageAnalyzer(_storage, false);
        _analyzer.FillComponentStorage("..\\..\\..\\Storages\\сomponents.txt");

        openFileDialog1.Filter = $"Bitmap files (*.bmp)|*.bmp";

        _analyzer.IsInteractive = true;
        _bitmap = new Bitmap(10, 10);
    }

    /// <summary>
    /// Открыть изображение для анализа
    /// </summary>
    private void OpenClick(object sender, EventArgs e)
    {
        // отмена
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            return;

        // получаем выбранный файл
        string filename = openFileDialog1.FileName;

        // установка изображение
        _bitmap = new Bitmap(filename);
        pictureBox1.Image = _bitmap;
    }

    /// <summary>
    /// Найти образы на открытом изображении
    /// </summary>
    private void FindClick(object sender, EventArgs e)
    {
        // установка линейного фильтра
        MatrixParser parser = new MatrixParser("..\\..\\..\\ImageFilter\\filter.txt");
        _analyzer.SetLinearFilter(new LinearFilter(parser.Matrix, parser.Coefficient));

        // Распознавание образов, определённых и не найденных 
        var results = _analyzer.AnalyzeImage(_bitmap, comboBox1.SelectedIndex == 1);

        var finded = results.ResultFindedItems;

        // Если есть не расспознанные образы
        if (results.ResultNewComponentItems.Any())
        {
            // Спросить пользователя о добавлении новых образов
            var result = MessageBox.Show(
                "Программа не смогла распознать некоторые образы",
                "Вы хотите их добавить?", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // Открытие формы добавления образов
                AddNewComponentForm addForm = new AddNewComponentForm(results.ResultNewComponentItems, _bitmap);
                addForm.ShowDialog();

                // добавление образов
                _storage.AddComponent(addForm.FindedComponents);

                // добавление в результат новых образов
                finded.AddRange(addForm.FindedItems);
            }
        }

        // Отсортировать слева направо и сверху вниз
        finded.Sort();
        string str = "";

        pictureBox1.Image = new Bitmap(_bitmap);

        using Graphics graphics = Graphics.FromImage(pictureBox1.Image);

        for (int i = 0; i < finded.Count; i++)
        {
            // выделение прямоугольником
            using Pen pen = new Pen(RGBcolorCreator.GetRandomColor(), 4);
            graphics.DrawRectangle(pen, finded[i].Location);

            // Добавление значения
            str += finded[i].MetaData.Name;
        }

        // вывод строки на экран
        textBox1.Text = str;
    }

    /// <summary>
    /// Установить степень юинаризации
    /// </summary>
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        _analyzer.BinarizationMeasure = trackBar1.Value / 10.0;
    }
}

/// <summary>
/// Генератор цветов
/// </summary>
public static class RGBcolorCreator
{
    private static Random _random = new Random();

    public static Color GetRandomColor()
    {
        byte red = (byte)_random.Next(1, 255);
        byte green = (byte)_random.Next(1, 255);
        byte blue = (byte)_random.Next(1, 255);

        return Color.FromArgb(255, red, green, blue);
    }
}