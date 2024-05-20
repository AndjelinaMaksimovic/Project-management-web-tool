using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace Codedberries.Services
{
    public class CategoryService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public CategoryService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public async System.Threading.Tasks.Task CreateNewCategory(HttpContext httpContext, CreateCategoryDTO categoryDTO)
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

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (categoryDTO.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var project = await _databaseContext.Projects.FirstOrDefaultAsync(p => p.Id == categoryDTO.ProjectId);
           
            if (project == null)
            {
                throw new ArgumentException($"Project with ID {categoryDTO.ProjectId} does not exist in the database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == categoryDTO.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {categoryDTO.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRoleOnProject = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRoleOnProject == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRoleOnProject.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Project! Cannot create category!");
            }
            // ---------------- //

            if (string.IsNullOrWhiteSpace(categoryDTO.CategoryName))
            {
                throw new ArgumentException("CategoryName cannot be null or empty!");
            }

            bool categoryExists = _databaseContext.Categories.Any(c => c.Name == categoryDTO.CategoryName && c.ProjectId == categoryDTO.ProjectId);

            if (categoryExists)
            {
                throw new InvalidOperationException("Category with the same name already exists in the project!");
            }

            Category newCategory = new Category(categoryDTO.CategoryName, categoryDTO.ProjectId);
            
            _databaseContext.Categories.Add(newCategory);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<List<CategoryDTO>> GetAllProjectCategories(HttpContext httpContext, ProjectIdDTO request)
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
                throw new UnauthorizedAccessException("User role not found!");
            }

            if (request.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

            var categories = await _databaseContext.Categories
                .Where(c => c.ProjectId == request.ProjectId)
                .Select(c => new CategoryDTO { Id = c.Id, Name = c.Name })
                .ToListAsync();

            if (categories == null || !categories.Any())
            {
                throw new InvalidOperationException("No categories found for the specified project.");
            }

            return categories;
        }

        public async System.Threading.Tasks.Task DeleteCategory(HttpContext httpContext, CategoryIdDTO request)
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

            var userRole = _databaseContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);

            if (userRole == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (request.CategoryId <= 0)
            {
                throw new ArgumentException("CategoryId must be greater than 0!");
            }

            var providedCategory = _databaseContext.Categories.FirstOrDefault(c => c.Id == request.CategoryId);

            if (providedCategory == null)
            {
                throw new ArgumentException($"Provided Category with ID {request.CategoryId} does not exist in database!");
            }

            // UserProjects --- //
            var userProject = _databaseContext.UserProjects
                .FirstOrDefault(up => up.UserId == userId && up.ProjectId == providedCategory.ProjectId);

            if (userProject == null)
            {
                throw new UnauthorizedAccessException($"No match for UserId {userId} and ProjectId {providedCategory.ProjectId} in UserProjects table!");
            }

            var userRoleId = userProject.RoleId;
            var userRoleOnProject = _databaseContext.Roles.FirstOrDefault(r => r.Id == userRoleId);

            if (userRoleOnProject == null)
            {
                throw new UnauthorizedAccessException("User role not found in database!");
            }

            if (userRoleOnProject.CanEditProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to edit Project! Cannot delete category!");
            }
            // ---------------- //

            // check if any task has this category
            var categoryAssignedToTask = _databaseContext.Tasks.Any(t => t.CategoryId == request.CategoryId);

            if (categoryAssignedToTask)
            {
                throw new ArgumentException($"Category with ID {request.CategoryId} is already assigned to a task and cannot be deleted!");
            }

            _databaseContext.Categories.Remove(providedCategory);
            await _databaseContext.SaveChangesAsync();
        }

    }
}
