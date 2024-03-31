using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.EntityFrameworkCore;

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


            var permission = userId.HasValue ? _authorizationService.CanCreateTask(userId.Value, request.ProjectId) : false;

            if (!permission)
            {
                throw new UnauthorizedAccessException("User does not have permission to create a task!");
            }


            Models.Task task = new Models.Task(request.Name, request.Description, request.DueDate, userId.Value, request.StatusId, request.PriorityId, request.DifficultyLevel, request.CategoryId, request.ProjectId);
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

        public List<ProjectTasksInfoDTO> GetTasksByFilters(HttpContext httpContext, TaskFilterParamsDTO filterParams)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

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
                if (filterParams.StatusId.HasValue)
                {
                    query = query.Where(t => t.StatusId == filterParams.StatusId);
                }
                if (filterParams.PriorityId.HasValue)
                {
                    query = query.Where(t => t.PriorityId == filterParams.PriorityId);
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
                TaskId = t.Id,
                Name = t.Name,
                Description = t.Description,
                CategoryId = t.CategoryId,
                PriorityId = t.PriorityId,
                StatusId = t.StatusId,
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

        public void DeleteTask(HttpContext httpContext, int taskId)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole != null && !userRole.CanRemoveTask)
            {
                throw new UnauthorizedAccessException("User does not have permission to remove task!");
            }

            var task = _databaseContext.Tasks.Find(taskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} does not exist.");
            }

            // other tasks depend on this one
            var dependentTasks = _databaseContext.Set<TaskDependency>().Where(td => td.TaskId == taskId).ToList();

            if (dependentTasks.Any())
            {
                throw new InvalidOperationException($"Task with ID {taskId} cannot be deleted because it has dependent tasks!");
            }

            // this task depends on others, if so - delete that relation
            var tasksDependentOnThis = _databaseContext.Set<TaskDependency>().Where(td => td.DependentTaskId == taskId).ToList();

            foreach (var dependentTask in tasksDependentOnThis)
            {
                _databaseContext.Set<TaskDependency>().Remove(dependentTask);
            }

            _databaseContext.Tasks.Remove(task);
            _databaseContext.SaveChanges();
        }

        public async Task<UpdatedTaskInfoDTO> UpdateTask(HttpContext httpContext, TaskUpdateRequestDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole != null && userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit task!");
            }

            if (request.TaskId <= 0 || request.IsEmpty())
            {
                throw new ArgumentException("Not enough parameters for task update!");
            }
        }
    }
}
