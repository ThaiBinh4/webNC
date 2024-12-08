document.addEventListener("DOMContentLoaded", function () {
    const descriptionText = document.getElementById("description-text");
    const descriptionInput = document.getElementById("description-input");
    const descriptionActions = document.getElementById("description-actions");
    const saveButton = document.getElementById("save-description");
    const cancelButton = document.getElementById("cancel-description");

    const taskTitle = document.getElementById("task-title");
    const taskTitleInput = document.getElementById("task-title-input");
    const deleteButton = document.getElementById("delete-button");

    const dueDateInput = document.getElementById("due-date");
    const saveDateButton = document.getElementById("save-date");

    const resultInput = document.getElementById("result-input");
    const addResultButton = document.getElementById("add-result");

    const taskId = /* ID công việc của bạn (lấy từ server-side) */;
    const apiBaseUrl = "https://localhost:7217"; // URL API của bạn

    async function callUpdateAPI(updatedData) {
        try {
            const response = await fetch(`${apiBaseUrl}/updateTask/${taskId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(updatedData),
            });

            if (!response.ok) {
                alert("Cập nhật thất bại, vui lòng thử lại.");
            }
        } catch (error) {
            console.error("Error while calling update API:", error);
            alert("Có lỗi xảy ra khi gọi API.");
        }
    }

    // Khi nhấn vào tiêu đề, chuyển sang chế độ nhập
    taskTitle.addEventListener("click", function () {
        taskTitleInput.value = taskTitle.textContent.trim();
        taskTitle.classList.add("hidden");
        taskTitleInput.classList.remove("hidden");
        taskTitleInput.focus();
    });

    taskTitleInput.addEventListener("blur", saveTitle);
    taskTitleInput.addEventListener("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            saveTitle();
        } else if (e.key === "Escape") {
            cancelTitleEdit();
        }
    });

    function saveTitle() {
        const newTitle = taskTitleInput.value.trim() || "Tiêu đề công việc";
        taskTitle.textContent = newTitle;
        taskTitle.classList.remove("hidden");
        taskTitleInput.classList.add("hidden");

        callUpdateAPI({ Title: newTitle }); // Gửi API cập nhật
    }

    function cancelTitleEdit() {
        taskTitle.classList.remove("hidden");
        taskTitleInput.classList.add("hidden");
    }

    // Khi nhấn vào phần mô tả, chuyển sang chế độ nhập
    document.addEventListener("DOMContentLoaded", function () {
        const descriptionText = document.getElementById("description-text");
        const descriptionInput = document.getElementById("description-input");
        const descriptionActions = document.getElementById("description-actions");
        const saveButton = document.getElementById("save-description");
        const cancelButton = document.getElementById("cancel-description");

        // Khi nhấn vào phần mô tả, chuyển sang chế độ nhập
        descriptionText.addEventListener("click", function () {
            descriptionInput.value = descriptionText.textContent.trim() === "Thêm mô tả chi tiết hơn..." ? "" : descriptionText.textContent.trim();
            descriptionText.classList.add("hidden");   // Ẩn text
            descriptionInput.classList.remove("hidden"); // Hiển thị ô nhập
            descriptionActions.classList.remove("hidden"); // Hiển thị nút Lưu và Hủy
            descriptionInput.focus(); // Focus vào ô nhập
        });

        // Lưu nội dung mô tả và chuyển về chế độ xem
        saveButton.addEventListener("click", function () {
            if (descriptionInput.value.trim() !== "") {
                descriptionText.textContent = descriptionInput.value.trim();
                descriptionText.style.fontStyle = "normal";
                descriptionText.style.color = "#333";
            } else {
                descriptionText.textContent = "Thêm mô tả chi tiết hơn...";
                descriptionText.style.fontStyle = "italic";
                descriptionText.style.color = "#888";
            }
            closeEditMode(); // Đóng chế độ chỉnh sửa
        });

        // Hủy chỉnh sửa và quay về trạng thái ban đầu
        cancelButton.addEventListener("click", function () {
            closeEditMode();
        });

        // Thêm sự kiện nhấn Enter để lưu và Esc để hủy
        descriptionInput.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault(); // Ngăn form bị submit
                saveButton.click(); // Kích hoạt nút lưu
            } else if (e.key === "Escape") {
                cancelButton.click(); // Kích hoạt nút hủy
            }
        });

        // Đóng chế độ chỉnh sửa mô tả
        function closeEditMode() {
            descriptionText.classList.remove("hidden"); // Hiển thị lại mô tả
            descriptionInput.classList.add("hidden");  // Ẩn ô nhập
            descriptionActions.classList.add("hidden"); // Ẩn các nút Lưu và Hủy
        }
    });

    // Khi thêm ngày hết hạn
    saveDateButton.addEventListener("click", function () {
        const newDueDate = dueDateInput.value;
        if (newDueDate) {
            document.getElementById("display-date").textContent = newDueDate;
            document.getElementById("due-date-display").classList.remove("hidden");

            callUpdateAPI({ Deadline: newDueDate }); // Gửi API cập nhật
        }
    });

    // Khi thêm kết quả
    addResultButton.addEventListener("click", function () {
        const newResult = resultInput.value.trim();
        if (newResult) {
            const resultLog = document.getElementById("result-log");
            const resultItem = document.createElement("div");
            resultItem.textContent = newResult;
            resultLog.appendChild(resultItem);

            resultInput.value = "";
            callUpdateAPI({ Ketqua: newResult }); // Gửi API cập nhật
        }
    });

    // Khi nhấn nút xóa
    deleteButton.addEventListener("click", function () {
        if (confirm("Bạn có chắc chắn muốn xóa công việc này không?")) {
            fetch(`${apiBaseUrl}/deleteTask/${taskId}`, { method: "DELETE" })
                .then(response => {
                    if (response.ok) {
                        document.querySelector(".container").remove();
                    } else {
                        alert("Xóa công việc thất bại.");
                    }
                })
                .catch(error => {
                    console.error("Error while deleting task:", error);
                    alert("Có lỗi xảy ra khi xóa công việc.");
                });
        }
    });
});
