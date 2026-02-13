// Professional Toast Notification System
const NotificationSystem = (function () {
    'use strict';

    const toastElement = document.getElementById('notificationToast');
    let toastInstance = null;

    function init() {
        if (toastElement) {
            toastInstance = new bootstrap.Toast(toastElement, {
                animation: true,
                autohide: true,
                delay: 5000
            });
        }
    }

    function show(message, type = 'info') {
        if (!toastInstance) {
            init();
        }

        const toast = document.getElementById('notificationToast');
        const toastIcon = toast.querySelector('.toast-icon');
        const toastMessage = toast.querySelector('.toast-message');

        // Remove all previous type classes
        toast.classList.remove('bg-success', 'bg-danger', 'bg-info', 'bg-warning', 'text-white');

        // Set icon and colors based on type
        switch (type) {
            case 'success':
                toast.classList.add('bg-success', 'text-white');
                toastIcon.className = 'toast-icon me-2 fas fa-check-circle';
                toastInstance._config.delay = 4000;
                break;
            case 'error':
            case 'danger':
                toast.classList.add('bg-danger', 'text-white');
                toastIcon.className = 'toast-icon me-2 fas fa-exclamation-triangle';
                toastInstance._config.delay = 6000;
                break;
            case 'warning':
                toast.classList.add('bg-warning', 'text-dark');
                toastIcon.className = 'toast-icon me-2 fas fa-exclamation-circle';
                toast.querySelector('.btn-close').classList.remove('btn-close-white');
                toastInstance._config.delay = 5000;
                break;
            case 'info':
            default:
                toast.classList.add('bg-info', 'text-white');
                toastIcon.className = 'toast-icon me-2 fas fa-info-circle';
                toastInstance._config.delay = 4000;
                break;
        }

        // Ensure close button has proper class
        if (type !== 'warning') {
            toast.querySelector('.btn-close').classList.add('btn-close-white');
        }

        toastMessage.textContent = message;
        toastInstance.show();
    }

    function success(message) {
        show(message, 'success');
    }

    function error(message) {
        show(message, 'error');
    }

    function warning(message) {
        show(message, 'warning');
    }

    function info(message) {
        show(message, 'info');
    }

    // Initialize on page load
    document.addEventListener('DOMContentLoaded', init);

    // Public API
    return {
        show,
        success,
        error,
        warning,
        info
    };
})();

// Make it globally accessible
window.showNotification = NotificationSystem.show;
window.showSuccess = NotificationSystem.success;
window.showError = NotificationSystem.error;
window.showWarning = NotificationSystem.warning;
window.showInfo = NotificationSystem.info;
