// Đảm bảo DOM đã sẵn sàng trước khi khởi tạo Swiper
document.addEventListener("DOMContentLoaded", function () {
    // Khởi tạo Swiper
    const swiper_product = new Swiper('.product-slider', {
        slidesPerView: 1, // Số slide hiển thị mặc định
        spaceBetween: 20, // Khoảng cách giữa các slide
        loop: true, // Lặp vô tận
        autoplay: {
            delay: 1000, // Tự động chuyển sau 1 giây
            disableOnInteraction: false, // Không tắt autoplay khi tương tác
        },
        navigation: {
            nextEl: '.swiper-button-next', // Nút điều hướng kế tiếp
            prevEl: '.swiper-button-prev', // Nút điều hướng quay lại
        },
        breakpoints: {
            // Cấu hình hiển thị theo từng độ rộng màn hình
            350: {
                slidesPerView: 2,
            },
            480: {
                slidesPerView: 2,
            },
            570: {
                slidesPerView: 2,
            },
            640: {
                slidesPerView: 2,
            },
            768: {
                slidesPerView: 3,
            },
            1024: {
                slidesPerView: 4,
            },
            1400: {
                slidesPerView: 5,
            },
        },
    });
});
//-------------------------------------------------------------------------------------------------
document.addEventListener("DOMContentLoaded", function () {
    const colorOptions = document.querySelectorAll('.color-option');

    colorOptions.forEach(option => {
        option.addEventListener('mouseover', function () {
            const productCard = option.closest('.product-card');
            const primaryImage = productCard.querySelector('.primary-image');

            // Lấy ảnh mới khi di chuột vào màu
            const newImageUrl = option.getAttribute('data-image');

            // Thay đổi ảnh chính khi di chuột vào màu
            primaryImage.src = newImageUrl;
        });

        // Khôi phục lại ảnh ban đầu khi di chuột ra ngoài
        option.addEventListener('mouseout', function () {
            const productCard = option.closest('.product-card');
            const primaryImage = productCard.querySelector('.primary-image');

            // Khôi phục lại ảnh ban đầu
            const originalImageUrl = productCard.querySelector('.primary-image').getAttribute('src');

            // Đặt lại ảnh chính về ảnh ban đầu
            primaryImage.src = originalImageUrl;
        });
    });
});



// Code xử lí khi thêm sản phẩm vào giỏ hàng ---------------------------------------------------------------------------------------------
$(document).ready(function () {
    // Xử lý chọn màu (cho cả desktop và mobile)
    $(".color-option").click(function () {
        $(".color-option").removeClass("selected");
        $(this).addClass("selected");
        var selectedColor = $(this).data("color");
        $("#selectedColor").val(selectedColor);
        $("#mobileSelectedColor").val(selectedColor);
    });

    // Xử lý chọn kích thước (cho cả desktop và mobile)
    $(".size-option").click(function () {
        $(".size-option").removeClass("selected");
        $(this).addClass("selected");
        var selectedSize = $(this).data("size");
        $("#selectedSize").val(selectedSize);
        $("#mobileSelectedSize").val(selectedSize);
    });

    // Xử lý thêm giỏ hàng (cho cả desktop và mobile)
    $(".btn_add_left, .btn_buy_left").click(function () {
        // Lấy ID từ desktop hoặc mobile
        var productId = $("#productId").val() || $("#mobileProductId").val();
        var color = $("#selectedColor").val() || $("#mobileSelectedColor").val() || "Không chọn";
        var size = $("#selectedSize").val() || $("#mobileSelectedSize").val() || "Không chọn";

        // Bỏ kiểm tra bắt buộc chọn màu và kích thước
        // Người dùng có thể thêm sản phẩm mà không cần chọn màu/kích thước

        $.ajax({
            url: "/GioHang/AddToCart",
            type: "POST",
            data: {
                productId: productId,
                color: color,
                size: size,
                quantity: 1
            },
            dataType: "json",
            success: function (response) {
                if (response.success) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: response.message,  // 💡 Dùng response.message thay vì biến chưa định nghĩa
                        showConfirmButton: false,
                        timer: 2500
                    }).then(() => {
                        window.location.href = "/GioHang/Index";
                    });

                    // Cập nhật giỏ hàng trên giao diện nếu cần
                    $("#cartSidebar").html(response.cartHtml);
                    $("#totalItems").text(response.totalItems);
                    $("#totalPrice").text(response.totalPrice);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi!",
                        text: response.message,
                        confirmButtonText: "OK"
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Lỗi hệ thống!",
                    text: "Có lỗi xảy ra, vui lòng thử lại sau.",
                    confirmButtonText: "OK"
                });
            }
        });
    });


});


