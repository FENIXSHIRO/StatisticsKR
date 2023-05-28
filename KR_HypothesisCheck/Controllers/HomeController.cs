using KR_HypothesisCheck.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KR_HypothesisCheck.Controllers
{
    public class HomeController : Controller
    {
        //Размер массива со случайными величинами 
        int n = 5;
        const int nT = 5;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Функция создания диапазонов
        double[] RangeSorting (double[] array)
        {
            double RangeMax = 0;
            double RangeMin;
            for (int i = 0; i < array.Length; i++)
            {
                if (RangeMax < array[i])
                    RangeMax = array[i];
            }
            RangeMin = RangeMax;
            for (int i = 0; i < array.Length; i++)
            {
                if (RangeMin > array[i])
                    RangeMin = array[i];
            }
            double[] ArrayOfRanges = new double[n];
            double diff = (RangeMax - RangeMin) / n;
            for (int i = 0; i < n; i++) 
            {
                RangeMin += diff;
                ArrayOfRanges[i] = RangeMin;
                //Для тестов
                Console.WriteLine(ArrayOfRanges[i]);
            }
            return ArrayOfRanges;
        }

        //Функция попадания выборки в диапазоны
        double[] SortNum(double[] array, double[] arrDiapT)
        {
            int edgeNum = 0;
            double[] arrNumT = new double[n];
            double diff = arrDiapT[1] - arrDiapT[0];
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < arrDiapT.Length; j++)
                {
                    if (array[i] < arrDiapT[j] && array[i] >= arrDiapT[j] - diff)
                    {
                        arrNumT[j]++;
                    }
                }
                if (array[i] == arrDiapT[arrDiapT.Length - 1])
                    edgeNum++;
            }
            arrNumT[arrNumT.Length - 1] += edgeNum;
            for (int i = 0; i < arrNumT.Length; i++)
            {
                //Для тестов
                Console.WriteLine(arrNumT[i]);
            }
            return arrNumT;
        }

        public DataModel CheckHypothesis(double[] ArrayOfRange, double[] ArrayOfNumbers)
        {
            var DataModel = new DataModel();
            DataModel.Distribution = new List<double>();

            //Переменная содержащая значения длины информационного интервала
            double diff = ArrayOfRange[1] - ArrayOfRange[0];

            //Переменная содержащая значение количества всех выборов 
            double alln = 0;

            //Массив содержащий сдернюю величину каждого информационного интервала
            double[] AvgIter = new double[n];

            //Массив критических значений распределения х^2
            double[] CritDisValuesFor005 = new double[10] { 3.8, 6.0, 7.8, 9.5, 11.1, 12.6, 14.1, 15.5, 16.9, 18.3 };

            //Вспомагательные переменные
            double xn = 0;
            double x2n = 0;

            //переменные для вычесления моды 
            double modax = 0, moda = 0;



            for (int i = 0; i < n; i++)
            {
                //Заполнение массива средних значений интервалов
                AvgIter[i] = (ArrayOfRange[i] + (ArrayOfRange[i] - diff)) / 2;
                //Подсчет всех выборов
                alln += ArrayOfNumbers[i];
                xn += AvgIter[i] * ArrayOfNumbers[i];
                x2n += Math.Pow(AvgIter[i], 2) * ArrayOfNumbers[i];
                //Получение моды
                if (ArrayOfNumbers[i] > modax)
                {
                    modax = ArrayOfNumbers[i];
                    moda = ArrayOfRange[i];
                }
            }



            //Выборочная средняя
            double AvgSelect = xn / alln;

            //Выборочная дисперсия
            double VarSelect = (x2n / alln) - Math.Pow(AvgSelect, 2);

            //Выборочное стандартное отклонение
            double DeviaSelect = Math.Pow(VarSelect, 0.5);

            //Массив содеражащий теоретические частоты
            double[] TheoFreq = new double[n];



            for (int i = 0; i < n; i++)
            {
                //Испльзование формулы Гаусса для рсчета теоритеческих частот
                TheoFreq[i] = ((diff * alln) / DeviaSelect) * ((1 / (Math.Pow(2 * 3.14, 0.5)) * Math.Exp((Math.Pow((AvgIter[i] - AvgSelect) / DeviaSelect, 2) / -2))));
                //Тестирование значений
                Console.WriteLine(TheoFreq[i]);
                DataModel.Distribution.Add(TheoFreq[i]);
            }

            //Наблюдаемое значение критерия
            double ObserValue = 0;
            for (int i = 0; i < n; i++)
            {
                //Вычисление наблюдаемого значения критерия
                ObserValue += (ArrayOfNumbers[i] - TheoFreq[i]) / TheoFreq[i];
            }

            //Медиана выборки 
            double median = 0;
            if (n % 2 == 0)
                median = (ArrayOfRange[n / 2 - 1] + ArrayOfRange[n / 2]) / 2;
            else
                median = (ArrayOfRange[n / 2] + ArrayOfRange[n / 2 + 1]) / 2;

            //Тестовые значения
            DataModel.moda = moda;
            DataModel.median = median;
            DataModel.AvgSelect = AvgSelect;

            //Количество степеней свободы
            int NumDegFree = n - 2 - 1;

            //Проверка гипотезы
            if (ObserValue < CritDisValuesFor005[NumDegFree] && 
                moda <= AvgSelect + diff && moda >= AvgSelect - diff && 
                median <= AvgSelect + diff && median >= AvgSelect - diff && 
                AvgSelect - 3 * DeviaSelect < ArrayOfRange[0] && 
                AvgSelect + 3 * DeviaSelect > ArrayOfRange[n - 1]
                )
            {
                DataModel.Conclusion = true;
                return DataModel;

            } else {

                DataModel.Conclusion = false;
                return DataModel;
            }
        }

        public JsonResult GetResult(IFormFile file)
        {
            var DataModel = new DataModel();
            DataModel.Distribution = new List<double>();

            List<string> listA = new List<string>();

            var filePath = Path.GetTempFileName();

            using (var reader = new StreamReader( file.OpenReadStream() ) )
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    listA.AddRange(values);
                }
            }

            double[] arrTempT = new double[listA.Count];

            for (int i = 0; i < listA.Count; i++)
            {
                arrTempT[i] = Convert.ToDouble(listA[i]);
            }

            //Массив содержащий диапазоны
            double[] arrDiap = new double[nT] { 975, 1000, 1025, 1050, 1075 };
            double[] arrDiapT = RangeSorting(arrTempT);

            //Массив содержащий количества попаданий в диапазоны
            double[] arrNum = new double[nT] { 6, 38, 44, 34, 8 };
            double[] arrNumT = SortNum(arrTempT, arrDiapT);

            DataModel = CheckHypothesis(arrDiapT, arrNumT);
            DataModel.LabelData = arrDiapT.ToList();
            DataModel.StatisticData = arrNumT.ToList();

            // Вывод JSON
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(DataModel, options);

            return Json(jsonString);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}