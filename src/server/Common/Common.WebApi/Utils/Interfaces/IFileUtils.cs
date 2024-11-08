using Microsoft.AspNetCore.Http;

namespace Common.WebApi.Utils.Interfaces;

public interface IFileUtils
{
    Task<string> SaveFile(IFormFile file, string path);
}