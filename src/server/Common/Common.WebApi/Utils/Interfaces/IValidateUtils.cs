using Common.WebApi.Results;

namespace Common.WebApi.Utils.Interfaces;

public interface IValidateUtils
{
    bool ValidateModel<TModel, TResponseModel>(TModel model, ref Result<TResponseModel>? result);
}