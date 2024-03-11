﻿using Codedberries.Models;
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

        public Session LoginUser(string email, string password)
        {
            /*
            User user = _databaseContext.Users.FirstOrDefault(u => u.Email == email);

            if (user != null && VerifyPassword(password, user.Password, user.PasswordSalt))
            {
            */
            User user = new User();     // hard-code
            user.UserId = 5;            // hard-code
            user.Email = email;         // hard-code
            user.Password = password;   // hard-code

            if (email == "admin@gmail.com" && password == "admin")
            { // hard-code (only "if" line)
                // create new session
                var sessionToken = GenerateSessionToken();
                var expirationTime = DateTime.UtcNow.AddHours(SessionDurationHours);
                var session = new Session { UserId = user.UserId, Token = sessionToken, ExpirationTime = expirationTime };

                _databaseContext.Sessions.Add(session);
                _databaseContext.SaveChanges();

                return session;
            }

            return null; // user not found or password incorrect
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

        public bool ValidateSession(string sessionToken)
        {
            var session = _databaseContext.Sessions.FirstOrDefault(s => s.Token == sessionToken);

            if (session != null && session.ExpirationTime > DateTime.UtcNow)
            {
                // update session
                session.ExpirationTime = DateTime.UtcNow.AddHours(SessionDurationHours);
                _databaseContext.SaveChanges();

                return true;
            }
            return false; // session not found or expired
        }

        private string GenerateSessionToken()
        {
            // unique session token
            return Guid.NewGuid().ToString();
        }

        public void CreateSessionCookie(HttpContext httpContext, Session session)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Expires = session.ExpirationTime
            };

            httpContext.Response.Cookies.Append("sessionId", session.Token, cookieOptions);
        }
    }
}
