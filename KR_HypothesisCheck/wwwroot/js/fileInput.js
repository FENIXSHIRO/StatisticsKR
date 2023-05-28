const fileInput = document.getElementById('fileInput');

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


    // Получить данные из контроллера и вывести график из resulChart.js
    $.ajax({
        url: '/Home/CheckHypothesis',
        type: 'Post',
        dataType: 'json',
        success: function (json) {
            DrawChart(json);
        },
        error: function (error) {
            alert(error);
        }
    });

    // код для загрузки и обработки файла CSV
});