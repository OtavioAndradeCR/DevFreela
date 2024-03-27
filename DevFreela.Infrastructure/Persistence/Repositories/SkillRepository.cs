using DevFreela.Core.DTOs;
using DevFreela.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using DevFreela.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevFreela.Infrastructure.Persistence.Repositories
{
	public class SkillRepository : ISkillRepository
	{
		private readonly DevFreelaDbContext _dbContext;

		public SkillRepository(DevFreelaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Skill>> GetallSkillsAsync()
		{
			return await _dbContext.Skills.ToListAsync();
		}
    }
}