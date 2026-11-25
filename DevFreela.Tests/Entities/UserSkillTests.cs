using DevFreela.Core.Entities;
using Xunit;

namespace DevFreela.Tests.Entities;

public class UserSkillTests
{
    [Fact]
    public void Constructor_ShouldInitializeUserAndSkillIds()
    {
        var userSkill = new UserSkill(idUser: 10, idSkill: 20);

        Assert.Equal(10, userSkill.IdUser);
        Assert.Equal(20, userSkill.IdSkill);
        Assert.Null(userSkill.Skill);
    }
}
