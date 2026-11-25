using DevFreela.Core.DTOs;
using Xunit;

namespace DevFreela.Tests.DTOs;

public class SkillDTOTests
{
    [Fact]
    public void Properties_ShouldAllowGetAndSet()
    {
        var dto = new SkillDTO
        {
            Id = 42,
            Description = "ASP.NET Core"
        };

        Assert.Equal(42, dto.Id);
        Assert.Equal("ASP.NET Core", dto.Description);
    }
}
