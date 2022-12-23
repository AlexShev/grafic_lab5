using grafic_lab5.ImageAnalizer;
using grafic_lab5.ImageFilter;
using grafic_lab5.Storages;

namespace grafic_lab5;

/// <summary>
/// �������� �����
/// </summary>
public partial class Form1 : Form
{
    /// <summary>
    /// ���������� �������
    /// </summary>
    private ImageAnalyzer _analyzer;
    /// <summary>
    /// ��������� �������
    /// </summary>
    private ComponentStorage _storage;
    /// <summary>
    /// �����������, ������� �������������
    /// </summary>
    private Bitmap _bitmap;

    public Form1()
    {
        InitializeComponent();

        _storage = new ComponentStorage();
        _analyzer = new ImageAnalyzer(_storage, false);
        _analyzer.FillComponentStorage("..\\..\\..\\Storages\\�omponents.txt");

        openFileDialog1.Filter = $"Bitmap files (*.bmp)|*.bmp";

        _analyzer.IsInteractive = true;
        _bitmap = new Bitmap(10, 10);
    }

    /// <summary>
    /// ������� ����������� ��� �������
    /// </summary>
    private void OpenClick(object sender, EventArgs e)
    {
        // ������
        if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            return;

        // �������� ��������� ����
        string filename = openFileDialog1.FileName;

        // ��������� �����������
        _bitmap = new Bitmap(filename);
        pictureBox1.Image = _bitmap;
    }

    /// <summary>
    /// ����� ������ �� �������� �����������
    /// </summary>
    private void FindClick(object sender, EventArgs e)
    {
        // ��������� ��������� �������
        MatrixParser parser = new MatrixParser("..\\..\\..\\ImageFilter\\filter.txt");
        _analyzer.SetLinearFilter(new LinearFilter(parser.Matrix, parser.Coefficient));

        // ������������� �������, ����������� � �� ��������� 
        var results = _analyzer.AnalyzeImage(_bitmap, comboBox1.SelectedIndex == 1);

        var finded = results.ResultFindedItems;

        // ���� ���� �� ������������� ������
        if (results.ResultNewComponentItems.Any())
        {
            // �������� ������������ � ���������� ����� �������
            var result = MessageBox.Show(
                "��������� �� ������ ���������� ��������� ������",
                "�� ������ �� ��������?", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                // �������� ����� ���������� �������
                AddNewComponentForm addForm = new AddNewComponentForm(results.ResultNewComponentItems, _bitmap);
                addForm.ShowDialog();

                // ���������� �������
                _storage.AddComponent(addForm.FindedComponents);

                // ���������� � ��������� ����� �������
                finded.AddRange(addForm.FindedItems);
            }
        }

        // ������������� ����� ������� � ������ ����
        finded.Sort();
        string str = "";

        pictureBox1.Image = new Bitmap(_bitmap);

        using Graphics graphics = Graphics.FromImage(pictureBox1.Image);

        for (int i = 0; i < finded.Count; i++)
        {
            // ��������� ���������������
            using Pen pen = new Pen(RGBcolorCreator.GetRandomColor(), 4);
            graphics.DrawRectangle(pen, finded[i].Location);

            // ���������� ��������
            str += finded[i].MetaData.Name;
        }

        // ����� ������ �� �����
        textBox1.Text = str;
    }

    /// <summary>
    /// ���������� ������� �����������
    /// </summary>
    private void trackBar1_Scroll(object sender, EventArgs e)
    {
        _analyzer.BinarizationMeasure = trackBar1.Value / 10.0;
    }
}

/// <summary>
/// ��������� ������
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