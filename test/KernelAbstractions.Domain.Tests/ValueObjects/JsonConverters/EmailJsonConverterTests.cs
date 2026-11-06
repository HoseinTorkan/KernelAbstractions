using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class EmailJsonConverterTests
    : StringValueObjectJsonConverterTests<EmailJsonConverter, Email>
{
    protected override string ValidJson => "\"user@example.com\"";
    protected override Email ExpectedValue => Email.Create("user@example.com").Value;
}
