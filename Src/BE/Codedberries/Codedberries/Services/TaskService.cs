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

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("Project ID must be greater than zero!");
            }

            var requestedProject = _databaseContext.Projects.FirstOrDefault(p => p.Id == request.ProjectId);

            if (requestedProject == null)
            {
                throw new ArgumentException("Project with the provided ID does not exist in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == request.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {request.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanCreateTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create Task!");
            }
            // ---------------- //

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

            if (request.UserIds == null || !request.UserIds.Any())
            {
                throw new ArgumentException("At least one user ID must be provided!");
            }

            foreach (var providedUserId in request.UserIds)
            {
                if (providedUserId <= 0)
                {
                    throw new ArgumentException("Provided user ID must be greater than zero!");
                }

                var requestedUser = _databaseContext.Users.FirstOrDefault(u => u.Id == providedUserId);

                if (requestedUser == null)
                {
                    throw new ArgumentException($"User with the provided ID {providedUserId} does not exist in database!");
                }

                if (requestedUser.RoleId == null)
                {
                    throw new UnauthorizedAccessException("Requested user does not have any role assigned!");
                }

                // is provided user assigned on project where new task is being created?
                var userOnTask = _databaseContext.UserProjects
                    .FirstOrDefault(up => up.UserId == requestedUser.Id && up.ProjectId == request.ProjectId);

                if (userOnTask == null)
                {
                    throw new UnauthorizedAccessException($"No match for provided UserId {requestedUser.Id} and ProjectId {request.ProjectId} in UserProjects table!");
                }
            }

            foreach (var providedUserId in request.UserIds)
            {
                var existingTask = _databaseContext.Tasks.FirstOrDefault(t => t.Name == request.Name && t.ProjectId == request.ProjectId && t.UserId == providedUserId);

                if (existingTask != null)
                {
                    throw new ArgumentException($"Provided user {providedUserId} is already on Task with name '{request.Name}' in the database for the specified project {request.ProjectId}!");
                }
            }

            var tasks = new List<Models.Task>();

            foreach (var providedUserId in request.UserIds)
            {
                Models.Task task = new Models.Task(request.Name, request.Description, request.DueDate, request.StartDate, providedUserId, request.StatusId, request.PriorityId, request.DifficultyLevel, request.CategoryId, request.ProjectId);
                tasks.Add(task);
            }

            if (!tasks.Any())
            {
                throw new InvalidOperationException("No tasks were created. The task list is empty!");
            }

            if (request.DependencyIds != null && request.DependencyIds.Any())
            {
                foreach (int dependency_id in request.DependencyIds)
                {
                    var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id && u.ProjectId == request.ProjectId);

                    if (taskDependency == null)
                    {
                        throw new ArgumentException($"Dependency task with ID {dependency_id} does not exist for the provided project {request.ProjectId} in database!");
                    }
                }
            }

            // if there is circual dependency, task won't be created
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var task in tasks)
                {
                    _databaseContext.Tasks.Add(task);
                }

                await _databaseContext.SaveChangesAsync();

                // adding dependencies to TaskDependency
                if (request.DependencyIds != null && request.DependencyIds.Any())
                {
                    foreach (int dependency_id in request.DependencyIds)
                    {
                        var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id && u.ProjectId == request.ProjectId);

                        if (taskDependency == null)
                        {
                            throw new ArgumentException($"Dependency task with ID {dependency_id} does not exist for the provided project {request.ProjectId} in database!");
                        }

                        var cyclicDependencyDetected = DetectCyclicDependency(task.Id, dependency_id);

                        if (cyclicDependencyDetected)
                        {
                            throw new ArgumentException($"Creating dependency for {dependency_id} and new task would result in a circular dependency!");
                        }

                        TaskDependency newDependency = new TaskDependency
                        {
                            TaskId = taskDependency.Id,
                            DependentTaskId = task.Id
                        };

                        _databaseContext.Set<TaskDependency>().Add(newDependency);
                    }
                }

                await _databaseContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"{ex.Message}");
            }
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

                    var existingPriority = _databaseContext.Priorities.FirstOrDefault(p => p.Id == priorityId);

                    if (existingPriority == null)
                    {
                        throw new ArgumentException($"Priority with ID {priorityId} does not exist in the database!");
                    }

                    query = query.Where(t => t.PriorityId == filterParams.PriorityId);
                }

                if (filterParams.DifficultyLevelGreaterThan.HasValue)
                {
                    var difficultyLevelGreaterThan = filterParams.DifficultyLevelGreaterThan.Value;

                    if (difficultyLevelGreaterThan <= 0)
                    {
                        throw new ArgumentException("DifficultyLevelGreaterThan must be greater than 0!");
                    }

                    query = query.Where(t => t.DifficultyLevel > filterParams.DifficultyLevelGreaterThan.Value);
                }

                if (filterParams.DifficultyLevelLesserThan.HasValue)
                {
                    var difficultyLevelLesserThan = filterParams.DifficultyLevelLesserThan.Value;

                    if (difficultyLevelLesserThan <= 0)
                    {
                        throw new ArgumentException("DifficultyLevelLesserThan must be greater than 0!");
                    }

                    query = query.Where(t => t.DifficultyLevel < filterParams.DifficultyLevelLesserThan.Value);
                }

                if (filterParams.DifficultyLevelEquals.HasValue)
                {
                    var difficultyLevelEquals = filterParams.DifficultyLevelEquals.Value;

                    if (difficultyLevelEquals <= 0)
                    {
                        throw new ArgumentException("DifficultyLevelEquals must be greater than 0!");
                    }

                    query = query.Where(t => t.DifficultyLevel == filterParams.DifficultyLevelEquals.Value);
                }

                if (filterParams.DueDateAfter.HasValue)
                {
                    var dueDateAfter = filterParams.DueDateAfter.Value;

                    if (dueDateAfter <= DateTime.MinValue || dueDateAfter >= DateTime.MaxValue)
                    {
                        throw new ArgumentException("DueDateAfter must be a valid date!");
                    }

                    query = query.Where(t => t.DueDate >= filterParams.DueDateAfter);
                }

                if (filterParams.DueDateBefore.HasValue)
                {
                    var dueDateBefore = filterParams.DueDateBefore.Value;

                    if (dueDateBefore <= DateTime.MinValue || dueDateBefore >= DateTime.MaxValue)
                    {
                        throw new ArgumentException("DueDateBefore must be a valid date!");
                    }

                    query = query.Where(t => t.DueDate <= filterParams.DueDateBefore);
                }

                if (filterParams.CategoryId.HasValue)
                {
                    var categoryId = filterParams.CategoryId.Value;

                    if (categoryId <= 0)
                    {
                        throw new ArgumentException("CategoryId must be greater than 0!");
                    }

                    var existingCategory = _databaseContext.Categories.FirstOrDefault(c => c.Id == categoryId);

                    if (existingCategory == null)
                    {
                        throw new ArgumentException($"Category with ID {categoryId} does not exist in the database!");
                    }

                    query = query.Where(t => t.CategoryId == filterParams.CategoryId);
                }
            }

            List<Codedberries.Models.Task> tasks = query.ToList();

            List<ProjectTasksInfoDTO> tasksDTO = new List<ProjectTasksInfoDTO>();

            foreach (var task in tasks)
            {
                var dependentTaskIds = _databaseContext.Set<TaskDependency>()
                    .Where(td => td.TaskId == task.Id)
                    .Select(td => td.DependentTaskId)
                    .ToList();

                var taskDTO = new ProjectTasksInfoDTO
                {
                    TaskId = task.Id,
                    Name = task.Name,
                    Description = task.Description,
                    CategoryId = task.CategoryId,
                    PriorityId = task.PriorityId,
                    StatusId = task.StatusId,
                    CategoryName = task.CategoryId != null ? _databaseContext.Categories.FirstOrDefault(c => c.Id == task.CategoryId).Name : null,
                    PriorityName = task.PriorityId != null ? _databaseContext.Priorities.FirstOrDefault(p => p.Id == task.PriorityId).Name : null,
                    StatusName = task.StatusId != null ? _databaseContext.Statuses.FirstOrDefault(s => s.Id == task.StatusId).Name : null,
                    DueDate = task.DueDate,
                    AssignedTo = _databaseContext.Users
                        .Where(u => u.Id == task.UserId)
                        .Select(u => new TaskUserInfoDTO
                        {
                            Id = u.Id,
                            FirstName = u.Firstname,
                            LastName = u.Lastname,
                            ProfilePicture = u.ProfilePicture
                        })
                        .ToList(),
                    DependentTasks = dependentTaskIds,
                    DifficultyLevel = task.DifficultyLevel
                };

                tasksDTO.Add(taskDTO);
            }

            if (tasksDTO.Count == 0)
            {
                throw new ArgumentException("No tasks found matching the specified criteria!");
            }

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
                throw new ArgumentException($"Task with ID {taskId} does not exist!");
            }

            // other tasks depend on this one
            var dependentTasks = _databaseContext.Set<TaskDependency>().Where(td => td.DependentTaskId == taskId).ToList();

            if (dependentTasks.Any())
            {
                throw new InvalidOperationException($"Task with ID {taskId} cannot be deleted because it has dependent tasks!");
            }

            // this task depends on others, if so - delete that relation
            var tasksDependentOnThis = _databaseContext.Set<TaskDependency>().Where(td => td.TaskId == taskId).ToList();

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
                throw new UnauthorizedAccessException("User not found in database!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            if(request.IsEmpty())
            {
                throw new ArgumentException("Not enough parameters for task update!");
            }

            if (request.TaskId <= 0)
            {
                throw new ArgumentException("TaskId must be greater than 0!");
            }

            var task = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} not found in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == task.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {task.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Task!");
            }
            // ---------------- //

            if (!string.IsNullOrEmpty(request.Name))
            {
                task.Name = request.Name;
            }

            if (!string.IsNullOrEmpty(request.Description))
            {
                task.Description = request.Description;
            }

            if (request.ProjectId.HasValue)
            {
                if (request.ProjectId <= 0)
                {
                    throw new ArgumentException("ProjectId must be greater than 0!");
                }

                var project = await _databaseContext.Projects.FindAsync(request.ProjectId.Value);

                if (project == null)
                {
                    throw new ArgumentException($"Project with ID {request.ProjectId} not found in database!");
                }

                if (!request.CategoryId.HasValue || request.CategoryId <= 0)
                {
                    throw new ArgumentException("CategoryId must be provided and greater than 0 when ProjectId is specified and getting changed!");
                }

                if (!request.StatusId.HasValue || request.StatusId <= 0)
                {
                    throw new ArgumentException("StatusId must be provided and greater than 0 when ProjectId is specified and getting changed!");
                }

                task.ProjectId = request.ProjectId.Value;
            }

            if (request.CategoryId.HasValue)
            {
                if (request.CategoryId <= 0)
                {
                    throw new ArgumentException("CategoryId must be greater than 0!");
                }

                var category = await _databaseContext.Categories.FindAsync(request.CategoryId.Value);
                
                if (category == null)
                {
                    throw new ArgumentException($"Category with ID {request.CategoryId} not found in database!");
                }

                if (category.ProjectId != task.ProjectId)
                {
                    throw new ArgumentException($"Category with ID {request.CategoryId} does not belong to the same project as the task!");
                }

                task.CategoryId = request.CategoryId.Value;
            }

            if (request.PriorityId.HasValue)
            {
                if (request.PriorityId <= 0)
                {
                    throw new ArgumentException("PriorityId must be greater than 0!");
                }

                var priority = await _databaseContext.Priorities.FindAsync(request.PriorityId.Value);
                
                if (priority == null)
                {
                    throw new ArgumentException($"Priority with ID {request.PriorityId} not found in database!!");
                }
                
                task.PriorityId = request.PriorityId.Value;
            }

            if (request.StatusId.HasValue)
            {
                if (request.StatusId <= 0)
                {
                    throw new ArgumentException("StatusId must be greater than 0!");
                }

                var status = await _databaseContext.Statuses.FindAsync(request.StatusId.Value);
                
                if (status == null)
                {
                    throw new ArgumentException($"Status with ID {request.StatusId} not found in database!");
                }

                if (status.ProjectId != task.ProjectId)
                {
                    throw new ArgumentException($"Status with ID {request.StatusId} does not match the project of the task!");
                }

                var currentStatus = await _databaseContext.Statuses.FindAsync(task.StatusId);

                if (currentStatus == null)
                {
                    throw new ArgumentException($"Current status with ID {task.StatusId} not found in database!");
                }

                if (currentStatus.ProjectId != task.ProjectId)
                {
                    throw new ArgumentException($"Current status with ID {currentStatus.Id} and {currentStatus.ProjectId} does not match the project of the task {task.Project}!");
                }

                if (currentStatus.Name == "Done" && status.Name != "Done")
                {
                    task.FinishedDate = null; // from Done to other
                }
                else if (currentStatus.Name != "Done" && status.Name == "Done")
                {
                    task.FinishedDate = DateTime.UtcNow; // from other to Done
                }

                task.StatusId = request.StatusId.Value;
            }

            if(request.StartDate.HasValue && request.DueDate.HasValue)
            {
                if(Helper.IsDateRangeValid(request.StartDate.Value, request.DueDate.Value) == false)
                {
                    throw new ArgumentException("Invalid StartDate and DueDate range!");
                }
            }

            if (request.StartDate.HasValue)
            {
                if (request.StartDate <= DateTime.MinValue || request.StartDate >= DateTime.MaxValue)
                {
                    throw new ArgumentException("StartDate must be a valid date!");
                }

                if(request.StartDate > task.DueDate)
                {
                    if(!request.DueDate.HasValue)
                    {
                        throw new ArgumentException("StartDate cannot be after DueDate!");
                    }
                }

                task.StartDate = request.StartDate.Value;
            }

            if (request.DueDate.HasValue)
            {
                if (request.DueDate <= DateTime.MinValue || request.DueDate >= DateTime.MaxValue)
                {
                    throw new ArgumentException("DueDate must be a valid date!");
                }

                if (request.DueDate < task.StartDate)
                {
                    if (!request.StartDate.HasValue)
                    {
                        throw new ArgumentException("DueDate cannot be before StartDate!");
                    }
                }

                task.DueDate = request.DueDate.Value;
            }

            if (request.UserId.HasValue)
            {
                if (request.UserId <= 0)
                {
                    throw new ArgumentException("UserId must be greater than 0!");
                }

                var userToAssign = await _databaseContext.Users.FindAsync(request.UserId.Value);
                
                if (userToAssign == null)
                {
                    throw new ArgumentException($"User with ID {request.UserId} not found in database!");
                }

                if (userToAssign.RoleId == null)
                {
                    throw new InvalidOperationException($"User with ID {request.UserId} does not have a role assigned!");
                }

                task.UserId = request.UserId.Value;
            }

            if (request.DifficultyLevel.HasValue)
            {
                if (request.DifficultyLevel <= 0)
                {
                    throw new ArgumentException("DifficultyLevel must be greater than 0!");
                }

                task.DifficultyLevel = request.DifficultyLevel.Value;
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
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                FinishedDate = task.FinishedDate,
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
                throw new UnauthorizedAccessException("User not found in database!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            if (request.TaskId <= 0)
            {
                throw new ArgumentException("Task ID must be greater than zero!");
            }

            var task = await _databaseContext.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} not found in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == task.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {task.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }
            // ---------------- //

            if (string.IsNullOrEmpty(request.Comment))
            {
                throw new ArgumentException("Comment is required!");
            }

            DateTime currentDate = DateTime.Now;
            Models.TaskComment taskComment = new TaskComment(request.Comment, user.Id ,request.TaskId, currentDate);

            _databaseContext.TaskComments.Add(taskComment);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<TaskCommentInfoDTO>> GetTasksComments(HttpContext httpContext, TaskIdDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            if (request.TaskId <= 0)
            {
                throw new ArgumentException("Task ID must be greater than zero!");
            }

            var task = await _databaseContext.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} not found in database!");
            }

            var comments = await _databaseContext.TaskComments
                .Where(tc => tc.TaskId == request.TaskId)
                .ToListAsync();

            if (!comments.Any())
            {
                throw new ArgumentException($"There are no comments for task with ID {request.TaskId}!");
            }

            List<TaskCommentInfoDTO> commentsDTO = new List<TaskCommentInfoDTO>();

            foreach (var comment in comments)
            {
                var commentUser = await _databaseContext.Users.FindAsync(comment.UserId);

                if (commentUser == null)
                {
                    throw new ArgumentException($"User with ID {comment.UserId} not found!");
                }

                var commentDTO = new TaskCommentInfoDTO
                {
                    Comment = comment.Comment,
                    CommentId = comment.CommentId,
                    TaskId = comment.TaskId,
                    UserId = comment.UserId,
                    FirstName = commentUser.Firstname,
                    LastName = commentUser.Lastname,
                    CommentDate = comment.CommentDate
                };

                commentsDTO.Add(commentDTO);
            }

            return commentsDTO;
        }

        // function is used to detect cyclic dependencies between tasks
        private bool DetectCyclicDependency(int taskId, int dependencyId)
        {
            var visited = new HashSet<int>();
            visited.Add(taskId);

            var queue = new Queue<int>();
            queue.Enqueue(dependencyId);

            while (queue.Count > 0)
            {
                var currentTaskId = queue.Dequeue();

                var dependentTasks = _databaseContext.Set<TaskDependency>()
                    .Where(td => td.TaskId == currentTaskId)
                    .Select(td => td.DependentTaskId)
                    .ToList();

                foreach (var dependentTaskId in dependentTasks)
                {
                    if (dependentTaskId == taskId)
                    {
                        return true; // circular dependency detected
                    }

                    if (!visited.Contains(dependentTaskId))
                    {
                        visited.Add(dependentTaskId);
                        queue.Enqueue(dependentTaskId);
                    }
                }
            }

            return false; // no circular dependency detected
        }
    }
}
