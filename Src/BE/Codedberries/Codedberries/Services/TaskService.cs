using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Codedberries.Helpers;
using Microsoft.AspNetCore.Http;

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

        public async System.Threading.Tasks.Task CreateTask(HttpContext httpContext, TaskCreationRequestDTO request)
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

            if (userRole != null && userRole.CanCreateTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create Task!");
            }

            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException("Task name is required!");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Description must not be empty!");
            }

            if (Helper.IsDateRangeValid(request.StartDate, request.DueDate) == false)
            {
                throw new ArgumentException("Invalid date range!");
            }

            if (request.PriorityId <= 0)
            {
                throw new ArgumentException("Priority ID must be greater than zero!");
            }

            var requestedPriority = _databaseContext.Priorities.FirstOrDefault(p => p.Id == request.PriorityId);

            if (requestedPriority == null)
            {
                throw new ArgumentException("Priority with the provided ID does not exist!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("Project ID must be greater than zero!");
            }

            var requestedProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (requestedProject == null)
            {
                throw new ArgumentException("Project with the provided ID does not exist!");
            }

            if (request.StatusId <= 0)
            {
                throw new ArgumentException("Status ID must be greater than zero!");
            }

            var requestedStatus = _databaseContext.Statuses.FirstOrDefault(s => s.Id == request.StatusId && s.ProjectId == request.ProjectId);

            if (requestedStatus == null)
            {
                throw new ArgumentException("Status with the provided ID does not exist for the specified project!");
            }

            if (request.CategoryId <= 0)
            {
                throw new ArgumentException("Category ID must be greater than zero!");
            }

            var requestedCategory = _databaseContext.Categories.FirstOrDefault(c => c.Id == request.CategoryId && c.ProjectId == request.ProjectId);

            if (requestedCategory == null)
            {
                throw new ArgumentException("Category with the provided ID does not exist for the specified project!");
            }

            if (request.DifficultyLevel <= 0)
            {
                throw new ArgumentException("Invalid difficulty level!");
            }

            if (request.UserId <= 0)
            {
                throw new ArgumentException("User ID must be greater than zero!");
            }

            var requestedUser = _databaseContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (requestedUser == null)
            {
                throw new ArgumentException("User with the provided ID does not exist!");
            }

            if (requestedUser.RoleId == null)
            {
                throw new UnauthorizedAccessException("Requested user does not have any role assigned!");
            }

            var existingTask = _databaseContext.Tasks.FirstOrDefault(t => t.Name == request.Name && t.ProjectId == request.ProjectId);

            if (existingTask != null)
            {
                throw new ArgumentException($"Task with name '{request.Name}' already exists in the database for the specified project!");
            }

            Models.Task task = new Models.Task(request.Name, request.Description, request.DueDate, request.StartDate ,request.UserId, request.StatusId, request.PriorityId, request.DifficultyLevel, request.CategoryId, request.ProjectId);
            
            if (request.DependencyIds != null && request.DependencyIds.Any())
            {
                foreach (int dependency_id in request.DependencyIds)
                {
                    var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id && u.ProjectId == request.ProjectId);

                    if (taskDependency == null)
                    {
                        throw new ArgumentException($"Dependency task with ID {dependency_id} does not exist for the provided project in database!");
                    }
                }
            }

            _databaseContext.Tasks.Add(task);
            await _databaseContext.SaveChangesAsync();

            // adding dependencies to TaskDependency
            if (request.DependencyIds != null && request.DependencyIds.Any())
            {
                foreach (int dependency_id in request.DependencyIds)
                {
                    var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id && u.ProjectId == request.ProjectId);

                    TaskDependency newDependency = new TaskDependency
                    {
                        TaskId = taskDependency.Id,
                        DependentTaskId = task.Id
                    };

                    _databaseContext.Set<TaskDependency>().Add(newDependency);
                }
            }

            await _databaseContext.SaveChangesAsync();
        }

        public List<ProjectTasksInfoDTO> GetTasksByFilters(HttpContext httpContext, TaskFilterParamsDTO filterParams)
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

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
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
                    if (filterParams.ProjectId <= 0)
                    {
                        throw new ArgumentException("ProjectId must be greater than 0!");
                    }

                    var existingProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == filterParams.ProjectId);
                    
                    if (existingProject == null)
                    {
                        throw new ArgumentException($"Project with ID {filterParams.ProjectId} does not exist in the database!");
                    }

                    query = query.Where(t => t.ProjectId == filterParams.ProjectId);
                }

                if (filterParams.AssignedTo.HasValue)
                {
                    var assignedToUserId = filterParams.AssignedTo.Value;

                    if (assignedToUserId <= 0)
                    {
                        throw new ArgumentException("AssignedTo must be greater than 0!");
                    }

                    var existingUser = _databaseContext.Users.FirstOrDefault(u => u.Id == assignedToUserId);

                    if (existingUser == null)
                    {
                        throw new ArgumentException($"User with ID {assignedToUserId} does not exist in the database!");
                    }

                    query = query.Where(t => t.UserId == filterParams.AssignedTo);
                }

                if (filterParams.StatusId.HasValue)
                {
                    var statusId = filterParams.StatusId.Value;

                    if (statusId <= 0)
                    {
                        throw new ArgumentException("StatusId must be greater than 0!");
                    }

                    var existingStatus = _databaseContext.Statuses.FirstOrDefault(s => s.Id == statusId);

                    if (existingStatus == null)
                    {
                        throw new ArgumentException($"Status with ID {statusId} does not exist in the database!");
                    }

                    query = query.Where(t => t.StatusId == filterParams.StatusId);
                }

                if (filterParams.PriorityId.HasValue)
                {
                    var priorityId = filterParams.PriorityId.Value;

                    if (priorityId <= 0)
                    {
                        throw new ArgumentException("PriorityId must be greater than 0!");
                    }

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
                CategoryName = t.CategoryId != null ? _databaseContext.Categories.FirstOrDefault(c => c.Id == t.CategoryId).Name : null,
                PriorityName = t.PriorityId != null ? _databaseContext.Priorities.FirstOrDefault(p => p.Id == t.PriorityId).Name : null,
                StatusName = t.StatusId != null ? _databaseContext.Statuses.FirstOrDefault(s => s.Id == t.StatusId).Name : null,
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

            var task = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} not found!");
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                task.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                task.Description = request.Description;
            }

            if (request.CategoryId.HasValue && request.CategoryId > 0)
            {
                var category = await _databaseContext.Categories.FindAsync(request.CategoryId.Value);
                if (category == null)
                {
                    throw new ArgumentException($"Category with ID {request.CategoryId} not found!");
                }
                task.CategoryId = request.CategoryId.Value;
            }

            if (request.PriorityId.HasValue && request.PriorityId > 0)
            {
                var priority = await _databaseContext.Priorities.FindAsync(request.PriorityId.Value);
                if (priority == null)
                {
                    throw new ArgumentException($"Priority with ID {request.PriorityId} not found!");
                }
                task.PriorityId = request.PriorityId.Value;
            }

            if (request.StatusId.HasValue && request.StatusId > 0)
            {
                var status = await _databaseContext.Statuses.FindAsync(request.StatusId.Value);
                if (status == null)
                {
                    throw new ArgumentException($"Status with ID {request.StatusId} not found!");
                }
                task.StatusId = request.StatusId.Value;
            }

            if (request.DueDate.HasValue)
            {
                task.DueDate = request.DueDate.Value;
            }

            if (request.StartDate.HasValue)
            {
                task.StartDate = request.StartDate.Value;
            }

            if (request.UserId.HasValue && request.UserId > 0)
            {
                var userToAssign = await _databaseContext.Users.FindAsync(request.UserId.Value);
                
                if (userToAssign == null)
                {
                    throw new ArgumentException($"User with ID {request.UserId} not found!");
                }

                task.UserId = request.UserId.Value;
            }

            if (request.DifficultyLevel.HasValue && request.DifficultyLevel > 0)
            {
                task.DifficultyLevel = request.DifficultyLevel.Value;
            }

            if (request.ProjectId.HasValue && request.ProjectId > 0)
            {
                var project = await _databaseContext.Projects.FindAsync(request.ProjectId.Value);
                
                if (project == null)
                {
                    throw new ArgumentException($"Project with ID {request.ProjectId} not found!");
                }

                task.ProjectId = request.ProjectId.Value;
            }

            await _databaseContext.SaveChangesAsync();

            var updatedTaskInfo = new UpdatedTaskInfoDTO
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                CategoryId = task.CategoryId,
                PriorityId = task.PriorityId,
                StatusId = task.StatusId,
                DueDate = task.DueDate,
                StartDate = task.StartDate,
                DifficultyLevel = task.DifficultyLevel,
                ProjectId = task.ProjectId 
            };

            var tasksWithSameNameAndProjectId = await _databaseContext.Tasks
                .Where(t => t.Name == task.Name && t.ProjectId == task.ProjectId)
                .ToListAsync();

            var userIds = tasksWithSameNameAndProjectId
                .SelectMany(t => new[] { t.UserId })
                .Distinct()
                .ToList();

            var assignedUsers = await _databaseContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();

            var userDTOs = assignedUsers.Select(u => new UserDTO
            {
                Id = u.Id,
                FirstName = u.Firstname,
                LastName = u.Lastname,
                ProfilePicture = u.ProfilePicture
            }).ToList();

            updatedTaskInfo.AssignedUsers = userDTOs;

            return updatedTaskInfo;
        }

        public void ArchiveTask(HttpContext httpContext,int taskId)
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

            if (userRole != null && !userRole.CanEditTask)
            {
                throw new UnauthorizedAccessException("User does not have permission to remove task!");
            }

            var task = _databaseContext.Tasks.Find(taskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} not found!");
            }

            // Toggle archived status
            task.Archived = !task.Archived;

            _databaseContext.SaveChanges();
            }

        public async System.Threading.Tasks.Task CreateTaskComment(HttpContext httpContext, TaskCommentCreationRequestDTO request)
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

            if (string.IsNullOrEmpty(request.Comment))
            {
                throw new ArgumentException("Comment is required!");
            }

            if (request.TaskId <= 0)
            {
                throw new ArgumentException("Task ID must be greater than zero!");
            }

            var task = _databaseContext.Tasks.Find(request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} does not exist.");
            }

            User currentUser =_databaseContext.Users.FirstOrDefault(r => r.Id == userId);


            Models.TaskComment taskComment = new TaskComment(request.Comment, currentUser.Id ,request.TaskId);

            _databaseContext.TaskComments.Add(taskComment);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
