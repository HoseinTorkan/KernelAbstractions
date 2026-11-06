using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class EmailTypeConverterTests
    : StringValueObjectTypeConverterTests<EmailTypeConverter, Email>
{
    protected override string ValidInput => "user@example.com";
    protected override string InvalidInput => "not-an-email";
    protected override string ExpectedErrorCode => "Kernel.ValueObjects.Email.INVALID_FORMAT";
}
