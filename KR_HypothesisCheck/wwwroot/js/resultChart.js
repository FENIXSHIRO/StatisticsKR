function mapArr(arr) {
    return arr = arr.map(num => Number(num.toFixed(2)));
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

    /*
    
    addRow(obj.LabelData, obj.LabelData.length, "Группы частот");
    addRow(obj.StatisticData, obj.LabelData.length, "Количество");
    addRow(obj.Distribution, obj.LabelData.length, "Количество по функции");

    */

    addRow(mapArr(obj.LabelData), obj.LabelData.length, "Группы частот, Гц");
    addRow(mapArr(obj.StatisticData), obj.LabelData.length, "Количество, Шт");
    addRow(mapArr(obj.Distribution), obj.LabelData.length, "Количество по функции");
}

function DrawChart(json) {
    let obj = JSON.parse(json);
    let labels = mapArr(obj.LabelData);

    const data = {
        labels: labels,
        datasets: [{
            label: 'Функция',
            type: 'line',
            borderColor: 'rgb(10, 10, 10)',
            backgroundColor: 'rgb(255, 255, 255)',
            data: mapArr(obj.Distribution),
            tension: 0.3
        }, {
            label: 'Исходные данные',
            type: 'bar',
            backgroundColor: 'rgba(250, 200, 10, 0.9)',
            borderColor: 'rgb(230, 180, 10)',
            borderWidth: 1,
            data: mapArr(obj.StatisticData),
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
                        text: 'Количество, Шт.'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Тактовая сачтота, Гц.'
                    }
                }
            }
        }
    };

    const myChart = new Chart(document.getElementById('myChart'), config);

    document.getElementById('text').textContent = obj.moda + " | " + obj.median + " | " + obj.AvgSelect + " | " + obj.Conclusion;
}
