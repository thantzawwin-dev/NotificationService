using AplusExtension;
using Newtonsoft.Json;

namespace NotificationService;

public class OTPVerification
{
    public string _mobile { get; set; }
    public int _otp { get; set; }
    public OTPVerification(string mobile, int otp)
    {
        _mobile = mobile;
        _otp = otp;
    }

    public OTPVerificationRequest Request()
    {
        if (this._mobile.IsNotNullOrEmpty() && this._otp.IsNotNullOrEmpty())
        {
            return new OTPVerificationRequest
            {
                MobileNo = this._mobile,
                OTP = this._otp,
            };
        }
        else
        {
            throw new Exception("Mobile number and otp are required for otp verification.");
        }
    }

    public NotificationServiceContract Contract(){
        return new NotificationServiceContract {
            type = ServiceTypes.VerifyOTP,
            request = JsonConvert.SerializeObject(this.Request())
        };
    }
}










