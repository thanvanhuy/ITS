"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dashboardHub").build();
var warnedVehicles = [];
$(function () {
    startConnection();
});

function startConnection() {
    connection.start().then(function () {
        
        InvokeVehicles();
    }).catch(function (err) {
        console.error(err.toString());
    });
}

// Update new vehicles
function InvokeVehicles() {
    if (connection.state === "Connected") {
        connection.invoke("SendVehicles").catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection();
    }
}
// Update new vehicles when seach
function InvokeVehiclesseach(search) {
    if (connection.state === "Connected") {
        connection.invoke("SendVehicles1",search).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection();
    }
}
// Update new vehicles with filtered locations
function InvokeFilterVehicleByLocation(locations) {
    connection.invoke("FilterVehicleByLocation", locations).catch(function (err) {
        return console.error(err.toString());
    });
}
function InvokeFilterVehicleByPlate(plate) {
    if (connection.state === "Connected") {
        connection.invoke("FilterVehicleByPlate", plate).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection(); 
    }
}
function InvokeFilterVehicleByTime(starttime, endtime) {
    if (connection.state === "Connected") {
        connection.invoke("FilterVehicleByDate", starttime,endtime).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection(); 
    }
}

function InvokeFilterVehicleBySpeed(speed) {
    if (connection.state === "Connected") {
        connection.invoke("FilterVehicleBySpeed", speed).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection(); 
    }
}

function InvokeFilterVehicleByvehicleType(vehicleType) {
    if (connection.state === "Connected") {
        connection.invoke("FilterVehicleByvehicleType", vehicleType).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection();
    }
}
function InvokeFilterVehicleByDirection(direction) {
    if (connection.state === "Connected") {
        connection.invoke("FilterVehicleByDirection", direction).catch(function (err) {
            console.error(err.toString());
        });
    } else {
        console.error("Connection is not in the 'Connected' state.");
        startConnection();
    }
}


try {
    connection.on("ReceivedVehicles", function (vehicles) {
        ShowNewVehiclesToPage(vehicles);
    });
} catch (error) {
    console.error("Error handling received vehicles:", error);
}

function ShowNewVehiclesToPage(vehicles) {
    $("#listVehicleDashboard tbody").empty();
   // console.log("Danh sách các phần tử trong warnedVehicles:", warnedVehicles);
    var tr;   
    $.each(vehicles, function (index, vehicle) {
        //console.log(vehicle);
        tr = $('<tr/>');
        //console.log(vehicle.plate_Image);
        var speed = vehicle.speed;
        if (speed > 54) {
            if (warnedVehicles.indexOf(vehicle.id) === -1 ){
                warnedVehicles.push(vehicle.id);
                var warningAudio = document.getElementById('warningAudio');
                if (warningAudio.paused) {
                    warningAudio.play();
                   // console.log("đã phát rồi nha");
                }
            }
            tr.css('background-color', 'red');
            if (speed < 60) {
                vehicle.speed = vehicle.speed + " => Xe vi phạm tốc độ 5 - <10km/h";
            }
            if (speed >= 60 && speed < 70) {
                vehicle.speed = vehicle.speed + " => Xe vi phạm tốc độ 10 - <20km/h";
            }
            if (speed >= 70 && speed < 80) {
                vehicle.speed = vehicle.speed + " => Xe vi phạm tốc độ 20 - <35km/h";
            }
            if (speed >= 80) {
                vehicle.speed = vehicle.speed + " => Xe vi phạm tốc độ >35km/h";
            }
        } else if (speed > 40) {
            vehicle.speed = vehicle.speed + " => Xe không vi phạm";
            if (speed > 50) {
                tr.css('background-color', 'yellow');
            }
        } else {
            if (warnedVehicles.length > 50) {
                warnedVehicles.splice(0, warnedVehicles.length); // Xóa hết phần tử
            }
            vehicle.speed = vehicle.speed + " => Xe không vi phạm";
            tr.css('text-align', 'center');
        }
        //console.log(vehicle);
        //tr.append('<td>${(index+1)}</td>');
        tr.append('<input type="hidden" value="' + vehicle.id + '"/>');
        tr.append('<td>' + vehicle.device + '</td>');
        tr.append('<td>' + vehicle.time.replace("T"," ") + '</td>');
        tr.append('<td>' + vehicle.plate + '</td>');
        tr.append('<td>' + vehicle.type + '</td>');
        tr.append('<td>' + vehicle.plate_Color + '</td>');
        tr.append('<td>' + vehicle.speed + '</td>');
        tr.append('<td>' + vehicle.direction + '</td>');
        tr.append('<td>' + vehicle.vehicle_Type + '</td>');
        tr.append('<td>' + vehicle.vehicle_Color + '</td>');
        tr.append('<td>' + vehicle.vehicle_Brand + '</td>');

        //tr.append('<td><img class="img-fluid img-thumbnail" src="data: image/png;base64, ' + vehicle.plate_Image + '" data-image="data:image/png; base64, ' + vehicle.plate_image + '" width="100" height="100"></td>');
        tr.append('<td><img class="img-fluid img-thumbnail" src="' + vehicle.plate_Image + '" width="100" height="100"></td>');
        tr.append('<td><img class="img-fluid img-thumbnail" src="' + vehicle.full_Image + '" width="100" height="100"></td>');
        //tr.append('<td><img class="img-fluid img-thumbnail" src="data: image/png;base64, ' + vehicle.full_Image + '" data-image="data:image/png; base64, ' + vehicle.full_image + '" width="100" height="100"></td>');
       



        tr.append('<td id="xxx" value="' + vehicle.id + '"><i class="fas fa-eye"></i></a></td>');
        //<a href="/Dashboard/GetVehicleDetailsById/' + vehicle.id + '">
        $("#listVehicleDashboard").append(tr);
    });
}
