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

// --source code here--
// This is my code for a razor pages project I need to add these:

// This week, you will improve the table you created last week in your Razor Pages project. You
// will add filtering and pagination features. However, filtering will be done on the data list in
// the backend, not on the frontend.
// The filtering logic must be written inside the OnGet methods.
// You will also create a new model class called ClassInformationTable. This model will store
// the filtered version of your main model and will be used to display data in the table. In this
// model, the ID should not be shown in the table, but the ID will still be used in the
// background for actions like edit, delete, or details.
// In addition to filtering, you are required to implement pagination. To properly test the
// pagination feature, you need to generate synthetic data. Make sure to create a list with at
// least 100 sample records so you can see how the pagination works across multiple pages.
// Tip :
// When a filter value changes, the form should submit automatically or the user should click a
// "Filter" button. This will trigger the OnGet method with the selected filter values passed as
// query parameters.
// What is Pagination?
// Pagination is the method of dividing a large list of data into smaller parts, called pages. Instead of
// showing all records at once, you only display a limited number of items per page. This improves
// performance and makes the user interface easier to use.
// For example, if you have 100 records and you show 10 per page, then there will be 10 pages in total.
// The user can navigate between these pages using buttons or links like “Previous”, “Next”, or by
// choosing a page number.
// The filtering and pagination must be done in the backend using LINQ, and the table must be updated
// according to the filtered and paginated data. All logic should be inside the OnGet function.

// My filter system doesnt work but when I fix it the add/update button doesnt work. completely replace the filtering in the code
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

// --- Page Model Definition ---
namespace MyRazorApp.Pages
{
    using MyRazorApp.Models;

    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> ClassList = new();
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        // --- Filtering and Pagination Properties ---
        [BindProperty(SupportsGet = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Filter { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

        public List<ClassInformationTable> DisplayList { get; set; } = new();

        public void OnGet()
        {
            if (!ClassList.Any())
            {
                GenerateSyntheticData();
            }
            UpdateDisplayList();

            if (!EditId.HasValue)
            {
                if (ClassInfo == null || ClassInfo.Id == 0) {
                     ClassInfo = new ClassInformationModel();
                     ModelState.Clear();
                }
            }
            else
            {
                 if (ClassInfo == null || ClassInfo.Id != EditId.Value)
                 {
                     var classToEdit = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                     if (classToEdit != null)
                     {
                         ClassInfo = classToEdit;
                     }
                     else
                     {
                         TempData["ErrorMessage"] = "The item you were trying to edit could not be found.";
                         EditId = null;
                         ClassInfo = new ClassInformationModel();
                     }
                 }
            }
        }

        // --- POST Handlers ---
        public IActionResult OnPostAdd()
        {

            if (!ModelState.IsValid)
            {
                UpdateDisplayList();
                return Page();
            }

            bool isUpdate = EditId.HasValue;

            if (isUpdate)
            {
                var existing = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                    TempData["SuccessMessage"] = "Class updated successfully.";
                }
                else
                {
                     ModelState.AddModelError(string.Empty, "The item you were trying to edit could not be found. It might have been deleted.");
                     UpdateDisplayList();
                     return Page();
                }
                EditId = null;
            }
            else // Add new item
            {
                int newId = ClassList.Any() ? ClassList.Max(c => c.Id) + 1 : 1;
                var newClass = new ClassInformationModel
                {
                    Id = newId,
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description
                };
                ClassList.Add(newClass);
                TempData["SuccessMessage"] = "Class added successfully.";
            }

            ClassInfo = new ClassInformationModel();
            ModelState.Clear();

            string currentFilter = this.Filter ?? string.Empty;
            int currentPage = this.PageNumber;

            return RedirectToPage(new { Filter = currentFilter, PageNumber = currentPage });
        }

        public IActionResult OnPostEdit(int id)
        {
            this.Filter ??= string.Empty;

            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInfo = classToEdit; 
                EditId = id; 
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to edit was not found.";
                string currentFilter = this.Filter ?? string.Empty;
                int currentPage = this.PageNumber;
  
                return RedirectToPage(new { Filter = currentFilter, PageNumber = currentPage });
            }

            UpdateDisplayList();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            this.Filter ??= string.Empty;

            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassList.Remove(classToDelete);
                TempData["SuccessMessage"] = "Class deleted successfully.";
            }
            else
            {
                 TempData["ErrorMessage"] = "The item you tried to delete was not found.";
            }

            UpdateDisplayList();

            int pageNum = this.PageNumber;
            if (pageNum > TotalPages && TotalPages > 0)
            {
                pageNum = TotalPages;
            }
            else if (TotalPages == 0)
            {
                pageNum = 1;
            }


            string currentFilter = this.Filter ?? string.Empty;
            return RedirectToPage(new { Filter = currentFilter, PageNumber = pageNum });
        }

        // --- Helper Methods ---
        private void GenerateSyntheticData()
        {
            ClassList = new List<ClassInformationModel>();
            for (int i = 1; i <= 105; i++)
            {
                ClassList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i:000}",
                    StudentCount = (i % 15) + 5,
                    Description = $"Description for Class {i:000}"
                });
            }
        }

        private void UpdateDisplayList()
        {
            string currentFilter = this.Filter ?? string.Empty;

            IQueryable<ClassInformationModel> query = ClassList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(currentFilter))
            {
                string lowerFilter = currentFilter.ToLowerInvariant();
                query = query.Where(c =>
                    (c.ClassName != null && c.ClassName.ToLowerInvariant().Contains(lowerFilter)) ||
                    (c.Description != null && c.Description.ToLowerInvariant().Contains(lowerFilter))
                );
            }

            TotalItems = query.Count();
            query = query.OrderBy(c => c.Id);

            // Apply pagination
            List<ClassInformationModel> pagedList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();


            DisplayList = pagedList.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }
    }
}