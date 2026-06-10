// Register Form Validation
document.addEventListener('DOMContentLoaded', function() {
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            if (!validateForm()) {
                e.preventDefault();
                return false;
            }
            const submitBtn = document.getElementById('submitBtn');
            const spinner = document.getElementById('spinner');
            if (submitBtn) submitBtn.disabled = true;
            if (spinner) spinner.style.display = 'inline-block';
        });
    }

    // Full Name validation
    const fullNameInput = document.getElementById('fullName');
    if (fullNameInput) {
        fullNameInput.addEventListener('blur', function() {
            const fullNameError = document.getElementById('fullNameError');
            if (this.value.trim().length < 3) {
                fullNameError.textContent = 'Họ tên phải từ 3 ký tự trở lên';
            } else {
                fullNameError.textContent = '';
            }
        });
    }

    // Email validation
    const emailInput = document.getElementById('email');
    if (emailInput) {
        emailInput.addEventListener('blur', function() {
            const emailError = document.getElementById('emailError');
            if (this.value && (!this.value.includes('@') || !this.value.includes('.'))) {
                emailError.textContent = 'Email không hợp lệ';
            } else {
                emailError.textContent = '';
            }
        });
    }

    // Phone Number validation
    const phoneInput = document.getElementById('phoneNumber');
    if (phoneInput) {
        phoneInput.addEventListener('blur', function() {
            const phoneError = document.getElementById('phoneError');
            if (this.value && (this.value.length !== 10 || !this.value.startsWith('0'))) {
                phoneError.textContent = 'Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số';
            } else {
                phoneError.textContent = '';
            }
        });
    }

    // Password validation
    const passwordInput = document.getElementById('password');
    if (passwordInput) {
        passwordInput.addEventListener('blur', function() {
            const passwordError = document.getElementById('passwordError');
            if (this.value && this.value.length < 6) {
                passwordError.textContent = 'Mật khẩu phải có ít nhất 6 ký tự';
            } else {
                passwordError.textContent = '';
            }
        });
    }

    // Address validation
    const addressInput = document.getElementById('address');
    if (addressInput) {
        addressInput.addEventListener('blur', function() {
            const addressError = document.getElementById('addressError');
            if (!this.value.trim()) {
                addressError.textContent = 'Vui lòng nhập địa chỉ';
            } else {
                addressError.textContent = '';
            }
        });
    }
});

function validateForm() {
    let isValid = true;

    // Validate Full Name
    const fullName = document.getElementById('fullName').value.trim();
    const fullNameError = document.getElementById('fullNameError');
    if (fullName.length < 3) {
        fullNameError.textContent = 'Họ tên phải từ 3 ký tự trở lên';
        isValid = false;
    } else {
        fullNameError.textContent = '';
    }

    // Validate Email
    const email = document.getElementById('email').value.trim();
    const emailError = document.getElementById('emailError');
    if (!email.includes('@') || !email.includes('.')) {
        emailError.textContent = 'Email không hợp lệ';
        isValid = false;
    } else {
        emailError.textContent = '';
    }

    // Validate Phone Number
    const phoneNumber = document.getElementById('phoneNumber').value.trim();
    const phoneError = document.getElementById('phoneError');
    if (phoneNumber.length !== 10 || !phoneNumber.startsWith('0')) {
        phoneError.textContent = 'Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số';
        isValid = false;
    } else {
        phoneError.textContent = '';
    }

    // Validate Password
    const password = document.getElementById('password').value;
    const passwordError = document.getElementById('passwordError');
    if (password.length < 6) {
        passwordError.textContent = 'Mật khẩu phải có ít nhất 6 ký tự';
        isValid = false;
    } else {
        passwordError.textContent = '';
    }

    // Validate Address
    const address = document.getElementById('address').value.trim();
    const addressError = document.getElementById('addressError');
    if (!address) {
        addressError.textContent = 'Vui lòng nhập địa chỉ';
        isValid = false;
    } else {
        addressError.textContent = '';
    }

    return isValid;
}
