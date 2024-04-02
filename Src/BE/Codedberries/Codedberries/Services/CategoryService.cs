using Codedberries.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace Codedberries.Services
{
    public class CategoryService
    {
        private readonly AppDatabaseContext _context;
        private readonly AuthorizationService _authorizationService;

        public CategoryService(AppDatabaseContext context, AuthorizationService authorizationService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        public bool CreateNewCategory(HttpContext httpContext, CreateCategoryDTO categoryDTO)
        {
            var userId = _authorizationService.GetUserIdFromSession(httpContext);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("Invalid session!");
            }

            return true;
        }
    }
}
