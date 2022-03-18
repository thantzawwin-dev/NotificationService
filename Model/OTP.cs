
public class OTP {
    public int Id {get;set;}
	public string? Code {get;set;}
	public string? MobileNo {get;set;}
	public int FailCount {get;set;}
	public int SendCount {get;set;} = 0;
	public int Status {get;set;}
	public bool? DeleteFlag {get;set;}
	public DateTimeOffset? EditedAt {get;set;}
	public DateTimeOffset? CreatedAt {get;set;}
	public DateTimeOffset? LastSendAt {get;set;}
	public DateTimeOffset? LastFailAt {get;set;}
   
}