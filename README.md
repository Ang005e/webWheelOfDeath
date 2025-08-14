# Developer Handover Notes

---


### OVERVIEW
This project is a Fullstack SPA split into 4 Visual Studio projects, under one solution (webWheelOfDeath).
1. LibEntity (aka LibEntity.NetCore) -- class library. Base CRUDS framework.
2. LibWheelOfDeath -- class library. CRUDS functions that map to specific database entities.
3. UnitTests -- MSTest unit test project. 
4. webWheelOfDeath -- ASP .Net Core MVC web application.

### GENERAL CONVENTIONS
Largely, each partial view, class, interface, and controller class is contained to a single file. 
All partial view files are prefixed with a "_".
All controllers and corresponding files are named "{Controllername}Controller"
All interfaces are prefixed with "I" (i.e., ICredentials). Some are paired with a class, in its file.
All enums are prefixed with "Enum" (i.e. EnumStatus). They'll be located with a class in the most relevant file (under the same namespace).
Most classes and corresponding files are prefixed with "C" (i.e. CEntity, CAppModals). This is consistent across JavaScript and C# classes.

## Frontend

### Application information

#### Upfront: please note that at this stage, all validation logic is being moved closer to the frontend (the Model classes). There may be deprecated artifacts floating around LibEntity (under the LibEntity.NetCore.Infrastructure namespace).

#### Innovation: Removal of in-document <script>s via custom tag attributes & JavaScript (CAjaxNavigator)
Throughout the app, you'll see \<a> links, \<button>s, and \<form>s with (custom) data attributes, including:
- data-ajax-nav
- data-ajax-form
- data-action
- data-target
- data-url
...and many, many, many commented out <script> sections.
These [data-xyz] thingies are custom attributes that I added, alongside a JS class (CAjaxNavigator) which, in tandem, remove 85% of the tedium in setting up AJAX. No more countless \<script> tags... just a single, central, smart JS object attached to the window which collects all events on these elements and reroutes them depending on A: which tags are attached, and B: the data those tags contain (i.e. names of actions).
I am currently rerouting my save logic through the tag system too--as the logic is different (a progress/confirmation modal is displayed).

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
