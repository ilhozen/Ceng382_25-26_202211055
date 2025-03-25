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

// the id isnt updating 1 by 1
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using MyRazorApp.Models;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> ClassList = new();

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        public List<ClassInformationModel> DisplayList => ClassList;

        public void OnGet() { }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                var existing = ClassList.FirstOrDefault(c => c.Id == EditId);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                }
            }
            else
            {
                int newId = (ClassList.Count > 0) ? ClassList.Max(c => c.Id) + 1 : 1;
                
                ClassList.Add(new ClassInformationModel
                {
                    Id = newId,
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description
                });
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInfo = classToEdit;
                EditId = id;
            }
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
                ClassList.Remove(classToDelete);

            return RedirectToPage();
        }
    }
}
