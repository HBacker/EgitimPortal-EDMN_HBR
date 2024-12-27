using AspNetCoreHero.ToastNotification.Abstractions;
using EgitimPortalFinal.Models;
using EgitimPortalFinal.Repositories;
using EgitimPortalFinal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AutoMapper;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using EgitimPortalFinal.Hubs;
using Microsoft.AspNetCore.SignalR;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly AdminRepository _adminRepository;
    private readonly IFileProvider _fileProvider;
    private readonly INotyfService _notify;
    private readonly AuthRepository _authRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHubContext<GeneralHub> _hubContext;


    public AdminController(AdminRepository adminRepository,
        AuthRepository authRepository,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IFileProvider fileProvider,
        INotyfService notify,
        IMapper mapper,
        IWebHostEnvironment webHostEnvironment,
        IHubContext<GeneralHub> hubContext)
    {
        _adminRepository = adminRepository;
        _authRepository = authRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _fileProvider = fileProvider;
        _notify = notify;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _hubContext = hubContext;

    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> List()
    {
        var courses = await _adminRepository.GetAllCoursesAsync();
        var courseModels = _mapper.Map<IEnumerable<CourseModel>>(courses);
        return View(courseModels);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CourseModel newCourse)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var photoUrl = "-";

                if (newCourse.PhotoFile != null)
                {
                    var filename = Path.GetFileName(newCourse.PhotoFile.FileName);
                    var photoPath = Path.Combine(rootFolder.First(x => x.Name == "CoursePhotos").PhysicalPath, filename);
                    using var stream = new FileStream(photoPath, FileMode.Create);
                    await newCourse.PhotoFile.CopyToAsync(stream);
                    photoUrl = filename;
                }

                newCourse.PhotoUrl = photoUrl;

                await _adminRepository.AddCourseAsync(newCourse);

                await _hubContext.Clients.All.SendAsync("ReceiveCourseNotification",
                    new
                    {
                        message = $"{newCourse.Title} kursu başarıyla eklendi.",
                        courseTitle = newCourse.Title
                    });
                _notify.Success("Ders başarıyla eklendi.");

                return RedirectToAction(nameof(List));
            }

            return View(newCourse);
        }
        catch (Exception ex)
        {
            _notify.Error($"Bir hata oluştu: {ex.Message}");
            return View(newCourse);
        }
    }




    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _adminRepository.GetCourseByIdAsync(id.Value);
        if (course == null)
        {
            return NotFound();
        }

        var courseModel = _mapper.Map<CourseModel>(course);
        return View(courseModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(CourseModel updatedCourse)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var existingCourse = await _adminRepository.GetCourseByIdAsync(updatedCourse.Id);
                if (existingCourse == null)
                {
                    return NotFound();
                }

                if (updatedCourse.PhotoFile != null)
                {
                    var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                    var filename = Path.GetFileName(updatedCourse.PhotoFile.FileName);
                    var photoPath = Path.Combine(rootFolder.First(x => x.Name == "CoursePhotos").PhysicalPath, filename);
                    using var stream = new FileStream(photoPath, FileMode.Create);
                    await updatedCourse.PhotoFile.CopyToAsync(stream);
                    updatedCourse.PhotoUrl = filename;
                }
                else
                {
                    updatedCourse.PhotoUrl = existingCourse.PhotoUrl;
                }

                await _adminRepository.UpdateCourseAsync(updatedCourse);
                _notify.Success("Ders başarıyla güncellendi.");
                return RedirectToAction(nameof(List));
            }
            return View(updatedCourse);
        }
        catch (Exception ex)
        {
            _notify.Error($"Bir hata oluştu: {ex.Message}");
            return View(updatedCourse);
        }
    }


    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _adminRepository.GetCourseByIdAsync(id.Value);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var course = await _adminRepository.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(course.PhotoUrl) && course.PhotoUrl != "-")
            {
                var rootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                var photoPath = Path.Combine(rootFolder.First(x => x.Name == "CoursePhotos").PhysicalPath, course.PhotoUrl);
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
            }

            await _adminRepository.DeleteCourseAsync(id);
            _notify.Success("Ders başarıyla silindi.");
            return RedirectToAction(nameof(List));
        }
        catch (Exception ex)
        {
            _notify.Error($"Bir hata oluştu: {ex.Message}");
            return RedirectToAction(nameof(List));
        }
    }




    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetUserList()
    {
        var users = await _authRepository.GetUsersAsync();

        var userModels = users.Select(x => new UserModel()
        {
            Id = x.Id,
            FullName = x.FullName,
            Email = x.Email,
            UserName = x.UserName,
            City = x.City
        }).ToList();

        return View(userModels);
    }

    [HttpGet]
    public async Task<IActionResult> UserListAjax()
    {
        var users = await _authRepository.GetUsersAsync();

        var userModels = users.Select(x => new
        {
            id = x.Id,
            fullName = x.FullName,
            userName = x.UserName,
            email = x.Email,
            city = x.City
        }).ToList();

        return Json(userModels);
    }

    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetRoleList()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [Authorize(Roles = "admin")]
    public IActionResult RoleAdd()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> RoleAdd(AppRole model)
    {
        var role = await _roleManager.FindByNameAsync(model.Name);
        if (role == null)
        {
            var newRole = new AppRole { Name = model.Name };
            await _roleManager.CreateAsync(newRole);
        }
        return RedirectToAction("GetRoleList");
    }

    public async Task<IActionResult> ManageUserRoles()
    {
        var users = await _userManager.Users.ToListAsync();
        var roles = await _roleManager.Roles.ToListAsync();

        var userRolesDict = new Dictionary<string, List<string>>();

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            userRolesDict[user.Id] = userRoles.ToList();
        }

        ViewBag.UserRoles = userRolesDict;
        ViewBag.Roles = roles;

        return View(users);
    }


    [HttpPost]
    public async Task<IActionResult> ManageUserRoles(string userId, string selectedRoles)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(selectedRoles))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, userRoles);

        var role = await _roleManager.FindByIdAsync(selectedRoles);
        if (role != null)
        {
            await _userManager.AddToRoleAsync(user, role.Name);
        }

        return RedirectToAction(nameof(ManageUserRoles));
    }

[HttpGet]
    public async Task<IActionResult> CreateLesson()
    {
        try
        {
            ViewBag.Courses = new SelectList(await _adminRepository.GetAllCoursesAsync(), "Id", "Title");
            return View(new CourseLessonModel());
        }
        catch (Exception ex)
        {
            _notify.Error($"Hata oluştu: {ex.Message}");
            return RedirectToAction(nameof(LessonList));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLesson(CourseLessonsCreateModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = new SelectList(await _adminRepository.GetAllCoursesAsync(), "Id", "Title");
                return View(model);
            }

            foreach (var lesson in model.Lessons)
            {
                lesson.CourseId = model.CourseId;
                await _adminRepository.AddLessonAsync(lesson);
            }

            _notify.Success("Dersler başarıyla eklendi");
            return RedirectToAction("LessonList");
        }
        catch (Exception ex)
        {
            _notify.Error($"Hata oluştu: {ex.Message}");
            ViewBag.Courses = new SelectList(await _adminRepository.GetAllCoursesAsync(), "Id", "Title");
            return View(model);
        }
    }

    public async Task<IActionResult> LessonList(int? courseId = null)
    {
        try
        {
            ViewBag.Courses = new SelectList(await _adminRepository.GetAllCoursesAsync(), "Id", "Title", courseId);
            var lessons = await _adminRepository.GetAllLessonsAsync(courseId);
            return View(lessons);
        }
        catch (Exception ex)
        {
            _notify.Error($"Hata oluştu: {ex.Message}");
            return View(new List<CourseLessonModel>());
        }

    }
    [HttpPost]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        try
        {
            var result = await _adminRepository.DeleteLessonAsync(id);
            if (result)
            {
                _notify.Success("Ders başarıyla silindi");
            }
            else
            {
                _notify.Error("Ders silinirken bir hata oluştu");
            }
        }
        catch (Exception ex)
        {
            _notify.Error($"Hata: {ex.Message}");
        }

        return RedirectToAction(nameof(LessonList));
    }

    [HttpGet]
    public async Task<IActionResult> EditLesson(int courseId)
    {
        try
        {
            var lessons = await _adminRepository.GetAllLessonsAsync(courseId);
            if (!lessons.Any())
            {
                _notify.Error("Belirtilen kursa ait ders bulunamadı.");
                return RedirectToAction(nameof(LessonList));
            }

            ViewBag.CourseTitle = lessons.FirstOrDefault()?.Course?.Title ?? "Kurs";
            return View(lessons);
        }
        catch (Exception ex)
        {
            _notify.Error($"Hata: {ex.Message}");
            return RedirectToAction(nameof(LessonList));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLesson([FromForm] List<CourseLessonModel> lessons, int courseId)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new { success = false, message = "Validasyon hatası", errors });
        }

        try
        {
            foreach (var lesson in lessons)
            {
                lesson.CourseId = courseId;
            }

            if (lessons == null || !lessons.Any())
            {
                return BadRequest(new { success = false, message = "Ders verileri bulunamadı" });
            }

            var files = Request.Form.Files;
            if (files == null)
            {
                return BadRequest(new { success = false, message = "Dosya verileri bulunamadı" });
            }

            for (int i = 0; i < lessons.Count; i++)
            {
                var videoFile = files.GetFile($"Videos[{i}]");
                var thumbnailFile = files.GetFile($"Thumbnails[{i}]");

                if (videoFile != null && videoFile.Length > 2147483648)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Video dosyası çok büyük (max: 2GB)",
                        lessonName = lessons[i].Name
                    });
                }

                if (thumbnailFile != null && thumbnailFile.Length > 5242880)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Thumbnail dosyası çok büyük (max: 5MB)",
                        lessonName = lessons[i].Name
                    });
                }

                var updateResult = await _adminRepository.UpdateLessonAsync(lessons[i], videoFile, thumbnailFile);
                if (!updateResult)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"{lessons[i].Name} dersi güncellenirken hata oluştu"
                    });
                }
            }

            var saveResult = await _adminRepository.SaveChangesAsync();
            if (!saveResult)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Değişiklikler kaydedilirken hata oluştu"
                });
            }

            return Json(new
            {
                success = true,
                message = "Dersler başarıyla güncellendi",
                redirectUrl = Url.Action("LessonList", "Admin", new { courseId = courseId })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                success = false,
                message = $"İşlem sırasında hata: {ex.Message}"
            });
        }
    }
}
