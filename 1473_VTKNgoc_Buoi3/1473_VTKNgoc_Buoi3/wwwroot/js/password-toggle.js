document.querySelectorAll('.password-toggle-btn').forEach(button => {
    button.addEventListener('click', () => {
        const input = button.parentElement?.querySelector('.password-toggle-input');
        const icon = button.querySelector('i');

        if (!input) {
            return;
        }

        const showPassword = input.type === 'password';
        input.type = showPassword ? 'text' : 'password';
        button.setAttribute('aria-label', showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu');

        if (icon) {
            icon.classList.toggle('fa-eye', !showPassword);
            icon.classList.toggle('fa-eye-slash', showPassword);
        }
    });
});
