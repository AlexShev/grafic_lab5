using grafic_lab5.ImageAnalizer;
using grafic_lab5.ImageFilter;
using grafic_lab5.Images;
using grafic_lab5.ImagesData;

namespace grafic_lab5
{
    public partial class Form1 : Form
    {
        private ImageAnalyzer _analyzer;

        public Form1()
        {
            InitializeComponent();

            _analyzer = new ImageAnalyzer(new Storages.ComponentStorage());
            // _analyzer.FillComponentStorage("");

            // ����� ����� ����������� 8 �� 8
            // ���������� ������ ��� � ���� ����� (����� ������ ������)
            string fillName = "C:\\Users\\ALEX\\Desktop\\ComputerGraphics\\code\\grafic_lab5\\grafic_lab5\\Images\\Image1.bmp";

            Bitmap bitmap = new Bitmap(fillName);

            // ����� ���������� ��������� ����� �� ������� �������
            // � �� ���������� �� ������� � ������

            // ������� �� ������� ���������� �������������� (��������������, ����������� �� �������������)
            GrayImage image = GrayImage.Create(bitmap);

            // ��������� ������ ������ (� ��������� ���� ��� ����������)
            MatrixParser parser = new MatrixParser("C:\\Users\\ALEX\\Desktop\\ComputerGraphics\\code\\grafic_lab5\\grafic_lab5\\ImageFilter\\filter.txt");

            LinearFilter linearFilter = new LinearFilter(parser.Matrix, parser.Coefficient);

            // ��������� ����� �� ������� � ���������� (���������� ��������� �� ������������ ������ �������)
            image = linearFilter.Filter(image);

            // ����� ������ �� ������� ���������� ������� ������������ ��������
            // ��� �� ���������� ������� ������������ �������� ��� ����� ���� (�������� �������)
            double binarizationBarrier = BinaryImage.CuclBinarizationBarrier(image);

            BinaryImage binaryImage = BinaryImage.Create(image, binarizationBarrier, true);

            // ����� ������ ���������� ����� �� ������� � ������������ �� ������� � ������
            // �� ���������� ���������� ������������
            binaryImage = MorphologicalFilter.OpeningFilter(binaryImage);

            // ������� �� ���������
            // ���������� ����� ��������� ���������
            ComponentMap componentMap = ComponentMap.Create(binaryImage);
            // ���������� ��������� ��������� �� �����
            var components = ComponentDeterminator.FindComponents(componentMap);

            if (components.Any())
            {
                // ���������, ��� ���������� ������ ����� �����������
                BinaryImage component = componentMap.ClipComponent(components[0].Item1, components[0].Item2);

                // ��������� ������ �������������� � ���������� ����
                var hash = PerceptualHash.CalcPerceptualHash(component);
                
                byte d1 = PerceptualHash.HammingDistances(hash, ulong.MaxValue);
                byte d2 = PerceptualHash.HammingDistances(hash, 0);
                byte d3 = PerceptualHash.HammingDistances(hash, 0b0101010101);
            }
        }
    }
}