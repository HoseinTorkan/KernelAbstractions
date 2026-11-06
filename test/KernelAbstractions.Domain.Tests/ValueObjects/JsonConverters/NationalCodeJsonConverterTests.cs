using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class NationalCodeJsonConverterTests
    : StringValueObjectJsonConverterTests<NationalCodeJsonConverter, NationalCode>
{
    protected override string ValidJson => "\"0499370899\"";
    protected override NationalCode ExpectedValue => NationalCode.Create("0499370899").Value;
}
