using KernelAbstractions.Domain.ValueObjects;
using KernelAbstractions.Domain.ValueObjects.JsonConverters;

namespace KernelAbstractions.Domain.Tests.ValueObjects.JsonConverters;

public class PersonNameJsonConverterTests
    : StringValueObjectJsonConverterTests<PersonNameJsonConverter, PersonName>
{
    protected override string ValidJson => "\"علی\"";
    protected override PersonName ExpectedValue => PersonName.Create("علی").Value;
}
