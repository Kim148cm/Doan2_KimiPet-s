
document.addEventListener('DOMContentLoaded', function () {
    fetch('/Wishlist/GetIds')
        .then(function (r) { return r.json(); })
        .then(function (ids) {
            // Đánh dấu tất cả nút tim đã lưu
            ids.forEach(function (id) {
                document.querySelectorAll('[data-wl-id="' + id + '"]').forEach(function (btn) {
                    btn.classList.add('active');
                    var i = btn.querySelector('i');
                    if (i) i.className = 'fa-solid fa-heart';
                });
            });

            // Cập nhật badge nav
            wlUpdateNavBadge(ids.length);
        })
        .catch(function () { /* silent */ });
});

// ── Toggle yêu thích ─────────────────────────────────────────
function wlToggle(btn, productId) {
    // Disable tạm để tránh double click
    btn.disabled = true;

    fetch('/Wishlist/Toggle', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: 'productId=' + productId
    })
        .then(function (r) { return r.json(); })
        .then(function (data) {
            btn.disabled = false;
            if (!data.success) return;

            // Sync TẤT CẢ nút tim cùng productId trên trang
            document.querySelectorAll('[data-wl-id="' + productId + '"]').forEach(function (b) {
                if (data.added) {
                    b.classList.add('active');
                    var i = b.querySelector('i');
                    if (i) i.className = 'fa-solid fa-heart';
                } else {
                    b.classList.remove('active');
                    var i = b.querySelector('i');
                    if (i) i.className = 'fa-regular fa-heart';
                }
            });

            // Badge nav
            wlUpdateNavBadge(data.count);

            // Toast
            wlToast(data.message, data.added ? 'success' : 'remove');
        })
        .catch(function () { btn.disabled = false; });
}

// ── Cập nhật badge số lượng trên icon Yêu thích ──────────────
function wlUpdateNavBadge(count) {
    var badge = document.getElementById('wl-nav-count');
    if (!badge) return;
    badge.textContent = count;
    badge.style.display = count > 0 ? 'inline-flex' : 'none';
}

// ── Toast notification ────────────────────────────────────────
(function () {
    // Tạo toast element 1 lần
    var toastEl = document.createElement('div');
    toastEl.id = 'wl-global-toast';
    toastEl.style.cssText = [
        'position:fixed',
        'bottom:28px',
        'left:50%',
        'transform:translateX(-50%) translateY(16px)',
        'padding:10px 20px',
        'border-radius:8px',
        'font-size:13.5px',
        'font-weight:600',
        'font-family:Nunito,sans-serif',
        'box-shadow:0 4px 20px rgba(0,0,0,.18)',
        'z-index:99999',
        'opacity:0',
        'transition:opacity .25s,transform .25s',
        'pointer-events:none',
        'white-space:nowrap',
        'display:flex',
        'align-items:center',
        'gap:8px'
    ].join(';');
    document.body.appendChild(toastEl);

    var toastTimer = null;

    window.wlToast = function (msg, type) {
        if (type === 'success') {
            toastEl.style.background = '#1a7a3c';
            toastEl.style.color = '#fff';
            toastEl.innerHTML = '<i class="fa-solid fa-heart"></i> ' + msg;
        } else {
            toastEl.style.background = '#333';
            toastEl.style.color = '#fff';
            toastEl.innerHTML = '<i class="fa-regular fa-heart"></i> ' + msg;
        }
        toastEl.style.opacity = '1';
        toastEl.style.transform = 'translateX(-50%) translateY(0)';

        clearTimeout(toastTimer);
        toastTimer = setTimeout(function () {
            toastEl.style.opacity = '0';
            toastEl.style.transform = 'translateX(-50%) translateY(16px)';
        }, 2200);
    };
})();