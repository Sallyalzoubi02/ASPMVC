

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
    const steps = document.querySelectorAll(".step");
    const step1 = document.getElementById("step1");
    const step2 = document.getElementById("step2");
    const step3 = document.getElementById("step3");
    const success = document.getElementById("success");

    const sendCodeButton = step1.querySelector("button");
    const verifyCodeButton = step2.querySelector("button");
    const resetPasswordButton = step3.querySelector("button");

    const emailInput = document.getElementById("email");
    const verificationInputs = [
        document.getElementById("verificationCode1"),
        document.getElementById("verificationCode2"),
        document.getElementById("verificationCode3"),
        document.getElementById("verificationCode4"),
        document.getElementById("verificationCode5"),
        document.getElementById("verificationCode6")
    ];
    const passwordInput = document.getElementById("newPassword");
    const confirmPasswordInput = document.getElementById("confirmPassword");

    let generatedOTP = "";
    let userEmail = "";

    // إضافة التحقق التلقائي بين حقول كود التحقق
    verificationInputs.forEach((input, index) => {
        input.addEventListener("input", function () {
            if (this.value.length === 1 && index < verificationInputs.length - 1) {
                verificationInputs[index + 1].focus();
            }
        });

        input.addEventListener("keydown", function (e) {
            if (e.key === "Backspace" && this.value.length === 0 && index > 0) {
                verificationInputs[index - 1].focus();
            }
        });
    });

    function hideAllSteps() {
        step1.classList.add("d-none");
        step2.classList.add("d-none");
        step3.classList.add("d-none");
        success.classList.add("d-none");
    }

    function updateStepIndicator(index) {
        steps.forEach((step, i) => {
            step.classList.toggle("active", i === index);
        });
    }

    function showStep(stepIndex) {
        hideAllSteps();
        if (stepIndex === 0) step1.classList.remove("d-none");
        if (stepIndex === 1) step2.classList.remove("d-none");
        if (stepIndex === 2) step3.classList.remove("d-none");
        if (stepIndex === 3) success.classList.remove("d-none");

        updateStepIndicator(stepIndex);
    }

    // إرسال كود التحقق
    sendCodeButton.addEventListener("click", async function (event) {
        event.preventDefault();

        if (!emailInput.checkValidity()) {
            alert("يرجى إدخال بريد إلكتروني صحيح.");
            return;
        }

        userEmail = emailInput.value.trim();

        try {
            // التحقق من وجود البريد في قاعدة البيانات
            const response = await fetch(`/Home/CheckEmailExists?email=${encodeURIComponent(userEmail)}`);
            const result = await response.json();

            if (!result.exists) {
                alert("البريد الإلكتروني غير مسجل في النظام.");
                return;
            }

            // إنشاء وإرسال OTP
            generatedOTP = Math.floor(100000 + Math.random() * 900000).toString();

            const otpResponse = await fetch("/Home/SendOTP", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    email: userEmail,
                    otp: generatedOTP
                })
            });

            if (otpResponse.ok) {
                showStep(1);
            } else {
                alert("حدث خطأ أثناء إرسال كود التحقق. يرجى المحاولة لاحقًا.");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("حدث خطأ أثناء الاتصال بالخادم.");
        }
    });

    // التحقق من كود OTP
    verifyCodeButton.addEventListener("click", function (event) {
        event.preventDefault();

        const enteredCode = verificationInputs.map(input => input.value).join("");

        if (enteredCode.length !== 6) {
            alert("يرجى إدخال كود التحقق المكون من 6 أرقام.");
            return;
        }

        if (enteredCode === generatedOTP) {
            showStep(2);
        } else {
            alert("كود التحقق غير صحيح. يرجى المحاولة مرة أخرى.");
        }
    });
    // التحقق من قوة كلمة المرور أثناء الكتابة
    document.getElementById('newPassword').addEventListener('input', function (e) {
        const password = e.target.value;
        checkPasswordRequirements(password);
        updatePasswordStrength(password);
    });

    function checkPasswordRequirements(password) {
        // التحقق من الطول (8 أحرف على الأقل)
        const lengthValid = password.length >= 8;
        updateRequirement('length-requirement', lengthValid);

        // التحقق من وجود حرف كبير
        const hasUppercase = /[A-Z]/.test(password);
        updateRequirement('uppercase-requirement', hasUppercase);

        // التحقق من وجود رمز خاص
        const hasSpecialChar = /[!@@#$%^&*(),.?":{}|<>_-]/.test(password);
        updateRequirement('special-char-requirement', hasSpecialChar);

        // التحقق من وجود رقم
        const hasNumber = /[0-9]/.test(password);
        updateRequirement('number-requirement', hasNumber);
    }

    function updateRequirement(elementId, isValid) {
        const element = document.getElementById(elementId);
        const icon = element.querySelector('i');

        if (isValid) {
            element.classList.add('valid');
            icon.classList.remove('fa-circle');
            icon.classList.add('fa-check-circle');
        } else {
            element.classList.remove('valid');
            icon.classList.remove('fa-check-circle');
            icon.classList.add('fa-circle');
        }
    }

    function updatePasswordStrength(password) {
        const strengthMeter = document.querySelector('.strength-meter');

        // إعادة تعيين كلاس الألوان
        strengthMeter.classList.remove('bg-danger', 'bg-warning', 'bg-success');

        if (password.length === 0) {
            strengthMeter.style.width = '0%';
            return;
        }

        let strength = 0;
        if (password.length >= 8) strength++;
        if (/[A-Z]/.test(password)) strength++;
        if (/[!@@#$%^&*(),.?":{}|<>_-]/.test(password)) strength++;
        if (/[0-9]/.test(password)) strength++;

        const width = (strength / 4) * 100;
        strengthMeter.style.width = `${width}%`;

        // إضافة كلاس اللون المناسب
        if (strength <= 1) {
            strengthMeter.classList.add('bg-danger');
        } else if (strength <= 3) {
            strengthMeter.classList.add('bg-warning');
        } else {
            strengthMeter.classList.add('bg-success');
        }
    }
    // إعادة تعيين كلمة المرور
    resetPasswordButton.addEventListener("click", async function (event) {
        event.preventDefault();

        if (passwordInput.value !== confirmPasswordInput.value) {
            alert("كلمات المرور غير متطابقة.");
            return;
        }

        if (passwordInput.value.length < 8) {
            alert("كلمة المرور يجب أن تكون 8 أحرف على الأقل.");
            return;
        }

        try {
            const response = await fetch("/Home/ResetPassword", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    email: userEmail,
                    newPassword: passwordInput.value
                })
            });

            if (response.ok) {
                showStep(3);
            } else {
                alert("حدث خطأ أثناء إعادة تعيين كلمة المرور.");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("حدث خطأ أثناء الاتصال بالخادم.");
        }
    });

    // إظهار الخطوة الأولى عند تحميل الصفحة
    showStep(0);
});


