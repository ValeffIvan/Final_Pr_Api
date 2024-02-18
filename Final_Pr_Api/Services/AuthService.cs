using Final_Pr_Api.Data;
using Final_Pr_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
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

        public AuthResponse Authenticate(string email, string password)
        {
            try
            {
                var user = ValidateCredentials(email, password);

                if (user != null)
                {

                    if (user.Role != null)
                    {
                        var token = JwtService.GenerateToken(user.username, email, user.Role);

                        var userDetails = new UserDetails
                        {
                            idUsers = user.idUsers,
                            username = user.username,
                            email = email,
                            Role = user.Role,
                            createTime = user.createTime,
                        };

                        return new AuthResponse
                        {
                            Success = true,
                            Token = token,
                            User = userDetails,
                            Message = "Inicio de sesión exitoso"
                        };
                    }
                }

                return new AuthResponse
                {
                    Success = false,
                    Token = null,
                    User = null,
                    Message = "Credenciales inválidas"
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Token = null,
                    User = null,
                    Message = ex.Message,
                };
            }
        }



        private UserDetails ValidateCredentials(string email, string password)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.email == email);
                if (user != null && VerifyPassword(password, user.password))
                {
                    var roleName = _context.Roles
                        .Where(r => r.idRol == user.idRol)
                        .Select(r => r.name)
                        .FirstOrDefault();

                    var userDetails = new UserDetails
                    {
                        idUsers = user.idUsers,
                        username = user.username,
                        email = user.email,
                        createTime = user.createTime,
                        Role = roleName
                    };
                    return userDetails;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> GetRoleByIdAsync(int idRol)
        {
            try
            {
                var rol = await _context.Roles.FirstOrDefaultAsync(r => r.idRol == idRol);
                if (rol != null)
                {
                    return rol.name;
                }
                else
                {
                    throw new Exception($"No se encontró rol");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el rol: {ex.Message}");
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
