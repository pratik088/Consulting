# Consulting

Ms Web Tech – Final Exam
A consulting company has you creating a web site to track contracts and the time worked on each contract.  Currently you’re doing the page to record the consultants’ time worked.  Each record has the consultant that did the work, the contract it is to be charged against, date & hours worked, rate charged and the total cost of that work session.
1.	Your initial project and database script is on your exam account’s G: drive.  Work on C: or G: but only what’s on the exam account’s G: drive can be marked. You’re responsible for copying the project from C: to G:.
a.	Create the consulting database from the SQL script.  The models, context, and Session support have already been added to the project.  
b.	In _layout.cshtml:
i.	Add a menu entry for the Contract controller … consider one for Consultant controller
ii.	Display the TempData variable “message” before the guest page, in bold & red.
iii.	Put your name and section in the footer. 
2.	Generate the Contract controller with full views and EF support for the contract table. 
a.	Index View: Add a hyperlink to each contract, passing its contractId to WorkSession’s Index action. 
3.	Generate the WorkSession controller with full views and EF support for the workSession table.
a.	If a contractId has been passed, persist it.  If not, and one has not been saved to a cookie or session variable, return to the contract listing and display a message requesting they select one.
b.	Filter the Index listing to workSessions for the selected contract.  
i.	In the heading, display total hours and total cost for all work sessions on this contract.
c.	On the Create view, do not display contractId, hourlyRate, provincialTax or totalChargeBeforeTax.  
i.	Replace the consultantId dropdown with a textbox to check your edits and add a span tag for any field validation errors.
d.	On the Edit view, do not allow users to edit tax, total or contract ID.
e.	Use a try-catch in all actions that modify database tables:
i.	If the action succeeds, display a message to that effect on the Index page
ii.	If not, re-display the user’s input and the innermost exception’s message, if one was thrown.
f.	On all views, modify the asp-validation-for span elements to display field errors.
4.	WorkSession edits must be performed through the model except for Remote & custom annotation code:
a.	Do not apply edits, annotations or property changes to the generated model or controller actions.  
i.	Use a metadata class to apply them to the model through a like-named partial class.
[ModelMetadataType(typeof(xxx))] might be useful
ii.	Add the following to the top of the partial class, to provide access to database tables:
ConsultingContext _context = new ConsultingContext();
b.	If the contract for this worksession record is closed, produce an edit error to prevent the record from being updated or added to the database.
c.	contractId and consultantId must be validated against the appropriate database tables.
d.	dateWorked is required and cannot be in the future nor before the contract start date.
e.	hoursWorked must be greater than zero and total no more than 24 for all workSession records for this consultant on the day worked (regardless of which contract they’re for).
f.	If creating a new workSession record, load the hourlyRate from the selected consultant’s record.  
i.	When editing an existing record, the user can modify hourlyRate, but it cannot exceed 1.5 times what’s on the consultant’s record, nor can it be less than zero.
g.	Compute the totalChargeBeforeTax using the hourlyRate and hoursWorked.
h.	Compute the provincialTax amount using the totalChargeBeforeTax and the tax rate from the customer’s province.  
_layout.cshtml:
•	TempData message variable is displayed before the guest page, red and bold
•	Student name & section is in the footer	

Contract controller
•	Is accessible from the menu
•	Passes the contractId of the selected record to the WorkSession’s Index action
WorkSession controller & views
•	Index page is limited to the selected contractId on initial load and on return from actions
o	If contractId was not passed or saved, displays a message on the contract listing
o	The heading shows total hours and total cost for the current contract.
•	Create & Edit do not display contractId, provincialTax or totalChargeBeforeTax
o	On Create, consultantId is a textbox, hourlyRate does not show; Edit can modify hourlyRate
•	Create, Edit & Delete work, displaying a message if successful 
•	Create, Edit & Delete show the innermost exception (if thrown) and edit errors, with user input	

workSession Edits: these must be in the metadata & partial class: 50% loss if done in controller
•	Annotations and edits are not added to the generated model or controller actions
o	Annotations are applied to model via a metadata class to a like-named partial class 
o	The like-named partial class is self-validating
•	consultantId and contractId are validated against the appropriate tables
•	dateWorked cannot be in the future nor before the contract start date
•	hoursWorked must be greater than zero
o	… and total no more than 24 on all workSession records for this consultant on this day
•	Code correctly determines if user is creating a new record or editing an existing one:
o	For a new record, the consultant’s current hourlyRate is retrieved & inserted
o	On Edit, hourlyRate came from the original workSession record, and can be changed to range from zero to 1.5 times consultant’s current rate, inclusive
•	totalChargeBeforeTax is calculated correctly & inserted into record
•	Correct tax rate is retrieved from the customer’s province
o	provincialTax amount is calculated correctly & inserted into record
•	If the contract is closed, an edit error to that effect is produced, stopping Edit & Create	
