using System.Reflection;
using Microsoft.CodeAnalysis;

namespace AHI.Infrastructure.Interceptor.Abstraction
{
    public interface ILanguageService
    {
        SyntaxTree ParseText(string sourceCode, SourceCodeKind kind);
        Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations);
    }
}