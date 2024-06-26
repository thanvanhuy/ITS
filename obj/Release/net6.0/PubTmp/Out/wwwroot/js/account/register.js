// Reference: https://www.daterangepicker.com/#config
$(function () {
    // Thay đổi trạng thái active của Menu
    $('#sidebarMenu .active').removeClass('active');
    $('#menuUser').addClass('active');
    $('#menuRegisterUser').addClass('active');

    // Datemask dd/mm/yyyy
    $('#dateOfBirth').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' })

    // Date Range Picker
    $('#dateOfBirth').daterangepicker({
        "locale": {
            "format": "DD/MM/YYYY",
            "separator": " - ",
            "applyLabel": "Áp dụng",
            "cancelLabel": "Hủy bỏ",
            "fromLabel": "Từ ",
            "toLabel": "Đến",
            "customRangeLabel": "Thay đổi",
            "weekLabel": "W",
            "daysOfWeek": [
                "CN",
                "T2",
                "T3",
                "T4",
                "T5",
                "T6",
                "T7"
            ],
            "monthNames": [
                "Tháng 1",
                "Tháng 2",
                "Tháng 3",
                "Tháng 4",
                "Tháng 5",
                "Tháng 6",
                "Tháng 7",
                "Tháng 8",
                "Tháng 9",
                "Tháng 10",
                "Tháng 11",
                "Tháng 12"
            ],
            "firstDay": 1
        },
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxYear: parseInt(moment().format('YYYY'), 10)
    }, function (start, end, label) {
        var years = moment().diff(start, 'years');
        alert("Tuổi hiện nay của bạn là " + years + " ?");
    });

    // Mailmask
    $('#email').inputmask({ alias: "email" })

    // Reset value of all inputs
    $("#btnReset").click(function () {
        $(":input").val("");
        $("#sex").val("male");
        $('#roleIDs').val(null).trigger('change');
    });

    // Handle uploading image event
    $("#profileImage").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
        var file = $(this).get(0).files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function () {
                $("#previewImage").attr("src", reader.result);
            }
            reader.readAsDataURL(file);
        }
    });

    //Initialize Select2 Elements
    $('#roleIDs').select2();
});