 // Получение элемента ввода файла
const fileInput = document.getElementById('fileInput');

// Элементы для скрытия/отображения (Вводные данные)
const inputDiv = document.getElementById('inputDiv');

// Кнопка смены файла
const btnChangeFile = document.getElementById('btnChangeFile');

// Элементы для скрытия/отображения (Вывод)
const returnBlock = document.getElementById('returnBlock');
const textBlock = document.getElementById('textBlock');

// Селектор тпа операции
const typeSelect = document.getElementById('typeSelect');

// Блок с выбором количества интервалов
const intervalRangeDiv = document.getElementById('intervalRangeDiv');



// Селектор стиля графика
const firstChartStyle = document.getElementById('firstChartStyle');
const secondChartStyle = document.getElementById('secondChartStyle');

// Опции графика
let ChartOp = {
    firstChart: 'line',
    secondChart: 'bar',
    firstChartTension: 0.3,
    secondChartTension: 0.3,
    fill: false
};

// Переключатель заливки
const fillSwitch = document.getElementById('fillSwitch');



// Обработчик range
const intervalRange = document.querySelector("#intervalRange");

const rangeValue = document.querySelector("#rangeValue");
rangeValue.textContent = intervalRange.value;

intervalRange.addEventListener("input", (event) => {
    rangeValue.textContent = event.target.value;
})



// Выбор типа операции
if (typeSelect.value == "normal") {
    intervalRangeDiv.style.display = "block";
} else {
    intervalRangeDiv.style.display = "none";
}
// Обработка выбора типа операци
typeSelect.addEventListener('change', (event) => {
    if (event.target.value == "normal") {
        intervalRangeDiv.style.display = "block";
    } else {
        intervalRangeDiv.style.display = "none";
    }
})


// Выбор типа ПЕРВОГО графика
if (firstChartStyle.value == "polygonLine") {

    ChartOp.firstChart = 'line';
    ChartOp.firstChartTension = 0;

} else {

    ChartOp.firstChart = firstChartStyle.value;

}

// Обработка выбора типа ПЕРВОГО графика
firstChartStyle.addEventListener('change', (event) => {
    if (event.target.value == "polygonLine") {

        ChartOp.firstChart = 'line';
        ChartOp.firstChartTension = 0;

    } else {

        ChartOp.firstChart = event.target.value;

    }
})

// Выбор типа ВТОРОГО графика
if (secondChartStyle.value == "polygonLine") {

    ChartOp.secondChart = 'line';
    ChartOp.secondChartTension = 0;

} else {

    ChartOp.secondChart = secondChartStyle.value;

}

// Обработка выбора типа ВТОРОГО графика
secondChartStyle.addEventListener('change', (event) => {
    if (event.target.value == "polygonLine") {

        ChartOp.secondChart = 'line';
        ChartOp.secondChartTension = 0;

    } else {

        ChartOp.secondChart = event.target.value;

    }
})

// Обработка выбора заливки графика
ChartOp.fill = fillSwitch.checked;

fillSwitch.addEventListener('change', (event) => {
    ChartOp.fill = event.target.checked;
})

// Обработчик ввода файла
fileInput.addEventListener('change', () => {
    const file = fileInput.files[0];
    const fileName = file.name;

    const toastNotify = document.getElementById('liveToast')
    const toastNotifyOperation = document.getElementById('opLiveToast')


    if (!fileName.endsWith('.csv')) {

        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastNotify);
        toastBootstrap.show();



        fileInput.value = ''; // очищаем поле ввода файла
        return;
    }

    if (typeSelect.value == "none") {

        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastNotifyOperation);
        toastBootstrap.show();

        fileInput.value = ''; // очищаем поле ввода файла
        return
    }

    // Данные для передачи на сервер
    var inpFile = $('#fileInput').get(0).files[0];
    var formData = new FormData;
    formData.append('file', inpFile);
    formData.append('type', typeSelect.value);
    formData.append('intervals', intervalRange.value);

    // Получить данные из контроллера и вывести график из resulChart.js
    $.ajax({
        url: '/Home/GetResult',
        type: 'Post',
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (json) { // При успешном получении данных

            // Отобразить блок вывода
            returnBlock.style.display = "block";
            textBlock.style.display = "block";

            // Вывести таблицу
            DrawTable(json); 

            // Вывести график
            DrawChart( json, JSON.stringify(ChartOp) );

            // Блокировка загрузки файлов (просто прячем ввод)
            inputDiv.style.display = "none";

            btnChangeFile.style.display = "block";

        },
        error: function (error) { // При ошибке
            alert("Введены неверные данные: " + error);
        }
    });

    // код для загрузки и обработки файла CSV
});