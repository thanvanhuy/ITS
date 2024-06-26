$(document).ready(function () {
    // Thay đổi trạng thái active của Menu
    $('#sidebarMenu .active').removeClass('active');
    $('#menuRole').addClass('active');
    $('#menuListRole').addClass('active');

    // Hiện thực chức năng tìm kiếm Roles khi nhấn nút tìm kiếm
    $('#btnSearchRoles').on('click', function () {
        var roleInfo = $('#inpSearchRoles').val();

        $.ajax({
            type: 'post',
            url: '/Role/SearchRoles',
            data: { 'roleInfo': roleInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListRoles > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Phân quyền không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Phân quyền không tồn tại trong hệ thống");
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
            // End of ajax
        });
    });


    // Hiện thực chức năng tìm kiếm Roles khi Enter vào khung tìm kiếm
    // Ref: https://howtodoinjava.com/jquery/jquery-detect-if-enter-key-is-pressed/
    $('#inpSearchRoles').keypress(function (event) {
        var roleInfo = $('#inpSearchRoles').val();

        $.ajax({
            type: 'post',
            url: '/Role/SearchRoles',
            data: { 'roleInfo': roleInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListRoles > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Phân quyền không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Phân quyền không tồn tại trong hệ thống");
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
            // End of ajax
        });
    });


    // Hiện thực chức năng tìm kiếm Roles khi đóng khung tìm kiếm
    $('#btnCloseSearch').on('click', function () {
        $('#inpSearchRoles').val('');
        var roleInfo = $('#inpSearchRoles').val();

        $.ajax({
            type: 'post',
            url: '/Role/SearchRoles',
            data: { 'roleInfo': roleInfo },
            success: function (response) {
                if (response.responseCode == 0) $("#tblListRoles > tbody").html(response.responseMessage);
                else {
                    //bootbox.alert("Phân quyền không tồn tại trong hệ thống");
                    //toast
                    toastr.options = {
                        "positionClass": "toast-bottom-right",
                        "closeButton": true,
                    }
                    toastr.error("Phân quyền không tồn tại trong hệ thống");
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
            // End of ajax
        });
    });


    // Hiện thực chức năng điều chỉnh thông tin phân quyền khi click vào nút chi tiết
    $('.btn-edit-role').on('click', function () {
        //Ex: href = "/Role/Update?roleID=428579c8-8ba4-4dae-8307-a67cca10a678"
        var roleId = $(this).parents('tr').find('input[type="hidden"]').val();
        window.location.href = "/Role/Update?roleID=" + roleId;
    });


    // Hiện thực chức năng xóa phân quyền khi click vào nút xóa
    $('.btn-delete-role').on('click', function () {
        // Thay đổi trạng thái active của Menu xóa phân quyền
        $('#sidebarMenu .active').removeClass('active');
        $('#menuRole').addClass('active');
        $('#menuDeleteRole').addClass('active');
        var roleId = $(this).parents('tr').find('input[type="hidden"]').val();
        var roleName = $(this).parents('tr').find('.role-name').html();

        // Reference: https://bootboxjs.com/examples.html
        bootbox.confirm({
            title: '<font style="font-weight:bold">Xóa phân quyền?</font>',
            message: 'Bạn có muốn xóa phân quyền <font style="color: red; font-weight:bold">' + roleName + '</font>?'
                + ' <font style="font-weight:bold">Lưu ý</font>: Phân quyền đã xóa không thể khôi phục được!!',
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
                    $.post('/Role/Delete', { 'roleId': roleId }, function () {
                        var message = 'Xóa thành công phân quyền ' + roleName;
                        window.location.href = "/Role/Index?ToastrMessage=" + message;
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

        // Quay về trạng thái active của Menu Index
        $('#sidebarMenu .active').removeClass('active');
        $('#menuRole').addClass('active');
        $('#menuListRole').addClass('active');
    });


    // Hiện thực chức năng thêm phân quyền khi click vào nút thêm phân quyền
    $('#btnCreateRole').on('click', function () {
        // Thay đổi trạng thái active của Menu tạo phân quyền
        $('#sidebarMenu .active').removeClass('active');
        $('#menuRole').addClass('active');
        $('#menuCreateRole').addClass('active');

        // Reference: https://bootboxjs.com/examples.html
        bootbox.prompt({
            title: 'Nhập tên phân quyền mới',
            inputType: 'text',
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancel'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Confirm'
                }
            },
            callback: function (result) {
                if (result != null) {
                    $.post('/Role/Create', { 'roleName': result }, function () {
                        var message = 'Tạo thành công phân quyền ' + result;
                        //window.location.href = "/Account/Index/" + message;
                        window.location.href = "/Role/Index?ToastrMessage=" + message;
                    })
                        .fail(function () {
                            //toast
                            toastr.options = {
                                "positionClass": "toast-bottom-right",
                                "closeButton": true,
                            }
                            toastr.error("Có lỗi xảy ra trong quá trình tạo dữ liệu");
                        });
                }
            }
        });

        // Thay đổi trạng thái active của Menu Index
        $('#sidebarMenu .active').removeClass('active');
        $('#menuRole').addClass('active');
        $('#menuListRole').addClass('active');
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