using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Common.WebApi.Validation;

public class AllowedExtensionAttribute(string[] extensions) : ValidationAttribute
{
    private readonly string[] _extensions = extensions;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is null)
        {
            return new ValidationResult("File is null.");
        }
        else
        {
            string extension = Path.GetExtension(file.FileName);

            if (!_extensions.Contains(extension.ToLower()))
                return new ValidationResult($"This photo extension is not allowed!");

            return ValidationResult.Success!;
        }
    }
}