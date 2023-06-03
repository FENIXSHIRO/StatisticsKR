const fileInput = document.getElementById('fileInput'); // Получение элемента ввода файла

// Элементы для скрытия/отображения
const inputDiv = document.getElementById('inputDiv');
const btnChangeFile = document.getElementById('btnChangeFile');
const returnBlock = document.getElementById('returnBlock');
const typeSelect = document.getElementById('typeSelect');


fileInput.addEventListener('change', () => {
    const file = fileInput.files[0];
    const fileName = file.name;
    const toastNotify = document.getElementById('liveToast')


    if (!fileName.endsWith('.csv')) {


        const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastNotify)
        toastBootstrap.show()



        fileInput.value = ''; // очищаем поле ввода файла
        return;
    }

    var inpFile = $('#fileInput').get(0).files[0];
    var formData = new FormData;
    formData.append('file', inpFile);
    formData.append('type', typeSelect.value);

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

            // Вывести таблицу
            DrawTable(json); 

            // Вывести график
            DrawChart(json);

            // Блокировка загрузки файлов
            inputDiv.style.display = "none";
            btnChangeFile.style.display = "block";

        },
        error: function (error) { // При ошибке
            alert("Ajax err: " + error);
        }
    });

    // код для загрузки и обработки файла CSV
});