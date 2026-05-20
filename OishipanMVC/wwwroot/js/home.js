// Add to cart function
function addToCart(cakeName) {
    const toastElement = document.getElementById('cartToast');
    const toastName = document.getElementById('toastCakeName');
    const toast = new bootstrap.Toast(toastElement);
    
    toastName.innerText = cakeName;
    toast.show();
    
    // Update cart badge
    const badge = document.querySelector('.cart-badge');
    badge.innerText = parseInt(badge.innerText) + 1;
}

// Smooth scroll effect for navbar
window.addEventListener('scroll', function() {
    const navbar = document.querySelector('.navbar');
    if (window.scrollY > 50) {
        navbar.classList.add('shadow-sm');
    } else {
        navbar.classList.remove('shadow-sm');
    }
});

// Initialize tooltips and popovers if needed
document.addEventListener('DOMContentLoaded', function() {
    // Bootstrap tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});
