using DevFreela.Core.Entities;
using Xunit;

namespace DevFreela.Tests.Entities;

public class SkillTests
{
    [Fact]
    public void Constructor_ShouldInitializeSkillWithDescriptionAndCreatedAt()
    {
        var beforeCreation = DateTime.Now;

        var skill = new Skill("C#");

        var afterCreation = DateTime.Now;
        Assert.Equal("C#", skill.Description);
        Assert.InRange(skill.CreatedAt, beforeCreation, afterCreation);
    }
}
