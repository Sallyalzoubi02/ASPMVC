const faqItems = document.querySelectorAll('.faq-item');

faqItems.forEach(item => {
    const question = item.querySelector('.faq-question');
    const answer = item.querySelector('.faq-answer');
    const icon = item.querySelector('.faq-icon');

    question.addEventListener('click', () => {
        const isOpen = answer.classList.contains('open');

        // Close all answers
        document.querySelectorAll('.faq-answer').forEach(a => a.classList.remove('open'));
        document.querySelectorAll('.faq-icon').forEach(i => i.innerHTML = '<img src="/image/plus-square.png" alt="">');

        if (!isOpen) {
            answer.classList.add('open');
            icon.innerHTML = '<img src="/image/minus-square.png" alt="">';
        }
    });
});

$(document).ready(function () {
    $('form').submit(function (e) {
        e.preventDefault(); // منع الإرسال التقليدي للفورم

        var form = $(this);
        var url = form.attr('action');

        // عرض رسالة تحميل
        Swal.fire({
            title: 'جاري الإرسال',
            html: 'Please wait...',
            allowOutsideClick: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        $.ajax({
            type: "POST",
            url: url,
            data: form.serialize(),
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                Swal.close();

                // عرض رسالة النجاح مع تصميم عربي
                Swal.fire({
                    icon: 'success',
                    title: 'تم بنجاح',
                    text: 'تم إرسال رسالتك بنجاح، سنتواصل معك قريباً!',
                    confirmButtonText: 'حسناً',
                    confirmButtonColor: '#28a745',
                    customClass: {
                        confirmButton: 'btn-arabic'
                    }
                });

                // تفريغ حقول الفورم
                form.trigger('reset');
            },
            error: function (xhr) {
                Swal.close();

                // عرض رسالة الخطأ مع تفاصيل
                Swal.fire({
                    icon: 'error',
                    title: 'خطأ',
                    text: xhr.responseJSON && xhr.responseJSON.error
                        ? xhr.responseJSON.error
                        : 'حدث خطأ أثناء الإرسال. الرجاء المحاولة لاحقاً',
                    confirmButtonText: 'حسناً',
                    confirmButtonColor: '#dc3545',
                    customClass: {
                        confirmButton: 'btn-arabic'
                    }
                });
            }
        });
    });
});
