using Codedberries.Models;
using Codedberries.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Codedberries.Services
{
    public class RequestsService
    {
        private readonly AppDatabaseContext _databaseContext;

        public RequestsService(AppDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public UserFullNameDTO GetUserFullNameById(int userId)
        {
            User user = _databaseContext.Users.FirstOrDefault(u => u.Id == userId);
            
            if (user == null)
            {
                return null;
            }

            return new UserFullNameDTO
            {
                FirstName = user.Firstname,
                LastName = user.Lastname
            };
        }

        public AllRolesNamesDTO GetRoleNames()
        {
            List<string> roleNames = _databaseContext.Roles.Select(r => r.Name).ToList();
            
            return new AllRolesNamesDTO { RoleNames = roleNames };
        }
    }
}
