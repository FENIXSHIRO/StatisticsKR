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
        //Массив критических значений распределения х^2
        double[] CritDisValuesFor005 = new double[10] { 3.8, 6.0, 7.8, 9.5, 11.1, 12.6, 14.1, 15.5, 16.9, 18.3 };

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Функция счета факториала
        int Factorial(int n)
        {
            if (n == 1 || n == 0) return 1;
            return n * Factorial(n - 1);
        }

        //Функция создания диапазонов для Нормального
        double[] RangeSortingNorm (double[] array)
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

        //Функция создания диапазонов для Пуассона
        double[] RangeSortingPyass(double[] array)
        {
            double RangeMax = 1;
            for (int i = 0; i < array.Length; i++)
                if (RangeMax < array[i]) RangeMax = array[i];
            double[] ArrayOfRanges = new double[Convert.ToInt32(RangeMax) + 1];
            for (int i = 0; i < ArrayOfRanges.Length; i++)
                ArrayOfRanges[i] = i;
            return ArrayOfRanges;
        }

        //Функция попадания выборки в диапазоны для Нормального
        double[] SortNumNorm(double[] array, double[] arrDiapT)
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
        //Функция попадания выборки в диапазоны для Пуассона
        double[] SortNumPyass(double[] array, double[] arrDiapP)
        {
            double[] arrNumP = new double[arrDiapP.Length]; 
            for (int i = 0; i < array.Length; i++)
                for (int j = 0;j < arrDiapP.Length; j++)
                    if (array[i] == arrDiapP[j]) arrNumP[j]++;
            return arrNumP;
        }
        //Функция эмпирических вероятностей для Пуассона
        double[] SortProbPyass(double[] arrNumP)
        {
            double[] arrProbP = new double[arrNumP.Length];
            double AllNum = 0;
            for (int i = 0; i < arrNumP.Length; i++)
                AllNum += arrNumP[i];
            for (int i = 0; i < arrNumP.Length; i++)
                arrProbP[i] = arrNumP[i]/AllNum;
            return arrProbP;
        }
        //Проверка гипотезы по закону Пуассона
        public DataModel CheckHypothesisPyass(double[] ArrayOfRange, double[] arrNumP)
        {
            var DataModel = new DataModel();
            DataModel.Distribution = new List<double>();
            //Среднее число брака в партии
            double alpha = 0;
            for(int i = 0; i < ArrayOfRange.Length; i++)
                alpha += arrNumP[i] * ArrayOfRange[i];
            double AllNum = 0;
            for (int i = 0; i < arrNumP.Length; i++)
                AllNum += arrNumP[i];
            alpha /= AllNum;
            double[] TheoProb = new double[arrNumP.Length];
            double ObserValue = 0;
            for (int i = 0; i < TheoProb.Length; i++)
            {
                TheoProb[i] = (Math.Pow(alpha, i) * Math.Exp(-alpha))/Factorial(i);
                ObserValue += Math.Pow(arrNumP[i] - (AllNum * TheoProb[i]), 2)/(AllNum * TheoProb[i]);
                DataModel.Distribution.Add(TheoProb[i]);
            }
            int NumDegFree = arrNumP.Length - 1 - 1;
            if (ObserValue < CritDisValuesFor005[NumDegFree])
            {
                DataModel.Conclusion = "Распределение подчиняется закону Пуассона";
                return DataModel;
            }
            else
            {
                DataModel.Conclusion = "Распределение не подчиняется закону Пуассона";
                return DataModel;
            }
        }

        //Проверка гипотезы по Нормльному закону
        public DataModel CheckHypothesisNorm(double[] ArrayOfRange, double[] ArrayOfNumbers)
        {
            var DataModel = new DataModel();
            DataModel.Distribution = new List<double>();

            //Переменная содержащая значения длины информационного интервала
            double diff = ArrayOfRange[1] - ArrayOfRange[0];

            //Переменная содержащая значение количества всех выборов 
            double alln = 0;

            //Массив содержащий сдернюю величину каждого информационного интервала
            double[] AvgIter = new double[n];

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
                DataModel.Conclusion = "Распределение подчиняется нормальному закону";
                return DataModel;
            } 
            else 
            {
                DataModel.Conclusion = "Распределение не подчиняется нормальному закону";
                return DataModel;
            }
        }


        public JsonResult GetResult(IFormFile file, string type)
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

            switch (type)
            {
                case "pyasson":
                    double[] arrDiapP = RangeSortingPyass(arrTempT);
                    double[] arrNumP = SortNumPyass(arrTempT, arrDiapP);
                    double[] arrProbP = SortProbPyass(arrNumP);
                    DataModel = CheckHypothesisPyass(arrDiapP, arrNumP);
                    DataModel.LabelData = arrDiapP.ToList();
                    DataModel.StatisticData = arrProbP.ToList();
                    break;
                case "normal":
                    double[] arrDiapN = RangeSortingNorm(arrTempT);
                    double[] arrNumN = SortNumNorm(arrTempT, arrDiapN);
                    DataModel = CheckHypothesisNorm(arrDiapN, arrNumN);
                    DataModel.LabelData = arrDiapN.ToList();
                    DataModel.StatisticData = arrNumN.ToList();
                    break;
                default:
                    break;
            }

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