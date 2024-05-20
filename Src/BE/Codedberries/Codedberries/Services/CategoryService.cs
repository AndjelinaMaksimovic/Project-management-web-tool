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

            if (userRole.CanCreateProject == false)
            {
                throw new UnauthorizedAccessException("User does not have permission to create new category!");
            }

            if (categoryDTO.ProjectId <= 0)
            {
                throw new ArgumentException("ProjectId must be greater than 0!");
            }

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

        public async System.Threading.Tasks.Task DeleteCategory(HttpContext httpContext, CategoryDTO request)
        {
            var categoryToDelete = await _databaseContext.Categories.FindAsync(request.Id);

            if (categoryToDelete == null)
            {
                throw new InvalidOperationException($"Category with ID {request.Id} not found in the database!");
            }

            _databaseContext.Categories.Remove(categoryToDelete);
            await _databaseContext.SaveChangesAsync();
        }

    }
}
