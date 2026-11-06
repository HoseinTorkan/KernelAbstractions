using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class CurrencyCodeTypeConverterTests
    : StringValueObjectTypeConverterTests<CurrencyCodeTypeConverter, CurrencyCode>
{
    protected override string ValidInput => "IRR";
    protected override string InvalidInput => "XYZ";
    protected override string ExpectedErrorCode => "Kernel.ValueObjects.CurrencyCode.INVALID";
}
