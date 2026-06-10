// Login Form Validation
document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const submitBtn = document.getElementById('submitBtn');
            const spinner = document.getElementById('spinner');
            if (submitBtn) submitBtn.disabled = true;
            if (spinner) spinner.style.display = 'inline-block';
        });
    }

    const emailInput = document.getElementById('email');
    if (emailInput) {
        emailInput.addEventListener('blur', function() {
            const email = this.value;
            const emailError = document.getElementById('emailError');
            
            if (email && (!email.includes('@') || !email.includes('.'))) {
                emailError.textContent = 'Email không hợp lệ';
            } else {
                emailError.textContent = '';
            }
        });
    }

    const passwordInput = document.getElementById('password');
    if (passwordInput) {
        passwordInput.addEventListener('blur', function() {
            const password = this.value;
            const passwordError = document.getElementById('passwordError');
            
            if (password && password.length < 6) {
                passwordError.textContent = 'Mật khẩu phải có ít nhất 6 ký tự';
            } else {
                passwordError.textContent = '';
            }
        });
    }
});
