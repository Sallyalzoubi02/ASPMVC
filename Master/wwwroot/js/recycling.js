
function showContent(id) {
    document.querySelectorAll('.recycle-content').forEach(content => {
        content.classList.remove('active');
    });
    document.getElementById(id).classList.add('active');
}

