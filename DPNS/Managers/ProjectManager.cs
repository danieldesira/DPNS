using DPNS.Repositories;

namespace DPNS.Managers
{
    public interface IProjectManager
    {
        public void AddProject(string projectName);
    }

    public class ProjectManager : IProjectManager
    {
        private IProjectRepository _projectRepository;

        public ProjectManager(IProjectRepository projectRepository) => _projectRepository = projectRepository;

        public void AddProject(string projectName)
        {
            if (_projectRepository.GetProject(projectName) != null)
            {
                throw new InvalidOperationException("Project name already exists");
            }

            _projectRepository.AddProject(projectName);
        }
    }
}
