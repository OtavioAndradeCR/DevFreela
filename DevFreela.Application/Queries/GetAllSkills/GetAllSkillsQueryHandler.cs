using DevFreela.Application.ViewModels;
using DevFreela.Core.Repositories;
using MediatR;

namespace DevFreela.Application.Queries.GetAllSkills
{
	public class GetAllSkillsQueryHandler : IRequestHandler<GetAllSkillsQuery, List<SkillViewModel>>
	{
		private readonly ISkillRepository _skillRepository;

		public GetAllSkillsQueryHandler(ISkillRepository skillRepository)
		{
			_skillRepository = skillRepository;
		}

		public async Task<List<SkillViewModel>> Handle(GetAllSkillsQuery request, CancellationToken cancellationToken)
		{
			var skills = await _skillRepository.GetallSkillsAsync();

			var skiilsViewModel = skills
				.Select(skill => new SkillViewModel(skill.Id, skill.Description))
				.ToList();

			return skiilsViewModel;
		}
	}
}