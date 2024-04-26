using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HealthInsuranceSystem.Core.Security
{
    public interface IPasswordService
    {
        string CreateHash(string password, string salt);

        string CreateSalt();

        string GetCustomSalt(string salt);

        string EncryptString(string text, string salt);

        string DecryptString(string cipherText, string salt);

        bool IsValidPassword(string password);
    }

    public class PasswordService : IPasswordService
    {
        private readonly PasswordPolicy _passwordPolicy;

        public PasswordService(PasswordPolicy passwordPolicy)
        {
            _passwordPolicy = passwordPolicy;
        }

        public string CreateHash(string password, string salt)
        {
            var salted = string.Concat(password, salt);

            using (var hashService = new SHA256CryptoServiceProvider())
            {
                var hash = hashService.ComputeHash(Encoding.ASCII.GetBytes(salted));

                return Convert.ToBase64String(hash);
            }
        }

        public string CreateSalt()
        {
            byte[] bytes = new byte[128 / 8];
            using (var keyGenerator = RandomNumberGenerator.Create())
            {
                keyGenerator.GetBytes(bytes);
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public string EncryptString(string text, string salt)
        {
            var key = Encoding.UTF8.GetBytes(salt);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public string DecryptString(string cipherText, string salt)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(salt);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        public string GetCustomSalt(string salt)
        {
            if (salt.Length > 32)
            {
                salt = salt.Substring(0, 32);
            }
            else if (salt.Length < 32)
            {
                var addChar = 32 - salt.Length;
                salt += string.Concat(Enumerable.Repeat("x", addChar));
            }
            else { }
            return salt;
        }

        public bool IsValidPassword(string password)
            => _passwordPolicy.IsValid(password);
    }
}
