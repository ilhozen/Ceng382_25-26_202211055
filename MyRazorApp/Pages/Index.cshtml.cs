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

// This is your task:
// JSON Export Feature (New Task)
// • Add a new button to export the data to JSON. There should be two modes:
// o Unfiltered export (exports the entire data)
// o Filtered export (exports only the currently filtered rows)
// • Also add the ability to select specific columns for export:
// o If no column is selected, export all columns.
// o If certain columns (e.g., 1st and 4th) are selected, export only those columns.
// o The selected columns should visually change color to indicate selection.
// o The exported JSON should contain only the selected column data.
// 4. Utility Class for JSON Export
// • Create a new C# class file named Utils.cs.
// • Inside it, implement a generic method that can export any class to JSON.
// • The method should work with any model class.
// • This class must be implemented as a singleton, so it can be accessed from anywhere in the
// project.
// 5. Folder Structure Reminder (MVP)
// Since your project follows the MVP structure in a Razor Pages application:
// • Place the ClassInformationTable and related data models in the Models folder.
// • Place the Utils.cs class in a separate folder called Helpers or Utilities.
// • Place pagination logic, filtering logic, and UI-related code in the appropriate Pages folder.
// This is the code you are starting with:
// --source code--

// In this code instead of having checkboxes for the columns I want the columns 
//themselves to be clickable and when clicked I want the entire column to change
// color. The json export should still be functional.

// I need the entire column to change color instead of just the header when it is selected.

// I want to get rid of the export filtered button so that it exports filtered automatically if
//something is filtered and all if nothing is filtered. so basically I want it to export whatever 
//is visible to the user at that moment.

// The code currently downloads a json file. Now I want it to write to a json file in the file structure of the project.

// I want to get rid of the filter button so it automatically filters as I type.

// In my Razor Pages Class management system project I had been keeping everything in a list now I want to change
// it to use an sql database instead. When deleting an item I would like to set isActive to false and make it so 
//that doesnt show up on the page but that deleted class should still be in the database.
// Here is the example code to be added given by our instructor you may reference this but dont put too much trust in it:
//week9 pdf example code
//my source code
//Change the index.cshtml.cs file to use the database instead of the list. I don't know if the classinformationmodel 
//and table files are relevant but I gave them for context.
//Change what needs to be changed.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using MyRazorApp.Helpers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;
using MyRazorApp.Data;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;
        private readonly SchoolDbContext _context;

        public IndexModel(IWebHostEnvironment env, SchoolDbContext context)
        {
            _env = env;
            _context = context;
        }

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        [BindProperty(SupportsGet = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Filter { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

        public List<ClassInformationTable> DisplayList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {

            await GenerateSyntheticDataAsync();

            if (!IsAuthenticated())
                return RedirectToPage("Login");

            await UpdateDisplayListAsync();

            if (!EditId.HasValue)
            {
                if (ClassInfo == null || ClassInfo.Id == 0)
                {
                    ClassInfo = new ClassInformationModel();
                    ModelState.Clear();
                }
            }
            else
            {
                var classToEdit = await _context.Classes.FindAsync(EditId.Value);
                if (classToEdit != null)
                {
                    ClassInfo = new ClassInformationModel
                    {
                        Id = classToEdit.Id,
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
                }
                else
                {
                    TempData["ErrorMessage"] = "The item you were trying to edit could not be found.";
                    EditId = null;
                    ClassInfo = new ClassInformationModel();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await UpdateDisplayListAsync();
                return Page();
            }

            bool isUpdate = EditId.HasValue;

            if (isUpdate)
            {
                var existing = await _context.Classes.FindAsync(EditId.Value);
                if (existing != null)
                {
                    existing.ClassName = ClassInfo.ClassName;
                    existing.StudentCount = ClassInfo.StudentCount;
                    existing.Description = ClassInfo.Description;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Class updated successfully.";
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The item you were trying to edit could not be found. It might have been deleted.");
                    await UpdateDisplayListAsync();
                    return Page();
                }
                EditId = null;
            }
            else
            {
                var newClass = new Class
                {
                    ClassName = ClassInfo.ClassName,
                    StudentCount = ClassInfo.StudentCount,
                    Description = ClassInfo.Description,
                    IsActive = true
                };
                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class added successfully.";
            }

            ClassInfo = new ClassInformationModel();
            ModelState.Clear();

            return RedirectToPage(new { Filter, PageNumber });
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            Filter ??= string.Empty;

            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit != null)
            {
                ClassInfo = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
                EditId = id;
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to edit was not found.";
                return RedirectToPage(new { Filter, PageNumber });
            }

            await UpdateDisplayListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            Filter ??= string.Empty;

            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                classToDelete.IsActive = false;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "The item you tried to delete was not found.";
            }

            await UpdateDisplayListAsync();

            int pageNum = PageNumber;
            if (pageNum > TotalPages && TotalPages > 0)
            {
                pageNum = TotalPages;
            }
            else if (TotalPages == 0)
            {
                pageNum = 1;
            }

            return RedirectToPage(new { Filter, PageNumber = pageNum });
        }

        public async Task<IActionResult> OnPostExportJson(string selectedColumns = "")
        {
            try
            {
                var data = await GetFilteredDataAsync();
                var columns = string.IsNullOrEmpty(selectedColumns)
                    ? new List<string>()
                    : selectedColumns.Split(',').ToList();

                string json = Utils.Instance.ExportToJson(data, columns);

                var exportDir = Path.Combine(_env.ContentRootPath, "Exports");
                Directory.CreateDirectory(exportDir);

                var fileName = $"class-export-{DateTime.Now:yyyyMMdd-HHmmss}.json";
                var filePath = Path.Combine(exportDir, fileName);

                System.IO.File.WriteAllText(filePath, json);

                TempData["SuccessMessage"] = $"File exported successfully to Exports folder.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error exporting file: {ex.Message}";
            }

            return RedirectToPage(new { Filter, PageNumber });
        }

        private async Task GenerateSyntheticDataAsync()
        {
            if (!await _context.Classes.AnyAsync())
            {
                var classes = new List<Class>();
                for (int i = 1; i <= 105; i++)
                {
                    classes.Add(new Class
                    {
                        ClassName = $"Class {i:000}",
                        StudentCount = (i % 15) + 5,
                        Description = $"Description for Class {i:000}",
                        IsActive = true
                    });
                }
                await _context.Classes.AddRangeAsync(classes);
                await _context.SaveChangesAsync();
            }
        }
        private async Task UpdateDisplayListAsync()
        {
            string currentFilter = Filter ?? string.Empty;

            IQueryable<Class> query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(currentFilter))
            {
                string lowerFilter = currentFilter.ToLowerInvariant();
                query = query.Where(c =>
                    (c.ClassName != null && c.ClassName.ToLower().Contains(lowerFilter)) ||
                    (c.Description != null && c.Description.ToLower().Contains(lowerFilter))
                );
            }

            TotalItems = await query.CountAsync();

            var pagedList = await query
                .OrderBy(c => c.Id)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            DisplayList = pagedList.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }

        private async Task<List<ClassInformationModel>> GetFilteredDataAsync()
        {
            IQueryable<Class> query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                string lowerFilter = Filter.ToLowerInvariant();
                query = query.Where(c =>
                    (c.ClassName != null && c.ClassName.ToLower().Contains(lowerFilter)) ||
                    (c.Description != null && c.Description.ToLower().Contains(lowerFilter))
                );
            }

            var classes = await query.ToListAsync();

            return classes.Select(c => new ClassInformationModel
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();
        }

        private bool IsAuthenticated()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var cookieUsername = Request.Cookies["username"];
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieToken = Request.Cookies["token"];
            var sessionId = HttpContext.Session.GetString("session_id");
            var cookieSessionId = Request.Cookies["session_id"];

            return sessionUsername != null &&
                   cookieUsername == sessionUsername &&
                   cookieToken == sessionToken &&
                   cookieSessionId == sessionId;
        }
    }
}