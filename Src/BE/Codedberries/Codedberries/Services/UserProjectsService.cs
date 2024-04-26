using Codedberries.Models.DTOs;

namespace Codedberries.Services
{
    public class UserProjectsService
    {
        private readonly AppDatabaseContext _databaseContext;
        private readonly AuthorizationService _authorizationService;

        public UserProjectsService(AppDatabaseContext databaseContext, AuthorizationService authorizationService)
        {
            _databaseContext = databaseContext;
            _authorizationService = authorizationService;
        }

        // get all users that are on specific project
        public async Task<List<UserProjectsDTO>> GetUsersByProjectId(HttpContext httpContext, ProjectIdDTO request)
        {

        }
    }
}
