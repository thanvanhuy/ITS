
async function checkFileCount() {
    var fileInput = document.getElementById('file');
    var id = document.getElementById('idcan').value;
    var files = fileInput.files;

    if (files.length > 2) {
        alert("Chỉ được chọn tối đa 2 hình ảnh.");
        fileInput.value = null;
    } else {
        try {
            var formData = new FormData();

            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
            formData.append('id', id);
            var url = '/live/UpdateImage';

            var response = await fetch(url, {
                method: 'POST',
                body: formData
            });

            if (response.ok) {
                alert("Upload thành công.");

                fileInput.value = null;
            } else {
                alert("Không thể cập nhật.");
                fileInput.value = null;
            }
        } catch (error) {
            console.error('Error:', error);
            alert("Không thể cập nhật.");
            fileInput.value = null;
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    var showWeighElements = document.querySelectorAll(".showweigh");
    showWeighElements.forEach(function (element) {
        element.addEventListener("click", function () {
            var vehicleId = this.getAttribute("data-id");
            searchbyid(vehicleId);
        });
    });
});


document.getElementById("seachweigh").addEventListener("click", function () {
    var startDateValue = document.getElementById('start').value;
    var endDateValue = document.getElementById('end').value;

    if (!startDateValue || !endDateValue) {
        console.error("Start date and end date are required.");
        return;
    }
    var startDate = startDateValue.split(' ');
    var endDate = endDateValue.split(' ');
    if (startDate.length < 2 || endDate.length < 2) {
        console.error("Invalid date format.");
        return;
    }
    var startDateFormatted = startDate[0].split('/').reverse().join('-') + ' ' + startDate[1];
    var endDateFormatted = endDate[0].split('/').reverse().join('-') + ' ' + endDate[1];

    var plate = document.getElementById('PlateWeigh').value;
    var VehicleType = parseInt(document.getElementById("type_can").value, 10);
    var TypeViolation = parseInt(document.getElementById("TypeViolation").value, 10);
    var ViolationRatio = parseInt(document.getElementById("ViolationRatio").value, 10);

    var seachVehicles = {
        VehicleType: VehicleType,
        Plate: plate,
        TypeViolation: TypeViolation,
        starttime: startDateFormatted,
        endtime: endDateFormatted,
        ViolationRatio: ViolationRatio,
        ListStation: 0
    }
    seach(seachVehicles)
})
async function searchbyid(id) {
   
    var options = {
        method: 'POST',
    };
    var url = "/Live/SeachWeightById/"+id;
    const respons = await fetch(url, options);
   // console.log(respons)
    //
    if (respons != null) {
        var data = await respons.json();
        console.log(data)
        var timeArray = data.thoigian.split("T");
        let date = new Date(timeArray[0]);
        if (data.kieuxe.length <= 0) {
            data.kieuxe = 1;
        }
        var ketluan = "";
        if (parseFloat(data.quataitruc1) <= 0 && parseFloat(data.quataitruc2) <= 0 && parseFloat(data.quataitruc3) <= 0 && parseFloat(data.quataitong) <= 0 && parseFloat(data.quataitheogp.replace("%", "")) <= 0) {
            ketluan += `<h3 >Kết luận</h3>
                    <p>Xe không vi phạm</p>
                    `;
        } else {
            ketluan += `<h4 >Kết luận</h4>
                    `;
            if (parseFloat(data.quataitong) > 0) {
                ketluan += `<p style="color:red">Xe vượt tổng trọng lượng cho phép của cầu, đường: ${data.quataitong} %</p>`;
            }
            if (parseFloat(data.quataitheogp.replace("%", "")) > 0) {
                ketluan += `<p style="color:red">Xe vượt khối lượng hàng CC CPTGGT: ${data.quataitheogp}</p>`;
            }
            if (parseFloat(data.quataitruc1) > 0 || parseFloat(data.quataitruc2) > 0 || parseFloat(data.quataitruc3) > 0) {

                let maxQuataitruc = Math.max(parseFloat(data.quataitruc1), parseFloat(data.quataitruc2), parseFloat(data.quataitruc3));
                ketluan += `<p style="color:red">Xe vượt tải trọng trục cho phép, đường: ${maxQuataitruc}</p>`;
            }
        }
        var hinh = "/img/" + data.kieuxe + ".png";
        $("#inputDatecan").val(date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear());
        $("#inputTimecan").val(timeArray[1].substring(0,5));
        $("#inputPlatecan").val(data.biensotruoc);
        $("#inputPlatecan1").val(data.biensosau);
        $("#inputSpeedcan").val(data.tocdo);
        $("#xx").text("TLT3:"+data.tLtruc3+"kg");
        $("#xx1").text("TLT2:" + data.tLtruc2 + "kg");
        $("#xx2 ").text("TLT1:" + data.tLtruc1 + "kg");
        $("#inputDirectioncan").val(data.kieuxe > 6 ? "Container" : "Xe thân liền");
        $("#inputPlateImage").html('<img class="img-fluid" src="' + hinh + '" />');
        $("#idcan").val(data.id);
        $("#hinhtruoc").html('<img class="img-fluid" src="' + data.hinhtruoc + '" />');
        $("#hinhsau").html('<img class="img-fluid" src="' + data.hinhsau + '" />');
        $("#ketluan").html(ketluan);

        $("#modalWeighDetail").modal('show');
    }
}

async function seach(seachVehicles) { 
    var url = "/Live/SeachWeight";
    var options = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(seachVehicles), 
    }; 
    const respons = await fetch(url, options);
    var data = await respons.json();
    if (data != null) {
        var tbody = document.querySelector("#listcanxe tbody");
        if (tbody) {
            tbody.innerHTML = '';
        }
        data.forEach(vehicle => {
            var row = document.createElement('tr');
            row.style.cursor = 'grab';
            var kieuxe = vehicle.kieuxe > 6 ? "Container" : "Xe thân liền";
            var hinhxe = "";
           
            hinhxe = "/img/" + vehicle.kieuxe + ".png";

            var thoigian = new Date(vehicle.thoigian);
            var formattedDate = `${thoigian.getDate().toString().padStart(2, '0')}/${(thoigian.getMonth() + 1).toString().padStart(2, '0')}/${thoigian.getFullYear().toString().slice(2)} ${thoigian.getHours().toString().padStart(2, '0')}:${thoigian.getMinutes().toString().padStart(2, '0')}`;
            row.innerHTML = `
                    <td>KM6-CT NBLC</td>
                    <td>${formattedDate}</td>
                    <td>${vehicle.biensotruoc}</td>
                    <td>${vehicle.biensosau}</td>
                    <td>${kieuxe}</td>
                    <td>${vehicle.tocdo}</td>
                    <td>${vehicle.ttLtruc}</td>
                    <td>${vehicle.tLgiayphep}</td>
                    <td>${vehicle.tLtruc1}</td>
                    <td>${vehicle.quataitong}</td>
                    <td>${vehicle.quataitheogp}</td>
                    <td><img class="img-fluid img-thumbnail" loading="lazy" src="${hinhxe}" width="100" height="100"></td>
                    <td>001</td>
                    <td class="showweigh" data-id="${vehicle.id}">
                        <i class="fas fa-eye"></i>
                    </td>
                `;

            tbody.appendChild(row);
            var showWeighCell = row.querySelector('.showweigh');
            if (showWeighCell) {
                showWeighCell.addEventListener('click', function () {
                    var vehicleId = this.getAttribute('data-id');
                    searchbyid(vehicleId);
                });
            }
        });
    }
}
async function Createpdf() {
    try {
        var id1 = document.getElementById("idcan").value;
        var api = "/Live/Createpdf/" + id1;

        var options = {
            method: 'POST', 
        };

       
        const response = await fetch(api, options);

        if (!response.ok) {
            throw new Error('Failed to generate PDF: ' + response.status);
        }
        const blob = await response.blob();
        const url = window.URL.createObjectURL(blob);

        window.open(url, '_blank');
    } catch (error) {
        console.error('Error:', error);
    }
}
