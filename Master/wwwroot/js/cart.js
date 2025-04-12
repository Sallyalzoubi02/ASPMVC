
function selectPayment(element, type) {
    // إزالة التنشيط من جميع الخيارات
    document.querySelectorAll('.payment-method').forEach(opt => {
        opt.classList.remove('active');
    });

    // تفعيل الخيار المحدد
    element.classList.add('active');

    // إظهار/إخفاء تفاصيل البطاقة
    const cardDetails = document.getElementById('cardDetails');
    if (type === 'credit' || type === 'debit') {
        cardDetails.style.display = 'block';
    } else {
        cardDetails.style.display = 'none';
    }
}


window.onload = function () {
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
    }, 100); // Delay to ensure session is loaded
};