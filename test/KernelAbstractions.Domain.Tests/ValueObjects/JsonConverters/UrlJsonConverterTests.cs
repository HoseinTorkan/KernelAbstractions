using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class UrlJsonConverterTests
    : StringValueObjectJsonConverterTests<UrlJsonConverter, Url>
{
    protected override string ValidJson => "\"https://example.com\"";
    protected override Url ExpectedValue => Url.Create("https://example.com").Value;
}
