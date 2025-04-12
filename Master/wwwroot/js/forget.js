

// دوال التنقل بين الصفحات
function AboutUs() {
    window.location.href = "../aboutUs/about.html";
}

function Home() {
    window.location.href = "../home/home.html";
}

function Contact() {
    window.location.href = "../ContactUs/Contact.html";
}

function login() {
    window.location.href = "../SignUp/SignUp.html";
}

function register() {
    window.location.href = "../SignUp/SignUp.html?show=register";
}

function shop() {
    window.location.href = "../shop/shop.html";
}

function Cart() {
    window.location.href = "../cart/cart.html";
}

function recycling() {
    window.location.href = "../recycling/recycling.html";
}

// إضافة الأنيميشن عند تحميل الصفحة
document.addEventListener("DOMContentLoaded", () => {
    const elements = document.querySelectorAll(".animate");

    setTimeout(() => {
        elements.forEach(element => {
            element.classList.add("visible");
        });
    }, 500);
});

// خطوات تغيير كلمة المرور
document.addEventListener("DOMContentLoaded", function () {
    const steps = document.querySelectorAll(".step"); // تحديد الخطوات (1,2,3)
    const step1 = document.getElementById("step1");
    const step2 = document.getElementById("step2");
    const step3 = document.getElementById("step3");
    const success = document.getElementById("success");

    const sendCodeButton = step1.querySelector("button");
    const verifyCodeButton = step2.querySelector("button");
    const resetPasswordButton = step3.querySelector("button");

    const emailInput = step1.querySelector('input[type="email"]');
    const verificationInputs = step2.querySelectorAll('.verification-input');
    const passwordInput = step3.querySelectorAll('input[type="password"]')[0];
    const confirmPasswordInput = step3.querySelectorAll('input[type="password"]')[1];

    // إخفاء جميع الخطوات
    function hideAllSteps() {
        step1.classList.add("d-none");
        step2.classList.add("d-none");
        step3.classList.add("d-none");
        success.classList.add("d-none");
    }

    // تحديث حالة الخطوات في المؤشر
    function updateStepIndicator(index) {
        steps.forEach((step, i) => {
            step.classList.toggle("active", i === index);
        });
    }

    // إظهار خطوة معينة
    function showStep(stepIndex) {
        hideAllSteps();
        if (stepIndex === 0) step1.classList.remove("d-none");
        if (stepIndex === 1) step2.classList.remove("d-none");
        if (stepIndex === 2) step3.classList.remove("d-none");
        if (stepIndex === 3) success.classList.remove("d-none");

        updateStepIndicator(stepIndex);
    }

    // الانتقال إلى الخطوة 2 عند إدخال البريد الإلكتروني الصحيح
    sendCodeButton.addEventListener("click", function (event) {
        event.preventDefault();
        if (emailInput.value.trim() !== "" && emailInput.checkValidity()) {
            showStep(1);
        } else {
            alert("يرجى إدخال بريد إلكتروني صحيح.");
        }
    });

    // الانتقال إلى الخطوة 3 بعد إدخال الكود الصحيح
    verifyCodeButton.addEventListener("click", function (event) {
        event.preventDefault();
        let code = Array.from(verificationInputs).map(input => input.value).join("");

        if (code.length === 6) {
            showStep(2);
        } else {
            alert("يرجى إدخال كود التحقق المكون من 6 أرقام.");
        }
    });

    // إظهار رسالة النجاح بعد إدخال كلمة مرور صحيحة
    resetPasswordButton.addEventListener("click", function (event) {
        event.preventDefault();
        if (passwordInput.value === confirmPasswordInput.value && passwordInput.value.length >= 8) {
            showStep(3);
        } else {
            alert("كلمات المرور غير متطابقة أو قصيرة جدًا.");
        }
    });

    // إظهار الخطوة الأولى عند تحميل الصفحة
    showStep(0);
});


