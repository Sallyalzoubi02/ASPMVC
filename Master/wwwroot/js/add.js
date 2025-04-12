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

var fastDelivery = document.getElementById("fastDelivery");
var paymentMethod = document.getElementById("paymentMethod");
var cardInfo = document.getElementById("cardInfo");

fastDelivery.addEventListener("change", function () {
    paymentMethod.disabled = !fastDelivery.checked;
});

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