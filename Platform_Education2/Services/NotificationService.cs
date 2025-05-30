using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PlatformEduPro.Helper;
using PlatformEduPro.Models;
using PlatformEduPro.Services.Interfaces;
using System.Data;

namespace PlatformEduPro.Services
{
    public class NotificationService : INotificationService
    {
       private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly EduPlatformDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationService(UserManager<AppUser> userManager,IEmailSender email,EduPlatformDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = email;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            
        }
        public async Task SendNewCourseNotification()
        {
            // اختار الكورسات اللي اتنشرت النهاردة ومبعتش ليها إشعار
            var today = DateTime.UtcNow.Date;

            var courses = await _context.TbCourses
                .Where(c => c.CreatedDate.Date == today && !c.IsNotificationSent)
                .AsNoTracking()
                .ToListAsync();

            if (!courses.Any()) return;

            // جلب المستخدمين اللي إيميلهم متأكد
            var users = await _userManager.Users
                .Where(u => u.EmailConfirmed)
                .ToListAsync();

            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin.ToString()
                         ?? "https://www.linkedin.com/in/mohamed-montaser-1ab942314/";

            foreach (var course in courses)
            {
                foreach (var user in users)
                {
                    var placeholders = new Dictionary<string, string>
                {
                    { "{{name}}", user.FirstName ?? "عضو" },
                    { "{{CourseName}}", course.CourseName },
                    { "{{url}}", $"{origin}" }
                };

                    var body = EmailBodyBuilder.GenerateEmailBody("CourseNotification", placeholders);

                    await _emailSender.SendEmailAsync(
                        user.Email!,
                        $"📚 New Course Available Now: {course.CourseName}",
                        body);
                }

                // تحديث حالة الإشعار للكورس
                course.IsNotificationSent = true;
            }

            _context.UpdateRange(courses);
            await _context.SaveChangesAsync();
        }
    }
}
