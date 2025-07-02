
//import { CMessageModal } from './CAppModals.js';


// Centralised AJAX navigation handler for all partial views.
// Please note this class was written with assistance from the Claude.ai tool. I did not wirte this code from scratch.
// However, I have made significant modifications to the original code to suit my needs. And fixed all the bugs...
export class AjaxNavigator {

    constructor() {
        this.defaultTarget = '#page-content';
        this.init();
    }

    init() {
        // Use event delegation to handle all ajax navigation links
        $(document).on('click', '[data-ajax-nav]', (e) => {
            e.preventDefault();
            this.handleNavigation($(e.currentTarget));
        });

        // Handle forms with ajax submission
        $(document).on('submit', '[data-ajax-form]', (e) => {
            e.preventDefault();
            this.handleFormSubmit($(e.currentTarget));
        });

        // Handle save buttons
        $(document).on('click', '[data-ajax-save]:not([data-action="SaveGameRecord"])', (e) => {
            
            e.preventDefault();
            this.handleSave($(e.currentTarget));
        });

        $(document).on('click', '[data-ajax-save][data-action="SaveGameRecord"]', function (e) {
            e.preventDefault();
            e.stopPropagation();

            if (!window.lastGameResult) {
                alert('No game result to save');
                return;
            }

            $.ajax({
                url: '/Game/SaveGameRecord',
                type: 'POST',
                data: window.lastGameResult,
                contentType: 'application/x-www-form-urlencoded', // Explicitly set content type
                dataType: 'json',
                success: function (response) {
                    if (response.success) {
                        new window.CMessageModal('#modal-message-id').display("Game saved!", false, 3000);
                        new CustomEvent('game-save-success') // to pick up in the doc and try to disable the save button
                    } else {
                        new window.CMessageModal('#modal-message-id').display("Save failed: " + response.message, false, 5000);
                    }
                },
                error: function (xhr, status, error) {
                    console.error('Save error:', error);
                    new window.CMessageModal('#modal-message-id').display("Save failed: " + error, false, 5000);
                }
            });
        });
    }

    handleNavigation($element) {
        const nav = this.parseNavData($element);

        // Build URL based on convention or explicit URL
        const url = nav.url || this.buildUrl(nav);

        // Get form data if this is within a form context
        const formData = nav.includeForm ? $element.closest('form').serialize() : null;

        // Import and use partialLoader
        import('./partialLoader.js').then(module => {
            module.partialLoader(
                formData,
                url,
                nav.target || this.defaultTarget,
                nav.refreshEvent || 'nav-refresh',
                nav.method === 'POST'
            );
        });
    }

    handleFormSubmit($form) {
        const nav = this.parseNavData($form);
        const url = nav.url || $form.attr('action');
        const formData = $form.serialize();

        import('./partialLoader.js').then(module => {
            module.partialLoader(
                formData,
                url,
                nav.target || this.defaultTarget,
                nav.refreshEvent || 'form-refresh',
                true // forms are always POST
            );
        });
    }

    handleSave($button) {
        const modal = new window.CMessageModal('#modal-message-id');
        const formData = $button.closest('form').serialize();
        const action = $button.data('action') || 'Save';
        const controller = $button.data('controller') || window.currentController;
        const url = `/${controller}/${action}`;

        modal.display("Saving...", false);

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (response) {
                $button.prop('disabled', true); // disable after save is complete, to stop dupes
                modal.display("Saved!", false, 3000);
            },
            error: function () {
                modal.display("Save failed", false, 5000);
            }
        });
    }

    handleAjaxError(xhr, status, error, url) {
        // Use the central feedback manager
        FeedbackManager.error(`Request failed: ${error}`, 7000);
    }


    parseNavData($element) {
        // Parse all data-nav-* attributes
        const data = {};
        $.each($element[0].attributes, function () {
            if (this.name.startsWith('data-nav-')) {
                const key = this.name.substring(9).replace(/-(.)/g, (m, g) => g.toUpperCase());
                data[key] = this.value;
            }
        });

        // Also check for shorthand data attributes
        return {
            controller: data.controller || $element.data('controller'),
            action: data.action || $element.data('action'),
            id: data.id || $element.data('id'),
            model: data.model || $element.data('model'),
            target: data.target || $element.data('target'),
            url: data.url || $element.data('url'),
            method: data.method || $element.data('method') || 'GET',
            includeForm: data.includeForm === 'true' || $element.data('include-form'),
            refreshEvent: data.refreshEvent || $element.data('refresh-event')
        };
    }

    buildUrl(nav) {
        // If an explicit URL provided, use it
        if (nav.url) return nav.url;

        // Get controller from nav data or session
        const controller = nav.controller || this.getSessionController();

        // Build URL based on convention
        const action = nav.action || 'Index';
        let url = `/${controller}/${action}`;

        // Add ID as query parameter if present
        if (nav.id) {
            url += `?id=${nav.id}`;
        }

        return url;
    }

    getSessionController() {
        if (window.currentController === 'Default' || window.currentController === null) {
            console.error('No controller set in AjaxNavigator. Please ensure a controller is being set correctly.');
            throw new Error('No controller set in AjaxNavigator. Please ensure a controller is being set correctly.');
        }
        return window.currentController;
    }

    setSessionController(controller) {
        window.currentController = controller;
    }
}

// Initialise on document ready
$(document).ready(() => {
    window.ajaxNav = new AjaxNavigator();
});