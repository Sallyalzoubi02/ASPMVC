
// Initialize tabs function
function initializeTabs() {
    // Get all tab buttons
    const tabButtons = document.querySelectorAll('[data-bs-toggle="pill"]');

    // Add click event to each button
    tabButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Get the target tab content ID
            const targetId = this.getAttribute('data-bs-target');

            // Show the target tab using Bootstrap's Tab API
            const tabInstance = new bootstrap.Tab(this);
            tabInstance.show();

            // Update URL hash
            updateUrlHash(targetId);
        });
    });

    // Check URL hash on page load
    checkInitialTab();
}

function updateUrlHash(targetId) {
    // Remove # from targetId and update URL
    const hash = targetId.substring(1);
    if (history.pushState) {
        history.pushState(null, null, '#' + hash);
    } else {
        window.location.hash = '#' + hash;
    }
}

function checkInitialTab() {
    // Check if URL has a hash
    if (window.location.hash) {
        const targetId = '#' + window.location.hash.substring(1);
        const tabButton = document.querySelector(`[data-bs-target="${targetId}"]`);
        if (tabButton) {
            const tabInstance = new bootstrap.Tab(tabButton);
            tabInstance.show();
        }
    }
}

// Handle browser back/forward navigation
window.addEventListener('popstate', function () {
    checkInitialTab();
});

// Blur effect for dropdown
function bluer() {
    const dropdown = document.getElementById("dropdown-menu");
    const userIcon = document.getElementById("usericon");
    const mainContent = document.getElementById("content");

    const isOpen = dropdown.classList.contains("show");

    if (isOpen) {
        mainContent.classList.add("blurred");
    } else {
        mainContent.classList.remove("blurred");
    }

    document.addEventListener("click", function (e) {
        if (!dropdown.contains(e.target)) {
            dropdown.classList.remove("show");
            mainContent.classList.remove("blurred");
        }
    });
}


fetch('../nav/nav.html')
    .then(response => response.text())
    .then(data => {
        document.getElementById('nav').innerHTML = data;

        // إعادة تفعيل زر الـ Burger Menu
        const menuToggle = document.querySelector('.menu-toggle');
        const menu = document.querySelector('.menu');
        if (menuToggle) {
            menuToggle.addEventListener('click', () => {
                menu.classList.toggle('active');
            });
        }
    });

fetch('../footer/footer.html')
    .then(response => response.text())
    .then(data => {
        document.getElementById('footer').innerHTML = data;
    });

// Main initialization
window.onload = function () {

    // Check login status
    setTimeout(function () {
        var isLogged = sessionStorage.getItem("isLogged");
        console.log(isLogged);
        if (isLogged === "true") {
            const dropdown = document.getElementById("dropdown-menu");
            dropdown.innerHTML = `
                <li><a class="dropdown-item" onclick="profile()">الصفحة الشخصية</a></li>
                <li><hr class="dropdown-divider"></li>
                <li><a class="dropdown-item" onclick="logout()">تسجيل الخروج</a></li>
            `;
        }
    }, 100);

    // Load navigation and footer


    // Initialize tabs after everything is loaded
    setTimeout(initializeTabs, 200);
};

document.getElementById('passwordForm').addEventListener('submit', function (e) {
    e.preventDefault();

    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    if (newPassword !== confirmPassword) {
        alert('كلمة المرور الجديدة وتأكيدها غير متطابقين');
        return;
    }

    if (newPassword.length < 8) {
        alert('كلمة المرور يجب أن تكون 8 أحرف على الأقل');
        return;
    }

    // هنا يمكنك إضافة كود إرسال البيانات إلى الخادم
    alert('تم تغيير كلمة المرور بنجاح');
});