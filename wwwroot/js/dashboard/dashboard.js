function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    return `${day}-${month}-${year} ${hours}:${minutes}:${seconds}`;
}

function updateTime() {
    const now = new Date();
    const formattedTime = formatDate(now);
    document.getElementById('current-time').innerText = `Dữ liệu trực tuyến phương tiện ngày: ${formattedTime}`;
}
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


async function fetchAsync4() {
    try {
        const url = '/Dashboard/GetVehiclesType';

        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Network response was not ok. Status: ${response.status}`);
        }

        const data = await response.json();
       
       
        var datachar1 = document.getElementById("databarChart2");
        const recentDays = Array.from(new Set(data.map(item => item.date))).sort();
        const vehicleTypes = Array.from(new Set(data.map(item => item.vehicleTypeCombined)));

        // Initialize the result object
        const result = vehicleTypes.reduce((acc, type) => {
            acc[type] = recentDays.map(day => ({ Date: day, Count: 0 }));
            return acc;
        }, {});

        // Process the data to fill the result object
        data.forEach(item => {
            if (vehicleTypes.includes(item.vehicleTypeCombined)) {
                const dayIndex = recentDays.indexOf(item.date);
                if (dayIndex !== -1) {
                    result[item.vehicleTypeCombined][dayIndex].Count = item.vehicleCount;
                }
            }
        });

        // Convert result object to arrays for each vehicle type
        const arraysByVehicleType = vehicleTypes.map(type => {
            return result[type].map(dayData => dayData.Count);
        });

        return {
            recentDays,
            vehicleTypes,
            arraysByVehicleType
        };
    } catch (error) {
        console.error("Error updating chart data:", error);
    }
}

$(document).ready(async function () {
    // Cập nhật thời gian ngay khi trang tải xong
    updateTime();

    // Cập nhật thời gian mỗi giây
    setInterval(updateTime, 1000);
     //Handle click event on tr of table #listVehicleDashboard
    //$(document).on('click', '#xxx', function () {
    //    var vehicleID = $(this).attr("value"); // Get the vehicle ID from the value attribute of the clicked element
    //    //console.log(vehicleID);
    //    var url = '/Dashboard/GetVehicleDetailsById/'+ vehicleID;
    //    //var formData = new FormData();
    //    //formData.append('vehicleID', vehicleID);
    //    $.ajax({
    //        type: 'get',
    //        url: url,
    //        //data: formData,
    //        processData: false,
    //        contentType: false,
    //        success: function (response) {
    //            if (response.responseCode == 0) {
    //                var result = JSON.parse(response.responseMessage);
                    
    //                //console.log(result);
    //                var timeArray = result.Time.split("T");
    //                var speed = result.Speed;
    //                if (speed > 54) {
    //                    $("#inputresut").val("Vi phạm tốc độ");
    //                } else {
    //                    $("#inputresut").val("Không vi phạm");
    //                }
    //                let date = new Date(timeArray[0]);
    //                $("#idxe").val(result.Id);
    //                $("#inputDate").val(date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
    //                $("#inputTime").val(timeArray[1]);
    //                $("#inputType").val(result.Type.localeCompare("-") == 0 ? "Chưa xác định" : result.Type);
    //                $("#inputPlate").val(result.Plate.localeCompare("No plate") == 0 ? "Chưa xác định" : result.Plate);
    //                //$("#inputPlateImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Plate_Image + '" data-image="data:image/png; base64, ' + result.Plate_Image   + '" />');
    //                $("#inputPlateImage").html('<img class="img-fluid"  src="' + result.Plate_Image + '" />');
    //                $("#inputPlateColor").val(result.Plate_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Plate_Color);
    //                $("#inputSpeed").val(result.Speed);
    //                $("#inputDirection").val(result.Direction == "Away" ? "DT854" : "Chợ cái tàu hạ");
    //                $("#inputConfidence").val(result.Confidence.localeCompare("-") == 0 ? "Chưa xác định" : result.Confidence);
    //                $("#inputVehicleType").val(result.Vehicle_Type == "Motorbike" ? "Xe máy" : "Ô tô");
    //                $("#inputVehicleColor").val(result.Vehicle_Color.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Color);
    //                $("#inputVehicleBrand").val(result.Vehicle_Brand.localeCompare("-") == 0 ? "Chưa xác định" : result.Vehicle_Brand);
    //                $("#inputFullImage").html('<img class="img-fluid" src="' + result.Full_Image + '" />');
    //                //$("#inputFullImage").html('<img class="img-fluid" src="data: image/png;base64, ' + result.Full_Image + '" data-image="data:image/png; base64, ' + result.Full_Image + '" />');
    //                /*$("#modalVehicleDetail").modal('show'); */// Show modal after populating data
    //            } else {
    //                bootbox.alert(response.ResponseMessage);
    //            }
    //        },
    //        error: function (errorCallback) {
    //            // Handle errors
    //            bootbox.alert("Failed to get data!");
    //        }
    //    });
    //});
});

