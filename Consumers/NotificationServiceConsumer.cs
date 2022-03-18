
using MassTransit;
using Newtonsoft.Json;
using AplusExtension;

namespace NotificationService;

public class NotificationServiceConsumer : IConsumer<NotificationServiceContract> 
{
    private IBusinessAccess _bal;
    public NotificationServiceConsumer(IBusinessAccess bal)
    {
        _bal = bal;
    }

    public async Task Consume(ConsumeContext<NotificationServiceContract> context)
    {
        try
        {
            if( context.Message.type == ServiceTypes.SendOTP)
            {
                var request = JsonConvert.DeserializeObject<OTPSendingRequest>(context.Message.request);
                var result = await _bal.SendOTP(request);
                await context.RespondAsync<NotificationResponse>(result);
            }
            else if(context.Message.type == ServiceTypes.VerifyOTP) 
            {
                var request = JsonConvert.DeserializeObject<OTPVerificationRequest>(context.Message.request);
                var result = await _bal.VerifyOTP(request);
                await context.RespondAsync<NotificationResponse>(result);
            }
            else if(context.Message.type == ServiceTypes.Test)
            {
                await context.RespondAsync<NotificationResponse>(new 
                {
                    code = 200,
                    message=context.Message.request
                });
            }
        }
        catch(Exception ex)
        {
            await context.RespondAsync<NotificationResponse>(new 
            {
                code = StatusCodes.Status500InternalServerError,
                message = ex.Message
            });
        }
    }
}
