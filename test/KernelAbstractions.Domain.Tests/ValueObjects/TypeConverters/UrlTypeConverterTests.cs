using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class UrlTypeConverterTests
    : StringValueObjectTypeConverterTests<UrlTypeConverter, Url>
{
    protected override string ValidInput => "https://example.com";
    protected override string InvalidInput => "not-a-url";
    protected override string ExpectedErrorCode => "Kernel.ValueObjects.Url.INVALID_FORMAT";
}
