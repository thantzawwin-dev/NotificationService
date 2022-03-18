
using Newtonsoft.Json;
using AplusExtension;

namespace NotificationService;

public class BusinessAccessLayer : IBusinessAccess
{
    private INotification _noti;
    private IDataAccess _data;
    public BusinessAccessLayer(INotification noti, IDataAccess data)
    {
        _noti = noti;
        _data = data;
    }

    public async Task<NotificationResponse> SendOTP(OTPSendingRequest parameter)
    {
        // It may be dynamic later.
        const int SEND_LIMIT_COUNT = 5;
        const int OTP_SENDING_BLOCK_TIME = 60; // By seconds
        const int OTP_REQUEST_DURATION_TIME = 10; // By seconds
        const int OTP_LIMIT_AUTO_RESET_DAY = 1; // By days

        #region Send OTP Validation
        OTP? otpInfo = new OTP();
        DateTimeOffset dateTimeNow = (DateTimeOffset)DateTime.Now;

        var getOTPResult = await _data.GetOTPAsync(new GetOTPAsync_DA {
            MobileNo = parameter.MobileNo
        });
        
        if (getOTPResult.code == StatusCodes.Status200OK && getOTPResult.rows != null
        && getOTPResult.rows.Count > 0)
        {
            List<OTP> dataList = getOTPResult.rows.toList<OTP>();
            otpInfo = (dataList != null && dataList.Count > 0) ? dataList.FirstOrDefault() : null;
            
            if(otpInfo != null)//Check object is null
            {
                DateTimeOffset otpLastSendAt = otpInfo.LastSendAt != null ? (DateTimeOffset)otpInfo.LastSendAt : dateTimeNow;
                var difference = dateTimeNow.Subtract(otpLastSendAt);
                //Check previous OTP sending block by day, then reset sent count
                if(difference.Days >= OTP_LIMIT_AUTO_RESET_DAY)
                {
                    otpInfo.SendCount = 0;
                }
                //Check previous OTP sent count & OTP sending block seconds, then response error message
                else if(otpInfo.SendCount >= SEND_LIMIT_COUNT && difference.TotalSeconds <= OTP_SENDING_BLOCK_TIME)
                {
                    return new NotificationResponse
                    {
                        code = StatusCodes.Status403Forbidden,
                        message = "Forbidden",
                    };
                }
                // Check previous OTP sending seconds
                else if(difference.TotalSeconds < OTP_REQUEST_DURATION_TIME)
                {
                    return new NotificationResponse
                    {
                        code = StatusCodes.Status429TooManyRequests,
                        message = "Too Many Requests",
                    };
                }
                // Check previous OTP sent count & OTP sending block seconds, then reset sent count
                else if(otpInfo.SendCount >= SEND_LIMIT_COUNT && difference.TotalSeconds > OTP_SENDING_BLOCK_TIME)
                {
                    otpInfo.SendCount = 0;
                }
            }
            else
            {
                return new NotificationResponse
                {
                    code = StatusCodes.Status500InternalServerError,
                    message = "Internal Server Error",
                };
            }
        }
        else if (getOTPResult.code == StatusCodes.Status200OK && getOTPResult.rows != null
        && getOTPResult.rows.Count == 0)
        {
            //
        }
        else
        {
            return new NotificationResponse
            {
                code = StatusCodes.Status500InternalServerError,
                message = "Internal Server Error",
            };
        }
        #endregion

        #region Real OTP Sending
        
        //
        //
        //

        #endregion

        #region OTP Info Storing
        string otp = Helper.GenerateRandomDigit(6);

        // Update OTP previous record
        if(otpInfo != null && otpInfo.Id > 0)
        {
            var updateOTPResult = await _data.UpdateOTPAsync(new UpdateOTPAsync_DA {
                Id = otpInfo.Id,
                MobileNo = parameter.MobileNo,
                OTP = otp,
                SendCount = ++otpInfo.SendCount
            });
            if (updateOTPResult.code != 200)
            {
                return new NotificationResponse
                {
                    code = updateOTPResult.code,
                    message = updateOTPResult.message,
                };
            }
        }
        else // Create OTP new record
        {
            var insertOTPResult = await _data.InsertOTPAsync(new InsertOTPAsync_DA {
                MobileNo = parameter.MobileNo,
                OTP = otp
            });
            if (insertOTPResult.code != 200)
            {
                return new NotificationResponse
                {
                    code = insertOTPResult.code,
                    message = insertOTPResult.message,
                };
            }
        }
        #endregion

        return new NotificationResponse
        {
            code = StatusCodes.Status200OK,
            message = "OK",
        };
    }

    public async Task<NotificationResponse> VerifyOTP(OTPVerificationRequest parameter)
    {
        // It may be dynamic later.
        const int FAIL_LIMIT_COUNT = 5;
        const int LOGIN_ATTEMPT_BLOCK_TIME = 60; // By seconds
        const int OTP_EXPIRE_TIME = 60; // By seconds
        const int OTP_LIMIT_AUTO_RESET_DAY = 60; // By day

        #region Verify OTP Validation
        OTP? otpInfo = new OTP();
        DateTimeOffset dateTimeNow = (DateTimeOffset)DateTime.Now;

        var getOTPResult = await _data.GetOTPAsync(new GetOTPAsync_DA {
            MobileNo = parameter.MobileNo
        });
        
        if (getOTPResult.code == StatusCodes.Status200OK && getOTPResult.rows != null
        && getOTPResult.rows.Count > 0)
        {
            List<OTP> dataList = getOTPResult.rows.toList<OTP>();
            otpInfo = (dataList != null && dataList.Count > 0) ? dataList.FirstOrDefault(): null;

            if(otpInfo != null)
            {
                DateTimeOffset otpLastFailAt = otpInfo.LastFailAt != null ? (DateTimeOffset)otpInfo.LastFailAt : dateTimeNow;
                var difference =  dateTimeNow.Subtract(otpLastFailAt);

                //Check previous OTP failed block by day, then reset sent count
                if(difference.Days >= OTP_LIMIT_AUTO_RESET_DAY)
                {
                    otpInfo.FailCount = 0;
                }
                // Check previous failed count & login attempt block seconds, then reset failed count
                else if(otpInfo.FailCount >= FAIL_LIMIT_COUNT && difference.TotalSeconds > LOGIN_ATTEMPT_BLOCK_TIME)
                {
                    otpInfo.FailCount = 0;
                }
                // Check previous failed count & login attempt block seconds, then response error message.
                else if(otpInfo.FailCount >= FAIL_LIMIT_COUNT && difference.TotalSeconds <= LOGIN_ATTEMPT_BLOCK_TIME)
                {
                    return new NotificationResponse
                    {
                        code = StatusCodes.Status403Forbidden,
                        message = "Forbidden",
                    };
                }

                //Check wrong OTP and then update failed count
                if(!string.IsNullOrEmpty(otpInfo.Code) && int.Parse(otpInfo.Code) != parameter.OTP)
                {
                    #region Update OTP Failed Count
                    var updateOTPFailCountResult = await _data.UpdateOTPFailCountAsync(new UpdateOTPFailCountAsync_DA {
                        Id = otpInfo.Id,
                        MobileNo = parameter.MobileNo,
                        FailCount = ++otpInfo.FailCount,
                    });

                    if(updateOTPFailCountResult.code != 200)
                    {
                        return new NotificationResponse
                        {
                            code = StatusCodes.Status500InternalServerError,
                            message = "Internal Server Error",
                        };
                    }
                    #endregion

                    return new NotificationResponse
                    {
                        code = StatusCodes.Status401Unauthorized,
                        message = "Unauthorized",
                    };
                }
                //Check OTP expire by seconds
                else if(difference.TotalSeconds > OTP_EXPIRE_TIME)
                {
                    return new NotificationResponse
                    {
                        code = StatusCodes.Status410Gone,
                        message = "Gone",
                    };
                }
                else if(!string.IsNullOrEmpty(otpInfo.Code) && int.Parse(otpInfo.Code) == parameter.OTP)
                {
                    //Success OTP verification
                    #region OTP Reset Count
                    var resetOTPResult = await _data.ResetOTPAsync(new ResetOTPAsync_DA {
                        Id = otpInfo.Id,
                        MobileNo = parameter.MobileNo,
                    });

                    if(resetOTPResult.code != 200)
                    {
                        return new NotificationResponse
                        {
                            code = StatusCodes.Status500InternalServerError,
                            message = "Internal Server Error",
                        };
                    }
                    #endregion
                }
            }
            else
            {
                return new NotificationResponse
                {
                    code = StatusCodes.Status500InternalServerError,
                    message = "Internal Server Error",
                };
            }
        }
        else if (getOTPResult.code == StatusCodes.Status200OK && getOTPResult.rows != null
        && getOTPResult.rows.Count == 0)
        {
            return new NotificationResponse
            {
                code = StatusCodes.Status204NoContent,
                message = "No Content",
            };
        }
        else
        {
            return new NotificationResponse
            {
                code = StatusCodes.Status500InternalServerError,
                message = "Internal Server Error",
            };
        }
        #endregion
            
        return new NotificationResponse
        {
            code = StatusCodes.Status200OK,
            message = "OK",
        };
    }
}
