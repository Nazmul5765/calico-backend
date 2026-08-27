using lofi_backend.Controllers;
using lofi_backend.Service;
using lofi_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using Moq;
using System.Security.Claims;

namespace Testing.TimerTest;

public class ControllerTesting
{
    private Mock<ITaskTimerService> _mockService;
    private Mock<IProjectService> _mockProjectService;
    private TaskTimersController _taskTimerController;
    [SetUp]
    public void Setup()
    {
        _mockService = new Mock<ITaskTimerService>();
        _mockProjectService = new Mock<IProjectService>();
        _taskTimerController = new TaskTimersController(_mockService.Object, _mockProjectService.Object);
    }

    private void SetLoggedInUser(string userId)
    {
        var claims = new List<Claim> { new Claim("sub", userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");

        _taskTimerController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    private static Project OwnedProject(int id, string userId) => new Project
    {
        Id = id,
        Name = "Test Project",
        StartDate = new DateTime(2026, 1, 1),
        EndDate = new DateTime(2026, 1, 2),
        Timers = new List<TaskTimer>(),
        UserId = userId
    };

    [Test]
    public void GetTimerByTimerId_ShouldReturnOK()
    {
        var expectedTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };

        SetLoggedInUser("101");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(expectedTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = _taskTimerController.GetTimerByTimerId(1) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.ShouldBeOfType<OkObjectResult>();
        result.Value.ShouldBe(expectedTimer);
    }

    [Test]
    public void GetTimerByTimerId_ForbidsOtherUsersTimer()
    {
        var expectedTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };

        SetLoggedInUser("999");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(expectedTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = _taskTimerController.GetTimerByTimerId(1);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task CreateNewTimer_ShouldReturnCreated201()
    {
        var addNewtaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("101");
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));
        _mockService.Setup(service => service.CreateNewTimer(addNewtaskTimer)).ReturnsAsync(addNewtaskTimer);
        var result = await _taskTimerController.CreateNewTimer(addNewtaskTimer) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status201Created);

    }

    [Test]
    public async Task CreateNewTimer_ForbidsAddingToAnotherUsersProject()
    {
        var addNewtaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("999");
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = await _taskTimerController.CreateNewTimer(addNewtaskTimer);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task EditTimer_ShouldReturnUpdatedTimer()
    {
        var existingTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        var updatedTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("101");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(existingTaskTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));
        _mockService.Setup(service => service.EditTimer(updatedTaskTimer)).ReturnsAsync(updatedTaskTimer);

        var result = await _taskTimerController.EditTimer(updatedTaskTimer) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.ShouldBeOfType<OkObjectResult>();
        result.Value.ShouldBe(updatedTaskTimer);
    }

    [Test]
    public async Task EditTimer_ShouldReturnBadRequest()
    {
        var existingTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        var updatedTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("101");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(existingTaskTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));
        _mockService.Setup(service => service.EditTimer(updatedTaskTimer)).Throws(new Exception());

        var result = await _taskTimerController.EditTimer(updatedTaskTimer) as ObjectResult;
        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task EditTimer_ForbidsEditingAnotherUsersTimer()
    {
        var existingTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        var updatedTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("999");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(existingTaskTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = await _taskTimerController.EditTimer(updatedTaskTimer);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task DeleteTimer_ShouldReturnNoContent()
    {
        var deletedTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };

        SetLoggedInUser("101");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(deletedTaskTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));
        _mockService.Setup(service => service.DeleteTimer(1)).ReturnsAsync(deletedTaskTimer);

        var result = await _taskTimerController.DeleteTimer(1);

        result.ShouldBeOfType<NoContentResult>();
    }

    [Test]
    public async Task DeleteTimer_ShouldReturnNotFound_WhenTimerDoesNotExist()
    {
        SetLoggedInUser("101");
        _mockService.Setup(service => service.GetTimerByTimerId(99)).Throws(new Exception("Timer not found"));

        var result = await _taskTimerController.DeleteTimer(99) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task DeleteTimer_ForbidsDeletingAnotherUsersTimer()
    {
        var existingTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400,
            IsActive = false,
            ProjectId = 1
        };
        SetLoggedInUser("999");
        _mockService.Setup(service => service.GetTimerByTimerId(1)).Returns(existingTaskTimer);
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = await _taskTimerController.DeleteTimer(1);

        result.ShouldBeOfType<ForbidResult>();
    }

    [Test]
    public async Task GetTimerByProjectId_ShouldReturnOk()
    {
        var taskTimers = new List<TaskTimer>
        {
            new TaskTimer
            {
                Id = 1,
                DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
                DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
                Duration = 5400, // 1 hour 30 minutes
                IsActive = false,
                ProjectId = 1
            },

            new TaskTimer
            {
                Id = 2,
                DateCreated = new DateTime(2026, 6, 2, 14, 0, 0),
                DateUpdated = new DateTime(2026, 6, 2, 14, 25, 0),
                Duration = 1500, // 25 minute Pomodoro
                IsActive = false,
                ProjectId = 1
            },

            new TaskTimer
            {
                Id = 3,
                DateCreated = new DateTime(2026, 6, 3, 18, 0, 0),
                DateUpdated = new DateTime(2026, 6, 3, 19, 0, 0),
                Duration = 3600, // 1 hour
                IsActive = false,
                ProjectId = 1
            }
        };
        SetLoggedInUser("101");
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));
        _mockService.Setup(service => service.GetAllTimersByProjectId(1)).ReturnsAsync(taskTimers);

        var result = await _taskTimerController.GetTimerByProjectId(1) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status200OK);
        result.ShouldBeOfType<OkObjectResult>();
    }
    [Test]
    public async Task GetTimerByProjectId_ShouldReturnBadRequest()
    {
        var result = await _taskTimerController.GetTimerByProjectId(-1) as ObjectResult ;

        result.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task GetTimerByProjectId_ShouldReturnNotFound()
    {
        SetLoggedInUser("101");
        _mockProjectService.Setup(service => service.GetProject(99)).Throws(new Exception("ProjectId does not exist"));

        var result = await _taskTimerController.GetTimerByProjectId(99) as ObjectResult;

        result.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task GetTimerByProjectId_ForbidsOtherUsersProject()
    {
        SetLoggedInUser("999");
        _mockProjectService.Setup(service => service.GetProject(1)).Returns(OwnedProject(1, "101"));

        var result = await _taskTimerController.GetTimerByProjectId(1);

        result.ShouldBeOfType<ForbidResult>();
    }
}
