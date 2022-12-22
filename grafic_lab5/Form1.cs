using grafic_lab5.ImageFilter;
using grafic_lab5.Images;
using grafic_lab5.ImagesData;

namespace grafic_lab5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // ����� ����� ����������� 8 �� 8
            // ���������� ������ ��� � ���� ����� (����� ������ ������)
            string fillName = "���� �� �������� �����������";

            Bitmap bitmap = new Bitmap(fillName);

            // ����� ���������� ��������� ����� �� ������� �������
            // � �� ���������� �� ������� � ������

            // ������� �� ������� ���������� �������������� (��������������, ����������� �� �������������)
            GrayImage image = GrayImage.Create(bitmap);

            // ��������� ������ ������ (� ��������� ���� ��� ����������)
            MatrixParser parser = new MatrixParser("grafic_lab5\\grafic_lab5\\ImageFilter\\filter.txt");

            LinearFilter linearFilter = new LinearFilter(parser.Matrix, parser.Coefficient);

            // ��������� ����� �� ������� � ���������� (���������� ��������� �� ������������ ������ �������)
            image = linearFilter.Filter(image);

            // ����� ������ �� ������� ���������� ������� ������������ ��������
            // ��� �� ���������� ������� ������������ �������� ��� ����� ���� (�������� �������)
            BinaryImage binaryImage = BinaryImage.Create(image, 0.6, true);

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