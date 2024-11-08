using Common.WebApi.Utils.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Common.WebApi.Utils;

public class FileUtils : IFileUtils
{
    public async Task<string> SaveFile(IFormFile file, string path)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file!.FileName);
        string fullFilename = Path.Combine(currentDirectory, path, filename);
        string directory = Path.GetDirectoryName(fullFilename)!;

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using (FileStream stream = new(fullFilename, FileMode.Create))
            await file.CopyToAsync(stream);

        return filename;
    }
}