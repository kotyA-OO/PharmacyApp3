using System;
using System.Security.Cryptography;

namespace PharmacyApp3
{
    public static class PasswordHasher
    {
        // Генерируем случайную соль (16 байт)
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Хешируем пароль с солью (100 000 итераций, алгоритм SHA256)
        public static string HashPassword(string password, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 100000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}