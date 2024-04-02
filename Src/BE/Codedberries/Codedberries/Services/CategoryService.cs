using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.AspNetCore.Http;

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

            bool categoryExists = _databaseContext.Categories.Any(c => c.Name == categoryDTO.CategoryName && c.ProjectId == categoryDTO.ProjectId);

            if (categoryExists)
            {
                throw new InvalidOperationException("Category with the same name already exists in the project!");
            }

            Category newCategory = new Category(categoryDTO.CategoryName, categoryDTO.ProjectId);
            
            _databaseContext.Categories.Add(newCategory);
            await _databaseContext.SaveChangesAsync();
        }
    }
}
