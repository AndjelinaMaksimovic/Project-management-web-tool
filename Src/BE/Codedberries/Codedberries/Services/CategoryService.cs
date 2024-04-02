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
    }
}
