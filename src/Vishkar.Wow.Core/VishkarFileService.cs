using System;
using System.Collections.Generic;
using System.IO;

namespace Vishkar.Wow.Core
{
  public interface IVishkarFileService
  {
    Task EnsureDirectoryExistsAsync(string folderPath);
    Task<List<string>> GetAllFilesInFolderPathAsync(string folderPath, string searchString);
    Task<string> ReadAllTextAsync(string filePath);
    Task WriteAllTextAsync(string filePath, string textContents);
  }

  public class VishkarFileService : IVishkarFileService
  {
    public async Task EnsureDirectoryExistsAsync(string folderPath)
    {
      if (Directory.Exists(folderPath)) return;
      Directory.CreateDirectory(folderPath);
    }

    public async Task<List<string>> GetAllFilesInFolderPathAsync(string folderPath, string searchString)
    {
      var list = new List<string>();
      if (!Directory.Exists(folderPath)) return list;

      var di = new DirectoryInfo(folderPath);
      var fileList = di.GetFiles(searchString, SearchOption.TopDirectoryOnly);
      foreach ( var info in fileList )
      {
        list.Add(info.FullName);
      }
      return list;
    }

    public async Task<string> ReadAllTextAsync(string filePath)
    {
      //
      return await File.ReadAllTextAsync(filePath);
    }

    public async Task WriteAllTextAsync(string filePath, string textContents)
    {
      //
      await File.WriteAllTextAsync(filePath, textContents);
    }
  }
}
