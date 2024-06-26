$(document).ready(function () {
    // Thay đổi trạng thái active của Menu
    $('#sidebarMenu .active').removeClass('active');
    $('#menuUser').addClass('active');
    $('#menuListUser').addClass('active');

    // Thực hiện chức năng activate trạng thái của người dùng
    $('.activate-user-control').click(function (event) {
        event.preventDefault();
        var userID = this.id;
        var userName = $(this).attr('userName');
        var isActive = $(this).val();
        var thisCheckBox = $(this);
        var url = '/Account/Activate';
        var formData = new FormData();
        formData.append('userID', userID);
        if (isActive == 'false') formData.append('action', 'activate');
        else formData.append('action', 'deactivate');

        $.ajax({
            url: url,
            type: 'post',
            data: formData,
            dataType: "text",
            processData: false,
            contentType: false,
            success: function (result) {
                if (result == 'activated') {
                    thisCheckBox.prop('checked', true);
                    thisCheckBox.val('true');
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.success("Kích hoạt tài khoản " + userName + " thành công");

                } else if (result == 'deactivated') {
                    thisCheckBox.prop('checked', false);
                    thisCheckBox.val('false');
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.info("Bỏ kích hoạt tài khoản " + userName + " thành công");

                } else if (result == 'unchanged') {
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Không thay đổi được kích hoạt tài khoản");
                } else {
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Không tìm thấy tài khoản " + userName);
                }
            },
            error: function (errorCallback) {
                alert("Lỗi!!");
                //toast
                toastr.options = {
                    "positionClass": "toast-bottom-right",
                    "closeButton": true,
                }
                toastr.error("Có lỗi xảy ra trong quá trình kích hoạt tài khoản");

            }
            // End of ajax
        });
    });

    // Hiện thực chức năng tìm kiếm Users khi nhấn nút tìm kiếm
    $('#btnSearchUsers').on('click', function () {
        var userInfo = $('#inpSearchUsers').val();

        $.ajax({
            type: 'post',
            url: '/Account/SearchUsers',
            data: { 'userInfo': userInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListUsers > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Tài khoản không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Tài khoản không tồn tại trong hệ thống");
                }
            },
            error: function (errorCallback) {
                //bootbox.alert("Có lỗi xảy ra trong quá trình tìm dữ liệu");
                //toast
                toastr.options = {
                    "positionClass": "toast-bottom-right",
                    "closeButton": true,
                }
                toastr.error("Có lỗi xảy ra trong quá trình tìm dữ liệu");

            }
        });

    });

    // Hiện thực chức năng tìm kiếm Users khi Enter vào khung tìm kiếm
    // Ref: https://howtodoinjava.com/jquery/jquery-detect-if-enter-key-is-pressed/
    $('#inpSearchUsers').keypress(function (event) {
        var userInfo = $('#inpSearchUsers').val();

        $.ajax({
            type: 'post',
            url: '/Account/SearchUsers',
            data: { 'userInfo': userInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListUsers > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Tài khoản không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }                    
                    toastr.error("Tài khoản không tồn tại trong hệ thống");
                }
            },
            error: function (errorCallback) {
                //toast
                toastr.options = {
                    "positionClass": "toast-bottom-right",
                    "closeButton": true,
                }
                toastr.error("Có lỗi xảy ra trong quá trình tìm dữ liệu");
            }
            // End of ajax
        });
    });

    // Hiện thực chức năng tìm kiếm Users khi đóng khung tìm kiếm
    $('#btnCloseSearch, #btnCloseAdvancedSearch').on('click', function () {
        $('#inpSearchUsers').val('');
        var userInfo = $('#inpSearchUsers').val();

        $.ajax({
            type: 'post',
            url: '/Account/SearchUsers',
            data: { 'userInfo': userInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListUsers > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Tài khoản không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Tài khoản không tồn tại trong hệ thống");
                }
            },
            error: function (errorCallback) {
                //bootbox.alert("Có lỗi xảy ra trong quá trình tìm dữ liệu");
                //toast
                toastr.options = {
                    "positionClass": "toast-bottom-right",
                    "closeButton": true,
                }
                toastr.error("Có lỗi xảy ra trong quá trình tìm dữ liệu");
            }
        });

    });

    // Khởi tạo Datemask dd/mm/yyyy cho fromBirthday và toBirthday
    $('#inpFromBirthday').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' })
    $('#inpToBirthday').inputmask('dd/mm/yyyy', { 'placeholder': 'dd/mm/yyyy' })

    // Khởi tạo DateRangePicker cho fromBirthday và toBirthday
    // Date Range Picker
    $('#inpFromBirthday, #inpToBirthday').daterangepicker({
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
        //var years = moment().diff(start, 'years');
        //alert("Tuổi hiện nay của bạn là " + years + " ?");
    });

    //Hiện thực chức năng tìm kiếm nâng cao khi click vào nút tìm kiếm
    $('#btnAdvancedSearchUsers').on('click', function () {
        var fromDate = Date.parse($('#inpFromBirthday').val());
        var toDate = Date.parse($('#inpToBirthday').val());
        if (fromDate > toDate) {
            bootbox.alert("Ngày bắt đầu phải bé hơn ngày kết thúc. Vui lòng nhập lại!");
        } else {
            advancedSearchUsers($('#inpFromBirthday').val(), $('#inpToBirthday').val(), $('#inpIdentityNumber').val(), $('#inpPhoneNumber').val(), $('#inpAddress').val());
        }        
    });

    //Xây dựng hàm tìm kiếm nâng cao
    var advancedSearchUsers = function (fromBirthday, toBirthday, identityNumber, phoneNumber, address) {
        $.ajax({
            type: 'post',
            url: '/Account/AdvancedSearchUsers',
            data: {
                'fromBirthday': fromBirthday,
                'toBirthday': toBirthday,
                'identityNumber': identityNumber,
                'phoneNumber': phoneNumber,
                'address': address
            },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListUsers > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Tài khoản không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Tài khoản không tồn tại trong hệ thống");
                }
            },
            error: function (errorCallback) {
                //bootbox.alert("Có lỗi xảy ra trong quá trình tìm dữ liệu");
                //toast
                toastr.options = {
                    "positionClass": "toast-bottom-right",
                    "closeButton": true,
                }
                toastr.error("Có lỗi xảy ra trong quá trình tìm dữ liệu");
            }
        });
    }

    // Hiện thực chức năng điều chỉnh thông tin người dùng khi click vào nút chi tiết
    $('.btn-edit-user').on('click', function () {
        //Ex: href = "/Account/Update?userID=428579c8-8ba4-4dae-8307-a67cca10a678"
        var userId = $(this).parents('tr').find('input[type="hidden"]').val();
        window.location.href = "/Account/Update?userID=" + userId;
    });

    // Hiện thực chức năng xóa người dùng khi click vào nút xóa
    $('.btn-delete-user').on('click', function () {
        var userId = $(this).parents('tr').find('input[type="hidden"]').val();
        var fullName = $(this).parents('tr').find('.full-name').html();
        var userName = $(this).parents('tr').find('.user-name').html();
        //var userId = $(this).attr('userId');
        //alert('delete user ' + userId);

        // Reference: https://bootboxjs.com/examples.html
        bootbox.confirm({
            title: '<font style="font-weight:bold">Xóa người dùng?</font>',
            message: 'Bạn có muốn xóa tài khoản <font style="color: red; font-weight:bold">' + userName + '</font> của <font style="color: blue; font-weight:bold">' + fullName + '</font>?'
                        + ' <font style="font-weight:bold">Lưu ý</font>: Tài khoản đã xóa không thể khôi phục được!!',
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancel'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Confirm'
                }
            },
            callback: function (result) {
                if (result == true) {
                    $.post('/Account/Delete', { 'userId': userId }, function () {
                        var message = 'Xóa thành công tài khoản ' + userName;
                        //window.location.href = "/Account/Index/" + message;
                        window.location.href = "/Account/Index?ToastrMessage=" + message;
                    })
                    .fail(function () {
                        //bootbox.alert("Có lỗi xảy ra trong quá trình xóa dữ liệu");
                        //toast
                        toastr.options = {
                            "positionClass": "toast-bottom-right",
                            "closeButton": true,
                        }
                        toastr.error("Có lỗi xảy ra trong quá trình xóa dữ liệu");
                    });
                }
            }
        });
    });

    // Hiển thị thông báo Toastr Message của trang Index (nếu có)
    if ($('#inpToastrMessage').val() != '') {
        var message = $('#inpToastrMessage').val();
        var type = $('#inpToastrMessageType').val();
        toastr.options = {
            "positionClass": "toast-bottom-right",
            "closeButton": true,
        }
        switch (type) {
            case 'success': toastr.success(message); break;
            case 'error': toastr.error(message); break;
            case 'warning': toastr.warning(message); break;
            case 'info': toastr.info(message); break;
            default: toastr.success(message); break;
        }        
    }

});