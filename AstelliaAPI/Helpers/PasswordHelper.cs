using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using scr = Org.BouncyCastle.Crypto.Generators.SCrypt;

namespace AstelliaAPI.Helpers
{
    // always return true
    public class BCrypt
    {
        [DllImport(@"Libraries/BCrypt")]
        public static extern string generate_hash(string password, int rounds = 12);

        [DllImport(@"Libraries/BCrypt")]
        public static extern bool validate_password(string password, string hash);
    }

    public class SCrypt
    {
        public static string RandomString(int n)
        {
            var ret = new StringBuilder();
            const string ascii = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";

            for (var i = 0; i < n; i++)
                ret.Append(ascii[new Random().Next(0, ascii.Length)]);

            return ret.ToString();
        }

        public static byte[] PseudoSecureBytes(int n)
        {
            var provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[n];
            provider.GetBytes(byteArray);
            return byteArray;
        }

        public static byte[] generate_salt() => PseudoSecureBytes(new Random().Next(90, 100));

        public static (string password, byte[] salt) generate_hash(string password, int rounds = 20)
        {
            var pwBytes = Encoding.Default.GetBytes(password);
            var saltBytes = generate_salt();
            var pwHashBytes = scr.Generate(pwBytes, saltBytes, 262144 / 4, rounds, 1, 512);
            return (Convert.ToBase64String(pwHashBytes), saltBytes);
        }

        public static bool validate_password(string password, string hash, byte[] salt, int rounds = 20)
        {
            var pwBytes = Encoding.Default.GetBytes(password);
            var pwHashBytes = scr.Generate(pwBytes, salt, 262144 / 4, rounds, 1, 512);
            var hashBytes = Convert.FromBase64String(hash);

            return pwHashBytes.SequenceEqual(hashBytes);
        }
    }

    public class PasswordHelper
    {
        public static bool IsValidPassword(string dbPassword, string rawPassword, byte[] salt, bool is_bancho)
        {
            string md5Password = string.Empty;
            if (!is_bancho)
            {
                md5Password = MD5Helper.GetMd5(rawPassword);
            } 
            else
            {
                md5Password = rawPassword;
            }
            return SCrypt.validate_password(md5Password, dbPassword, salt);
        }
        public static (string password, byte[] salt) GeneratePassword(string password)
        {
            var md5Password = MD5Helper.GetMd5(password);
            return SCrypt.generate_hash(md5Password);
        }
    }

    public static class MD5Helper
    {
        public static byte[] GetMd5(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(data);
            }
        }

        public static string GetMd5(string data) => ToHex(GetMd5(Encoding.UTF8.GetBytes(data)));
        public static string ToHex(byte[] data)
        {
            var hex = new StringBuilder(data.Length * 2);
            foreach (var d in data)
                hex.AppendFormat("{0:x2}", d);
            return hex.ToString();
        }
        public static string Compute(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
