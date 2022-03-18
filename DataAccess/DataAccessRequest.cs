
namespace NotificationService;

public class GetOTPAsync_DA {
    public string? MobileNo {get;set;}
}
public class ResetOTPAsync_DA {
    public int Id {get;set;}
    public string? MobileNo {get;set;}
}
public class InsertOTPAsync_DA {
    public string? MobileNo {get;set;}
    public string? OTP {get;set;}
}
public class UpdateOTPAsync_DA {
    public int Id {get;set;}
    public string? MobileNo {get;set;}
    public string? OTP {get;set;}
    public int SendCount {get;set;} = 0;
}
public class UpdateOTPFailCountAsync_DA {
    public int Id {get;set;}
    public string? MobileNo {get;set;}
    public string? OTP {get;set;}
    public int FailCount {get;set;} = 1;
}
