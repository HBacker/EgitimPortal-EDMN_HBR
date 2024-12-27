using EgitimPortalFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CourseRepository : GenericRepository<Course>
{
    private readonly AppDbContext _context;

    public CourseRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync(string? searchString = null)
    {
        var courses = _context.Course.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            courses = courses.Where(c => c.Title.Contains(searchString));
        }

        return await courses.OrderByDescending(c => c.Id).ToListAsync();
    }

    public async Task<Course?> GetCourseByIdAsync(int id)
    {
        return await _context.Course.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Course>> GetCoursesByTagAsync(string tag)
    {
        return await _context.Course
                             .Where(c => c.Tag == tag)
                             .OrderByDescending(c => c.Id)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Course>> SearchCoursesAsync(string keyword)
    {
        var courses = await _context.Course
            .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword) || c.Tag.Contains(keyword))
            .ToListAsync();

        return courses;
    }

    
    public async Task<IEnumerable<Course>> GetAllWithSearchAsync(string searchString = null)
    {
        var query = _context.Course.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            query = query.Where(c =>
                c.Title.Contains(searchString) ||
                c.Description.Contains(searchString) ||
                c.Tag.Contains(searchString));
        }

        return await query.OrderByDescending(c => c.Id).ToListAsync();
    }
    public async Task<Course> GetCourseWithLessonsAsync(int courseId)
    {
        return await _context.Course
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == courseId);
    }


    public async Task<IEnumerable<Course>> GetCoursesAsync(string? tag)
    {
        IQueryable<Course> coursesQuery = _context.Course;

        if (!string.IsNullOrEmpty(tag))
        {
            coursesQuery = coursesQuery.Where(c => c.Tag.Contains(tag));
        }

        return await coursesQuery.ToListAsync();
    }
    
}
