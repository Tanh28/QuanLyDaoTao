// Xử lý hiệu ứng xuất hiện của thanh navbar khi cuộn xuống
document.addEventListener('DOMContentLoaded', function() {
    const navbar = document.querySelector('.nav-container');
    let lastScrollTop = 0;
    const scrollThreshold = 100; // Ngưỡng cuộn để kích hoạt hiệu ứng

    window.addEventListener('scroll', function() {
        const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
        
        // Kiểm tra hướng cuộn
        if (scrollTop > lastScrollTop && scrollTop > scrollThreshold) {
            // Cuộn xuống - ẩn thanh navbar
            navbar.style.top = '-100px';
        } else {
            // Cuộn lên hoặc ở đầu trang - hiện thanh navbar
            navbar.style.top = '0';
        }
        
        lastScrollTop = scrollTop;
    });
});
