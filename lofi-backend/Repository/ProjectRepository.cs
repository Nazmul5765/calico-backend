using lofi_backend.Database;
using lofi_backend.Models;

namespace lofi_backend.Repository
{
    public interface IProjectRepository
    {
        List<Project> GetAllProjects();
        Project GetProject(int id);
        Task<Project> CreateProject(Project project);
        Task<Project> DeleteProject(int id);

        Task<Project> EditProject(Project project);
    }

    public class ProjectRepository : IProjectRepository
    {
        private readonly LoFiDbContext _db;

        public ProjectRepository(LoFiDbContext dbContext)
        {
            _db = dbContext;
        }

        public List<Project> GetAllProjects()
        {
            return _db.Projects.ToList();
        }

        public Project GetProject(int id)
        {
            return _db.Projects.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Project not found");
        }

        public async Task<Project> CreateProject(Project project)
        {
            await _db.Projects.AddAsync(project);
            await _db.SaveChangesAsync();

            return project;
        }

        public async Task<Project> DeleteProject(int id)
        {
            var projectToDelete = _db.Projects.FirstOrDefault(p => p.Id == id);
            if (projectToDelete != null)
            {
                _db.Projects.Remove(projectToDelete);
            }
            await _db.SaveChangesAsync();
            
            return projectToDelete;
        }

        public async Task<Project> EditProject(Project project)
        {
            if (!_db.Projects.Contains(project)) throw new Exception("Project doesnt exists");

            var editProject = _db.Projects.Update(project).Entity;
            await _db.SaveChangesAsync();
            return editProject;
        }

    }
}
