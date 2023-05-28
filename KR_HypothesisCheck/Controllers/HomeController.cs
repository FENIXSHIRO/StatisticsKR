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
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public JsonResult CheckHypothesis()
        {
            var DataModel = new DataModel();
            DataModel.Distribution = new List<double>();

            //  Размер массива со случайными величинами 
            const int n = 5;
            //  Функция проверки гипотезы о нормальном распределении
            bool ProverkaGipotezy(double[,] arr)
            {
                //  Переменная содержащая значения длины информационного интервала
                double diff = arr[1, 0] - arr[0, 0];
                //  Переменная содержащая значение количества всех выборов 
                double alln = 0;
                //  Массив содержащий сдернюю величину каждого информационного интервала
                double[] AvgIter = new double[n];
                //  Массив критических значений распределения х^2
                double[] CritDisValuesFor005 = new double[10] { 3.8, 6.0, 7.8, 9.5, 11.1, 12.6, 14.1, 15.5, 16.9, 18.3 };
                //  Вспомагательные переменные
                double xn = 0;
                double x2n = 0;
                //  Переменные для вычесления моды 
                double modax = 0, moda = 0;
                for (int i = 0; i < n; i++)
                {
                    //  Заполнение массива средних значений интервалов
                    AvgIter[i] = (arr[i, 0] + (arr[i, 0] - diff)) / 2;
                    //  Подсчет всех выборов
                    alln += arr[i, 1];
                    xn += AvgIter[i] * arr[i, 1];
                    x2n += Math.Pow(AvgIter[i], 2) * arr[i, 1];
                    //  Получение моды
                    if (arr[i, 1] > modax)
                    {
                        modax = arr[i, 1];
                        moda = arr[i, 0];
                    }
                }
                // Выборочная средняя
                double AvgSelect = xn / alln;
                // Выборочная дисперсия
                double VarSelect = (x2n / alln) - Math.Pow(AvgSelect, 2);
                // Выборочное стандартное отклонение
                double DeviaSelect = Math.Pow(VarSelect, 0.5);
                // Массив содеражащий теоретические частоты
                double[] TheoFreq = new double[n];
                for (int i = 0; i < n; i++)
                {
                    // Испльзование формулы Гаусса для рсчета теоритеческих частот
                    TheoFreq[i] = ((diff * alln) / DeviaSelect) * ((1 / (Math.Pow(2 * 3.14, 0.5)) * Math.Exp((Math.Pow((AvgIter[i] - AvgSelect) / DeviaSelect, 2) / -2))));
                    // Тестирование значений
                    Console.WriteLine(TheoFreq[i]);
                    DataModel.Distribution.Add(TheoFreq[i]);
                }
                // Наблюдаемое значение критерия
                double ObserValue = 0;
                for (int i = 0; i < n; i++)
                {
                    // Вычисление наблюдаемого значения критерия
                    ObserValue += (arr[i, 1] - TheoFreq[i]) / TheoFreq[i];
                }
                // Медиана выборки 
                double median = 0;
                if (n % 2 == 0)
                    median = (arr[n / 2 - 1, 0] + arr[n / 2, 0]) / 2;
                else
                    median = (arr[n / 2, 0] + arr[n / 2 + 1, 0]) / 2;
                // Тестирование значений
                Console.WriteLine(moda);
                DataModel.moda = moda;
                Console.WriteLine(median);
                DataModel.median = median;
                Console.WriteLine(AvgSelect);
                DataModel.AvgSelect = AvgSelect;
                // Количество степеней свободы
                int NumDegFree = n - 2 - 1;
                // Проверка гипотезы
                if (ObserValue < CritDisValuesFor005[NumDegFree] && moda <= AvgSelect + diff && moda >= AvgSelect - diff && median <= AvgSelect + diff && median >= AvgSelect - diff && AvgSelect - 3 * DeviaSelect < arr[0, 0] && AvgSelect + 3 * DeviaSelect > arr[n - 1, 0])
                {
                    DataModel.Conclusion = true;
                    return true;
                }
                else
                {
                    DataModel.Conclusion = false;
                    return false;
                }
            }


            DataModel.LabelData = new List<double>
            {
                975, 1000, 1025, 1050, 1075
            };
            DataModel.StatisticData = new List<double>
            {
                6, 38, 44, 34, 8
            };


            // Массив со занчениями выборки(Отсортирован)
            double[,] temp = new double[n, 2] { { 975, 6 }, { 1000, 38 }, { 1025, 44 }, { 1050, 34 }, { 1075, 8 } };
            // Вызов функции
            Console.WriteLine(ProverkaGipotezy(temp));


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