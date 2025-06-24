

/*

courtesy of chatgpt.com

*/


// --------------------
// Form Field Validator
// --------------------

export default class FormValidator {
    #fields = new Map();          // id → {el, rules: []}
    #form = null;

    /**
     * @param {string} formId
     * @param  {...string} fieldIds  list of input element ids
     */
    constructor(formId, ...fieldIds) {
        this.#form = document.getElementById(formId);
        if (!this.#form) throw new Error(`Form '${formId}' not found`);

        fieldIds.forEach(id => {
            const el = document.getElementById(id);
            if (!el) throw new Error(`Element '${id}' not found`);
            this.#fields.set(id, { el, rules: [] });
        });

        // intercept submit
        this.#form.addEventListener('submit', e => {
            if (!this.validate()) e.preventDefault();
        });
    }

    /** Attach a rule to a field */
    addRule(fieldId, ruleFn) {
        const item = this.#fields.get(fieldId);
        if (!item) throw new Error(`Field '${fieldId}' not registered`);
        item.rules.push(ruleFn);
        return this; // chainable
    }

    /** Run all rules, colour inputs, write messages */
    validate() {
        let allOk = true;

        for (const [id, { el, rules }] of this.#fields) {
            const val = el.value;
            let error = null;

            for (const rule of rules) {
                error = rule(val, this.#collectValues());
                if (error) break;            // first failing rule wins
            }
            this.#setError(el, id, error);
            allOk &&= !error;
        }
        return allOk;
    }

    // Helpers --------------------------------------------------
    #collectValues() {
        const obj = {};
        for (const [id, { el }] of this.#fields) obj[id] = el;
        return obj;                    // map id → input element (not value) for cross-field rules
    }

    #setError(el, id, message) {
        el.classList.toggle('invalid', !!message);

        // Razor adds: <span data-valmsg-for="Id" ...>
        let span = this.#form.querySelector(`span[data-valmsg-for="${id}"]`);
        if (!span) {
            span = document.createElement('span');
            span.dataset.valmsgFor = id;
            span.className = 'text-danger';
            el.after(span);
        }
        span.textContent = message || '';
    }
}

// ------------------------
// Reusable rule factory functions
// ------------------------

export const required = (msg = 'Required') =>
    v => v.trim() ? null : msg;

export const maxLength = (n, msg) =>
    v => v.length <= n ? null : (msg || `Must be ≤ ${n} characters`);

export const minLength = (n, msg) =>
    v => v.length >= n ? null : (msg || `Must be ≥ ${n} characters`);

export const numeric = (msg = 'Must be a number') =>
    v => v === '' || !isNaN(Number(v)) ? null : msg;

export const minValue = (min, msg) =>
    v => v === '' || Number(v) >= min ? null : (msg || `Must be ≥ ${min}`);

export const maxValue = (max, msg) =>
    v => v === '' || Number(v) <= max ? null : (msg || `Must be ≤ ${max}`);

export const greaterThanField = (otherId, msg) =>
    (v, all) => {
        const ov = all[otherId]?.value;
        return (v !== '' && ov !== '' && Number(v) > Number(ov))
            ? null
            : (msg || `Must be greater than ${otherId}`);
    };
