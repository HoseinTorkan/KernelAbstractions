using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class CurrencyCodeJsonConverterTests
    : StringValueObjectJsonConverterTests<CurrencyCodeJsonConverter, CurrencyCode>
{
    protected override string ValidJson => "\"IRR\"";
    protected override CurrencyCode ExpectedValue => CurrencyCode.Create("IRR").Value;
}
