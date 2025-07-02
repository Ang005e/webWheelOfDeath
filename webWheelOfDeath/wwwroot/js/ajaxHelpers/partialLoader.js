
/**
 * Send form data via AJAX, update a target element, and dispatch a refresh event.
 *
 * @param {string} actionUrl     URL to POST the form to
 * @param {string} refreshElementId  jQuery selector for the element to replace
 * @param {string} formName      identifier passed in the CustomEvent detail
 * @param {bool} isPost   are you an idiot or do i have to annotate this for you?
 */
export function partialLoader(formData, actionUrl, refreshElementId, formName, isPost = false) {
    console.log('PartialLoader called:', { actionUrl, refreshElementId, formName, isPost });
    
    $.ajax({
        url: actionUrl,
        type: isPost ? 'POST' : 'GET',
        data: formData,
        success: function (partial) {
            $(refreshElementId).html(partial);
            document.dispatchEvent(new CustomEvent("partial-refresh", {
                bubbles: true,
                detail: { form: formName }
            }));
        },
        error: function (xhr, status, error) {
            // Use error handler
            if (window.ajaxNav && window.ajaxNav.handleAjaxError) {
                window.ajaxNav.handleAjaxError(xhr, status, error, actionUrl);
            } else {
                console.error('AJAX Error:', { xhr, status, error, url: actionUrl });
                new window.CMessageModal('#modal-message-id').display(
                    `Error: ${xhr.status} - ${xhr.statusText}\nURL: ${actionUrl}`, 
                    false, 
                    7000
                );
            }
        }
    });
}
