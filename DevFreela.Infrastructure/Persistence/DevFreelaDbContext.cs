using System.Collections.Generic;
using DevFreela.Core.Entities;

namespace DevFreela.Infrastructure.Persistence
{
    public class DevFreelaDbContext
    {
        public DevFreelaDbContext()
        {
            Projects = new List<Project>
            {
                new Project("Meu projeto ASPNET Core 1", "Minha Descrição do projeto 1", 1, 1, 10000),
                new Project("Meu projeto ASPNET Core 2", "Minha Descrição do projeto 2", 1, 1, 20000),
                new Project("Meu projeto ASPNET Core 3", "Minha Descrição do projeto 3", 1, 1, 30000)
            };

            Users = new List<User>
            {
                new User("Otavio Andrade", "otavio@hotmail.com", new DateTime(1994, 12, 1)),
                new User("Adriana Valeria", "adriana@hotmail.com", new DateTime(1971, 10, 1)),
                new User("Marcos Aurélio", "marcos@hotmail.com", new DateTime(1967, 3, 2)),
            };

            Skills = new List<Skill>
            {
                new Skill(".Net Core"),
                new Skill("C#"),
                new Skill("SQL")
            };
        }

        public List<Project> Projects { get; set; }
        public List<User> Users { get; set; }
        public List<Skill> Skills { get; set; }
        public List<ProjectComment> ProjectComments { get; set; }
    }
}
