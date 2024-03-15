using Codedberries.Models.DTOs;

namespace Codedberries.Services
{
    public class RoleService
    {
        private readonly AppDatabaseContext _databaseContext;

        public RoleService(AppDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public AllRolesNamesDTO GetRoleNames()
        {
            List<string> roleNames = _databaseContext.Roles.Select(r => r.Name).ToList();

            return new AllRolesNamesDTO { RoleNames = roleNames };
        }
    }
}
