using DataService;
using MassTransit;

namespace NotificationService;

public class ServiceDataAccess : IDataAccess {

    private IRequestClient<DataServiceContract> _client;

    public ServiceDataAccess(IRequestClient<DataServiceContract> client)
    {
        _client = client;
    }

    #region Data Service
    

    public async Task<ListResponse> GetOTPAsync(GetOTPAsync_DA parameter) {
        try
        {
            string filter = "mobile_no=@mobile_no";
            var filterParams = new {
                mobile_no = parameter.MobileNo,
            };
            
            var contract = new Query("otp").Select("*").Where(filter, filterParams).Order("id").Limit(1).Page(1).Contract();

            var result = await _client.GetResponse<ListData>(contract);
            
            return  result.Message.response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DataService.Response> InsertOTPAsync(InsertOTPAsync_DA parameter) 
    {
        try
        {
            var values = new {
                mobile_no = parameter.MobileNo,
                code = parameter.OTP,
                sendcount = 1,
                lastsendat = (DateTimeOffset)DateTime.Now,
                createdat = (DateTimeOffset)DateTime.Now,
                //edited_by = parameter.Id,
            };

            var contract = new Query("otp").Insert(values).Contract();

            var result = await _client.GetResponse<ResultData>(contract);
            
            return result.Message.response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DataService.Response> UpdateOTPAsync(UpdateOTPAsync_DA parameter) 
    {
        try
        {
            var values = new {
                code = parameter.OTP,
                sendcount = parameter.SendCount,
                lastsendat = (DateTimeOffset)DateTime.Now,
                editedat = (DateTimeOffset)DateTime.Now,
            };

            string filter = "id=@id AND mobile_no=@mobile_no";
            var filterParams = new {
                id = parameter.Id,
                mobile_no = parameter.MobileNo,
            };

            var contract = new Query("otp").Update(values).Where(filter, filterParams).Contract();

            var result = await _client.GetResponse<ResultData>(contract);
            
            return result.Message.response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DataService.Response> UpdateOTPFailCountAsync(UpdateOTPFailCountAsync_DA parameter) 
    {
        try
        {
            var values = new {
                failcount = parameter.FailCount,
                lastfailat = (DateTimeOffset)DateTime.Now,
                editedat = (DateTimeOffset)DateTime.Now,
            };

            string filter = "id=@id AND mobile_no=@mobile_no";
            var filterParams = new {
                id = parameter.Id,
                mobile_no = parameter.MobileNo,
            };

            var contract = new Query("otp").Update(values).Where(filter, filterParams).Contract();

            var result = await _client.GetResponse<ResultData>(contract);
            
            return result.Message.response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<DataService.Response> ResetOTPAsync(ResetOTPAsync_DA parameter) 
    {
        try
        {
            var values = new {
                failcount = 0,
                sendcount = 0,
                editedat = (DateTimeOffset)DateTime.Now,
            };

            string filter = "id=@id AND mobile_no=@mobile_no";
            var filterParams = new {
                id = parameter.Id,
                mobile_no = parameter.MobileNo,
            };

            var contract = new Query("otp").Update(values).Where(filter, filterParams).Contract();

            var result = await _client.GetResponse<ResultData>(contract);
            
            return result.Message.response;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

}