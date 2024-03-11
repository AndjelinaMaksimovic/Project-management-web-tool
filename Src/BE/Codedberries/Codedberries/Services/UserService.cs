using System.Security.Cryptography;

namespace Codedberries.Services
{
    public class UserService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        private readonly AppDatabaseContext _databaseContext;
        private const int SessionDurationHours = 24;

        public UserService(AppDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        private bool VerifyPassword(string password, string hashedPassword, byte[] salt)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] saltFromHash = new byte[SaltSize];
            Array.Copy(hashBytes, 0, saltFromHash, 0, SaltSize);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltFromHash, Iterations))
            {
                byte[] key = pbkdf2.GetBytes(KeySize);

                for (int i = 0; i < KeySize; i++)
                {
                    if (key[i] != hashBytes[i + SaltSize])
                    {
                        return false; // wrong password
                    }
                }
            }

            return true;
        }
    }
}
