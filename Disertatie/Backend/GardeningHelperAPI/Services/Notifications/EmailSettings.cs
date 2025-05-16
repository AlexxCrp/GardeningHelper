namespace GardeningHelperAPI.Services.Notifications
{
    public class EmailSettings
    {
        public string SendGridApiKey { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = "alexc13ro@gmail.com";
        public string SenderName { get; set; } = "Gardening Helper";
    }
}
