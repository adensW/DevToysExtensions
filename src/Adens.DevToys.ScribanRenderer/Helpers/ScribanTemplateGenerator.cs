using Scriban;
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
        var templateObj= Template.Parse(template);
        var result =await  templateObj.RenderAsync((object?)JsonSerializer.Deserialize<JsonElement>(json) ?? new { }, null);
        return result;
    }
}
