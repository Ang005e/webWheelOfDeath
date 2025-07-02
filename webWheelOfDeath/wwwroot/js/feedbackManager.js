
// Monitors for, intercepts, and processes feedback (informational, warning, success, or error) messages,
// sent from the server, that are intended to be displayed to the user.
// Please note this class was written with assistance from the Claude.ai tool. I did not wirte this code from scratch.
// However, I sat very patiently and drunk some tea while I waited for it to generate. It was quite taxing.
export class CFeedbackManager {
    static instance = null;

    constructor() {
        if (CFeedbackManager.instance) {
            return CFeedbackManager.instance;
        }

        this.modal = new window.CMessageModal('#modal-message-id');
        CFeedbackManager.instance = this;

        // Auto-display on page load
        this.checkForServerFeedback();

        // Intercept AJAX responses
        this.interceptAjax();
    }

    display(message, type, duration = 5000) {
        // Convert enum to string if it's a number
        const typeString = this.getTypeString(type);

        // Apply CSS class based on type
        const modalPanel = document.querySelector('#modal-message-id .main-panel');
        if (modalPanel) {
            // Remove existing type classes
            modalPanel.classList.remove('modal-success', 'modal-error', 'modal-warning', 'modal-info');

            // Add new type class
            const cssClass = this.getFeedbackClass(typeString);
            modalPanel.classList.add(cssClass);
        }

        // Display the message
        this.modal.display(message, false, duration);

        // Log for debugging
        console.log(`[Feedback ${typeString}]: ${message}`);
    }

    checkForServerFeedback() {
        const feedbackEl = document.getElementById('server-feedback');
        if (feedbackEl) {
            // Use type name if available, otherwise use numeric value
            const type = feedbackEl.dataset.feedbackTypeName || feedbackEl.dataset.feedbackType;
            const message = feedbackEl.dataset.feedbackMessage;
            const duration = parseInt(feedbackEl.dataset.feedbackDuration);
        
            this.display(message, type, duration);
            feedbackEl.remove(); // Clean up
        }
    }

    interceptAjax() {
        // Override jQuery AJAX
        const originalAjax = $.ajax;
        $.ajax = (options) => {
            const originalSuccess = options.success;
            const originalError = options.error;

            options.success = (data, status, xhr) => {
                // Check for feedback in header
                const feedbackHeader = xhr.getResponseHeader('X-Feedback');
                if (feedbackHeader) {
                    const feedback = JSON.parse(feedbackHeader);
                    this.displayFeedback(feedback);
                }

                // Check for feedback in response body
                if (typeof data === 'string') {
                    const $html = $(data);
                    const feedbackEl = $html.find('#server-feedback');
                    if (feedbackEl.length) {
                        this.displayFromElement(feedbackEl[0]);
                    }
                }

                if (originalSuccess) originalSuccess(data, status, xhr);
            };

            return originalAjax.call(this, options);
        };
    }

    displayFeedback(feedback) {
        this.display(feedback.Message, feedback.Type, feedback.Duration);
    }

    // Helpers for adding manual feedback
    static success(message, duration) {
        new CFeedbackManager().display(message, 'Success', duration);
    }

    static error(message, duration) {
        new CFeedbackManager().display(message, 'Error', duration);
    }

    static warning(message, duration) {
        new CFeedbackManager().display(message, 'Warning', duration);
    }

    static info(message, duration) {
        new CFeedbackManager().display(message, 'Info', duration);
    }

    getTypeString(type) {
        // Handle both string and numeric enum values
        if (typeof type === 'string') return type;

        const typeMap = {
            0: 'None',
            1: 'Success',
            2: 'Warning',
            3: 'Error',
            4: 'Info'
        };
        return typeMap[type] || 'Info';
    }

    getFeedbackClass(type) {
        const classMap = {
            'Success': 'modal-success',
            'Error': 'modal-error',
            'Warning': 'modal-warning',
            'Info': 'modal-info',
            'None': 'modal-info'
        };
        return classMap[type] || 'modal-info';
    }
}

// Initialize on document ready
$(document).ready(() => {
    window.feedbackManager = new CFeedbackManager();
});