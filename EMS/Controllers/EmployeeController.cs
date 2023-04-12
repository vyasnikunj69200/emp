using EMS.Data;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var employees = from e in _context.Employees
                            select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e => e.Name.Contains(searchString) ||
                e.Address.Contains(searchString) ||
                e.Gender.Contains(searchString)
                );

            }

            switch (sortOrder)
            {
                case "name_desc":
                    employees = employees.OrderByDescending(e => e.Name);
                    break;
                case "Date":
                    employees = employees.OrderBy(e => e.RecordCreatedOn);
                    break;
                case "date_desc":
                    employees = employees.OrderByDescending(s => s.RecordCreatedOn);
                    break;
                default:
                    employees = employees.OrderBy(e => e.Name);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Employee>.CreateAsync(employees.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Employee empobj)
        {
            if (ModelState.IsValid)
            {
                if (empobj.ImageFile != null && empobj.ImageFile.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(empobj.ImageFile.FileName);
                    var extension = Path.GetExtension(fileName);

                    // Generate a unique file name to prevent conflicts
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;

                    // Combine the unique file name with the upload path
                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "ImageUploads");
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Save the file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        empobj.ImageFile.CopyTo(fileStream);
                    }
                    // Update the employee object with the image path
                    empobj.ImagePath = uniqueFileName;


                }

                _context.Employees.Add(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Record Added Successfully !";
                return RedirectToAction("Index");
            }

            return View();
        }


        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var empfromdb = _context.Employees.Find(id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee empobj)
        {
            if (id != empobj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (empobj.ImageFile != null && empobj.ImageFile.Length > 0)
                {
                    // Get the file name and extension
                    var fileName = Path.GetFileName(empobj.ImageFile.FileName);
                    var extension = Path.GetExtension(fileName);

                    // Generate a unique file name to prevent conflicts
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;

                    // Combine the unique file name with the upload path
                    var uploadPath = Path.Combine(_hostingEnvironment.WebRootPath, "ImageUploads");
                    var filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Save the file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        empobj.ImageFile.CopyTo(fileStream);
                    }

                    // Update the employee object with the new image path
                    empobj.ImagePath = uniqueFileName;

                    // Delete the old image file
                    if (!string.IsNullOrEmpty(empobj.ImagePath))
                    {
                        var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, empobj.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }
                else
                {
                    if (empobj.ImageFile == null)
                    {
                        var newImageUpload = _context.Employees.Where(e => e.Id == id).Select(e => e.ImagePath);
                        empobj.ImagePath = newImageUpload.FirstOrDefault();
                    }
                }

                _context.Employees.Update(empobj);
                _context.SaveChanges();
                TempData["ResultOk"] = "Record Updated Successfully !";
                return RedirectToAction("Index");
            }

            return View(empobj);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var empfromdb = _context.Employees.Find(id);

            if (empfromdb == null)
            {
                return NotFound();
            }
            return View(empfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEmp(int? id)
        {
            var deleterecord = _context.Employees.Find(id);
            if (deleterecord == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(deleterecord);
            _context.SaveChanges();
            TempData["ResultOk"] = "Data Deleted Successfully !";
            return RedirectToAction("Index");
        }


    }
}