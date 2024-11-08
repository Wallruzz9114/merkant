using System.ComponentModel.DataAnnotations;
using Common.WebApi.Results;
using Common.WebApi.Utils.Interfaces;

namespace Common.WebApi.Utils;

public class ValidateUtils : IValidateUtils
{
    public bool ValidateModel<TModel, TResponseModel>(TModel model, ref Result<TResponseModel>? result)
    {
        ValidationContext context = new(model!);
        List<ValidationResult> validationResults = new();
        bool isValid = Validator.TryValidateObject(model!, context, validationResults, true);

        if (!isValid)
            result = Result.Fail<TResponseModel>(validationResults.Select(e => new ErrorResult(e.ErrorMessage!)).ToArray());

        return isValid;
    }
}