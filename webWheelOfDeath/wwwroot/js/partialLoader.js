
/**
 * Send form data via AJAX, update a target element, and dispatch a refresh event.
 *
 * @param {string} actionUrl         – URL to POST the form to
 * @param {string} refreshElementId  – jQuery selector for the element to replace
 * @param {string} formName          – identifier passed in the CustomEvent detail
 */
export function partialLoader(actionUrl, refreshElementId, formName) {
    // ToDo: replace with Javascript fetch
    $.ajax({
        url: actionUrl,  // URL to the Login action
        type: 'POST',
        data: $(this).serialize(),  // Serialize the form data
        success: function (result) {
            // Update the modal content with the returned partial view HTML
            $(refreshElementId).html(result);
            document.dispatchEvent(new CustomEvent("partial-refresh", {
                bubbles: true,
                detail:
                {
                    form: formName
                }
            }));
        },
        error: function () {
            alert("There was an error processing your request.");
        }
    });
}