using Codedberries.Models.DTOs;

namespace Codedberries.Services
{
    public class PriorityService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public PriorityService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        public async Task<List<PriorityInfoDTO>> GetAllPriorities(HttpContext httpContext)
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
        }
    }
}
