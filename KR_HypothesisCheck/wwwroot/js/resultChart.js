function DrawChart(json) {
    let obj = JSON.parse(json);
    let labels = obj.LabelData;
    let isFill

    const data = {
        labels: labels,
        datasets: [{
            label: 'Функция',
            type: 'line',
            borderColor: 'rgb(10, 10, 10)',
            backgroundColor: 'rgb(255, 255, 255)',
            data: obj.Distribution,
            tension: 0.4
        }, {
            label: 'Исходные данные',
            type: 'bar',
            backgroundColor: 'rgb(250, 200, 10)',
            data: obj.StatisticData,
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
                        text: 'Тактовая сачтота, Гц.'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Количество, Шт.'
                    }
                }
            }
        }
    };

    const myChart = new Chart(document.getElementById('myChart'), config);

    document.getElementById('text').textContent = obj.moda + " | " + obj.median + " | " + obj.AvgSelect + " | " + obj.Conclusion;
}
