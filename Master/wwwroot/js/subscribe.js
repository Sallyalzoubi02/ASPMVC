function AboutUs() {
    window.location.href = "../aboutUs/about.html"
}
function Home() {
    window.location.href = "../home/home.html"
}
function Contact() {
    window.location.href = "../ContactUs/Contact.html"
}
function login() {
    window.location.href = "../SignUp/SignUp.html"
}
function shop() {
    window.location.href = "../shop/shop.html"
}

function Cart() {
    window.location.href = "../cart/cart.html"
}

function next() {
    window.location.href = "add2.html"
}

function profile() {
    window.location.href = "../profile/profile.html"
}
function logout() {
    sessionStorage.setItem("isLogged", "false");
    window.location.href = "../home/home.html";
}

function confirm() {
    window.location.href = "../profile/profilevip.html"
}

function cancel() {
    window.location.href = "../profile/profile.html"

}
function recycling() {
    window.location.href = "../recycling/recycling.html";
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

function bluer() {
    const dropdown = document.getElementById("dropdown-menu");
    const userIcon = document.getElementById("usericon");
    const mainContent = document.getElementById("content");
    console.log(dropdown)
    console.log(userIcon)
    // userIcon.addEventListener("click", function (e) {
    // e.stopPropagation();
    const isOpen = dropdown.classList.contains("show");
    console.log(isOpen)

    // Toggle the blur effect
    if (isOpen) {
        mainContent.classList.add("blurred");

    } else {
        mainContent.classList.remove("blurred");

    }


    // Add a single event listener for clicking outside
    document.addEventListener("click", function (e) {
        // Check if the click is outside the dropdown and user icon
        if (!dropdown.contains(e.target) && !userIcon.contains(e.target)) {
            dropdown.classList.remove("show"); // Close the dropdown
            mainContent.classList.remove("blurred"); // Remove the blur
        }
    });
}



document.addEventListener("DOMContentLoaded", () => {
    const elements = document.querySelectorAll(".animate"); // اختيار العناصر
    const elements2 = document.querySelectorAll(".animate2"); // اختيار العناصر

    setTimeout(() => {
        elements.forEach(element => {
            element.classList.add("visible"); // إضافة الـ class لجعل العنصر يظهر
        });
    }, 500); // وقت التأخير (500 مللي ثانية)
    setTimeout(() => {
        elements2.forEach(element => {
            element.classList.add("visible2"); // إضافة الـ class لجعل العنصر يظهر
        });
    }, 500); // وقت التأخير (500 مللي ثانية)
});



// Initialize Leaflet Map
const map = L.map('map').setView([32.556, 35.85], 13); // Centered on Irbid as an example
L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
}).addTo(map);

// Draggable marker for user-selected location
const marker = L.marker([32.556, 35.85], { draggable: true }).addTo(map);

marker.on('dragend', function () {
    const position = marker.getLatLng();
    alert(`Selected Location: Latitude ${position.lat}, Longitude ${position.lng}`);
});


//payment total
// عناصر الحقول
const typeS = document.getElementById("typeS");
const quantity = document.getElementById("quantity");
const city = document.getElementById("city");
const totalElement = document.getElementById("total");

// أسعار ثابتة للاشتراكات والكمية والمحافظات
const prices = {
    subscription: {
        daily: 7,
        weekly: 5,
        monthly: 10,
    },
    quantity: {
        few: 0.5,
        mid: 1,
        lg: 1.5,
        xlg: 2,
        IDK: 1,
    },
    city: {
        Amman: 0, // بدون رسوم إضافية
        Irbid: 2,
        Jerash: 2,
        Ajloun: 2,
        "M'aan": 2.5,
        Aqaba: 3,
        Altafelah: 3,
        Alkarak: 3,
        Salt: 2,
        Madaba: 1,
        Zarqa: 2,
        Almafraq: 2,
    },
};

// حساب المبلغ الإجمالي
function calculateTotal() {
    const subscriptionPrice = prices.subscription[typeS.value] || 0;
    const quantityPrice = prices.quantity[quantity.value] || 0;
    const cityPrice = prices.city[city.value] || 0;

    // العملية الحسابية
    const total = subscriptionPrice + quantityPrice + cityPrice;

    // عرض النتيجة
    totalElement.textContent = `التكلفة الإجمالية: ${total} دينار`;
}

// إضافة مستمعي أحداث لتحديث النتيجة عند التغيير
typeS.addEventListener("change", calculateTotal);
quantity.addEventListener("change", calculateTotal);
city.addEventListener("change", calculateTotal);


var paymentMethod = document.getElementById("paymentMethod");
var cardInfo = document.getElementById("cardInfo");


paymentMethod.addEventListener("change", function () {
    if (paymentMethod.value === "card") {
        cardInfo.style.display = "block"; // Show card info
    } else {
        cardInfo.style.display = "none"; // Hide card info
    }
});


function back() {
    window.history.back();
}