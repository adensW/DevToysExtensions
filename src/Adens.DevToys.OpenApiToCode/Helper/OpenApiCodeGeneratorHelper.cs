using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OpenApi.Extensions;
using System.Text.RegularExpressions;
using Scriban;

namespace Adens.DevToys.OpenApiToCode.Helper;
public class CodeApiPathResponseSchemaProperty
{
    public string Name { get; set; }
    public string Type { get; set; }
}
public class CodeApiPathResponseSchema
{
    public List<CodeApiPathResponseSchemaProperty> Properties { get; set; } = new List<CodeApiPathResponseSchemaProperty>();
}
public class CodeApiPath
{
    public string Operation{get; set;}
    public string Path { get; set; }
    public CodeApiPathResponseSchema ResponseSchema { get; set; }=new();
}
public class CodeApiPathHandler
{
    public string OperationMatchRegex { get; set; }
    public string PathMatchRegex { get; set; }
    public string CodeTemplate { get; set; }
    public string PathTemplate { get; set; }
}
internal static class OpenApiCodeGeneratorHelper
{
    internal static async Task GenerateCode(string url,string filter,List<CodeApiPathHandler> handlers)
    {
        var httpClient = new HttpClient();

        var stream = await httpClient.GetStreamAsync(url);
        List<CodeApiPath> codeApiPaths = new();
        // Read V3 as YAML
        var openApiDocument = new OpenApiStreamReader().Read(stream, out var diagnostic);
        var paths= openApiDocument.Paths.Where(z => Regex.IsMatch( z.Key,filter)).ToList();
        foreach (var path in paths)
        {
            var operations = path.Value.Operations.ToList();
            foreach (var operation in operations)
            {
                var codeApiPath = new CodeApiPath();
                codeApiPath.Operation = operation.Key.ToString().ToLower();
                codeApiPath.Path = path.Key;
                codeApiPaths.Add(codeApiPath);
            }
        }
        await Generate(codeApiPaths, handlers);
       
    }
    private static async Task Generate(List<CodeApiPath> codeApiPaths, List<CodeApiPathHandler> handlers)
    {
        foreach (var item in codeApiPaths)
        {
            foreach (var handler in handlers)
            {
                if(Regex.IsMatch(item.Operation,handler.OperationMatchRegex)&& Regex.IsMatch(item.Path, handler.PathMatchRegex))
                {
                    Generate(item, handler);
                }
            }
            

        }
    }
    private static async Task Generate(CodeApiPath codeApiPath, CodeApiPathHandler handler)
    {
        //string path = codeApiPath.Path.Replace("/api/navigator-admin/", "")
        //            .Replace('/', Path.DirectorySeparatorChar)
        //            .Replace("{", "[")
        //            .Replace("}", "]")
        //            ;
        var pathTemplate = Template.Parse(handler.PathTemplate);
        string path = pathTemplate.Render(codeApiPath);
        var codeTemplate = Template.Parse(handler.CodeTemplate);
        string code = codeTemplate.Render(codeApiPath);
        // ensure filepath created
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        // write code to file path
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            await fs.WriteAsync(Encoding.UTF8.GetBytes(code));
            await fs.FlushAsync();
        }
    }
}
