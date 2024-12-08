// Thêm hiệu ứng click vào nút Add
document.querySelectorAll('.add a').forEach(button => {
    button.addEventListener('click', function (event) {
        event.preventDefault();
        alert('Bạn đã nhấn vào nút Add!');
        // Xử lý thêm task hoặc chuyển hướng tùy thuộc vào logic
        window.location.href = this.href;
    });
});

// Hiệu ứng hover cho các phần tử item-column
document.querySelectorAll('.item-column').forEach(item => {
    item.addEventListener('mouseover', function () {
        this.style.backgroundColor = '#c8e6c9';
    });

    item.addEventListener('mouseout', function () {
        this.style.backgroundColor = '#eaf8e8';
    });
});

// Thêm animation cho các column khi load
window.addEventListener('load', function () {
    const columns = document.querySelectorAll('.column');
    columns.forEach((col, index) => {
        setTimeout(() => {
            col.style.opacity = 1;
            col.style.transform = 'translateY(0)';
        }, index * 200); // Delay theo thứ tự
    });
});
