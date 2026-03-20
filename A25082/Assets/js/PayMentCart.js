$(document).ready(function () {
    // Lấy danh sách tỉnh/thành phố
    $.getJSON('https://esgoo.net/api-tinhthanh/1/0.htm', function (data_tinh) {
        if (data_tinh.error == 0) {
            $.each(data_tinh.data, function (key_tinh, val_tinh) {
                $("#tinh").append('<option value="' + val_tinh.id + '">' + val_tinh.full_name + '</option>');
            });
        }
    });

    // Khi chọn tỉnh/thành phố
    $("#tinh").on("change", function () {
        var idtinh = $(this).val();
        $("#quan").empty().append('<option value="0">Chọn quận/huyện</option>');
        $("#phuong").empty().append('<option value="0">Chọn phường/xã</option>');

        if (idtinh !== "0") {
            $.getJSON('https://esgoo.net/api-tinhthanh/2/' + idtinh + '.htm', function (data_quan) {
                if (data_quan.error == 0) {
                    $.each(data_quan.data, function (key_quan, val_quan) {
                        $("#quan").append('<option value="' + val_quan.id + '">' + val_quan.full_name + '</option>');
                    });
                }
            });
        }
    });

    // Khi chọn quận/huyện
    $("#quan").on("change", function () {
        var idquan = $(this).val();
        $("#phuong").empty().append('<option value="0">Chọn phường/xã</option>');

        if (idquan !== "0") {
            $.getJSON('https://esgoo.net/api-tinhthanh/3/' + idquan + '.htm', function (data_phuong) {
                if (data_phuong.error == 0) {
                    $.each(data_phuong.data, function (key_phuong, val_phuong) {
                        $("#phuong").append('<option value="' + val_phuong.id + '">' + val_phuong.full_name + '</option>');
                    });
                }
            });
        }
    });
});
