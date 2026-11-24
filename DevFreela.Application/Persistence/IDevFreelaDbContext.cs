using DevFreela.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Application.Persistence
{
    public interface IDevFreelaDbContext
    {
        DbSet<Project> Projects { get; }
        DbSet<User> Users { get; }
        DbSet<Skill> Skills { get; }
        DbSet<UserSkill> UserSkills { get; }
        DbSet<ProjectComment> ProjectComments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}