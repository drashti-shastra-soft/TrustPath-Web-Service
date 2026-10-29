using System;
using System.Linq;

namespace TrustPathWebService.Infrastructure.IService
{
    public interface IFileReaderService
    {
        string ReadFileContent(string filePath);
    }
}
