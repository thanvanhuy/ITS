
const exportdata = async () => {
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

    var search = {
        speedsend: parseInt(document.getElementById("overWeight").value, 10),
        directionsend: parseInt(document.getElementById("direction").value, 10),
        platecolor: parseInt(document.getElementById("platecolor").value, 10),
        vehiclebrand: parseInt(document.getElementById("vehiclebrand").value, 10),
        vehiclecolor: parseInt(document.getElementById("vehiclecolor").value, 10),
        platesend: document.getElementById('LicensePlate').value,
        starttime: startDateFormatted,
        endtime: endDateFormatted,
        VehicleType: parseInt(document.getElementById("vehicleType").value, 10),
    };
    
    var api = "/Dashboard/Generatexml";
    var options = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(search)
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

document.getElementById('btnSearch').addEventListener("click", function () {
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

    var search = {
        speedsend: parseInt(document.getElementById("overWeight").value, 10),
        directionsend: parseInt(document.getElementById("direction").value, 10),
        platecolor: parseInt(document.getElementById("platecolor").value, 10),
        vehiclebrand: parseInt(document.getElementById("vehiclebrand").value, 10),
        vehiclecolor: parseInt(document.getElementById("vehiclecolor").value, 10),
        platesend: document.getElementById('LicensePlate').value ,
        starttime: startDateFormatted,
        endtime: endDateFormatted,
        VehicleType: parseInt(document.getElementById("vehicleType").value, 10),
        page: 1 // Default to the first page
    };

    var url = 'Indexfind' +
        '?page=1&pageSize=10' +
        '&speedsend=' + search.speedsend +
        '&directionsend=' + search.directionsend +
        '&platecolor=' + search.platecolor +
        '&vehiclecolor=' + search.vehiclecolor +
        '&vehiclebrand=' + search.vehiclebrand +
        '&platesend=' + search.platesend +
        '&starttime=' + search.starttime +
        '&endtime=' + search.endtime +
        '&VehicleType=' + search.VehicleType;

    window.location.href = url;
});

function generatePDF1() {
    var id1 = document.getElementById("idxe").value;
    if (id1.length <= 0) {
        alert("Vui lòng chọn xe để in báo cáo");
        return;
    }

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

$(document).ready(async function () {
   
    //Handle click event on tr of table #listVehicleDashboard
    $(document).on('click', '#xxxx', function () {
        var vehicleID = $(this).attr("value"); // Get the vehicle ID from the value attribute of the clicked element
        //console.log(vehicleID);
        var url = '/Dashboard/GetVehicleDetailsById/' + vehicleID;
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
                        $("#inputresut1").val("Vi phạm tốc độ");
                    } else {
                        $("#inputresut1").val("Không vi phạm");
                    }
                    let date = new Date(timeArray[0]);
                    $("#idxe").val(result.Id);
                    $("#inputDate1").val(date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
                    $("#inputTime1").val(timeArray[1]);
                    $("#inputType1").val(result.Type.localeCompare("-") == 0 ? "Chưa xác định" : result.Type);
                    $("#inputPlate1").val(result.Plate.localeCompare("No plate") == 0 ? "Chưa xác định" : result.Plate);
                    //$("#inputPlateImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Plate_Image + '" data-image="data:image/png; base64, ' + result.Plate_Image   + '" />');
                    $("#inputPlateImage1").html('<img class="img-fluid"  src="' + result.Plate_Image + '" />');
                    $("#inputPlateColor1").val(result.Plate_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Plate_Color);
                    $("#inputSpeed1").val(result.Speed);
                    $("#inputDirection1").val(result.Direction == "Away" ? "DT854" : "Chợ cái tàu hạ");
                    $("#inputConfidence1").val(result.Confidence.localeCompare("-") == 0 ? "Chưa xác định" : result.Confidence);
                    $("#inputVehicleType1").val(result.Vehicle_Type == "Motorbike" ? "Xe máy" : "Ô tô");
                    $("#inputVehicleColor1").val(result.Vehicle_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Color);
                    $("#inputVehicleBrand1").val(result.Vehicle_Brand.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Brand);
                    $("#inputFullImage1").html('<img class="img-fluid" src="' + result.Full_Image + '" />');
                    //$("#inputFullImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Full_Image + '" data-image="data:image/png; base64, ' + result.Full_Image + '" />');
                    /*$("#modalVehicleDetail").modal('show'); */// Show modal after populating data
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
