const container = document.getElementById('container');
const registerBtn = document.getElementById('register');
const loginBtn = document.getElementById('login');

registerBtn.addEventListener('click', () => {
    container.classList.add("active");
});

loginBtn.addEventListener('click', () => {
    container.classList.remove("active");
});

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
function register() {
    window.location.href = "../SignUp/SignUp.html?show=register"
}
function shop() {
    window.location.href = "../shop/shop.html"
}
function Cart() {
    window.location.href = "../cart/cart.html"
}
function recycling() {
    window.location.href = "../recycling/recycling.html";
}
document.addEventListener("DOMContentLoaded", () => {
    const elements = document.querySelectorAll(".animate"); // اختيار العناصر


    setTimeout(() => {
        elements.forEach(element => {
            element.classList.add("visible"); // إضافة الـ class لجعل العنصر يظهر
        });
    }, 500); // وقت التأخير (500 مللي ثانية)

});


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
window.onload = OnLoadEventHandeler;


function OnLoadEventHandeler() {
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);

    const show = urlParams.get('formType')
    console.log(show);

    if (show == 'register') {
        registerBtn.click();
    }
    else {
        loginBtn.click();
    }
}
