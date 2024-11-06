using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Adens.DevToys.ScribanRenderer.Helpers;
public static class ScribanTemplateGenerator
{
    public static async Task<string> GenerateTemplate(string template, string json)
    {
        var scriptObject1 = ScriptObject.From((object?)JsonSerializer.Deserialize<JsonElement>(json) ?? new { });
        scriptObject1.Import("guid", new Func<string>(() => Guid.NewGuid().ToString()));
        var context = new TemplateContext();
        context.PushGlobal(scriptObject1);
        var templateObj= Template.Parse(template);
        var result =await  templateObj.RenderAsync(context);
        return result;
    }
}
