
// Code xóa sản phẩm trong giỏ hàng ---------------------------------------------------------------------------------------

$(document).ready(function () {
    $(".btn-remove").click(function () {
        var productId = $(this).data("productid");
        var color = $(this).data("color");
        var size = $(this).data("size");

        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Sản phẩm sẽ bị xóa khỏi giỏ hàng!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#d33",
            cancelButtonColor: "#3085d6",
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '@Url.Action("RemoveFromCart", "GioHang")',
                    type: 'POST',
                    data: { productId: productId, color: color, size: size },
                    success: function (response) {
                        if (response.success) {
                            Swal.fire({
                                icon: "success",
                                title: "Xóa thành công!",
                                text: response.message,
                                showConfirmButton: false,
                                timer: 2000
                            }).then(() => {
                                location.reload(); // Reload trang để cập nhật giỏ hàng
                            });
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "Lỗi!",
                                text: response.message
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: "error",
                            title: "Lỗi!",
                            text: "Đã xảy ra lỗi khi xóa sản phẩm!"
                        });
                    }
                });
            }
        });
    });
});



// Code thực hiện giảm giá khi sử dụng voucher -----------------------------------------------------------------------------------

$(document).ready(function () {
    $("#applyVoucher").click(function () {
        var voucherCode = $("#voucherCode").val().trim();

        if (voucherCode === "") {
            alert("Vui lòng nhập mã voucher!");
            return;
        }

        $.ajax({
            url: '@Url.Action("ApplyVoucher", "GioHang")',
            type: 'POST',
            data: { voucherCode: voucherCode },
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        icon: "success",
                        title: "Voucher áp dụng thành công!",
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 500,
                        timerProgressBar: true
                    }).then(() => {
                        location.reload(); // Reload lại trang sau khi hiển thị thông báo
                    });
                    $("#voucherMessage").text(`Voucher đang sử dụng: ${selectedVoucher}`).show();

                } else {
                    alert(response.message);
                }
            },
            error: function () {
                alert("Lỗi khi áp dụng voucher!");
            }
        });
    });

    //-----------------------------------------------------------------------------------------------------------------------------------



});

$(document).ready(function () {
    $(".btn-increase, .btn-decrease").click(function () {
        var isIncrease = $(this).hasClass("btn-increase");
        var productId = $(this).data("productid");
        var color = $(this).data("color");
        var size = $(this).data("size");

        $.ajax({
            url: '@Url.Action("UpdateQuantity", "GioHang")',
            type: 'POST',
            data: { productId: productId, color: color, size: size, increase: isIncrease },
            success: function (response) {
                if (response.success) {
                    location.reload();
                } else {
                    alert(response.message); // Hiển thị thông báo lỗi khi không thể giảm số lượng
                }
            },
            error: function () {
                alert("Lỗi khi cập nhật số lượng!");
            }
        });
    });
});


// Code chỉnh sửa thông tin sản phẩm trong giỏ hàng --------------------------------------------------------------------------------------

const Toast = Swal.mixin({
    toast: true,
    position: "top-end",
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.onmouseenter = Swal.stopTimer;
        toast.onmouseleave = Swal.resumeTimer;
    }
});

$(document).on("click", ".edit-item", function () {
    var productId = $(this).data("productid");
    $("#editProductId").val(productId);

    $.get("/GioHang/GetProductOptions", { productId: productId }, function (data) {
        var colorContainer = $("#editColorOptions").empty();
        var sizeContainer = $("#editSizeOptions").empty();

        // Cập nhật ảnh sản phẩm
        $("#editProductImage").attr("src", data.imageUrl.length ? data.imageUrl[0] : "/images/default.jpg");

        // Cập nhật giá sản phẩm
        $("#editProductPrice").text(data.price.toLocaleString() + " đ");
        $("#editDisPrice").text(data.disPrice.toLocaleString() + " đ");

        // Thêm màu sắc
        data.colors.forEach(color => {
            colorContainer.append(`
                <button type="button" class="btn btn-outline-primary color-option" data-value="${color}">${color}</button>
            `);
        });

        // Thêm kích thước
        data.sizes.forEach(size => {
            sizeContainer.append(`
                <button type="button" class="btn btn-outline-secondary size-option" data-value="${size}">${size}</button>
            `);
        });

        // Khi chọn màu
        $(".color-option").click(function () {
            $(".color-option").removeClass("active");
            $(this).addClass("active");

            let selectedColor = $(this).data("value");

            // Hiển thị thông báo SweetAlert khi chọn màu
            Toast.fire({
                icon: "success",
                title: `Bạn đã chọn màu: ${selectedColor}`
            });
        });

        // Khi chọn kích thước
        $(".size-option").click(function () {
            $(".size-option").removeClass("active");
            $(this).addClass("active");

            let selectedSize = $(this).data("value");

            // Hiển thị thông báo SweetAlert khi chọn kích thước
            Toast.fire({
                icon: "success",
                title: `Bạn đã chọn kích thước: ${selectedSize}`
            });
        });
    });

    $("#editModal").modal("show");
});

// Xử lý khi nhấn nút lưu thay đổi
$("#saveChanges").click(function () {
    var productId = $("#editProductId").val();
    var newColor = $(".color-option.active").data("value");
    var newSize = $(".size-option.active").data("value");

    if (!newColor || !newSize) {
        Swal.fire({
            icon: "warning",
            title: "Vui lòng chọn đầy đủ màu sắc và kích thước!",
            toast: true,
            position: "top-end",
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
        return;
    }

    $.post("/GioHang/UpdateCartItem", { productId: productId, color: newColor, size: newSize }, function (response) {
        if (response.success) {
            Swal.fire({
                icon: "success",
                title: "Cập nhật thành công!",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 500,
                timerProgressBar: true
            }).then(() => {
                location.reload();
            });
        } else {
            Swal.fire({
                icon: "error",
                title: "Cập nhật thất bại!",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
    });
});


// ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------


function updateCartTotalQuantity() {
    $.ajax({
        url: '/GioHang/GetCartTotalQuantity',
        type: 'GET',
        success: function (response) {
            $('#cartTotalQuantity').text(response.totalQuantity);
        }
    });
}

// Gọi hàm sau khi thêm vào giỏ hàng
$(".btn-add-to-cart").click(function () {
    updateCartTotalQuantity();
});



document.addEventListener("DOMContentLoaded", function () {
    var openModalBtn = document.getElementById("openVoucherModals");
    var voucherModal = new bootstrap.Modal(document.getElementById("voucherModal"));

    openModalBtn.addEventListener("click", function () {
        loadVouchers();
        voucherModal.show();
    });

    function loadVouchers() {
        $.ajax({
            url: "/GioHang/GetVouchers",
            type: "GET",
            dataType: "json",
            success: function (data) {
                var voucherList = $("#voucherList");
                voucherList.empty();

                if (data.length === 0) {
                    voucherList.html("<p class='text-muted'>Không có voucher khả dụng</p>");
                    return;
                }

                data.forEach(function (voucher) {
                    var formattedDiscountValue = voucher.DiscountType === "Percentage"
                        ? voucher.DiscountValue + "%"
                        : Number(voucher.DiscountValue).toLocaleString("vi-VN", { style: "currency", currency: "VND" });

                    var formattedMinOrderAmount = Number(voucher.MinOrderAmount).toLocaleString("vi-VN", { style: "currency", currency: "VND" });

                    var voucherHtml = `
                        <div class="voucher-item d-flex align-items-center">
                            <img src="${voucher.ImageVoucher}" alt="Voucher" class="voucher-icon">
                            <div class="ms-3 flex-grow-1">
                                <p class="mb-1">Mã: <strong>${voucher.Code}</strong></p>
                                <small>Giảm: ${formattedDiscountValue}</small>
                                <br>
                                <small>Đơn tối thiểu: ${formattedMinOrderAmount}</small>
                            </div>
                            <input type="radio" name="voucher" class="form-check-input" value="${voucher.Code}">
                        </div>
                    `;
                    voucherList.append(voucherHtml);
                });
            },
            error: function () {
                alert("Lỗi khi tải danh sách voucher!");
            }
        });
    }

    // Xác nhận áp dụng voucher
    $("#confirmVoucher").click(function () {
        var selectedVoucher = $("input[name='voucher']:checked").val();
        if (!selectedVoucher) {
            Swal.fire({
                icon: "warning",
                title: "Vui lòng chọn một voucher!",
                toast: true,
                position: "top-end",
                showConfirmButton: false,
                timer: 1000,
                timerProgressBar: true
            });
            return;
        }

        $.ajax({
            url: "/GioHang/ApplyVoucher",
            type: "POST",
            data: { voucherCode: selectedVoucher },
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    $("#voucherCode").val(response.voucherCode);
                    $("#voucherDiscount").text(`- ${response.discountAmount.toLocaleString("vi-VN")}₫`);
                    $("#tongThanhToan").text(`${response.newTotal.toLocaleString("vi-VN")}₫`);
                    $("#voucherMessage").text(`Voucher đang sử dụng: ${response.voucherCode}`).show(); // Hiển thị thông tin voucher
                    $("#cancelVoucher").show(); // Hiển thị nút Hủy Voucher
                    voucherModal.hide();

                    Swal.fire({
                        icon: "success",
                        title: "Áp dụng voucher thành công!",
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 1000,
                        timerProgressBar: true
                    }).then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: response.message,
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 1000,
                        timerProgressBar: true
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi khi áp dụng voucher!",
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 1000,
                    timerProgressBar: true
                });
            }
        });
    });

    // Hủy voucher
    $("#cancelVoucher").click(function () {
        $.ajax({
            url: "/GioHang/CancelVoucher",
            type: "POST",
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    $("#voucherDiscount").text("- 0₫");
                    $("#tongThanhToan").text(`${response.newTotal.toLocaleString("vi-VN")}₫`);
                    $("#voucherCode").val("");
                    $("#voucherMessage").text("").hide(); // Ẩn thông tin voucher
                    $("#cancelVoucher").hide(); // Ẩn nút Hủy Voucher

                    Swal.fire({
                        icon: "success",
                        title: "Voucher đã được hủy!",
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 1000,
                        timerProgressBar: true
                    }).then(() => {
                        location.reload();
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: response.message,
                        toast: true,
                        position: "top-end",
                        showConfirmButton: false,
                        timer: 1000,
                        timerProgressBar: true
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi khi hủy voucher!",
                    toast: true,
                    position: "top-end",
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                });
            }
        });
    });
});
