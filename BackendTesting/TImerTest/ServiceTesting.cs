using lofi_backend.Repository;
using lofi_backend.Service;
using lofi_backend.Models;
using Moq;
using Shouldly;

namespace Testing.TimerTest;

public class ServiceTesting
{
    private Mock<ITaskTimerRepository> _mockRepo;
    private TaskTimerService _taskTimerService;

    [SetUp]
    public void Setup()
    {
        _mockRepo = new Mock<ITaskTimerRepository>();
        _taskTimerService = new TaskTimerService(_mockRepo.Object);
    }

    [Test]
    public void GetTimerByTimerId_ReturnsTimer()
    {
       
        var taskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };
        _mockRepo.Setup(repo => repo.GetTimerByTimerId(1)).Returns(taskTimer);
        
        var result = _taskTimerService.GetTimerByTimerId(1);
        
        result.ShouldBe(taskTimer);
    }
    [Test]
    public async Task CreateNewTimer_ShouldReturnNewTimer()
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

        _mockRepo.Setup(repo => repo.CreateNewTimer(addNewtaskTimer)).ReturnsAsync(addNewtaskTimer);

        var result = await _taskTimerService.CreateNewTimer(addNewtaskTimer);

        result.ShouldBe(addNewtaskTimer);
    }
    [Test]
    public async Task EditTimer_ShouldReturnUpdatedTimer()
    {
        var updatedTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };
        _mockRepo.Setup(repo => repo.EditTimer(updatedTaskTimer)).ReturnsAsync(updatedTaskTimer);

        var result = await  _taskTimerService.EditTimer(updatedTaskTimer);

        Assert.That(result, Is.EqualTo(updatedTaskTimer));
    }
    [Test]
    public async Task DeleteTimer_ShouldReturnDeletedTimer()
    {
        var deleteTaskTimer = new TaskTimer
        {
            Id = 1,
            DateCreated = new DateTime(2026, 6, 1, 9, 0, 0),
            DateUpdated = new DateTime(2026, 6, 1, 10, 30, 0),
            Duration = 5400, // 1 hour 30 minutes
            IsActive = false,
            ProjectId = 1
        };

        _mockRepo.Setup(repo => repo.DeleteTimer(1)).ReturnsAsync(deleteTaskTimer);
        // Act
        var result = _taskTimerService.DeleteTimer(1);
        // Assert
        _mockRepo.Verify(repo => repo.DeleteTimer(1), Times.Once);
    }
    [Test]
    public async Task GetAllTimersByProjectId_ShouldReturnAllTimersByProjectId()
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

        _mockRepo.Setup(repo => repo.GetAllTimersByProjectId(1)).ReturnsAsync(taskTimers);
       
        var result = await _taskTimerService.GetAllTimersByProjectId(1);

        result.ShouldBe(taskTimers);
    }
}
