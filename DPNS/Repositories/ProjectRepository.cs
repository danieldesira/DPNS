using DPNS.DbModels;

namespace DPNS.Repositories
{
    public interface IProjectRepository
    {
        void AddProject(string name);
        Project? GetProject(Guid guid);
        Project? GetProject(string name);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly NeondbContext _dbContext;

        public ProjectRepository(NeondbContext dbContext) => _dbContext = dbContext;

        public void AddProject(string name)
        {
            _dbContext.Projects.Add(new Project
            {
                ProjectName = name,
                Guid = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            });
            _dbContext.SaveChanges();
        }

        public Project? GetProject(Guid guid)
        {
            return _dbContext.Projects.FirstOrDefault(p=> p.Guid == guid);
        }

        public Project? GetProject(string name)
        {
            return _dbContext.Projects.FirstOrDefault(p => p.ProjectName == name);
        }
    }
}
