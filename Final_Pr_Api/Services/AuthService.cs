using Final_Pr_Api.Data;
using Final_Pr_Api.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Final_Pr_Api.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public (string Token, Exception Error) Authenticate(string email, string password)
        {
            try
            {
                var authUser = ValidateCredentials(email, password);

                if (authUser != null)
                {
                    var token = JwtService.GenerateToken(authUser.username, email);
                    return (Token: token, Error: null);
                }

                return (Token: null, Error: new Exception("Credenciales inválidas."));
            }
            catch (Exception ex)
            {
                // Loggea la excepción para tener información detallada
                Console.WriteLine($"Error en la autenticación: {ex.Message}");
                return (Token: null, Error: ex);
            }
        }

        private User ValidateCredentials(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.email == email);
                if (user != null)
                {
                    if (VerifyPassword(password, user.password))
                    {
                        var authUser = new User { email = user.email, username = user.username, password = user.password };
                        return authUser;
                    }
                }
                return null;    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la autenticación: {ex.Message}");
                return null;
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public static bool VerifyPassword(string password, string hashedPassword)
       {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
