using DataExchange.Enums;
using GardeningHelperDatabase;
using GardeningHelperDatabase.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GardeningHelperAPI.Services.Notifications
{
    public class NotificationService
    {
        private readonly GardeningHelperDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<NotificationService> _logger;
        private readonly ISendGridClient _sendGridClient; // SendGrid client
        private readonly EmailSettings _emailSettings; // Configuration for email sender/name

        public NotificationService(
            GardeningHelperDbContext dbContext,
            UserManager<User> userManager,
            ILogger<NotificationService> logger,
            ISendGridClient sendGridClient,
            IOptions<EmailSettings> emailSettings)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
            _sendGridClient = sendGridClient;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendStatusChangeNotificationsAsync()
        {
            _logger.LogInformation("Starting status change notification process.");

            // Find all GardenPlants whose status changed from Normal to something else in the last run
            // We rely on the PreviousStatus being updated by PlantStatusService.UpdatePlantStatusAsync
            var plantsWithStatusChanges = await _dbContext.GardenPlants
                .Where(gp => gp.PreviousStatus == StatusEnum.Normal && gp.Status != StatusEnum.Normal)
                .Include(gp => gp.Plant) // Include Plant for name and details
                .Include(gp => gp.UserGarden) // Include UserGarden to get UserId
                    .ThenInclude(ug => ug.User) // Include User to get email and username
                .ToListAsync();

            if (!plantsWithStatusChanges.Any())
            {
                _logger.LogInformation("No plants found with status changes from Normal. No notifications to send.");
                return;
            }

            _logger.LogInformation($"Found {plantsWithStatusChanges.Count} plants with status changes from Normal.");

            // Group changes by User
            var usersWithNotifications = plantsWithStatusChanges
                .GroupBy(gp => gp.UserGarden.User)
                .ToList();

            _logger.LogInformation($"Found {usersWithNotifications.Count} users requiring notifications.");

            foreach (var userGroup in usersWithNotifications)
            {
                var user = userGroup.Key;
                var affectedPlants = userGroup.ToList();

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    _logger.LogWarning($"User {user.UserName} (ID: {user.Id}) has no email address configured. Skipping notification.");
                    continue;
                }

                // Construct the email
                var subject = "Important Update About Your Garden Plants!";
                var body = $"<p>Hello {user.UserName},</p>";
                body += "<p>The daily garden check has found that some of your plants require attention:</p>";
                body += "<ul>";

                foreach (var gardenPlant in affectedPlants)
                {
                    var plantName = gardenPlant.Plant.Name;
                    var newStatus = gardenPlant.Status.ToString(); // e.g., "NeedsWatering", "AtRisk"
                    var reason = gardenPlant.StatusChangeReason ?? "No specific reason provided."; // Use the stored reason

                    body += $"<li><b>{plantName}:</b> Status changed to <b>{newStatus}</b>. {reason}</li>";
                }

                body += "</ul>";
                body += "<p>Please log in to your Gardening Helper app for more details and to record actions (like watering).</p>";
                body += "<p>Happy Gardening!</p>";
                body += $"<p>--<br>{_emailSettings.SenderName}</p>";


                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    HtmlContent = body
                };
                msg.AddTo(new EmailAddress(user.Email, user.UserName));

                try
                {
                    var response = await _sendGridClient.SendEmailAsync(msg);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                    {
                        _logger.LogInformation($"Notification email sent successfully to {user.Email} for {affectedPlants.Count} plant(s).");
                        // Optionally, update plants to indicate a notification was sent,
                        // so we don't send daily emails for the *same* ongoing issue.
                        // For simplicity now, we won't track this.
                    }
                    else
                    {
                        var responseBody = await response.Body.ReadAsStringAsync();
                        _logger.LogError($"Failed to send notification email to {user.Email}. Status Code: {response.StatusCode}. Response Body: {responseBody}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while sending notification email to {user.Email}.");
                }
            }

            _logger.LogInformation("Status change notification process finished.");
        }

        // You might add other notification methods here in the future (e.g., for new features, reminders)
    }
}
