
//----------------------------------------
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

//----------------------------------------

const swiper_product_new = new Swiper('.product_slider_new', {
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


//----------------------------------------

function updateCountdown() {
    const hoursElement = document.getElementById('hours');
    const minutesElement = document.getElementById('minutes');
    const secondsElement = document.getElementById('seconds');

    let hours = parseInt(hoursElement.textContent);
    let minutes = parseInt(minutesElement.textContent);
    let seconds = parseInt(secondsElement.textContent);

    if (seconds > 0) {
        seconds--;
    } else if (minutes > 0) {
        minutes--;
        seconds = 59;
    } else if (hours > 0) {
        hours--;
        minutes = 59;
        seconds = 59;
    }

    hoursElement.textContent = hours.toString().padStart(2, '0');
    minutesElement.textContent = minutes.toString().padStart(2, '0');
    secondsElement.textContent = seconds.toString().padStart(2, '0');
}

setInterval(updateCountdown, 1000);

//----------------------------------------
document.addEventListener('DOMContentLoaded', function () {
    const section = document.querySelector('.seaction_two');
    const sectionTop = section.offsetTop;
    let lastScrollTop = 0;
    let ticking = false;

    function handleScroll() {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;

        if (!ticking) {
            window.requestAnimationFrame(function () {
                if (scrollTop > sectionTop) {
                    section.classList.add('sticky');
                    if (scrollTop < lastScrollTop) {
                        // Scrolling up
                        section.classList.add('show');
                    } else {
                        // Scrolling down
                        section.classList.remove('show');
                    }
                } else {
                    section.classList.remove('sticky', 'show');
                }
                lastScrollTop = scrollTop;
                ticking = false;
            });

            ticking = true;
        }
    }

    window.addEventListener('scroll', handleScroll, { passive: true });
});

//----------------------------------------

$(document).ready(function () {
    $("#click_ele, #click_ele_two").click(function () {
        $("#show_ele, #show_ele_two").slideToggle("slow");
    });
});

$(document).ready(function () {
    $("#click_title_sidebar").click(function () {
        $("#show_title_sidebar").slideToggle("slow");
    });
});
$(document).ready(function () {
    $("#click_title_small_sidebar").click(function () {
        $("#show_title_small_sidebar").slideToggle("slow");
    });
});
$(document).ready(function () {
    $("#click_title_Bag_sidebar").click(function () {
        $("#show_title_Bag_sidebar").slideToggle("slow");
    });
});
$(document).ready(function () {
    $("#click_title_PhuKien_sidebar").click(function () {
        $("#show_title_PhuKien_sidebar").slideToggle("slow");
    });
});
$(document).ready(function () {
    $("#click_title_dress_sidebar").click(function () {
        $("#show_title_dress_sidebar").slideToggle("slow");
    });
});
$(document).ready(function () {
    $("#click_title_collection_sidebar").click(function () {
        $("#show_title_collection_sidebar").slideToggle("slow");
    });
});
//--------------------------------------------------------------------------
// Trang chi tiết 


document.querySelectorAll('.title_two_detail').forEach(tab => {
    tab.addEventListener('click', function () {
        document.querySelectorAll('.title_two_detail').forEach(item => item.classList.remove('active'));
        this.classList.add('active');

        document.querySelectorAll('.body_two_detail_content .content').forEach(content => content.style.display = 'none');

        const tabContentId = this.getAttribute('data-tab');
        document.getElementById(tabContentId).style.display = 'block';
    });
});

document.querySelectorAll('.title_next_detail').forEach(tab => {
    tab.addEventListener('click', function () {
        document.querySelectorAll('.title_next_detail').forEach(item => item.classList.remove('active'));
        this.classList.add('active');

        document.querySelectorAll('.body_two_detail_next .next_title').forEach(content => content.style.display = 'none');

        const tabContentId = this.getAttribute('data-tab');
        document.getElementById(tabContentId).style.display = 'block';
    });
});




//--------------------------------------------------------------------------
// Thay đổi ảnh trong trang chi tiết khi chọn màu 
document.addEventListener('DOMContentLoaded', function () {
    const colorOptions = document.querySelectorAll('.color-option');
    const productImages = document.querySelectorAll('.body_img_left img');

    const imageUrls = {
        beige: {
            1: 'assets/image/dt1.webp',
            2: 'assets/image/dt2.webp',
            3: 'assets/image/dt3.webp',
            4: 'assets/image/dt4.webp',
            5: 'assets/image/dt5.webp'
        },
        blue: {
            1: 'assets/image/dt11.webp',
            2: 'assets/image/dt22.webp',
            3: 'assets/image/dt33.webp',
            4: 'assets/image/dt44.webp',
            5: 'assets/image/dt55.webp'
        },
    };

    function changeProductImages(color) {
        productImages.forEach(img => {
            const imageNumber = img.getAttribute('data-image');
            img.src = imageUrls[color][imageNumber];
        });
    }

    colorOptions.forEach(option => {
        option.addEventListener('click', function () {
            const selectedColor = this.getAttribute('data-color');
            changeProductImages(selectedColor);

            // Add active class to selected color and remove from others
            colorOptions.forEach(opt => opt.classList.remove('active'));
            this.classList.add('active');
        });
    });
});


//---------------------------------------------------------------------------
// chuyển ảnh trên mobile

document.addEventListener('DOMContentLoaded', function () {
    const colorOptions = document.querySelectorAll('.color-option');
    const productImages = document.querySelectorAll('.swiper-slide .img_phone_slider');

    const imageUrls = {
        beige: {
            1: 'assets/image/dt1.webp',
            2: 'assets/image/dt2.webp',
            3: 'assets/image/dt3.webp',
            4: 'assets/image/dt4.webp',
            5: 'assets/image/dt5.webp'
        },
        blue: {
            1: 'assets/image/dt11.webp',
            2: 'assets/image/dt22.webp',
            3: 'assets/image/dt33.webp',
            4: 'assets/image/dt44.webp',
            5: 'assets/image/dt55.webp'
        },
    };

    // Khởi tạo Swiper
    const swiper = new Swiper('.swiper-container', {
        pagination: {
            el: '.swiper-pagination',
            clickable: true,
        },
        loop: true,
        observer: true,
        observeParents: true,
    });

    function changeProductImages(color) {
        productImages.forEach(img => {
            const imageNumber = img.getAttribute('data-image');
            img.src = imageUrls[color][imageNumber];
        });


        swiper.update();
        swiper.pagination.render();
        swiper.pagination.update();
    }

    colorOptions.forEach(option => {
        option.addEventListener('click', function () {
            const selectedColor = this.getAttribute('data-color');
            changeProductImages(selectedColor);


            colorOptions.forEach(opt => opt.classList.remove('active'));
            this.classList.add('active');
        });
    });
});


















