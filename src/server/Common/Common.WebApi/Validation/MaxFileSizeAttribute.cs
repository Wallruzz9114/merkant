using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Common.WebApi.Validation;

public class MaxFileSizeAttribute(int maxFileSize) : ValidationAttribute
{
    private readonly int _maxFileSize = maxFileSize;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is null)
        {
            return new ValidationResult("File is null.");
        }
        else
        {
            if (file.Length > _maxFileSize)
                return new ValidationResult($"Maximum allowed file size is {_maxFileSize} bytes.");

            return ValidationResult.Success!;
        }
    }
}