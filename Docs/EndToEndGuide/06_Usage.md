# Usage Examples

## Basic Test Generation
```csharp
var generator = host.Services.GetRequiredService<ITestGenerator>();

// Simple login page test
var description = @"
Login page with:
- Username field (id: username)
- Password field (id: password)
- Login button (class: submit-btn)
- Error message (id: error-msg)
";

var test = await generator.GenerateTestAsync(description);
```

## Custom Templates

1. Define Template:
```handlebars
// Templates/CustomTest.hbs
namespace {{ns}}.{{pageName}}
{
    public class {{pageName}}Tests : PlaywrightTest
    {
        {{#each elements}}
        private ILocator {{name}} => Page.Locator("{{selector}}");
        {{/each}}

        {{#each testCases}}
        [Test]
        public async Task {{name}}()
        {
            {{#each steps}}
            await {{elementName}}.{{formatAction action}}({{#each parameters}}"{{this}}"{{/unless @last}}, {{/each}});
            {{/each}}
        }
        {{/each}}
    }
}
```

2. Use Custom Template:
```csharp
var options = new TestGenerationOptions {
    TemplatePath = "Templates/CustomTest.hbs",
    Namespace = "E2ETests"
};

var test = await generator.GenerateTestAsync(description, options);
```

## Advanced Scenarios

1. Multi-Step Workflow:
```csharp
var checkoutFlow = @"
Product page:
- Add to cart button (#add-cart)
- Quantity input (#qty)
- Cart total (.total)
- Checkout button (#checkout)

Cart requirements:
- Update quantity
- Verify total
- Proceed to checkout
";

var tests = await generator.GenerateTestAsync(new TestGenerationRequest {
    PageDescription = checkoutFlow,
    Options = new() {
        GenerateDataModels = true,
        IncludeComments = true
    }
});
```

2. Data-Driven Tests:
```csharp
var request = new TestGenerationRequestBuilder()
    .WithPageDescription(description)
    .WithContext("testData", "products.json")
    .WithOptions(opt => {
        opt.WithFramework("xUnit")
           .WithDataDriven(true);
    })
    .Build();

var tests = await generator.GenerateTestAsync(request);
```