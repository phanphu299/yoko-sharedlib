using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using AHI.Infrastructure.Interceptor.Abstraction;
using Microsoft.CodeAnalysis;

namespace AHI.Infrastructure.Interceptor
{
    public class CompilerService : ICompilerService
    {
        private readonly ICollection<MetadataReference> _references;
        private readonly ILanguageService _languageService;
        public CompilerService(ILanguageService languageService)
        {
            this._languageService = languageService;
            var list = new List<string>();
            list.Add("System");
            list.Add("System.Runtime");
            list.Add("System.Linq");
            list.Add("System.Linq.Expressions");
            list.Add("System.Collections");
            list.Add("System.Text.RegularExpressions");
            list.Add("Microsoft.AspNetCore.Http");
            list.Add("Microsoft.AspNetCore.Http.Abstractions");
            list.Add("Newtonsoft.Json");
            list.Add("netstandard");
            list.Add("System.ComponentModel");
            list.Add("AHI.Infrastructure.Interceptor");
            list.Add("AHI.Infrastructure.SharedKernel");
            var listAssembly = list.Select(x => MetadataReference.CreateFromFile(
                                                                    Assembly.Load(x).Location) as MetadataReference);
            _references = listAssembly.ToList();
        }

        public void AddMetadataReference(MetadataReference metadataReference)
        {
            _references.Add(metadataReference);
        }

        public Assembly CompileToAssembly(string name, string code)
        {
            SyntaxTree syntaxTree = _languageService.ParseText(code, SourceCodeKind.Regular);

            Compilation compilation = _languageService
                                        .CreateLibraryCompilation(assemblyName: name, enableOptimisations: true)
                                        .AddReferences(_references)
                                        .AddSyntaxTrees(syntaxTree);

            using (var stream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream, pdbStream);
                if (emitResult.Success)
                {
                    return Assembly.Load(stream.ToArray(), pdbStream.ToArray());
                }
                else
                {
                    throw new System.Exception(string.Join("\n", emitResult.Diagnostics.ToList().Select(x => x.ToString())));
                }
            }
        }
    }
}
