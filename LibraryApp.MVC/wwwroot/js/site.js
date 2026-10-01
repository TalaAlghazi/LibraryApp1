document.addEventListener('click', function (event) {
    const toggle = event.target.closest('[data-toggle-password]');
    if (!toggle) {
        return;
    }

    const input = document.getElementById(toggle.getAttribute('data-toggle-password'));
    if (!input) {
        return;
    }

    const show = input.type === 'password';
    input.type = show ? 'text' : 'password';
    toggle.classList.toggle('is-visible', show);
    toggle.setAttribute('aria-pressed', show ? 'true' : 'false');
    toggle.setAttribute('aria-label', show ? 'Hide password' : 'Show password');
});