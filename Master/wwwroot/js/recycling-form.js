document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('recyclingForm');
    const steps = document.querySelectorAll('.step');
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    let currentStep = 1;

    // دالة لتحديث ملخص الطلب
    function updateOrderSummary() {
        const quantity = document.querySelector('input[name="Quantity"]')?.value || '1';
        const isExpress = document.querySelector('input[name="IsExpress"]:checked') !== null;
        const paymentMethod = document.querySelector('input[name="PaymentMethod"]:checked')?.value || 'cash';

        // تحديث عناصر الملخص إذا كانت موجودة
        const summaryQuantity = document.getElementById('summaryQuantity');
        if (summaryQuantity) summaryQuantity.textContent = quantity;

        const summaryDeliveryType = document.getElementById('summaryDeliveryType');
        if (summaryDeliveryType) summaryDeliveryType.textContent = isExpress ? 'سريع' : 'عادي';

        const summaryPaymentMethod = document.getElementById('summaryPaymentMethod');
        if (summaryPaymentMethod) summaryPaymentMethod.textContent =
            paymentMethod === 'credit' ? 'بطاقة ائتمان' : 'الدفع عند الاستلام';

        const deliveryCost = isExpress ? '5 د.أ' : 'مجاني';
        const summaryDeliveryCost = document.getElementById('summaryDeliveryCost');
        if (summaryDeliveryCost) summaryDeliveryCost.textContent = deliveryCost;

        const summaryTotal = document.getElementById('summaryTotal');
        if (summaryTotal) summaryTotal.textContent = deliveryCost;
    }

    function showStep(step) {
        steps.forEach(s => s.classList.add('d-none'));
        document.querySelector(`.step[data-step="${step}"]`).classList.remove('d-none');

        prevBtn.style.display = step > 1 ? 'block' : 'none';
        nextBtn.textContent = step === steps.length ? 'إرسال الطلب' : 'التالي';

        // إصلاح أبعاد الخريطة عند الوصول للخطوة 2
        if (step === 2 && typeof map !== 'undefined' && map) {
            setTimeout(() => {
                map.invalidateSize();
            }, 300);
        }

        // تحديث الملخص عند الوصول للخطوة 3
        if (step === 3) {
            updateOrderSummary();
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
            const latitudeInput = document.getElementById('latitudeInput');
            if (!latitudeInput || !latitudeInput.value) {
                showAlert('الرجاء تحديد موقع التسليم على الخريطة');
                isValid = false;
            }
        }

        return isValid;
    }

    async function submitForm() {
        const formData = new FormData(form);
        const expressCheckbox = document.getElementById('expressDelivery');
        const selectedPaymentMethod = document.querySelector('input[name="PaymentMethod"]:checked');

        // تحقق إذا كان تسليم سريع ولم يتم اختيار طريقة دفع
        if (expressCheckbox && expressCheckbox.checked && (!selectedPaymentMethod || !selectedPaymentMethod.value)) {
            showAlert('يرجى اختيار طريقة الدفع للتسليم السريع');
            return;
        }

        try {
            nextBtn.disabled = true;
            nextBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> جاري الإرسال...';

            const response = await fetch(form.action || '/Recycling/Create', {
                method: 'POST',
                body: formData,
                headers: {
                    'Accept': 'application/json'
                }
            });

            const result = await response.json();

            if (result.success) {
                window.location.href = result.redirect || '/';
            } else {
                showAlert(result.message || result.error || 'حدث خطأ أثناء معالجة الطلب');
            }
        } catch (error) {
            console.error('Error:', error);
            showAlert('حدث خطأ غير متوقع، يرجى المحاولة لاحقاً');
        } finally {
            nextBtn.disabled = false;
            nextBtn.textContent = 'إرسال الطلب';
        }
    }

    function showAlert(message) {
        // استخدام SweetAlert إذا كان متاحاً أو alert عادي
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: message,
                confirmButtonText: 'حسناً'
            });
        } else {
            alert(message);
        }
    }

    // أحداث الاستماع
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

    // استمع لتغيير القيم المهمة
    document.addEventListener('change', function (e) {
        if (e.target.name === 'Quantity' || e.target.name === 'IsExpress' || e.target.name === 'PaymentMethod') {
            updateOrderSummary();
        }
    });

    // تهيئة أولية
    showStep(1);
});