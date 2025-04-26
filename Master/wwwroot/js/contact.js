const faqItems = document.querySelectorAll('.faq-item');

faqItems.forEach(item => {
    const question = item.querySelector('.faq-question');
    const answer = item.querySelector('.faq-answer');
    const icon = item.querySelector('.faq-icon');

    question.addEventListener('click', () => {
        const isOpen = answer.classList.contains('open');

        // Close all answers
        document.querySelectorAll('.faq-answer').forEach(a => a.classList.remove('open'));
        document.querySelectorAll('.faq-icon').forEach(i => i.innerHTML = '<img src="~/image/plus-square.png" alt="">');

        if (!isOpen) {
            answer.classList.add('open');
            icon.innerHTML = '<img src="~/image/minus-square.png" alt="">';
        }
    });
});
