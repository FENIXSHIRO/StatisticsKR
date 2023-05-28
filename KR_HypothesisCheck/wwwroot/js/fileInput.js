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

    var inpFile = $('#fileInput').get(0).files[0];
    var formData = new FormData;
    formData.append('file', inpFile);

    // Получить данные из контроллера и вывести график из resulChart.js
    $.ajax({
        url: '/Home/GetResult',
        type: 'Post',
        data: formData,
        contentType: false,
        processData: false,
        dataType: 'json',
        success: function (json) {
            DrawChart(json);
        },
        error: function (error) {
            alert("Ajax err: " + error);
        }
    });

    // код для загрузки и обработки файла CSV
});