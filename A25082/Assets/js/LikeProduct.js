



// Đảm bảo DOM đã sẵn sàng trước khi khởi tạo Swiper
document.addEventListener("DOMContentLoaded", function () {
    // Khởi tạo Swiper
    const swiper_product_new = new Swiper('.product-slider', {
        slidesPerView: 1,
        spaceBetween: 20,
        loop: true,
        autoplay: {
            delay: 1000,
            disableOnInteraction: false,
        },
        navigation: {
            nextEl: '.swiper-button-next',
            prevEl: '.swiper-button-prev',
        },
        breakpoints: {
            350: {
                slidesPerView: 2,
            },
            480: {
                slidesPerView: 2,
            },
            570: {
                slidesPerView: 2,
            },
            768: {
                slidesPerView: 3,
            },
            1024: {
                slidesPerView: 4,
            },
        },
    });
});