function HTMLtoPDF2() {
    var pdf = new jsPDF('p', 'pt', 'a4'); //orientation, unit, format
    var myHtml = document.getElementById('scrollArea').innerHTML
    var margins = {
        top: 50,
        bottom: 50,
        left: 20,
        width: 500
    };

    pdf.fromHTML(
          myHtml // HTML string or DOM elem ref.
        , margins.left // x coord
        , 100 // y coord
        , {
            'width': margins.width, // max width of content on PDF
            'table_2': true,
            'table_2_scaleBasis': 'font', // 'font' or 'width'
            'table_2_fontSize': 9
        },
        function (dispose) {
            pdf.save('Example.pdf');
        },
        margins
    )
}
function HTMLtoExcel(filename) {
    var blob = new Blob([document.getElementById('scrollArea').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".xls");
}

function HTMLtoExcelGL(filename) {
    var blob = new Blob([document.getElementById('scrollArea2').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".xls");
}

function HTMLtoExcelCC(filename2) {
    var blob = new Blob([document.getElementById('scrollArea3').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename2 + ".xls");
}

function HTMLtoExcelCS(filename) {
    var blob = new Blob([document.getElementById('scrollArea4').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".xls");
}

function HTMLtoExcelVD(filename) {
    var blob = new Blob([document.getElementById('scrollArea5').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".xls");
}

function HTMLtoExcelIO(filename) {
    var blob = new Blob([document.getElementById('scrollArea6').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".xls");
}

function HTMLtoPdf(filename) {
    var blob = new Blob([document.getElementById('scrollArea').innerHTML], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=utf-8"
    })
    saveAs(blob, filename + ".pdf");
}