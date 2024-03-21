using Codedberries.Models.DTOs;
using Codedberries.Models;

namespace Codedberries.Services
{
    public class TaskService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public TaskService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public async Task<Models.Task> CreateTask(HttpContext httpContext, TaskCreationRequestDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }


            var permission = userId.HasValue ? _authorizationService.CanCreateTask(userId.Value,request.ProjectId) : false;

            if (!permission)
            {
                throw new UnauthorizedAccessException("User does not have permission to create a task!");
            }

            
            Models.Task task = new Models.Task(request.Description, request.DueDate, userId.Value, request.Status, request.Priority, request.DifficultyLevel, request.CategoryId);
            if (request.DependencyIds != null && request.DependencyIds.Any())
            {
                foreach (int dependency_id in request.DependencyIds)
                {
                    var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id);

                    if (taskDependency != null)
                    {
                        task.Dependencies.Add(taskDependency);
                    }
                }
            }

            _databaseContext.Tasks.Add(task);
            await _databaseContext.SaveChangesAsync();

            return task;
        }
    }
}
