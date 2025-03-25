// Step 2 – Project Structure and Model Class
// • This task will be implemented on the Index page (Index.cshtml and Index.cshtml.cs).
// • Create a folder named Models inside the project folder (note: folder name is plural).
// • Inside this folder, create a class named ClassInformationModel.cs.
// • This class will store the following properties:
// o Id (auto-incremented)
// o ClassName
// o StudentCount
// o Description
// The Id property will be automatically incremented each time a new item is added to the list. The list
// will act like a simple in-memory database

// Step 3 – Page Functionality
// • On the left side of the page, there will be a form that collects:
// o Class Name
// o Student Count
// o Description
// • On the right side, there will be a table that displays all the submitted class data.
// • The table will have the following columns:
// o Id
// o Class Name
// o Student Count
// o Description
// o Actions (Edit and Delete)
// The data should be validated and added to a static list each time the form is submitted. The data will
// then be displayed in the table.
// Step 4 – Requirements and Constraints
// • Use Bootstrap to create a responsive layout with two columns (form on the left, table on the
// right).
// • Use Razor Pages only; no JavaScript is allowed.
// • All operations (Add, Edit, Delete) must be handled using C# methods in the PageModel.
// • Form validation should be done using C# attributes like [Required], [Range], etc.
// • When editing, pre-fill the form with the selected item's data.
// • After deletion or editing, refresh the page and update the table accordingly.
//the id isnt updating 1 by 1
using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Student Count is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Student count must be at least 1")]
        public int StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}
