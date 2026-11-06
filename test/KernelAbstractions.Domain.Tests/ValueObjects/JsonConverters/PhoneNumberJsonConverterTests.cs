using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class PhoneNumberJsonConverterTests
    : StringValueObjectJsonConverterTests<PhoneNumberJsonConverter, PhoneNumber>
{
    protected override string ValidJson => "\"09121234567\"";
    protected override PhoneNumber ExpectedValue => PhoneNumber.Create("09121234567").Value;
}
