function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    if (!container || !message) return;

    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-bg-${type} border-0 show mb-2`;
    toast.role = 'alert';

    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${message}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto"></button>
        </div>
    `;

    toast.querySelector('button').onclick = () => toast.remove();
    container.appendChild(toast);

    setTimeout(() => toast.remove(), 3000);
}


// ---------------- TEMP DATA TOAST ----------------
(function() {
    const success = document.getElementById('tempdata-success')?.value;
    const error = document.getElementById('tempdata-error')?.value;

    if (success) showToast(success, 'success');
    if (error) showToast(error, 'danger');
})();

// ---------------- UNIVERSAL AJAX HANDLER ----------------
function handleAjaxForm(formSelector, onSuccess) {
    const form = typeof formSelector === 'string' ? document.querySelector(formSelector) : formSelector;
    if (!form) return;

    form.addEventListener('submit', async function(e) {
        e.preventDefault();

        const formData = new FormData(form);
        try {
            const response = await fetch(form.action, {
                method: form.method,
                body: formData,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            const data = await response.json();

            if (data.message) showToast(data.message, data.success ? 'success' : 'danger');

            if (data.success && onSuccess) onSuccess(data);

        } catch (err) {
            console.error(err);
            showToast('Произошла ошибка', 'danger');
        }
    });
}

