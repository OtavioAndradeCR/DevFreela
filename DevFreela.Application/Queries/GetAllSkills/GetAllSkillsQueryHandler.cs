using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DevFreela.Application.ViewModels;
using DevFreela.Infrastructure.Persistence;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DevFreela.Application.Queries.GetAllSkills
{
	public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<SkillViewModel>>
	{
		private readonly DevFreelaDbContext _dbContext;

		public GetAllSkillsQueryHandler(DevFreelaDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<SkillViewModel>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
		{
			var skills = _dbContext.Skills;

			var skiilsViewModel = await skills
				.Select(skill => new SkillViewModel(skill.Id, skill.Description))
				.ToListAsync();

			return skiilsViewModel;
		}
	}
}