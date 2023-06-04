const exportAsPNG = document.getElementById('exportAsPNG');
function mapArr(arr) {
    return arr = arr.map(num => Number(num.toFixed(2)));
}

function conclusionDescription(conclusion) {
    if (conclusion == "Распределение подчиняется нормальному закону") {
        return "Исходя из анализа данных, отклонения производства в пределах нормы. Проведение проверки поизводственной линии не требуется."
    }

    if (conclusion == "Распределение не подчиняется нормальному закону") {
        return "Исходя из анализа данных выявлены нестандартные статистические значения. Требуется проведение проверки поизводственной линии."
    }

    if (conclusion == "Распределение подчиняется закону Пуассона") {
        return "Исходя из анализа данных представлены вероятности выпуска бракованого процесоора в выпущенной партии."
    }

    if (conclusion == "Распределение не подчиняется закону Пуассона") {
        return "Исходя из анализа данных выявлены нестандартные статистические значения. Требуется проведение проверки поизводственной линии."
    }

    if (conclusion == "Введенный набор данных не может подчиняется закону Пуассона") {
        return " "
    }

}

function addRow(data, length, label) {
    const table = document.getElementById('table');
    const tr = table.insertRow();

    const th = tr.insertCell().outerHTML = "<th>" + label + "</th>";

    for (let i = 0; i < length; i++) {     
        const td = tr.insertCell();
        td.appendChild( document.createTextNode(data[i]) );
    }
}

function DrawTable(json) {
    let obj = JSON.parse(json);

    if (obj.LabelData == null) {
        returnBlock.style.display = "none";
        return;
    }

    // Небольшой костыль
    if (obj.Conclusion == "Распределение подчиняется нормальному закону" || obj.Conclusion == "Распределение не подчиняется нормальному закону") {
        addRow(mapArr(obj.LabelData), obj.LabelData.length, "Интервалы частот частот, Гц");
        addRow(mapArr(obj.StatisticData), obj.LabelData.length, "Количество, Шт");
        addRow(mapArr(obj.Distribution), obj.LabelData.length, "Теоретическое количество, Шт");
    } else {
        addRow(mapArr(obj.LabelData), obj.LabelData.length, "Количество бракованных единиц в партии");
        addRow(mapArr(obj.StatisticData), obj.LabelData.length, "Эмпиричская вероятность обнаружения определенного количества брака в партии");
        addRow(mapArr(obj.Distribution), obj.LabelData.length, "Теоретическая вероятность обнаружения  определенного количества брака в партии");
    } 
}

function DrawChart(json, ops) {
    let obj = JSON.parse(json);
    let thisOps = JSON.parse(ops);

    document.getElementById('ConclusionText').textContent = obj.Conclusion;
    document.getElementById('text').textContent = conclusionDescription(obj.Conclusion);

    if (obj.Distribution == null) {
        return;
    }

    let labels = mapArr(obj.LabelData);

    const data = {
        labels: labels,
        datasets: [{
            label: 'Функция',
            type: thisOps.firstChart,
            borderColor: 'rgb(10, 10, 10)',
            borderWidth: 1,
            backgroundColor: 'rgba(255, 255, 255, 0.5)',
            data: mapArr(obj.Distribution),
            tension: thisOps.firstChartTension,
            fill: thisOps.fill
        }, {
            label: 'Исходные данные',
            type: thisOps.secondChart,
            backgroundColor: 'rgba(250, 200, 10, 0.9)',
            borderColor: 'rgb(230, 140, 20)',
            borderWidth: 1,
            data: mapArr(obj.StatisticData),
            tension: thisOps.secondChartTension,
            fill: thisOps.fill
        },]
    };

    const config = {
        type: 'line',
        data: data,
        options: {
            scales: {
                y: {
                    title: {
                        display: true,
                        // Костыльный тернарный оператор на проверку вида распределения
                        text: (obj.Conclusion == "Распределение подчиняется нормальному закону" || obj.Conclusion == "Распределение не подчиняется нормальному закону") ? 'Количество, Шт.' : 'Вероятность'
                    }
                },
                x: {
                    title: {
                        display: true,
                        // Тоже самое что и выше
                        text: (obj.Conclusion == "Распределение подчиняется нормальному закону" || obj.Conclusion == "Распределение не подчиняется нормальному закону") ? 'Тактовая сачтота, Гц.' : 'Случаи произведёного брака'
                    }
                }
            }
        }
    };

    const myChart = new Chart(document.getElementById('myChart'), config);

    document.getElementById('debugText').textContent = obj.moda + " | " + obj.median + " | " + obj.AvgSelect + " | " + obj.Conclusion;

    // Скачивание PNG графика
    exportAsPNG.onclick = function () {
        var a = document.createElement('a');
        a.href = myChart.toBase64Image();
        a.download = 'chart.png';

        // Скачивание пнг
        a.click();
    }
}
