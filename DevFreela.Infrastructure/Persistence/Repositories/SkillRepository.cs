using DevFreela.Core.DTOs;
using DevFreela.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;

namespace DevFreela.Infrastructure.Persistence.Repositories
{
	public class SkillRepository : ISkillRepository
	{
		private readonly string _connectionString;
		public SkillRepository(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString("DevFreelaCs");
		}

		public async Task<List<SkillDTO>> GetAll()
		{
			using (var SqlConnection = new SqlConnection(_connectionString))
			{
				SqlConnection.Open();

				var script = "SELECT Id, Description FROM Skills";

				var skills = await SqlConnection.QueryAsync<SkillDTO>(script);

				return skills.ToList();
			}
		}
	}
}