document.addEventListener("DOMContentLoaded", () => {
    const elements = document.querySelectorAll(".animate"); // اختيار العناصر
=
    setTimeout(() => {
        elements.forEach(element => {
            element.classList.add("visible"); // إضافة الـ class لجعل العنصر يظهر
        });
    }, 500); // وقت التأخير (500 مللي ثانية)
    
});
function showContent(id) {
    document.querySelectorAll('.recycle-content').forEach(content => {
        content.classList.remove('active');
    });
    document.getElementById(id).classList.add('active');
}

