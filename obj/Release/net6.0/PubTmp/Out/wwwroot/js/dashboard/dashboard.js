
function toggleSpinner() {
    var spinner = document.getElementById("spinner");
    if (spinner.style.display === "none") {
        spinner.style.display = "block";
    } else {
        spinner.style.display = "none";
    }
}


//var inputElement = document.getElementById('LicensePlate');
//inputElement.addEventListener('input', function (event) {
//    var inputValue = event.target.value;
//    if (inputValue.length > 6 || inputValue.length < 1) {
//        InvokeFilterVehicleByPlate(inputValue);
//    }
//    //console.log('Giá trị mới của input:', inputValue); 
//});
// test es6

document.getElementById('btnseach').addEventListener("click", function () {
    var startDateValue = document.getElementById('startDate').value;
    var endDateValue = document.getElementById('endDate').value;

    // Check if the dates are not empty
    if (!startDateValue || !endDateValue) {
        console.error("Start date and end date are required.");
        return;
    }

    var startDateParts = startDateValue.split(' ');
    var endDateParts = endDateValue.split(' ');

    // Check if date parts are in correct format
    if (startDateParts.length < 2 || endDateParts.length < 2) {
        console.error("Invalid date format.");
        return;
    }

    var startDateFormatted = startDateParts[0].split('/').reverse().join('-') + ' ' + startDateParts[1];
    var endDateFormatted = endDateParts[0].split('/').reverse().join('-') + ' ' + endDateParts[1];

    var plate = document.getElementById('LicensePlate').value;
    var vehicleType = parseInt(document.getElementById("vehicleType").value, 10);
    var direction = parseInt(document.getElementById("direction").value, 10);
    var speedSend = parseInt(document.getElementById("overWeight").value, 10);

    // Validate integer inputs
    if (isNaN(vehicleType) || isNaN(direction) || isNaN(speedSend)) {
        console.error("Invalid numeric input.");
        return;
    }

    var search = {
        speedsend: speedSend,
        directionsend: direction,
        platesend: plate,
        starttime: startDateFormatted,
        endtime: endDateFormatted,
        VehicleType: vehicleType
    };

    InvokeVehiclesseach(search);
});



//document.getElementById('startDate').addEventListener('change', function () {
//    var startDateValue = document.getElementById('startDate').value;
//    var endDateValue = document.getElementById('endDate').value;
//    var startDateParts = startDateValue.split(' ');
//    var startDateFormatted = startDateParts[0].split('/').reverse().join('-') + ' ' + startDateParts[1];
//    var endDateParts = endDateValue.split(' ');
//    var endDateFormatted = endDateParts[0].split('/').reverse().join('-') + ' ' + endDateParts[1];

//    console.log("Start Date:", startDateFormatted);
//    console.log("End Date:", endDateFormatted);

//    InvokeFilterVehicleByTime(startDateFormatted, endDateFormatted);
//});

//document.getElementById('endDate').addEventListener('change', function () {
//    var startDateValue = document.getElementById('startDate').value;
//    var endDateValue = document.getElementById('endDate').value;
//    var startDateParts = startDateValue.split(' ');
//    var startDateFormatted = startDateParts[0].split('/').reverse().join('-') + ' ' + startDateParts[1];
//    var endDateParts = endDateValue.split(' ');
//    var endDateFormatted = endDateParts[0].split('/').reverse().join('-') + ' ' + endDateParts[1];

//    console.log("Start Date:", startDateFormatted);
//    console.log("End Date:", endDateFormatted);

//    InvokeFilterVehicleByTime(startDateFormatted, endDateFormatted);
//});


//document.getElementById("vehicleType").addEventListener("change", function () {
//    const valueoption = document.getElementById("vehicleType");
//    const value = parseInt(valueoption.value, 10);
//    InvokeFilterVehicleByvehicleType(value);
//    console.log("value:", value);
//});

//const overWeighterr = () => {
//    const select = document.getElementById("overWeight");
//    const value = parseInt(select.value, 10);
//    InvokeFilterVehicleBySpeed(value);
//}
//document.getElementById("overWeight").addEventListener('change', overWeighterr);

//const directionerr = () => {
//    const select = document.getElementById("direction");
//    const value = parseInt(select.value, 10);
//    InvokeFilterVehicleByDirection(value);
//}

//document.getElementById("direction").addEventListener('change', directionerr);

const exportdata = async () => {
    var api = "/Dashboard/Generatexml";
    var options = {
        method: 'POST',
    };

    try {
        
        var response = await fetch(api, options);

        if (response.ok) {
           
            var filename = "";
            var disposition = response.headers.get('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }
            var blob = await response.blob();

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            link.click();
            window.URL.revokeObjectURL(link.href);
        } else {
            console.error('Failed to download file:', response.statusText);
        }
    } catch (error) {
        console.error('Error:', error);
    }
};


document.getElementById("btnExportData").addEventListener("click", exportdata);
function generatePDF() {
    var id1 = document.getElementById("idxe").value;

    // Thiết lập các tùy chọn cho fetch
    var options = {
        method: 'POST',
        processData: false,
        contentType: false,
    };
    var api = "/Dashboard/GeneratePdf/" + id1;
    
    fetch(api, options)
        .then(response => {
         
            if (!response.ok) {
                throw new Error('Failed to generate PDF: ' + response.status);
            }
            return response.blob(); 
        })
        .then(blob => {
           
            var url = window.URL.createObjectURL(blob);
            var link = document.createElement('a');
            link.href = url;
            link.download = 'violation_details.pdf';
            link.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}
function generatePDF1() {
    var id1 = document.getElementById("idxe").value;


    var options = {
        method: 'POST',
        processData: false,
        contentType: false,
    };
    var api = "/Dashboard/GeneratePdf/" + id1;
    
    fetch(api, options)
        .then(response => {
            
            if (!response.ok) {
                throw new Error('Failed to generate PDF: ' + response.status);
            }
            return response.blob(); 
        })
        .then(blob => {
            // Tạo một đường dẫn tới file PDF
            var url = window.URL.createObjectURL(blob);
            // Mở PDF trong một tab hoặc cửa sổ mới
            window.open(url, '_blank');
        })
        .catch(error => {
            console.error('Error:', error);
        });
}



function getRecentDays() {
    var labels = [];
    var today = new Date();
    for (var i = -7; i <= -1; i++) {
        var date = new Date(today);
        date.setDate(today.getDate() + i);
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        if (dd < 10) {
            dd = '0' + dd;
        }
        if (mm < 10) {
            mm = '0' + mm;
        }
        var formattedDate = dd + '/' + mm;
        labels.push(formattedDate);
    }
    return labels;
}

async function fetchAsync() {
    var url = '/Dashboard/GetVehiclesByday';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
        var recentDays = getRecentDays();

        //console.log("recentDays:", recentDays);
        //console.log("vehiclesByDay:", vehiclesByDay);

        recentDays.forEach(day => {
            if (vehiclesByDay.hasOwnProperty(day)) {
                data1.push(vehiclesByDay[day]);
            } else {
                data1.push(0);
            }
        });
        return data1;
    } else {
        console.error("Không thể lấy dữ liệu từ server");
        return null;
    }
}
async function fetchAsync1() {
    var url = '/Dashboard/GetVehiclesxemay';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
        var recentDays = getRecentDays();

        //console.log("recentDays:", recentDays);
        //console.log("vehiclesByDay:", vehiclesByDay);

        recentDays.forEach(day => {
            if (vehiclesByDay.hasOwnProperty(day)) {
                data1.push(vehiclesByDay[day]);
            } else {
                data1.push(0);
            }
        });
        return data1;
    } else {
        console.error("Không thể lấy dữ liệu từ server");
        return null;
    }
}
async function fetchAsync2() {
    var url = '/Dashboard/GetVehiclesoto';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
        var recentDays = getRecentDays();

        //console.log("recentDays:", recentDays);
        //console.log("vehiclesByDay:", vehiclesByDay);

        recentDays.forEach(day => {
            if (vehiclesByDay.hasOwnProperty(day)) {
                data1.push(vehiclesByDay[day]);
            } else {
                data1.push(0);
            }
        });
        return data1;
    } else {
        console.error("Không thể lấy dữ liệu từ server");
        return null;
    }
}
async function fetchAsync3() {
    var url = '/Dashboard/GetVehiclesotovipham';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
        var recentDays = getRecentDays();

        //console.log("recentDays:", recentDays);
        //console.log("vehiclesByDay:", vehiclesByDay);

        recentDays.forEach(day => {
            if (vehiclesByDay.hasOwnProperty(day)) {
                data1.push(vehiclesByDay[day]);
            } else {
                data1.push(0);
            }
        });
        return data1;
    } else {
        console.error("Không thể lấy dữ liệu từ server");
        return null;
    }
}

$(document).ready(async function () {

    //Initialize Select2 Elements
    $('.select2').select2();
    try {
        var dt = dt1 = dt2 = dt3 = 0;
        var [dt, dt1, dt2, dt3] = await Promise.all([
            fetchAsync(),
            fetchAsync1(),
            fetchAsync2(),
            fetchAsync3()
        ]);
    } catch (error) {
        console.error('Error fetching data:', error);
    }

    // ChartJS
    var areaChartData = {
       
        labels: getRecentDays(),
        datasets: [
            {
                label: 'Số lượng xe máy',
                backgroundColor: 'rgba(60,141,188,0.9)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: dt
            },
            {
                label: 'Số lượng xe máy vi phạm',
                backgroundColor: 'rgba(0, 255, 0, 1 )',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: dt1
            },
            {
                label: 'Số lượng xe oto',
                backgroundColor: 'rgba(102, 205, 170, 1 )',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: dt2
            },
            {
                label: 'Số lượng xe oto vi phạm',
                backgroundColor: 'rgba(175, 238, 238, 1)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                pointColor: '#3b8bba',
                pointStrokeColor: 'rgba(60,141,188,1)',
                pointHighlightFill: '#fff',
                pointHighlightStroke: 'rgba(60,141,188,1)',
                data: dt3
            },
        ]
    }

    var barChartCanvas = $('#barChart1').get(0).getContext('2d')
    var barChartData = $.extend(true, {}, areaChartData)
    var temp0 = areaChartData.datasets[0]
    var temp1 = areaChartData.datasets[1]
    var temp2 = areaChartData.datasets[2]
    var temp3 = areaChartData.datasets[3]
    barChartData.datasets[0] = temp0
    barChartData.datasets[1] = temp1
    barChartData.datasets[2] = temp2
    barChartData.datasets[3] = temp3

    var barChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        datasetFill: false
    }

    new Chart(barChartCanvas, {
        type: 'bar',
        data: barChartData,
        options: barChartOptions
    })

     //Handle click event on tr of table #listVehicleDashboard
    $(document).on('click', '#xxx', function () {
        var vehicleID = $(this).attr("value"); // Get the vehicle ID from the value attribute of the clicked element
        //console.log(vehicleID);
        var url = '/Dashboard/GetVehicleDetailsById/'+ vehicleID;
        //var formData = new FormData();
        //formData.append('vehicleID', vehicleID);
        $.ajax({
            type: 'get',
            url: url,
            //data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.responseCode == 0) {
                    var result = JSON.parse(response.responseMessage);
                    
                    //console.log(result);
                    var timeArray = result.Time.split("T");
                    var speed = result.Speed;
                    if (speed > 54) {
                        $("#inputresut").val("Xe vi phạm tốc độ");
                    } else {
                        $("#inputresut").val("Xe không vi phạm tốc độ");
                    }
                    let date = new Date(timeArray[0]);
                    $("#idxe").val(result.Id);
                    $("#inputDate").val(date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
                    $("#inputTime").val(timeArray[1]);
                    $("#inputType").val(result.Type.localeCompare("-") == 0 ? "Chưa xác định" : result.Type);
                    $("#inputPlate").val(result.Plate.localeCompare("No plate") == 0 ? "Chưa xác định" : result.Plate);
                    //$("#inputPlateImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Plate_Image + '" data-image="data:image/png; base64, ' + result.Plate_Image   + '" />');
                    $("#inputPlateImage").html('<img class="img-fluid" loading="lazy" src="' + result.Plate_Image + '" />');
                    $("#inputPlateColor").val(result.Plate_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Plate_Color);
                    $("#inputSpeed").val(result.Speed);
                    $("#inputDirection").val(result.Direction == "Away" ? "DT854" : "Chợ cái tàu hạ");
                    $("#inputConfidence").val(result.Confidence.localeCompare("-") == 0 ? "Chưa xác định" : result.Confidence);
                    $("#inputVehicleType").val(result.Vehicle_Type == "Motorbike" ? "Xe máy" : "Ô tô");
                    $("#inputVehicleColor").val(result.Vehicle_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Color);
                    $("#inputVehicleBrand").val(result.Vehicle_Brand.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Brand);
                    $("#inputFullImage").html('<img class="img-fluid" loading="lazy" src="' + result.Full_Image + '" />');
                    //$("#inputFullImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Full_Image + '" data-image="data:image/png; base64, ' + result.Full_Image + '" />');
                    $("#modalVehicleDetail").modal('show'); // Show modal after populating data
                } else {
                    bootbox.alert(response.ResponseMessage);
                }
            },
            error: function (errorCallback) {
                // Handle errors
                bootbox.alert("Failed to get data!");
            }
        });
    });
});

