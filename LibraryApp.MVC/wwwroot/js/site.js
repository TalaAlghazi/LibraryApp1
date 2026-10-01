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

(function () {
    const modalEl = document.getElementById('confirmModal');
    if (!modalEl || !window.bootstrap) {
        return;
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
    const titleEl = modalEl.querySelector('.confirm-modal-title');
    const messageEl = modalEl.querySelector('.confirm-modal-message');
    const okButton = modalEl.querySelector('[data-confirm-ok]');
    let pendingForm = null;

    document.addEventListener('submit', function (event) {
        const form = event.target;
        if (!form.matches('form[data-confirm]')) {
            return;
        }

        event.preventDefault();
        pendingForm = form;

        const isDanger = form.dataset.confirmVariant === 'danger';
        titleEl.textContent = form.dataset.confirmTitle || 'Please confirm';
        messageEl.textContent = form.dataset.confirm;
        okButton.textContent = form.dataset.confirmOk || 'Confirm';
        okButton.classList.toggle('btn-danger', isDanger);
        okButton.classList.toggle('btn-primary', !isDanger);
        okButton.disabled = false;

        modal.show();
    });

    okButton.addEventListener('click', function () {
        if (!pendingForm) {
            return;
        }

        okButton.disabled = true;
        pendingForm.querySelectorAll('button[type=submit]').forEach(function (button) {
            button.disabled = true;
        });
        pendingForm.submit();
    });

    modalEl.addEventListener('shown.bs.modal', function () {
        okButton.focus();
    });

    modalEl.addEventListener('hidden.bs.modal', function () {
        pendingForm = null;
    });
})();