using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class NationalCodeTypeConverterTests
    : StringValueObjectTypeConverterTests<NationalCodeTypeConverter, NationalCode>
{
    protected override string ValidInput => "0499370899";
    protected override string InvalidInput => "123";
    protected override string ExpectedErrorCode => "Kernel.ValueObjects.NationalCode.INVALID_FORMAT";
}
