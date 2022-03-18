
namespace NotificationService;
public interface INotification{
    Task<NotificationResponse> SendOTP(OTPSendingRequest parameter);
    Task<NotificationResponse> VerifyOTP(OTPVerificationRequest parameter);
}