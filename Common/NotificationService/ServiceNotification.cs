
using MassTransit;

namespace NotificationService;

class ServiceNotification : INotification {
    private IRequestClient<NotificationServiceContract> _client;

    public ServiceNotification(IRequestClient<NotificationServiceContract> client)
    {
        _client = client;
    }

    public async Task<NotificationResponse> SendOTP(OTPSendingRequest parameter)
    {
        try
        {
            var contract = new OTPSending(parameter.MobileNo).Contract();
            var result = await _client.GetResponse<NotificationResponse>(contract);
            return result.Message;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<NotificationResponse> VerifyOTP(OTPVerificationRequest parameter)
    {
        try
        {
            var contract = new OTPVerification(parameter.MobileNo, parameter.OTP).Contract();
            var result = await _client.GetResponse<NotificationResponse>(contract);
            return result.Message;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}