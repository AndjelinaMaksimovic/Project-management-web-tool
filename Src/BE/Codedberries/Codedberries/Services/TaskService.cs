using Codedberries.Models.DTOs;
using Codedberries.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Codedberries.Helpers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System;
using Microsoft.Extensions.DependencyModel;
using Microsoft.AspNetCore.SignalR;
using System.Collections;

namespace Codedberries.Services
{
    public class TaskService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHubContext;

        public TaskService(AppDatabaseContext databaseContext, AuthorizationService authorizationService, IHubContext<NotificationHub, INotificationClient> notificationHubContext)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
            _notificationHubContext = notificationHubContext;
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

            if (userRole.CanAddTaskToUser == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to add other users on Task!");
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

                        await CreateTaskDependency(httpContext, new TaskDependencyRequestDTO
                        {
                            TaskId = newTask.Id,
                            DependentTaskId = dependency_id,
                            TypeOfDependencyId = typeOfDependencyId
                        });

                        i++;
                    }
                }
                string taskUrl = $"http://localhost:4200/project/{request.ProjectId}/task/{newTask.Id}";
                Activity activity = new Activity(user.Id, newTask.ProjectId, $"User {user.Firstname} {user.Lastname} has created the task <a href=\"{taskUrl}\">{newTask.Name}</a>", DateTime.Now);
                _databaseContext.Activities.Add(activity);
                await _databaseContext.SaveChangesAsync();

                var projectUsers = _databaseContext.UserProjects
                .Where(up => up.ProjectId == request.ProjectId && up.UserId != userId)
                .Select(up => up.UserId)
                .ToList();

                // Create UserNotification for each user on the project
                foreach (var projectUser in projectUsers)
                {
                    UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                    _databaseContext.UserNotifications.Add(userNotification);
                    NotificationDTO notificationDTO = new NotificationDTO { ProjectId = newTask.ProjectId, UserId = (int)userId, ActivityDescription = activity.ActivityDescription, Seen = userNotification.Seen, Time = activity.Time };
                    var connectionIds = NotificationHub.UserConnections.GetValueOrDefault(projectUser.ToString());
                    if (connectionIds != null && connectionIds.Any())
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            await _notificationHubContext.Clients.Client(connectionId).ReceiveNotification(notificationDTO);
                        }
                    }
                }

                await _databaseContext.SaveChangesAsync();

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

                if (filterParams.Archived.HasValue)
                {
                    query = query.Where(t => t.Archived == filterParams.Archived);
                }
                else
                {
                    if (!filterParams.IncludeArchived)
                    {
                        query = query.Where(t => t.Archived == false);
                    }
                }
            }

            List<Codedberries.Models.Task> tasks = query.ToList();

            List<ProjectTasksInfoDTO> tasksDTO = new List<ProjectTasksInfoDTO>();

            foreach (var task in tasks)
            {
                var dependentTasks = _databaseContext.Set<TaskDependency>()
                    .Where(td => td.TaskId == task.Id)
                    .Select(td => new DependentTaskDTO
                    {
                        TaskId = td.DependentTaskId,
                        TypeOfDependencyId = td.TypeOfDependencyId
                    })
                    .ToList();

                var taskDTO = new ProjectTasksInfoDTO
                {
                    TaskId = task.Id,
                    ProjectId = task.ProjectId,
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
                    Archived=task.Archived,
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
                    DependentTasks = dependentTasks,
                    DifficultyLevel = task.DifficultyLevel,
                    Progress = task.Progress
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

            Activity activity = new Activity(user.Id, task.ProjectId, $"User {user.Firstname} {user.Lastname} has deleted the task {task.Name}", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == task.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
                NotificationDTO notificationDTO = new NotificationDTO { ProjectId = task.ProjectId, UserId = (int)userId, ActivityDescription = activity.ActivityDescription, Seen = userNotification.Seen, Time = activity.Time };
                var connectionIds = NotificationHub.UserConnections.GetValueOrDefault(projectUser.ToString());
                if (connectionIds != null && connectionIds.Any())
                {
                    foreach (var connectionId in connectionIds)
                    {
                         _notificationHubContext.Clients.Client(connectionId).ReceiveNotification(notificationDTO);
                    }
                }
            }

            _databaseContext.SaveChangesAsync();

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

            DateTime previousStartDate = task.StartDate;
            DateTime previousDueDate = task.DueDate;

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
                    task.Progress = 0; // set progress to 0 when changing from Done
                }
                else if (currentStatus.Name != "Done" && status.Name == "Done")
                {
                    task.FinishedDate = DateTime.UtcNow; // from other to Done
                    task.Progress = 100; // set progress to 100 when changing to Done
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

            if (request.StartDate.HasValue && request.DueDate.HasValue)
            {
                task.StartDate = request.StartDate.Value;
                task.DueDate = request.DueDate.Value;
            }

            if (request.StartDate.HasValue && !request.DueDate.HasValue)
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

                task.StartDate = request.StartDate.Value;
            }

            if (request.DueDate.HasValue && !request.StartDate.HasValue)
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

            if (request.DifficultyLevel.HasValue)
            {
                if (request.DifficultyLevel <= 0)
                {
                    throw new ArgumentException("DifficultyLevel must be greater than 0!");
                }

                task.DifficultyLevel = request.DifficultyLevel.Value;
            }

            if (request.UserIds != null)
            {
                if (userRole.CanAddTaskToUser == false)
                {
                    throw new UnauthorizedAccessException("User does not have permission to add other users on Task!");
                }

                var currentTaskUsers = _databaseContext.TaskUsers.Where(tu => tu.TaskId == task.Id).ToList();
                _databaseContext.TaskUsers.RemoveRange(currentTaskUsers);

                if (request.UserIds.Any())
                {
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

                        var userOnTask = _databaseContext.UserProjects
                            .FirstOrDefault(up => up.UserId == requestedUser.Id && up.ProjectId == task.ProjectId);

                        if (userOnTask == null)
                        {
                            throw new UnauthorizedAccessException($"No match for provided UserId {requestedUser.Id} and ProjectId {task.ProjectId} in UserProjects table!");
                        }

                        var newTaskUser = new TaskUser
                        {
                            TaskId = task.Id,
                            UserId = providedUserId
                        };

                        _databaseContext.TaskUsers.Add(newTaskUser);
                    }
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
                AssignedUsers = assignedUsers,
                Progress = task.Progress
            };
            string taskUrl = $"http://localhost:4200/project/{task.ProjectId}/task/{task.Id}";
            Activity activity = new Activity(user.Id, task.ProjectId, $"User {user.Firstname} {user.Lastname} has updated the task <a href=\"{taskUrl}\">{task.Name}</a>", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == task.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
                NotificationDTO notificationDTO = new NotificationDTO { ProjectId = task.ProjectId, UserId = (int)userId, ActivityDescription = activity.ActivityDescription, Seen = userNotification.Seen, Time = activity.Time };
                var connectionIds = NotificationHub.UserConnections.GetValueOrDefault(projectUser.ToString());
                if (connectionIds != null && connectionIds.Any())
                {
                    foreach (var connectionId in connectionIds)
                    {
                        await _notificationHubContext.Clients.Client(connectionId).ReceiveNotification(notificationDTO);
                    }
                }
            }

            await _databaseContext.SaveChangesAsync();

            if (request.StartDate.HasValue || request.DueDate.HasValue)
            {
                try
                {
                    bool result = await CheckIsDateValid(task, false);
                    var visitedTasks = new HashSet<int>();
                    await UpdateAllDependentTasks(task, visitedTasks);
                }
                catch(ArgumentException e)
                {
                    if (request.StartDate.HasValue) task.StartDate = previousStartDate;
                    if (request.DueDate.HasValue) task.DueDate = previousDueDate;

                    await _databaseContext.SaveChangesAsync();

                    throw e;
                }
            }

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
            string taskUrl = $"http://localhost:4200/project/{task.ProjectId}/task/{task.Id}";
            Activity activity = new Activity(user.Id, task.ProjectId, $"User {user.Firstname} {user.Lastname} has archived the task <a href=\"{taskUrl}\">{task.Name}</a>", DateTime.Now);
            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == task.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
                NotificationDTO notificationDTO = new NotificationDTO { ProjectId = task.ProjectId, UserId = (int)userId, ActivityDescription = activity.ActivityDescription, Seen = userNotification.Seen, Time = activity.Time };
                var connectionIds = NotificationHub.UserConnections.GetValueOrDefault(projectUser.ToString());
                if (connectionIds != null && connectionIds.Any())
                {
                    foreach (var connectionId in connectionIds)
                    {
                        _notificationHubContext.Clients.Client(connectionId).ReceiveNotification(notificationDTO);
                    }
                }
            }

            _databaseContext.SaveChangesAsync();

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

            var taskDependency = new TaskDependency
            {
                TaskId = request.TaskId,
                DependentTaskId = request.DependentTaskId,
                TypeOfDependencyId = request.TypeOfDependencyId
            };

            _databaseContext.Set<TaskDependency>().Add(taskDependency);
            await _databaseContext.SaveChangesAsync();

            try
            {
                await CheckIsDateValid(task, false);
                var visitedTasks = new HashSet<int>();
                //await UpdateDependentTasks(request.TaskId, request.DependentTaskId, request.TypeOfDependencyId, visitedTasks);`
                await UpdateAllDependentTasks(task, visitedTasks);
            }
            catch (Exception e) when (e is ArgumentException || e is InvalidOperationException)
            {
                var dependencyToDelete = await _databaseContext.Set<TaskDependency>()
                .FirstOrDefaultAsync(td => td.TaskId == request.TaskId && td.DependentTaskId == request.DependentTaskId);

                _databaseContext.Set<TaskDependency>().RemoveRange(dependencyToDelete);
                await _databaseContext.SaveChangesAsync();

                throw e;
            }
        }

        private async System.Threading.Tasks.Task UpdateDependentTasks(int taskId, int dependentTaskId, int typeOfDependencyId, HashSet<int> visitedTasks)
        {
            if (visitedTasks.Contains(dependentTaskId))
            {
                throw new InvalidOperationException("Circular dependency detected.");
            }

            visitedTasks.Add(taskId);

            var task = await _databaseContext.Tasks
                .Include(t => t.DependentTasks)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            var dependentTask = await _databaseContext.Tasks
                .Include(t => t.DependentTasks)
                .FirstOrDefaultAsync(t => t.Id == dependentTaskId);

            if (task == null || dependentTask == null) return;

            var taskDuration = dependentTask.DueDate - dependentTask.StartDate;

            // Adjust the dependent task's dates based on the type of dependency
            switch (typeOfDependencyId)
            {
                case 1: // Start to Start
                    dependentTask.StartDate = task.StartDate;
                    dependentTask.DueDate = dependentTask.StartDate.Add(taskDuration);
                    break;
                case 2: // Start to End
                    dependentTask.DueDate = task.StartDate;
                    dependentTask.StartDate = dependentTask.DueDate.Subtract(taskDuration);

                    break;
                case 3: // End to Start
                    dependentTask.StartDate = task.DueDate;
                    dependentTask.DueDate = dependentTask.StartDate.Add(taskDuration);

                    break;
                case 4: // End to End
                    dependentTask.DueDate = task.DueDate;
                    dependentTask.StartDate = dependentTask.DueDate.Subtract(taskDuration);
                    break;
            }

            _databaseContext.Tasks.Update(dependentTask);
            await _databaseContext.SaveChangesAsync();

            // Step 3: Recursively update all dependent tasks
            await UpdateAllDependentTasks(dependentTask, visitedTasks);
        }

        private async System.Threading.Tasks.Task UpdateAllDependentTasks(Models.Task task, HashSet<int> visitedTasks)
        {
            var dependentTasks = _databaseContext.Set<TaskDependency>()
                .Where(td => td.TaskId == task.Id)
                .Select(td => td.DependentTaskId)
                .ToList();

            foreach (var dependentTaskId in dependentTasks)
            {
                var taskDependency = await _databaseContext.Set<TaskDependency>()
                    .FirstOrDefaultAsync(td => td.TaskId == task.Id && td.DependentTaskId == dependentTaskId);

                if (taskDependency != null)
                {
                    await UpdateDependentTasks(task.Id, dependentTaskId, taskDependency.TypeOfDependencyId, visitedTasks);
                }
            }
        }

        private async System.Threading.Tasks.Task<bool> CheckIsDateValid(Models.Task task, bool asParent = true, Hashtable visitedTasks = null)
        {
            if (visitedTasks == null)
            {
                visitedTasks = new Hashtable();
            }

            if (visitedTasks.ContainsKey(task.Id))
            {
                return (bool)visitedTasks[task.Id]!;
            }

            bool result = true;

            if(asParent)
            {
                var dependentTasksParent = _databaseContext.Set<TaskDependency>()
                .Where(td => td.TaskId == task.Id)
                .ToList();

                foreach (var taskDependency in dependentTasksParent)
                {
                    var dependentTask = await _databaseContext.Tasks
                        .FirstOrDefaultAsync(t => t.Id == taskDependency.DependentTaskId);

                    result = IsDependencySatisfied(task, dependentTask, taskDependency.TypeOfDependencyId);
                    if (!result)
                    {
                        visitedTasks.Add(task.Id, result);
                        return false;
                    }

                    result = await CheckIsDateValid(dependentTask, asParent, visitedTasks);
                    if (!result)
                    {
                        visitedTasks.Add(task.Id, result);
                        return false;
                    }
                }
            }
            else
            {
                var dependentTasksChild = _databaseContext.Set<TaskDependency>()
                .Where(td => td.DependentTaskId == task.Id)
                .ToList();

                foreach (var taskDependency in dependentTasksChild)
                {
                    var dependentTask = await _databaseContext.Tasks
                        .FirstOrDefaultAsync(t => t.Id == taskDependency.TaskId);

                    result = IsDependencySatisfied(dependentTask, task, taskDependency.TypeOfDependencyId);
                    if (!result)
                    {
                        visitedTasks.Add(task.Id, result);
                        return false;
                    }

                    result = await CheckIsDateValid(dependentTask, asParent, visitedTasks);
                    if (!result)
                    {
                        visitedTasks.Add(task.Id, result);
                        return false;
                    }
                }
            }

            visitedTasks.Add(task.Id, result);
            return result;
        }

        private bool IsDependencySatisfied(Models.Task parentTask, Models.Task dependentTask, int typeOfDependencyId)
        {
            switch (typeOfDependencyId)
            {
                case 1: // Start to Start
                    if (dependentTask.StartDate < parentTask.StartDate)
                        throw new ArgumentException($"Start to Start Dependency Violation: Dependent task (ID: {dependentTask.Id}) must start after or at the same time as parent task (ID: {parentTask.Id}).");
                    break;
                case 2: // Start to End
                    if (dependentTask.DueDate < parentTask.StartDate)
                        throw new ArgumentException($"Start to End Dependency Violation: Dependent task (ID: {dependentTask.Id}) must end after or at the same time as parent task's start date (ID: {parentTask.Id}).");
                    break;
                case 3: // End to Start
                    if (dependentTask.StartDate < parentTask.DueDate)
                        throw new ArgumentException($"End to Start Dependency Violation: Dependent task (ID: {dependentTask.Id}) must start after or at the same time as parent task's end date (ID: {parentTask.Id}).");
                    break;
                case 4: // End to End
                    if (dependentTask.DueDate < parentTask.DueDate)
                        throw new ArgumentException($"End to End Dependency Violation: Dependent task (ID: {dependentTask.Id}) must end after or at the same time as parent task (ID: {parentTask.Id}).");
                    break;

                default:
                    throw new ArgumentException("Invalid type of dependency! Doesn't exist in database!");
            }
            return true;
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

        public async System.Threading.Tasks.Task ChangeTaskProgress(HttpContext httpContext, TaskProgressDTO request)
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

            var task = await _databaseContext.Tasks.FindAsync(request.TaskId);

            if (task == null)
            {
                throw new ArgumentException($"Task with ID {request.TaskId} does not exist in database!");
            }

            var project = await _databaseContext.Projects.FindAsync(task.ProjectId);

            if (project == null)
            {
                throw new ArgumentException($"Project with ID {task.ProjectId} does not exist in database!");
            }

            var taskUser = await _databaseContext.TaskUsers
                .FirstOrDefaultAsync(tu => tu.UserId == userId && tu.TaskId == task.Id);

            if (taskUser == null)
            {
                throw new UnauthorizedAccessException("User is not assigned to this task!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == project.Id);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {project.Id} in UserProjects table!");
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

            if (request.Progress < 0 || request.Progress > 100)
            {
                throw new ArgumentException("Progress must be between 0 and 100!");
            }

            task.Progress = request.Progress;
            string taskUrl = $"http://localhost:4200/project/{task.ProjectId}/task/{task.Id}";
            Activity activity = new Activity(user.Id, task.ProjectId, $"User {user.Firstname} {user.Lastname} has changed the progress on the task <a href=\"{taskUrl}\">{task.Name}</a>", DateTime.Now);

            _databaseContext.Activities.Add(activity);
            _databaseContext.SaveChangesAsync();

            var projectUsers = _databaseContext.UserProjects
            .Where(up => up.ProjectId == task.ProjectId && up.UserId != userId)
            .Select(up => up.UserId)
            .ToList();

            // Create UserNotification for each user on the project
            foreach (var projectUser in projectUsers)
            {
                UserNotification userNotification = new UserNotification(projectUser, activity.Id, seen: false);
                _databaseContext.UserNotifications.Add(userNotification);
                NotificationDTO notificationDTO = new NotificationDTO { ProjectId = project.Id, UserId = (int)userId, ActivityDescription = activity.ActivityDescription, Seen = userNotification.Seen, Time = activity.Time };
                var connectionIds = NotificationHub.UserConnections.GetValueOrDefault(projectUser.ToString());
                if (connectionIds != null && connectionIds.Any())
                {
                    foreach (var connectionId in connectionIds)
                    {
                        await _notificationHubContext.Clients.Client(connectionId).ReceiveNotification(notificationDTO);
                    }
                }
            }

            await _databaseContext.SaveChangesAsync();

            await _databaseContext.SaveChangesAsync();
        }
    }
}
