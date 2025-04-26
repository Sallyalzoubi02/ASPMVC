document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('recyclingForm');
    const steps = document.querySelectorAll('.step');
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    let currentStep = 1;

    function showStep(step) {
        steps.forEach(s => s.classList.add('d-none'));
        document.querySelector(`.step[data-step="${step}"]`).classList.remove('d-none');

        prevBtn.style.display = step > 1 ? 'block' : 'none';
        nextBtn.textContent = step === steps.length ? 'إرسال الطلب' : 'التالي';

        // 🗺️ لما نوصل للخطوة الثانية (الخريطة)، نصحح أبعاد الخريطة
        if (step === 2 && typeof map !== 'undefined' && map) {
            setTimeout(() => {
                map.invalidateSize();
            }, 300);
        }
    }

    function validateStep(step) {
        let isValid = true;
        const currentStepEl = document.querySelector(`.step[data-step="${step}"]`);

        // تحقق من الحقول المطلوبة
        currentStepEl.querySelectorAll('[required]').forEach(field => {
            if (!field.value.trim()) {
                field.classList.add('is-invalid');
                isValid = false;
            } else {
                field.classList.remove('is-invalid');
            }
        });

        // تحقق إضافي للخطوة 2 (الخريطة)
        if (step === 2) {
            if (typeof marker === 'undefined' || marker === null) {
                alert('الرجاء تحديد موقع التسليم على الخريطة');
                isValid = false;
            }
        }

        return isValid;
    }

    async function submitForm() {
        const formData = new FormData(form);

        // ✅ تحقق إضافي هنا قبل الإرسال
        const expressCheckbox = document.getElementById('expressDelivery');
        const selectedPaymentMethod = document.querySelector('input[name="PaymentMethod"]:checked');

        // تحقق إذا كان تسليم سريع ولازم طريقة دفع
        if (expressCheckbox.checked && !selectedPaymentMethod) {
            showAlert('يرجى اختيار طريقة الدفع للتسليم السريع.');
            return;
        }


        try {
            const response = await fetch('/Recycling/Create', {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                window.location.href = result.redirect;
            } else {
                showAlert(result.error);
            }
        } catch (error) {
            console.error('Error:', error);
            showAlert('حدث خطأ غير متوقع، حاول لاحقًا.');
        }
    }

    function showAlert(message) {
        Swal.fire({
            icon: 'error',
            title: 'خطأ',
            text: message,
            confirmButtonText: 'حسناً'
        });
    }



    nextBtn.addEventListener('click', function () {
        if (currentStep < steps.length) {
            if (validateStep(currentStep)) {
                currentStep++;
                showStep(currentStep);
            }
        } else {
            submitForm();
        }
    });

    prevBtn.addEventListener('click', function () {
        if (currentStep > 1) {
            currentStep--;
            showStep(currentStep);
        }
    });

    // تهيئة أولية
    showStep(1);
});
