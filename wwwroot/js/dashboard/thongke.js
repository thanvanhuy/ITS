
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
function formatDate(isoDateString) {
    
    const date = new Date(isoDateString);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); 
    return `${day}/${month}`;
}
async function fetchAsync(recentDays) {
    var url = '/Dashboard/GetVehiclesByday';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
       

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
async function fetchAsync1(recentDays) {
    var url = '/Dashboard/GetVehiclesxemay';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
        

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
async function fetchAsync2(recentDays) {
    var url = '/Dashboard/GetVehiclesoto';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];

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
async function fetchAsync3(recentDays) {
    var url = '/Dashboard/GetVehiclesotovipham';
    let response = await fetch(url);
    let data = await response.json();
    if (data && data.responseCode === 0) {
        let vehiclesByDay = JSON.parse(data.responseMessage);
        var data1 = [];
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
    var recentDays = getRecentDays();
    //Initialize Select2 Elements
    $('.select2').select2();

    try {
        // Fetch data
        var [dt, dt1, dt2, dt3,dt4] = await Promise.all([
            fetchAsync(recentDays),
            fetchAsync1(recentDays),
            fetchAsync2(recentDays),
            fetchAsync3(recentDays),
            fetchAsync4()
        ]);
        
    } catch (error) {
        console.error('Error fetching data:', error);
    }
    const totalDt = dt.reduce((sum, value) => sum + value, 0);
    const totalDt1 = dt1.reduce((sum, value) => sum + value, 0);
    const totalDt2 = dt2.reduce((sum, value) => sum + value, 0);
    const totalDt3 = dt3.reduce((sum, value) => sum + value, 0);
    
    //add data to table

    var tablexevipham = document.querySelector('#tablexevipham tbody');
    var tableloaixe = document.querySelector('#tableloaixe tbody');
    var tablevitri = document.querySelector('#tablevitri tbody');
    var tablehangxe = document.querySelector('#tablehangxe tbody');
    
    
    tablexevipham.innerHTML = ''; // Xóa nội dung hiện tại
    tableloaixe.innerHTML = ''; // Xóa nội dung hiện tại
    tablevitri.innerHTML = ''; // Xóa nội dung hiện tại
    tablehangxe.innerHTML = ''; // Xóa nội dung hiện tại
   
    for (var i = 0; i < dt4.recentDays.length; i++) {

        var row = document.createElement('tr');
        row.innerHTML = `
        
                    <td>${formatDate(dt4.recentDays[i])}</td>
                    <td>${dt4.arraysByVehicleType[0][i]}</td>
                    <td>${dt4.arraysByVehicleType[1][i]}</td>
                    <td>${dt4.arraysByVehicleType[2][i]}</td>
                    <td>${dt4.arraysByVehicleType[3][i]}</td>
                    <td>${dt4.arraysByVehicleType[4][i]}</td>
                    <td>${dt4.arraysByVehicleType[5][i]}</td>
                    <td>${dt4.arraysByVehicleType[6][i]}</td>
                    <td>${dt4.arraysByVehicleType[7][i]}</td>
                `;
        tableloaixe.appendChild(row);
    }
    //dt4.arraysByVehicleType.forEach(function (item, index) {
    //    console.log(item)

    //    var row = document.createElement('tr');
    //    row.innerHTML = `
    //                <td>${recentDays[index]}</td>
    //                <td>${item[0]}</td>
    //                <td>${item[1]}</td>
    //                <td>${item[2]}</td>
    //                <td>${item[3]}</td>
    //                <td>${item[4]}</td>
    //                <td>${item[5]}</td>
    //                <td>${item[6]}</td>
    //                <td>${item[7]}</td>
    //            `;
    //    tableloaixe.appendChild(row);
    //});
    dt.forEach(function (item, index) {
        
        var row = document.createElement('tr');
        row.innerHTML = `
                    <td>${recentDays[index]}</td>
                    <td>${item}</td>
                    <td>${dt1[index]}</td>
                    <td>${dt2[index]}</td>
                    <td>${dt3[index]}</td>
                   
                `;
        tablexevipham.appendChild(row);
    });
    // hết

    //console.log(dt4.arraysByVehicleType[0])
    // ChartJS
    var areaChartData = {
        labels: recentDays,
        datasets: [
            {
                label: 'Số lượng xe máy 7 ngày:' + totalDt,
                backgroundColor: 'rgba(60,141,188,0.9)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt
            },
            {
                label: 'Số lượng xe máy vi phạm 7 ngày:' + totalDt1,
                backgroundColor: 'rgba(0, 255, 0, 1)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt1
            },
            {
                label: 'Số lượng xe oto 7 ngày:' + totalDt2,
                backgroundColor: 'rgba(102, 205, 170, 1)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt2
            },
            {
                label: 'Số lượng xe oto vi phạm 7 ngày:' + totalDt3,
                backgroundColor: 'rgba(175, 238, 238, 1)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt3
            },
        ]
    };
   
    var areaChartData1 = {
        labels: recentDays,
        datasets: [
            {
                label: 'Số lượng xe Bus',
                backgroundColor: 'rgb(255, 0, 0)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[0]
            },
            {
                label: 'Số lượng xe Car',
                backgroundColor: 'rgb(255, 99, 71)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[1]
            },
            {
                label: 'Số lượng xe Motorbike',
                backgroundColor: 'rgb(238, 130, 238)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[2]
            },
            {
                label: 'Số lượng xe CXĐ',
                backgroundColor: 'rgba(255, 99, 71, 0.8)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[3]
            },
            {
                label: 'Số lượng xe SUV',
                backgroundColor: 'rgba(255, 231, 71, 0.5)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[4]
            },
            {
                label: 'Số lượng xe Truck',
                backgroundColor: 'rgba(255, 231, 149, 0.5)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[5]
            },
            {
                label: 'Số lượng xe Van',
                backgroundColor: 'rgba(60,141,188,0.9)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[6]
            },
            {
                label: 'Số lượng xe Bicycle',
                backgroundColor: 'rgba(255, 231, 149, 1)',
                borderColor: 'rgba(60,141,188,0.8)',
                pointRadius: false,
                data: dt4.arraysByVehicleType[7]
            },
        ]
    };
    var barChartCanvas1 = $('#barChart1').get(0).getContext('2d');
    var barChartCanvas2 = $('#barChart2').get(0).getContext('2d');

    var barChartData1 = $.extend(true, {}, areaChartData);
    var barChartData2 = $.extend(true, {}, areaChartData1);

    var barChartOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                display: true
            }
        },
        scales: {
            x: {
                beginAtZero: true
            },
            y: {
                beginAtZero: true
            }
        }
    };


    new Chart(barChartCanvas1, {
        type: 'bar',
        data: barChartData1,
        options: barChartOptions
    });

    new Chart(barChartCanvas2, {
        type: 'bar',
        data: barChartData2,
        options: barChartOptions
    });



    // biểu đồ thứ 3
    var donutChartCanvas = $('#donutChart').get(0).getContext('2d')
    var donutChartCanvas1 = $('#donutChart1').get(0).getContext('2d')
    var donutData = {
        labels: [
            'Speed CAM',
            'VL-NVVOI',
            'CTHa-DT854',
            'DT854-CTHa',
            '237PVC',
        ],
        datasets: [
            {
                data: [700, 500, 400, 600, 300],
                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc'],
            }
        ]
    }
    var donutData1 = {
        labels: [
            'Audi',
            'Land Rover',
            'Honda',
            'Nissan',
            'Toyota',
        ],
        datasets: [
            {
                data: [900, 530, 400, 1000, 1020],
                backgroundColor: ['#f56954', '#00a65a', '#f39c12', '#00c0ef', '#3c8dbc'],
            }
        ]
    }
    var donutOptions = {
        maintainAspectRatio: false,
        responsive: true,
        legend: {
            display: true,
            position: "left"
        }
    }
    //Create pie or douhnut chart
    // You can switch between pie and douhnut using the method below.
    new Chart(donutChartCanvas, {
        type: 'doughnut',
        data: donutData,
        options: donutOptions
    })
    new Chart(donutChartCanvas1, {
        type: 'doughnut',
        data: donutData1,
        options: donutOptions
    })

});

