using Microsoft.AspNetCore.SignalR;

namespace EgitimPortalFinal.Hubs
{
    public class GeneralHub : Hub
    {
        public async Task SendCourseNotification(string message, string courseTitle)
        {
            await Clients.All.SendAsync("ReceiveCourseNotification", new
            {
                message = message,
                courseTitle = courseTitle,
                timestamp = DateTime.Now
            });
        }
    }
}