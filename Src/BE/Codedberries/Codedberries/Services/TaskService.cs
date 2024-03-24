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

            
            Models.Task task = new Models.Task(request.Name, request.Description, request.DueDate, userId.Value, request.Status, request.Priority, request.DifficultyLevel, request.CategoryId, request.ProjectId);
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

        public List<ProjectTasksInfoDTO> GetTasksByFilters(TaskFilterParamsDTO filterParams)
        {
            System.Linq.IQueryable<Codedberries.Models.Task> query = _databaseContext.Tasks;

            if (filterParams == null || filterParams.IsEmpty())
            {
                throw new ArgumentException("No filters were chosen!");
            }
            else // applying filters if there any
            {
                if (filterParams.ProjectId != 0)
                {
                    query = query.Where(t => t.ProjectId == filterParams.ProjectId);
                }
                if (filterParams.AssignedTo.HasValue)
                {
                    query = query.Where(t => t.UserId == filterParams.AssignedTo);
                }
                if (!string.IsNullOrEmpty(filterParams.Status))
                {
                    query = query.Where(t => t.Status == filterParams.Status);
                }
                if (!string.IsNullOrEmpty(filterParams.Priority))
                {
                    query = query.Where(t => t.Priority == filterParams.Priority);
                }
                if (filterParams.DifficultyLevelGreaterThan.HasValue)
                {
                    query = query.Where(t => t.DifficultyLevel > filterParams.DifficultyLevelGreaterThan.Value);
                }

                if (filterParams.DifficultyLevelLesserThan.HasValue)
                {
                    query = query.Where(t => t.DifficultyLevel < filterParams.DifficultyLevelLesserThan.Value);
                }

                if (filterParams.DifficultyLevelEquals.HasValue)
                {
                    query = query.Where(t => t.DifficultyLevel == filterParams.DifficultyLevelEquals.Value);
                }
                if (filterParams.DueDateAfter.HasValue)
                {
                    query = query.Where(t => t.DueDate >= filterParams.DueDateAfter);
                }
                if (filterParams.DueDateBefore.HasValue)
                {
                    query = query.Where(t => t.DueDate <= filterParams.DueDateBefore);
                }
                if (filterParams.CategoryId.HasValue)
                {
                    query = query.Where(t => t.CategoryId == filterParams.CategoryId);
                }
            }


            List<Codedberries.Models.Task> tasks = query.ToList();

            List<ProjectTasksInfoDTO> tasksDTO = tasks.Select(t => new ProjectTasksInfoDTO
            {
                Name = t.Name,
                Description = t.Description,
                Category = t.Category.Name,
                Priority = t.Priority,
                Status = t.Status,
                DueDate = t.DueDate,
                AssignedTo = _databaseContext.Users
                    .Where(u => u.Id == t.UserId)
                    .Select(u => new TaskUserInfoDTO
                    {
                        Id = u.Id,
                        FirstName = u.Firstname,
                        LastName = u.Lastname,
                        ProfilePicture = u.ProfilePicture
                    })
                    .ToList()
            }).ToList();

            return tasksDTO;
        }
    }
}
