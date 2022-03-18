using AplusExtension;
using Newtonsoft.Json;

namespace NotificationService;

public class OTPSending
{
    public string _mobile { get; set; }
    public OTPSending(string mobile)
    {
        _mobile = mobile;
    }

    public OTPSendingRequest Request()
    {
        if (this._mobile.IsNotNullOrEmpty())
        {
            return new OTPSendingRequest
            {
                MobileNo = this._mobile,
            };
        }
        else
        {
            throw new Exception("Mobile number is required for otp sending.");
        }
    }

    public NotificationServiceContract Contract(){
        return new NotificationServiceContract {
            type = ServiceTypes.SendOTP,
            request = JsonConvert.SerializeObject(this.Request())
        };
    }

}










