using Soenneker.Quark.DatePickers.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Quark.DatePickers.Tests;

[Collection("Collection")]
public sealed class DatePickerTests : FixturedUnitTest
{
    private readonly IDatePickerInterop _blazorlibrary;

    public DatePickerTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _blazorlibrary = Resolve<IDatePickerInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
