using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HeadRepositoryNet.Entities
{
    public sealed class Password
    {
        private static readonly int SaltLength = 128;
        private static readonly int HashLength = 512;
        private static readonly int PbkdfIterrationsCount = 3;

        public Password(string password)
        {
            Salt = MakeSalt();
            Hash = HashPassword(password, Salt);
        }

        public Password(string password, string extSalt)
        {            
            Salt = extSalt;
            Hash = HashPassword(password, Salt);
        }

        public string Hash { get; private set; }

        public string Salt { get; private set; }

        public string HashFull { get { return Salt+Hash; } }

        public bool Check(string oldPassword)
        {
            string targerHash = HashPassword(oldPassword, Salt);

            bool areSame = true;
            for (int i = 0; i < targerHash.Length; i++)
                areSame &= (Hash[i] == targerHash[i]);

            return areSame;

        }

        public static bool EqualPassword(string password, string hashFull)
        {
            if (hashFull.Length < SaltLength)
                return false;
            string salt = hashFull.Substring(0,SaltLength);
            var pass2 = new Password(password, salt);
            return pass2.HashFull == hashFull;
        }

        private static string HashPassword(string password, string salt)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(password), Encoding.ASCII.GetBytes(salt), PbkdfIterrationsCount);
            return Convert.ToBase64String(pbkdf2.GetBytes(HashLength));
        }

        private static string MakeSalt()
        {
            using (RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltLength];
                rngCryptoServiceProvider.GetBytes(salt);

                return Encoding.ASCII.GetString(salt);
            }
        }
    }
}






