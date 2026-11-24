using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InstructorInformationSystem.Models;

namespace InstructorInformationSystem.Controllers;

[Authorize]
public class InstructorController : Controller
{
    private readonly ApplicationDbContext _context;

    public InstructorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var instructors = await _context.Instructors
            .AsNoTracking()
            .OrderBy(instructor => instructor.FullName)
            .ToListAsync();

        return View(instructors);
    }

    [HttpGet]
    public IActionResult Create() => View(new Instructor());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("FullName,Age,Address,Designation,Gender,ContactNumber,Majorship,BachelorDegree,EmailAddress")] Instructor instructor)
    {
        if (ModelState.IsValid)
        {
            var email = instructor.EmailAddress.Trim().ToLowerInvariant();
            var duplicateEmail = await _context.Instructors
                .AnyAsync(item => item.EmailAddress.ToLower() == email);

            if (duplicateEmail)
            {
                ModelState.AddModelError(nameof(instructor.EmailAddress), "An instructor with this e-mail address already exists.");
            }
            else
            {
                instructor.FullName = instructor.FullName.Trim();
                instructor.EmailAddress = email;
                _context.Instructors.Add(instructor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor record created successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.InstructorId == id.Value);

        return instructor is null ? NotFound() : View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors.FindAsync(id.Value);
        return instructor is null ? NotFound() : View(instructor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("InstructorId,FullName,Age,Address,Designation,Gender,ContactNumber,Majorship,BachelorDegree,EmailAddress")] Instructor instructor)
    {
        if (id != instructor.InstructorId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var email = instructor.EmailAddress.Trim().ToLowerInvariant();
            var duplicateEmail = await _context.Instructors
                .AnyAsync(item => item.InstructorId != id && item.EmailAddress.ToLower() == email);

            if (duplicateEmail)
            {
                ModelState.AddModelError(nameof(instructor.EmailAddress), "Another instructor is already using this e-mail address.");
            }
            else
            {
                instructor.FullName = instructor.FullName.Trim();
                instructor.EmailAddress = email;
                _context.Update(instructor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Instructor record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
        }

        return View(instructor);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var instructor = await _context.Instructors
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.InstructorId == id.Value);

        return instructor is null ? NotFound() : View(instructor);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var instructor = await _context.Instructors.FindAsync(id);
        if (instructor is null)
        {
            return NotFound();
        }

        _context.Instructors.Remove(instructor);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Instructor record deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
