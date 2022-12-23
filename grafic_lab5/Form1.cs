using grafic_lab5.ImageAnalizer;
using grafic_lab5.ImageFilter;
using grafic_lab5.Images;
using grafic_lab5.ImagesData;

namespace grafic_lab5;

public partial class Form1 : Form
{
    private ImageAnalyzer _analyzer;
    private Bitmap _bitmap;

    public Form1()
    {
        InitializeComponent();

        _analyzer = new ImageAnalyzer(new Storages.ComponentStorage(), false);
        _analyzer.FillComponentStorage("..\\..\\..\\Storages\\сomponents.txt");

        openFileDialog1.Filter = $"Bitmap files (*.bmp)|*.bmp";

        _analyzer.IsInteractiv = true;
    }

    private void OpenClick(object sender, EventArgs e)
    {
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            return;

        // получаем выбранный файл
        string filename = openFileDialog1.FileName;

        _bitmap = new Bitmap(filename);
        pictureBox1.Image = _bitmap;
    }

    private void FindClick(object sender, EventArgs e)
    {
        // проверить процес чтения (с указанием путя мог проебаться)
        MatrixParser parser = new MatrixParser("..\\..\\..\\ImageFilter\\filter.txt");

        _analyzer.SetLinearFilter(new LinearFilter(parser.Matrix, parser.Coefficient));

        var results = _analyzer.AnalyzeImage(_bitmap, comboBox1.SelectedIndex == 1);

        pictureBox1.Image = new Bitmap(_bitmap);

        using Graphics graphics = Graphics.FromImage(pictureBox1.Image);

        results.Sort();
        string str = "";

        for (int i = 0; i < results.Count; i++)
        {
            using Pen pen = new Pen(RGBcolorCreator.GetRandomColor(), 4);

            graphics.DrawRectangle(pen, results[i].Location);

            str += results[i].Name;
        }

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