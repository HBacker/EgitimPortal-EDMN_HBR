using EgitimPortalFinal.Models;
using EgitimPortalFinal.ViewModels;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace EgitimPortalFinal.Repositories
{
    public class AdminRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public AdminRepository(AppDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

        }


        public async Task<CourseModel> GetCourseByIdAsync(int id)
        {
            var course = await _context.Course
                .FirstOrDefaultAsync(c => c.Id == id);

            return _mapper.Map<CourseModel>(course);
        }

        public async Task AddCourseAsync(CourseModel courseModel)
        {
            var course = _mapper.Map<Course>(courseModel);
            _context.Course.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCourseAsync(CourseModel courseModel)
        {
            var course = await _context.Course.FindAsync(courseModel.Id);
            if (course != null)
            {
                course.Title = courseModel.Title;
                course.Description = courseModel.Description;
                course.Content = courseModel.Content;
                course.Tag = courseModel.Tag;
                course.PhotoUrl = courseModel.PhotoUrl;

                _context.Course.Update(course);
                await _context.SaveChangesAsync();
            }
        }


        public async Task DeleteCourseAsync(int id)
        {
            var course = await _context.Course.FindAsync(id);
            if (course != null)
            {
                _context.Course.Remove(course);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> CourseExistsAsync(int id)
        {
            return await _context.Course.AnyAsync(c => c.Id == id);
        }
        public async Task<List<CourseLessonModel>> GetAllLessonsAsync(int? courseId = null)
        {
            var query = _context.CourseLessons
                .Include(x => x.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(l => l.CourseId == courseId.Value);
            }

            var lessons = await query
                .OrderBy(l => l.OrderNo)
                .ToListAsync();

            return _mapper.Map<List<CourseLessonModel>>(lessons);
        }

        public async Task<CourseLessonModel> AddLessonAsync(CourseLessonModel model)
        {
            var lesson = _mapper.Map<CourseLesson>(model);

            if (model.VideoFile != null)
            {
                lesson.Video = await SaveFileAsync(model.VideoFile, "videos");
            }

            if (model.ThumbnailFile != null)
            {
                lesson.Thumbnail = await SaveFileAsync(model.ThumbnailFile, "thumbnails");
            }

            await _context.CourseLessons.AddAsync(lesson);
            await _context.SaveChangesAsync();

            return _mapper.Map<CourseLessonModel>(lesson);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            try
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
                string filePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<CourseModel>> GetAllCoursesAsync()
        {
            var courses = await _context.Course.ToListAsync();
            return _mapper.Map<List<CourseModel>>(courses);
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> DeleteFileAsync(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) return true;

                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

            public async Task<bool> DeleteLessonAsync(int id)
        {
            try
            {
                var lesson = await _context.CourseLessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lesson == null)
                    return false;

                if (!string.IsNullOrEmpty(lesson.Video))
                {
                    var videoPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos", lesson.Video);
                    if (File.Exists(videoPath))
                    {
                        File.Delete(videoPath);
                    }
                }

                if (!string.IsNullOrEmpty(lesson.Thumbnail))
                {
                    var thumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails", lesson.Thumbnail);
                    if (File.Exists(thumbnailPath))
                    {
                        File.Delete(thumbnailPath);
                    }
                }

                _context.CourseLessons.Remove(lesson);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<CourseLessonModel> GetLessonDetailsByIdAsync(int id)
        {
            var lesson = await _context.CourseLessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            return _mapper.Map<CourseLessonModel>(lesson);
        }

        public async Task<bool> UpdateLessonAsync(CourseLessonModel model, IFormFile videoFile, IFormFile thumbnailFile)
        {
            try
            {
                var lesson = await _context.CourseLessons.FindAsync(model.Id);
                if (lesson == null)
                    return false;

                lesson.Name = model.Name;
                lesson.OrderNo = model.OrderNo;

                if (videoFile != null && videoFile.Length > 0)
                {
                    string videoFileName = $"{Guid.NewGuid()}{Path.GetExtension(videoFile.FileName)}";
                    string videoPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos", videoFileName);

                    // Önce dosyayı kaydet
                    using (var stream = new FileStream(videoPath, FileMode.Create))
                    {
                        await videoFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(lesson.Video))
                    {
                        var oldVideoPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos", lesson.Video);
                        if (File.Exists(oldVideoPath))
                        {
                            File.Delete(oldVideoPath);
                        }
                    }
                    lesson.Video = videoFileName;
                }

                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    string thumbnailFileName = $"{Guid.NewGuid()}{Path.GetExtension(thumbnailFile.FileName)}";
                    string thumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails", thumbnailFileName);

                    using (var stream = new FileStream(thumbnailPath, FileMode.Create))
                    {
                        await thumbnailFile.CopyToAsync(stream);
                    }

                    if (!string.IsNullOrEmpty(lesson.Thumbnail))
                    {
                        var oldThumbnailPath = Path.Combine(_webHostEnvironment.WebRootPath, "thumbnails", lesson.Thumbnail);
                        if (File.Exists(oldThumbnailPath))
                        {
                            File.Delete(oldThumbnailPath);
                        }
                    }
                    lesson.Thumbnail = thumbnailFileName;
                }

                _context.Update(lesson);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
 }



