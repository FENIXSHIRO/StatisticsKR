const exportAsXLS = document.getElementById('exportAsXLS');
const exportAsCSV = document.getElementById('exportAsCSV');

// Сохранение как XLS
exportAsXLS.onclick = (function () {
	var uri = 'data:application/vnd.ms-excel;base64,'
		, template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
		, base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
		, format = function (s, c) {
			return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; })
		}
		, downloadURI = function (uri, name) {
			var link = document.createElement("a");
			link.download = name;
			link.href = uri;
			link.click();
		}

		table = document.getElementById('table')
		var ctx = { worksheet: 'Результат' || 'Worksheet', table: table.innerHTML }
		var resuri = uri + base64(format(template, ctx))
		downloadURI(resuri, 'Result.xls');
});


// Сохранение как CSV
exportAsCSV.onclick = function tableToCSV() {

    var csv_data = [];

    var rows = document.getElementsByTagName('tr');
    for (var i = 0; i < rows.length; i++) {

        var cols = rows[i].querySelectorAll('td,th');


        var csvrow = [];
        for (var j = 0; j < cols.length; j++) {

            csvrow.push(cols[j].innerHTML);
        }

        csv_data.push(csvrow.join(","));
    }


    csv_data = csv_data.join('\n');


    downloadCSVFile(csv_data);

}

function downloadCSVFile(csv_data) {

    CSVFile = new Blob([csv_data], {
        type: "text/csv"
    });

    var temp_link = document.createElement('a');

    temp_link.download = "CSV_Result.csv";
    var url = window.URL.createObjectURL(CSVFile);
    temp_link.href = url;

    temp_link.style.display = "none";
    document.body.appendChild(temp_link);

    temp_link.click();
    document.body.removeChild(temp_link);
}
