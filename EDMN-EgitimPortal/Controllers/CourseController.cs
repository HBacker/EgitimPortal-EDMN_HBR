using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using EgitimPortalFinal.Models;
using EgitimPortalFinal.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class CourseController : Controller
{
    private readonly CourseRepository _courseRepository;
    private readonly IMapper _mapper;
    private readonly INotyfService _notify;
    private readonly IWebHostEnvironment _environment;

    public CourseController(
        CourseRepository courseRepository,
        IMapper mapper,
        INotyfService notify,
        IWebHostEnvironment environment)
    {
        _courseRepository = courseRepository;
        _mapper = mapper;
        _notify = notify;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string searchString)
    {
        try
        {
            var courses = await _courseRepository.GetAllWithSearchAsync(searchString);
            var coursesViewModel = _mapper.Map<IEnumerable<CourseModel>>(courses);
            return View(coursesViewModel);
        }
        catch (Exception ex)
        {
            _notify.Error($"Bir hata oluştu: {ex.Message}");
            return RedirectToAction("Index", "Home");
        }
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _courseRepository.GetAllAsync();
        var models = _mapper.Map<IEnumerable<CourseModel>>(courses);
        return View(models);
    }
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _courseRepository.GetCourseByIdAsync(id.Value);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    public async Task<IActionResult> Course(string searchString)
    {
        var courses = await _courseRepository.GetAllCoursesAsync(searchString);
        return View(courses);
    }

    public async Task<IActionResult> Wordpress()
    {
        var courses = await _courseRepository.GetCoursesByTagAsync("Wordpress");
        return View(courses);
    }

    public async Task<IActionResult> Programlama()
    {
        var courses = await _courseRepository.GetCoursesByTagAsync("Programlama");

        return View(courses);
    }

    public async Task<IActionResult> SEO()
    {
        var courses = await _courseRepository.GetCoursesByTagAsync("SEO");
        return View(courses);
    }



    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            _notify.Warning("Lütfen bir anahtar kelime girin.");
            return RedirectToAction("Course");
        }

        var courses = await _courseRepository.SearchCoursesAsync(keyword);

        if (!courses.Any())
        {
            _notify.Warning("Aradığınız kriterlere uygun kurs bulunamadı.");
        }

        return View(courses);
    }
    [HttpGet]
    [Route("Course/Details/{id:int}/{lessonId?}")]
    public async Task<IActionResult> Details(int id, int? lessonId = null)
    {
        try
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);

            if (course == null || course.Lessons == null || !course.Lessons.Any())
            {
                return NotFound("Kurs veya dersler bulunamadı.");
            }

            var viewModel = new CourseDetailsViewModel
            {
                Course = new CourseModel { Id = course.Id, Title = course.Title, Description = course.Description },
                Lessons = course.Lessons.Select(l => new CourseLessonModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    Video = l.Video,
                    OrderNo = l.OrderNo
                }),
                CurrentLessonId = lessonId ?? course.Lessons.First().Id
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
        
            return RedirectToAction("Index", "Home");
        }
    }
    [Route("[controller]/v1/ListCourses")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Course>>> GetCourses([FromQuery] string? tag)
    {
        var courses = await _courseRepository.GetCoursesAsync(tag);
        return Ok(courses);
    }
}
