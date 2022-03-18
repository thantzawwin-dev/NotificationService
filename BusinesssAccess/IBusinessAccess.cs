

namespace NotificationService;
public interface IBusinessAccess {
    Task<NotificationResponse> SendOTP(OTPSendingRequest parameter);
    Task<NotificationResponse> VerifyOTP(OTPVerificationRequest parameter);
    
}