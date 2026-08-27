using lofi_backend.Controllers;
using lofi_backend.Service;
using lofi_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using System.Security.Claims;

namespace Testing.Projects;

public class ControllerTesting
{
    private Mock<IProjectService> _projectServiceMock;
    private ProjectsController _projectsController;
    [SetUp]
    public void Setup()
    {
        _projectServiceMock = new Mock<IProjectService>();
        _projectsController = new ProjectsController(_projectServiceMock.Object);
    }

    private void SetLoggedInUser(string userId)
    {
        var claims = new List<Claim> { new Claim("sub", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");

        _projectsController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    [Test]
    public void GetAllProjects_ReturnsOnlyOwnProjects()
    {
        var projectList = new List<Project>
        {
            new Project
            {
                Id = 1,
                Name = "Website Redesign",
                StartDate = new DateTime(2026, 1, 15),
                EndDate = new DateTime(2026, 4, 30),
                Timers = new List<TaskTimer>(),
                UserId = "101"
            },
            new Project
            {
                Id = 2,
                Name = "Mobile App Development",
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 9, 15),
                Timers = new List<TaskTimer>(),
                UserId = "1"
            },
            new Project
            {
                Id = 3,
                Name = "Data Migration Strategy",
                StartDate = new DateTime(2026, 5, 20),
                EndDate = new DateTime(2026, 7, 1),
                Timers = new List<TaskTimer>(),
                UserId = "102"
            }
        };
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetAllProjects()).Returns(projectList);

        var result = _projectsController.GetAllProjects() as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        var returnedProjects = result.Value as IEnumerable<Project>;
        returnedProjects.ShouldNotBeNull();
        returnedProjects.Count().ShouldBe(1);
        returnedProjects.First().Id.ShouldBe(1);
    }

    [Test]
    public void GetProject_ShouldReturnProjectById()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(project);

        var result = _projectsController.GetProject(1) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.Value.ShouldBe(project);
    }

    [Test]
    public void GetProject_ForbidsOtherUsersProject()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("999");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(project);

        var result = _projectsController.GetProject(1);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task CreateProject_ShouldReturnCreatedProject()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.CreateProject(project)).ReturnsAsync(project);

        var result = await _projectsController.CreateProject(project) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status201Created);
        result.Value.ShouldBe(project);
        project.UserId.ShouldBe("101");
    }

    [Test]
    public async Task DeleteProject_ShouldDeleteProject()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(project);
        _projectServiceMock.Setup(service => service.DeleteProject(1)).ReturnsAsync(project);

        var result = await _projectsController.DeleteProject(1) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.Value.ShouldBe(project);
    }

    [Test]
    public async Task DeleteProject_ReturnsBadRequest_WhenIdIsZero()
    {
        var result = await _projectsController.DeleteProject(0) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task DeleteProject_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetProject(999)).Throws(new Exception());

        var result = await _projectsController.DeleteProject(999) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task DeleteProject_ForbidsOtherUsersProject()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("999");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(project);

        var result = await _projectsController.DeleteProject(1);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task EditProject_ReturnsUpdatedProject()
    {
        var project = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        var updatedProject = new Project
        {
            Id = 1,
            Name = "Mobile App Development",
            StartDate = new DateTime(2026, 3, 1),
            EndDate = new DateTime(2026, 9, 15),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };

        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(project);
        _projectServiceMock.Setup(service => service.EditProject(updatedProject)).ReturnsAsync(updatedProject);

        var result = await _projectsController.EditProject(updatedProject) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.ShouldBeOfType<OkObjectResult>();

    }

    [Test]
    public async Task EditProject_ProjectDoesNotExist()
    {
        var updatedProject = new Project
        {
            Id = 1,
            Name = "Mobile App Development",
            StartDate = new DateTime(2026, 3, 1),
            EndDate = new DateTime(2026, 9, 15),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("101");
        _projectServiceMock.Setup(service => service.GetProject(1)).Throws(new Exception());

        var result = await _projectsController.EditProject(updatedProject) as ObjectResult;
        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        result.ShouldBeOfType<BadRequestObjectResult>();

    }

    [Test]
    public async Task EditProject_ForbidsEditingAnotherUsersProject()
    {
        var existingProject = new Project
        {
            Id = 1,
            Name = "Website Redesign",
            StartDate = new DateTime(2026, 1, 15),
            EndDate = new DateTime(2026, 4, 30),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        var updatedProject = new Project
        {
            Id = 1,
            Name = "Mobile App Development",
            StartDate = new DateTime(2026, 3, 1),
            EndDate = new DateTime(2026, 9, 15),
            Timers = new List<TaskTimer>(),
            UserId = "101"
        };
        SetLoggedInUser("999");
        _projectServiceMock.Setup(service => service.GetProject(1)).Returns(existingProject);

        var result = await _projectsController.EditProject(updatedProject);

        result.ShouldBeOfType<ForbidResult>();
    }
}
