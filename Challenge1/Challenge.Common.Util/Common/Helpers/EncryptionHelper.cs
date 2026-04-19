using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using challenge1.Common.Util.Common;
using System.Security.Cryptography;
using System.Text;

namespace challenge1.Common.Util.Common.Helpers
{
    public static class EncryptionHelper
    {
        private const string Algorithm = "AES/GCM/NoPadding";

        public static string ComputeSha256Hash(string plainText)
        {
            try
            {
                using var sha256Hash = SHA256.Create();
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
            catch
            {
                //throw exception to parent method
                throw;
            }
        }

        public static string EncryptData(string PlainText, string salt = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salt))
                {
                    salt = Secrets.GetSecret("salt") ?? "";
                }

                var key = CreateKey(salt);
                //var random = new SecureRandom();
                //var iv = random.GenerateSeed(AesIvSize);
                var iv = CreateIV(salt);

                if (!string.IsNullOrWhiteSpace(PlainText) && !string.IsNullOrWhiteSpace(salt))
                {
                    AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);
                    var cipher = CipherUtilities.GetCipher(Algorithm);
                    cipher.Init(true, parameters);

                    var plainTextData = Encoding.UTF8.GetBytes(PlainText);
                    var cipherText = cipher.DoFinal(plainTextData);

                    //return PackCipherData(cipherText, iv);
                    return Convert.ToBase64String(cipherText.ToArray()); ;
                }
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return string.Empty;
        }

        public static string DecryptData(string cipherText, string salt = "", long readLength = 2097152)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salt))
                {
                    salt = Secrets.GetSecret("salt") ?? "";
                }

                var key = CreateKey(salt);
                var iv = CreateIV(salt);


                if (!string.IsNullOrWhiteSpace(cipherText) && !string.IsNullOrWhiteSpace(salt))
                {
                    //var (encryptedBytes, iv, tagSize) = UnpackCipherData(cipherText);


                    AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);
                    var cipher = CipherUtilities.GetCipher(Algorithm);
                    cipher.Init(false, parameters);

                    //var decryptedData = cipher.DoFinal(encryptedBytes);
                    var cipherData = Convert.FromBase64String(cipherText);
                    var decryptedData = cipher.DoFinal(cipherData);

                    return Encoding.UTF8.GetString(decryptedData);
                }
            }
            catch (CryptographicException)
            {
                return cipherText;
            }
            catch (FormatException)
            {
                return cipherText;
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return string.Empty;
        }

        public static byte[]? CreateKey(string salt = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(salt))
                {
                    //using SHA512Managed sha512 = new SHA512Managed();
                    using SHA512 sha512 = SHA512.Create();
                    byte[] results = sha512.ComputeHash(Encoding.ASCII.GetBytes(salt));
                    if (results != null && results.Length > 32)
                    {
                        return results.Take(32).ToArray();
                    }
                }
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return null;
        }

        public static byte[]? CreateIV(string salt = "")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(salt))
                {
                    //using SHA512Managed sha512 = new SHA512Managed();
                    using SHA512 sha512 = SHA512.Create();
                    byte[] results = sha512.ComputeHash(Encoding.ASCII.GetBytes(salt));
                    if (results != null && results.Length > 48)
                    {
                        return results.Skip(32).Take(16).ToArray();
                    }
                }
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return null;
        }

        public static string Base64Encode(string plainText)
        {
            string encodedText = string.Empty;

            try
            {
                encodedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return encodedText;
        }

        public static string Base64Decode(string encodedText)
        {
            string plainText = string.Empty;

            try
            {
                plainText = Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
            }
            catch
            {
                //throw exception to parent method
                throw;
            }

            return plainText;
        }
    }
}