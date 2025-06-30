# Developer Handover Notes

---

## Frontend

### Application information

#### Innovation: Tag attributes & CAjaxNavigator
Throughout the app, you'll see \<a> links, \<button>s, and \<form>s with (custom) data attributes, including:
- data-ajax-nav
- data-ajax-form
- data-action
- data-target
- data-url
...and so on.
These are custom attributes that I added, alongside a JS class (CAjaxNavigator) which, in tandem, remove 85% of the tedium in setting up AJAX. No more countless \<script> tags... just a single, smart JS object attached to the window.

#### Integral setup
- ViewBag.IsLoggedIn must be set to `true` when a user successfully authenticates.
	- It is used by ajax to know when to display/hide content.

#### _Layout partial setup
- window.sessionController must be set upon controller domain change (so, between the admin and game pages).
	- It’s used (in a js module, CAjaxNavigator) when building URLs for ajax navigation & form submission.
- Any shared CSS should be imported in the _Layout for each major domain page.

##### Global Script Imports 
**The following scripts MUST be imported in each _Layout:**
JQuery 
 <script src="~/lib/jquery/dist/jquery.min.js"></script>
AJAX
<script src="/js/ajaxHelpers/partialLoader.js" type="module"></script>
<script src="/js/ajaxHelpers/saveHandler.js" type="module"></script>
<script src="/js/ajaxHelpers/CAjaxNavigator.js" type="module"></script>
Modals
<script src="/js/CAppModals.js" type="module"></script>

#### Use of session variables
- ViewData and ViewBag are used only for temporary (single-response) communication--NOT storage.
- HttpContext.Session (controllers)/@Context.Session (partials) are to be used for multi-request storage.

---

## Database

### Database concerns
The database shall primarily be concerned with enforcing vital data structure and safety constraints (such as foreign key constraints, unique field check constraints, and type checking/field character limits)
Account Type Tables
Each separated account type will have a corresponding table which inherits from the base Account table (tblAccount). The ID of each field in the inheriting tables shall be equivalent to the ID of the corresponding tblAccount field.
I.e. John Doe signs up as a Player with username JDoe. If his field in tblAccount consists of ID=97, FirstName=John, LastName=Doe, password=123456789Jd!, the tblPlayer field MUST consist of ID=97 (THE SAME ID), Username=JDoe.
**PLEASE NOTE**
Each “separated account type” defines (for example) the separation between “Players” and “Administrators”. It does not cover Admin sub-types (i.e. standard admin, super admin).
Database Unit Tests
Unit tests will be written in Visual Studio, using a visual studio database project, to isolate production code from unit tests.
The LocalDB database should be linked to the project, so the database schema can be synced, a separate database initialized (for tests), and unit tests automatically run when changes are synced.

---

## API (class library)

### API concerns
The API shall be concerned with enforcing higher-level data requirements, not so much related to database structure. For example, input validation such as password strength rules, field length rules (relating to field character limits in the database schema and client requirements), and filtering rules.
##### A CValidator class system will be implemented to allow central rule-setting and validation.