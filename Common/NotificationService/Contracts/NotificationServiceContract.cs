using Newtonsoft.Json;

namespace NotificationService;

public enum ServiceTypes 
{
    Test,
    SendOTP,
    VerifyOTP,
}

public interface INotificationServiceContract {
    ServiceTypes type {get;set;}
    string request{get;set;}
}

public class NotificationServiceContract : INotificationServiceContract
{
    public ServiceTypes type { get;set; }
    public string request { get;set; }
}


