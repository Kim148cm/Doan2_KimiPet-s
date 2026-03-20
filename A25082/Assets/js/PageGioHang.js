
$(document).ready(function () {
    $(".remove-from-cart").click(function () {
        var button = $(this);
        var productId = button.data("productid");
        var color = button.data("color");
        var size = button.data("size");

        $.ajax({
            url: '@Url.Action("ViewCart", "GioHang")',
            type: 'POST',
            data: { productId: productId, color: color, size: size },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    location.reload(); // Tải lại trang sau khi xóa
                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Đã xảy ra lỗi! Vui lòng thử lại.");
            }
        });
    });
});