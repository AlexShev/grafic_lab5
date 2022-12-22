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

            // можно малое изображение 8 на 8
            // жнлательно чёрный фон и пару пятен (можно разных цветов)
            string fillName = "C:\\Users\\ALEX\\Desktop\\ComputerGraphics\\code\\grafic_lab5\\grafic_lab5\\Images\\Image1.bmp";

            Bitmap bitmap = new Bitmap(fillName);

            // везде желательно проверить выход за пределы массива
            // и не перепутаны ли столбцы и строки

            // оценить на сколько получилось преобразование (приблизительно, основываясь на интенсивности)
            GrayImage image = GrayImage.Create(bitmap);

            // проверить процес чтения (с указанием путя мог проебаться)
            MatrixParser parser = new MatrixParser("C:\\Users\\ALEX\\Desktop\\ComputerGraphics\\code\\grafic_lab5\\grafic_lab5\\ImageFilter\\filter.txt");

            LinearFilter linearFilter = new LinearFilter(parser.Matrix, parser.Coefficient);

            // проверить выход за пределы и ориентацию (желательно проверить на переполнение отклик фильтра)
            image = linearFilter.Filter(image);

            // кроме выхода за пределы желательно оценить правильность перевода
            // так же желательно оценить правильность перевода при белом фоне (возможны приколы)
            double binarizationBarrier = BinaryImage.CuclBinarizationBarrier(image);

            BinaryImage binaryImage = BinaryImage.Create(image, binarizationBarrier, true);

            // можно просто проследить выход за пределы и неперепутаны ли столбцы и строки
            // но желательно проследить корректность
            binaryImage = MorphologicalFilter.OpeningFilter(binaryImage);

            // задание со звёздочкой
            // нахождение карты компонент связности
            ComponentMap componentMap = ComponentMap.Create(binaryImage);
            // нахождение компонент связности по карте
            var components = ComponentDeterminator.FindComponents(componentMap);

            if (components.Any())
            {
                // проверить, что выделяется нужная часть изображения
                BinaryImage component = componentMap.ClipComponent(components[0].Item1, components[0].Item2);

                // проверить работу маштабирования и вычисления хэша
                var hash = PerceptualHash.CalcPerceptualHash(component);
                
                byte d1 = PerceptualHash.HammingDistances(hash, ulong.MaxValue);
                byte d2 = PerceptualHash.HammingDistances(hash, 0);
                byte d3 = PerceptualHash.HammingDistances(hash, 0b0101010101);
            }
        }
    }
}