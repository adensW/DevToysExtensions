
using Adens.DevToys.ScribanRenderer.Helpers;

namespace Adens.DevToys.ScribanRenderer.Tests;
public class ScribanTemplateGeneratorTests:TestBase
{

    [Fact]
    public async Task ScibanTemplateGenerateShouldBeOk() {
        string json = "{ \"name\" : \"Bob Smith\", \"address\" : \"1 Smith St, Smithville\", \"orderId\" : \"123455\", \"total\" : 23435.34, \"items\" : [ { \"name\" : \"1kg carrots\", \"quantity\" : 1, \"total\" : 4.99 }, { \"name\" : \"2L Milk\", \"quantity\" : 1, \"total\" : 3.5 } ] }";
        string templateStr = @"Dear {{ name }},

Your order, {{ orderId}}, is now ready to be collected.

Your order shall be delivered to {{ address }}.  If you need it delivered to another location, please contact as ASAP.

Order: {{ orderId}}
Total: {{ total | math.format ""c"" ""en-US"" }}

Items:
------
{{- for item in items }}
 * {{ item.quantity }} x {{ item.name }} - {{ item.total | math.format ""c"" ""en-US"" }}
{{- end }}

Thanks,
BuyFromUs";
        string output = @"Dear Bob Smith,

Your order, 123455, is now ready to be collected.

Your order shall be delivered to 1 Smith St, Smithville.  If you need it delivered to another location, please contact as ASAP.

Order: 123455
Total: $23,435.34

Items:
------
 * 1 x 1kg carrots - $4.99
 * 1 x 2L Milk - $3.50

Thanks,
BuyFromUs";
        var result = await ScribanTemplateGenerator.GenerateTemplate(templateStr, json);
        Assert.Equal(output, result);
    }
  

}
