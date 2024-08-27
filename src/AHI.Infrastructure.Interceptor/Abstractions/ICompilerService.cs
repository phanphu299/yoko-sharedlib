using System.Reflection;
using Microsoft.CodeAnalysis;

namespace AHI.Infrastructure.Interceptor.Abstraction
{
    public interface ICompilerService
    {
        void AddMetadataReference(MetadataReference metadataReference);
        Assembly CompileToAssembly(string name, string code);
    }
}