import { partialLoader } from "/js/ajaxHelpers/partialLoader.js";

import $ from "jquery";
/** start the login/admin panel */
export function initLoginPanel(firstUrl) {
    $(document).ready(() =>
        partialLoader(null, firstUrl, "#page-content", "login-form", false));
}
