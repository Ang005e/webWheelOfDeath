
// Centralized AJAX navigation handler for all partial views
// Please note this class was written with assistance from the Claude.ai tool
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
    }

    handleNavigation($element) {
        const nav = this.parseNavData($element);

        // Build URL based on convention or explicit URL
        const url = nav.url || this.buildUrl(nav);

        // Get form data if this is within a form context
        const formData = nav.includeForm ? $element.closest('form').serialize() : null;

        // Import and use your existing partialLoader
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
        // If explicit URL provided, use it
        if (nav.url) return nav.url;

        // Get controller from nav data or session
        const controller = nav.controller || this.getSessionController();

        // Build URL based on your convention
        if (nav.model && nav.action) {
            // Your proposed convention: Controller/Model_Action
            return `/${controller}/${nav.model}_${nav.action}`;
        }

        // Standard MVC convention with optional ID
        const action = nav.action || 'Index';
        const url = `/${controller}/${action}`;

        return nav.id ? `${url}/${nav.id}` : url;
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

// Initialize on document ready
$(document).ready(() => {
    window.ajaxNav = new AjaxNavigator();
});










