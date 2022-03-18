
using Microsoft.AspNetCore.Mvc;
using AplusExtension;
using CommonService;

namespace NotificationService.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class OTPController : ControllerBase
{
    //IRequestClient<NotificationServiceContract> _client;

    INotification _noti;
    private readonly ILogger<OTPController> _logger;

    public OTPController(ILogger<OTPController> logger, INotification noti)
    {
        _logger = logger;
        _noti = noti;
    }

    [HttpPost]
    public async Task<Response> SendOTP(string mobileNo)
    {
        var response = new Response();
        try
        {
            var result = await _noti.SendOTP(new OTPSendingRequest() {
                MobileNo = mobileNo
            });

            if (result.code == StatusCodes.Status200OK)
            {
                response.SetHTTPResponse(StatusCodes.Status200OK, "OK");
            }
            else
            {
                response.SetHTTPResponse(result.code, result.message);
            }
        }
        catch (Exception ex)
        {
            response.SetHTTPResponse(StatusCodes.Status500InternalServerError, ex.Message);
        }
        return response;
    }
    
    [HttpPost]
    public async Task<Response> VerifyOTP(string mobileNo, int otp)
    {
        var response = new Response();
        try
        {
            var result = await _noti.VerifyOTP(new OTPVerificationRequest() {
                MobileNo = mobileNo,
                OTP = otp
            });

            if (result.code == StatusCodes.Status200OK)
            {
                response.SetHTTPResponse(StatusCodes.Status200OK, "OK");
            }
            else
            {
                response.SetHTTPResponse(result.code, result.message);
            }
        }
        catch (Exception ex)
        {
            response.SetHTTPResponse(StatusCodes.Status500InternalServerError, ex.Message);
        }
        return response;
    }
}

