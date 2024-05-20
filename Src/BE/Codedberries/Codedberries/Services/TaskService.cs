using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Codedberries.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

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

            var existingTask = _databaseContext.Tasks.FirstOrDefault(t => t.Name == request.Name && t.ProjectId == request.ProjectId);

            if (existingTask != null)
            {
                throw new ArgumentException($"Task with name '{request.Name}' already exists in the project with ID {request.ProjectId}!");
            }

            var newTask = new Models.Task(request.Name, request.Description, request.DueDate, request.StartDate, request.StatusId, request.PriorityId, request.DifficultyLevel, request.CategoryId, request.ProjectId);

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

            if (request.DependencyIds != null && request.DependencyIds.Count != request.TypeOfDependencyIds.Count)
            {
                throw new ArgumentException("The number of DependencyIds must match the number of TypeOfDependencyIds!");
            }

            if (request.DependencyIds != null && request.TypeOfDependencyIds != null)
            {
                foreach (var typeOfDependencyId in request.TypeOfDependencyIds)
                {
                    var typeOfDependencyExists = await _databaseContext.TypesOfTaskDependency
                        .AnyAsync(t => t.Id == typeOfDependencyId);

                    if (!typeOfDependencyExists)
                    {
                        throw new ArgumentException($"TypeOfTaskDependency with ID {typeOfDependencyId} does not exist in the database!");
                    }
                }
            }

            // if there is circual dependency, task won't be created
            using var transaction = await _databaseContext.Database.BeginTransactionAsync();

            try
            {
                _databaseContext.Tasks.Add(newTask);
                await _databaseContext.SaveChangesAsync();

                // add users to TaskUser
                foreach (var providedUserId in request.UserIds)
                {
                    var taskUser = new TaskUser
                    {
                        TaskId = newTask.Id,
                        UserId = providedUserId
                    };

                    _databaseContext.TaskUsers.Add(taskUser);
                }

                // adding dependencies to TaskDependency
                if (request.DependencyIds != null && request.DependencyIds.Any())
                {
                    var i = 0;

                    foreach (int dependency_id in request.DependencyIds)
                    {
                        var taskDependency = _databaseContext.Tasks.FirstOrDefault(u => u.Id == dependency_id && u.ProjectId == request.ProjectId);
                        var typeOfDependencyId = request.TypeOfDependencyIds[i];

                        if (taskDependency == null)
                        {
                            throw new ArgumentException($"Dependency task with ID {dependency_id} does not exist for the provided project {request.ProjectId} in database!");
                        }

                        var cyclicDependencyDetected = DetectCyclicDependency(newTask.Id, dependency_id);

                        if (cyclicDependencyDetected)
                        {
                            throw new ArgumentException($"Creating dependency for {dependency_id} and new task would result in a circular dependency!");
                        }

                        // check if the dependency condition is met
                        if (!CheckDependencyCondition(taskDependency, newTask, typeOfDependencyId))
                        {
                            switch (typeOfDependencyId)
                            {
                                case 1: // Start to Start Dependency
                                    throw new ArgumentException($"Task with ID {newTask.Id} cannot start before the dependent task {taskDependency.Id}!");

                                case 2: // Start to End Dependency
                                    throw new ArgumentException($"Task with ID {newTask.Id} cannot start before the dependent task {taskDependency.Id} ends!");

                                case 3: // End to Start Dependency
                                    throw new ArgumentException($"Task with ID {newTask.Id} cannot end after the dependent task {taskDependency.Id} starts!");

                                case 4: // End to End Dependency
                                    throw new ArgumentException($"Task with ID {newTask.Id} cannot end before the dependent task {taskDependency.Id} ends!");

                                default:
                                    throw new ArgumentException("Invalid type of dependency");
                            }
                        }

                        TaskDependency newDependency = new TaskDependency
                        {
                            TaskId = taskDependency.Id,
                            DependentTaskId = newTask.Id,
                            TypeOfDependencyId = typeOfDependencyId
                        };

                        i++;

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

                    var taskIds = _databaseContext.TaskUsers
                         .Where(tu => tu.UserId == assignedToUserId)
                         .Select(tu => tu.TaskId)
                         .ToList();

                    query = query.Where(t => taskIds.Contains(t.Id));
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

                if (!string.IsNullOrEmpty(filterParams.TaskName))
                {
                    query = query.Where(t => t.Name.Contains(filterParams.TaskName));
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
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    FinishedDate = task.FinishedDate != null ? task.FinishedDate : null,
                    AssignedTo = _databaseContext.TaskUsers
                        .Where(tu => tu.TaskId == task.Id)
                        .Select(tu => new TaskUserInfoDTO
                        {
                            Id = tu.UserId,
                            FirstName = tu.User.Firstname,
                            LastName = tu.User.Lastname,
                            ProfilePicture = tu.User.ProfilePicture
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
                throw new ArgumentException($"Task with ID {taskId} does not exist in database!");
            }

            // other tasks depend on this one
            var dependentTasks = _databaseContext.Set<TaskDependency>()
                .Where(td => td.DependentTaskId == taskId)
                .ToList();

            if (dependentTasks.Any())
            {
                throw new InvalidOperationException($"Task with ID {taskId} cannot be deleted because it has dependent tasks!");
            }

            // this task depends on others, if so - delete that relation
            var tasksDependentOnThis = _databaseContext.Set<TaskDependency>()
                .Where(td => td.TaskId == taskId)
                .ToList();

            // verify and handle each type of dependency
            foreach (var dependency in tasksDependentOnThis)
            {
                var dependentTask = _databaseContext.Tasks.Find(dependency.DependentTaskId);

                if (dependentTask == null)
                {
                    throw new InvalidOperationException($"Dependent task with ID {dependentTask.Id} not found in database! Cannot delete task {taskId}!");
                }

                switch (dependency.TypeOfDependencyId)
                {
                    case 1: // Start to Start Dependency
                        if (task.StartDate < dependentTask.StartDate)
                        {
                            throw new InvalidOperationException($"Cannot delete task ID {taskId} due to Start to Start dependency with task ID {dependentTask.Id}!");
                        }
                        break;

                    case 2: // Start to End Dependency
                        if (task.StartDate < dependentTask.DueDate)
                        {
                            throw new InvalidOperationException($"Cannot delete task ID {taskId} due to Start to End dependency with task ID {dependentTask.Id}!");
                        }
                        break;

                    case 3: // End to Start Dependency
                        if (task.DueDate > dependentTask.StartDate)
                        {
                            throw new InvalidOperationException($"Cannot delete task ID {taskId} due to End to Start dependency with task ID {dependentTask.Id}!");
                        }
                        break;

                    case 4: // End to End Dependency
                        if (task.DueDate > dependentTask.DueDate)
                        {
                            throw new InvalidOperationException($"Cannot delete task ID {taskId} due to End to End dependency with task ID {dependentTask.Id}!");
                        }
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid TypeOfDependencyId: {dependency.TypeOfDependencyId}!");
                }
            }

            if (tasksDependentOnThis.Any())
            {
                _databaseContext.Set<TaskDependency>().RemoveRange(tasksDependentOnThis);
            }

            // remove users assigned to this task
            var taskUsers = _databaseContext.TaskUsers.Where(tu => tu.TaskId == taskId).ToList();
            _databaseContext.TaskUsers.RemoveRange(taskUsers);

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

            if (request.StartDate.HasValue && request.DueDate.HasValue)
            {
                if (Helper.IsDateRangeValid(request.StartDate.Value, request.DueDate.Value) == false)
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

                if (request.StartDate > task.DueDate)
                {
                    if (!request.DueDate.HasValue)
                    {
                        throw new ArgumentException("StartDate cannot be after DueDate!");
                    }
                }

                bool forceDateChange = request.ForceDateChange ?? false;

                var dependencies = await _databaseContext.Set<TaskDependency>()
                    .Where(td => td.TaskId == task.Id || td.DependentTaskId == task.Id)
                    .ToListAsync();

                if (!dependencies.Any())
                {
                    // no dependencies, update the task start date directly
                    task.StartDate = request.StartDate.Value;
                }
                else
                {
                    await UpdateTaskDate(task.Id, request.StartDate.Value, forceDateChange);
                }
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

                bool forceDateChange = request.ForceDateChange ?? false;

                var dependencies = await _databaseContext.Set<TaskDependency>()
                    .Where(td => td.TaskId == task.Id || td.DependentTaskId == task.Id)
                    .ToListAsync();

                if (!dependencies.Any())
                {
                    // no dependencies, update the task due date directly
                    task.DueDate = request.DueDate.Value;
                }
                else
                {
                    await UpdateTaskDate(task.Id, request.DueDate.Value, forceDateChange);
                }
            }

            if (request.DifficultyLevel.HasValue)
            {
                if (request.DifficultyLevel <= 0)
                {
                    throw new ArgumentException("DifficultyLevel must be greater than 0!");
                }

                task.DifficultyLevel = request.DifficultyLevel.Value;
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

                var existingTaskUser = await _databaseContext.TaskUsers
                    .FirstOrDefaultAsync(tu => tu.TaskId == task.Id && tu.UserId == request.UserId.Value);

                if (existingTaskUser == null)
                {
                    // if the user is not already assigned to the task, add them
                    var newTaskUser = new TaskUser
                    {
                        TaskId = task.Id,
                        UserId = request.UserId.Value
                    };

                    _databaseContext.TaskUsers.Add(newTaskUser);
                }
                else
                {
                    throw new ArgumentException($"User with ID {request.UserId.Value} is already assigned to task with ID {task.Id}!");
                }
            }

            await _databaseContext.SaveChangesAsync();

            var assignedUsers = _databaseContext.TaskUsers
                .Where(tu => tu.TaskId == task.Id)
                .Select(tu => new UserDTO
                {
                    Id = tu.User.Id,
                    FirstName = tu.User.Firstname,
                    LastName = tu.User.Lastname,
                    ProfilePicture = tu.User.ProfilePicture
                })
                .ToList();

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
                ProjectId = task.ProjectId,
                AssignedUsers = assignedUsers
            };

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

        public async System.Threading.Tasks.Task CreateTaskDependency(HttpContext httpContext, TaskDependencyRequestDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = await _databaseContext.Users.FindAsync(userId);

            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found in database!");
            }

            if (user.RoleId == null)
            {
                throw new UnauthorizedAccessException("User does not have any role assigned!");
            }

            if (request.TypeOfDependencyId <= 0)
            {
                throw new ArgumentException("Type Of Dependency ID must be greater than zero!");
            }

            var typeOfDependencyExists = await _databaseContext.TypesOfTaskDependency
                .AnyAsync(t => t.Id == request.TypeOfDependencyId);

            if (!typeOfDependencyExists)
            {
                throw new ArgumentException($"TypeOfTaskDependency with ID {request.TypeOfDependencyId} does not exist in the database!");
            }

            if (request.TaskId <= 0)
            {
                throw new ArgumentException("Task ID must be greater than zero!");
            }

            var task = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} does not exist in the database!");
            }

            if (request.DependentTaskId <= 0)
            {
                throw new ArgumentException("Dependent Task ID must be greater than zero!");
            }

            var dependentTask = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.DependentTaskId);

            if (dependentTask == null)
            {
                throw new ArgumentException($"Task with ID {request.DependentTaskId} does not exist in the database!");
            }

            if (request.TaskId == request.DependentTaskId)
            {
                throw new ArgumentException("Both provided tasks IDs are the same!");
            }

            if (task.ProjectId != dependentTask.ProjectId)
            {
                throw new ArgumentException("Both tasks must belong to the same project!");
            }

            var projectId = task.ProjectId;

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {projectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Task dependency!");
            }
            // ---------------- //

            // does dependency already exist
            var existingDependency = await _databaseContext.Set<TaskDependency>()
                .AnyAsync(td => td.TaskId == request.TaskId && td.DependentTaskId == request.DependentTaskId);

            if (existingDependency)
            {
                throw new InvalidOperationException("A dependency between these two tasks already exists!");
            }

            var cyclicDependencyDetected = DetectCyclicDependency(request.TaskId, request.DependentTaskId);

            if (cyclicDependencyDetected)
            {
                throw new ArgumentException($"Creating dependency for {request.TaskId} and {request.DependentTaskId} would result in a circular dependency!");
            }

            if (!CheckDependencyCondition(task, dependentTask, request.TypeOfDependencyId))
            {
                switch (request.TypeOfDependencyId)
                {
                    case 1: // Start to Start Dependency
                        throw new ArgumentException($"Task with ID {task.Id} cannot start before the dependent task {dependentTask.Id}!");

                    case 2: // Start to End Dependency
                        throw new ArgumentException($"Task with ID {task.Id} cannot start before the dependent task {dependentTask.Id} ends!");

                    case 3: // End to Start Dependency
                        throw new ArgumentException($"Task with ID {task.Id} cannot end after the dependent task {dependentTask.Id} starts!");

                    case 4: // End to End Dependency
                        throw new ArgumentException($"Task with ID {task.Id} cannot end before the dependent task {dependentTask.Id} ends!");

                    default:
                        throw new ArgumentException("Invalid type of dependency");
                }
            }

            var taskDependency = new TaskDependency
            {
                TaskId = request.TaskId,
                DependentTaskId = request.DependentTaskId,
                TypeOfDependencyId = request.TypeOfDependencyId
            };

            _databaseContext.Set<TaskDependency>().Add(taskDependency);
            await _databaseContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteTaskDependencies(HttpContext httpContext, TaskDependencyDeletionDTO request)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            var user = await _databaseContext.Users.FindAsync(userId);

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

            var task = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} does not exist in the database!");
            }

            if (request.DependentTaskId <= 0)
            {
                throw new ArgumentException("Dependent Task ID must be greater than zero!");
            }

            var dependentTask = await _databaseContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.DependentTaskId);

            if (dependentTask == null)
            {
                throw new ArgumentException($"Task with ID {request.DependentTaskId} does not exist in the database!");
            }

            if (request.TaskId == request.DependentTaskId)
            {
                throw new ArgumentException("Both provided tasks IDs are the same!");
            }

            if (task.ProjectId != dependentTask.ProjectId)
            {
                throw new ArgumentException("Both tasks must belong to the same project!");
            }

            var projectId = task.ProjectId;

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == projectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {projectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRole.CanEditTask == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to delete Task dependency!");
            }
            // ---------------- //

            var dependencyToDelete = await _databaseContext.Set<TaskDependency>()
                .FirstOrDefaultAsync(td => td.TaskId == request.TaskId && td.DependentTaskId == request.DependentTaskId);

            if (dependencyToDelete == null)
            {
                throw new InvalidOperationException("A dependency between these two tasks doesn't exists in database!");
            }

            _databaseContext.Set<TaskDependency>().RemoveRange(dependencyToDelete);
            await _databaseContext.SaveChangesAsync();
        }

        public bool CheckDependencyCondition(Codedberries.Models.Task taskOne, Codedberries.Models.Task taskTwo, int typeOfDependency)
        {
            switch (typeOfDependency)
            {
                case 1: // Start to Start Dependency
                    return taskOne.StartDate >= taskTwo.StartDate;

                case 2: // Start to End Dependency
                    return taskOne.StartDate >= taskTwo.DueDate;

                case 3: // End to Start Dependency
                    return taskTwo.StartDate >= taskOne.DueDate;

                case 4: // End to End Dependency
                    return taskOne.DueDate >= taskTwo.DueDate;

                default:
                    throw new ArgumentException("Invalid type of dependency! Doesn't exist in database!");
            }
        }

        public async System.Threading.Tasks.Task UpdateTaskDate(int taskId, DateTime newDate, bool forceDateChange)
        {
            var task = await _databaseContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new ArgumentException($"Task with ID {taskId} does not exist in the database!");
            }

            var dependencies = await _databaseContext.Set<TaskDependency>()
                .Where(td => td.TaskId == taskId || td.DependentTaskId == taskId)
                .ToListAsync();

            var taskIds = dependencies.Select(td => td.TaskId)
                .Union(dependencies.Select(td => td.DependentTaskId))
                .Distinct()
                .ToList();

            var tasks = await _databaseContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToListAsync();

            var taskDictionary = tasks.ToDictionary(t => t.Id);

            foreach (var dependency in dependencies)
            {
                var taskOne = taskDictionary[dependency.TaskId];
                var taskTwo = taskDictionary[dependency.DependentTaskId];

                bool conditionMet;
                string dependencyTypeMessage;

                var validTypeOfDependencyIds = new HashSet<int> { 1, 2, 3, 4 };
                int typeOfDependency = dependency.TypeOfDependencyId;

                if (!validTypeOfDependencyIds.Contains(typeOfDependency))
                {
                    throw new InvalidOperationException($"Dependency {dependency.TaskId} - {dependency.DependentTaskId} does not have a valid TypeOfDependency!");
                }

                switch (typeOfDependency)
                {
                    case 1: // Start to Start Dependency
                        conditionMet = taskOne.StartDate >= taskTwo.StartDate;
                        dependencyTypeMessage = "Start to Start";
                        break;

                    case 2: // Start to End Dependency
                        conditionMet = taskOne.StartDate >= taskTwo.DueDate;
                        dependencyTypeMessage = "Start to End";
                        break;

                    case 3: // End to Start Dependency
                        conditionMet = taskTwo.StartDate >= taskOne.DueDate;
                        dependencyTypeMessage = "End to Start";
                        break;

                    case 4: // End to End Dependency
                        conditionMet = taskOne.DueDate >= taskTwo.DueDate;
                        dependencyTypeMessage = "End to End";
                        break;

                    default:
                        throw new ArgumentException("Invalid type of dependency! Doesn't exist in database!");
                }

                if (!conditionMet)
                {
                    if (forceDateChange)
                    {
                        // adjust dates according to the dependency type
                        switch (typeOfDependency)
                        {
                            case 1: // Start to Start Dependency
                                taskOne.StartDate = newDate;
                                taskTwo.StartDate = newDate;
                                break;

                            case 2: // Start to End Dependency
                                taskOne.StartDate = newDate;
                                taskTwo.DueDate = newDate;
                                break;

                            case 3: // End to Start Dependency
                                taskOne.DueDate = newDate;
                                taskTwo.StartDate = newDate;
                                break;

                            case 4: // End to End Dependency
                                taskOne.DueDate = newDate;
                                taskTwo.DueDate = newDate;
                                break;
                        }
                    }
                    else
                    {
                        // notify about the affected tasks
                        var affectedTasks = dependencies.Select(d => d.DependentTaskId == taskId ? taskDictionary[d.TaskId] : taskDictionary[d.DependentTaskId]).ToList();
                        string affectedTasksMessage = string.Join(", ", affectedTasks.Select(t => $"Task ID: {t.Id}"));
                        throw new ArgumentException($"Changing the date violates the {dependencyTypeMessage} dependency condition. Affected tasks: {affectedTasksMessage}!");
                    }
                }
                else
                {
                    // if the condition is still met, update the task date
                    if (dependency.TaskId == taskId)
                    {
                        task.StartDate = newDate;
                    }
                    else
                    {
                        task.DueDate = newDate;
                    }
                }
            }

            await _databaseContext.SaveChangesAsync();
        }

    }
}
