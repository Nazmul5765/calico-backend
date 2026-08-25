using lofi_backend.Models;
using lofi_backend.Repository;

namespace lofi_backend.Service
{
    public interface IProjectService
    {
        List<Project> GetAllProjects();
        Project GetProject(int id);
        Task<Project> CreateProject(Project project);
        Task<Project> DeleteProject(int id); 
        Task<Project> EditProject(Project project);
    }
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;

        public ProjectService(IProjectRepository repository)
        {
            _repository = repository;
        }

        public List<Project> GetAllProjects()
        {
            return _repository.GetAllProjects();
        }

        public Project GetProject(int id)
        {
            try
            {
                return _repository.GetProject(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching project: {ex.Message}");
                throw;
            }
        }

        public async Task<Project> CreateProject(Project project)
        {
            return await _repository.CreateProject(project);
        }

        public async Task<Project> DeleteProject(int id)
        {
            return await _repository.DeleteProject(id);
        }

        public async Task<Project> EditProject(Project project)
        {
            return await _repository.EditProject(project);
        }
    }
}
