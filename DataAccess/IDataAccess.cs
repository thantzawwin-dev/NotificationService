using DataService;

namespace NotificationService;
public interface IDataAccess {
    Task<ListResponse> GetOTPAsync(GetOTPAsync_DA parameter);
    Task<DataService.Response> InsertOTPAsync(InsertOTPAsync_DA parameter);
    Task<DataService.Response> UpdateOTPAsync(UpdateOTPAsync_DA parameter);
    Task<DataService.Response> UpdateOTPFailCountAsync(UpdateOTPFailCountAsync_DA parameter);
    Task<DataService.Response> ResetOTPAsync(ResetOTPAsync_DA parameter);
    
}