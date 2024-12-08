const navbarMenu = document.querySelector(".navbar .links");
const hamburgerBtn = document.querySelector(".hamburger-btn");
const hideMenuBtn = navbarMenu.querySelector(".close-btn");
const showPopupBtn = document.querySelector(".login-btn");
const formPopup = document.querySelector(".form-popup");
const hidePopupBtn = formPopup.querySelector(".close-btn");
const signupLoginLink = formPopup.querySelectorAll(".bottom-link a");

// Lấy các phần tử
const fileInput = document.getElementById("fileInput");
const fileName = document.getElementById("fileName");
const avatarPreview = document.getElementById("avatarPreview");

// Khi người dùng chọn file
fileInput.addEventListener("change", function () {
    const file = fileInput.files[0];

    if (file) {
        fileName.textContent = file.name;  // Cập nhật tên file

        // Kiểm tra xem file có phải là ảnh không
        if (file.type.startsWith("image/")) {
            const reader = new FileReader();

            reader.onload = function (e) {
                avatarPreview.src = e.target.result;  // Đặt nguồn ảnh
                avatarPreview.style.display = "block"; // Hiển thị ảnh
            };

            reader.readAsDataURL(file);  // Đọc file dưới dạng URL
        } else {
            avatarPreview.src = "";
            avatarPreview.style.display = "none";  // Ẩn ảnh nếu file không phải là ảnh
        }
    } else {
        fileName.textContent = "Không có tệp nào được chọn";
        avatarPreview.style.display = "none";  // Ẩn ảnh nếu không có file
    }
});

// Kiểm tra các phần tử tồn tại trước khi thêm sự kiện
if (hamburgerBtn && navbarMenu) {
    // Hiển thị menu di động
    hamburgerBtn.addEventListener("click", () => {
        navbarMenu.classList.toggle("show-menu");
    });
}

if (hideMenuBtn && hamburgerBtn) {
    // Ẩn menu di động
    hideMenuBtn.addEventListener("click", () => {
        navbarMenu.classList.remove("show-menu");
    });
}

if (showPopupBtn && formPopup) {
    // Hiển thị popup đăng nhập
    showPopupBtn.addEventListener("click", () => {
        document.body.classList.toggle("show-popup");
    });
}

if (hidePopupBtn && formPopup) {
    // Ẩn popup đăng nhập
    hidePopupBtn.addEventListener("click", () => {
        document.body.classList.remove("show-popup");
    });
}

// Hiển thị hoặc ẩn form đăng ký
signupLoginLink.forEach(link => {
    link.addEventListener("click", (e) => {
        e.preventDefault();
        if (link.id === 'signup-link') {
            formPopup.classList.add("show-signup");
        } else {
            formPopup.classList.remove("show-signup");
        }
    });
});

document.querySelector(".form-content form").addEventListener("submit", async function (e) {
    e.preventDefault(); // Ngăn form reload trang

    // Lấy giá trị từ các input field
    const email = document.querySelector('input[type="text"]').value;
    const password = document.querySelector('input[type="password"]').value;

    // Gửi yêu cầu đến API đăng nhập
    try {
        const response = await fetch("/api/login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ IdUser: email, Pass: password })
        });

        if (response.ok) {
            const data = await response.json();
            alert(`Đăng nhập thành công! Token: ${data.Token}`);
            // Lưu token vào localStorage nếu cần
            localStorage.setItem("token", data.Token);
        } else {
            const error = await response.json();
            alert(`Đăng nhập thất bại: ${error.Message}`);
        }
    } catch (err) {
        console.error("Error:", err);
        alert("Đã xảy ra lỗi, vui lòng thử lại.");
    }
});

// DOM Elements
// DOM Elements
// document.addEventListener("DOMContentLoaded", function() {
//     const loginButton = document.querySelector(".login-btn");
//     const signupLink = document.getElementById("signup-link");
//     const loginLink = document.getElementById("login-link");
//     const closeBtn = document.querySelector(".close-btn");
//     const popup = document.querySelector(".form-popup");
//     const overlay = document.querySelector(".blur-bg-overlay");

//     loginButton.addEventListener("click", function() {
//         popup.classList.add("show-popup");
//         overlay.classList.add("show-popup");
//     });

//     closeBtn.addEventListener("click", function() {
//         popup.classList.remove("show-popup");
//         overlay.classList.remove("show-popup");
//     });

//     overlay.addEventListener("click", function() {
//         popup.classList.remove("show-popup");
//         overlay.classList.remove("show-popup");
//     });

//     signupLink.addEventListener("click", function() {
//         popup.querySelector(".login").style.display = "none";
//         popup.querySelector(".signup").style.display = "block";
//     });

//     loginLink.addEventListener("click", function() {
//         popup.querySelector(".signup").style.display = "none";
//         popup.querySelector(".login").style.display = "block";
//     });
// });