using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.TypeConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.TypeConverters;

public class PersonNameTypeConverterTests
    : StringValueObjectTypeConverterTests<PersonNameTypeConverter, PersonName>
{
    protected override string ValidInput => "علی";
    protected override string InvalidInput => "علی123";
    protected override string ExpectedErrorCode => "Kernel.ValueObjects.PersonName.INVALID_CHARACTERS";
}
