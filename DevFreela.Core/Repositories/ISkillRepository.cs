using DevFreela.Core.DTOs;
using DevFreela.Core.Entities;

namespace DevFreela.Core.Repositories;

public interface ISkillRepository
{
	public Task<List<Skill>> GetallSkillsAsync();
}
