using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HeadRepositoryNet.Entities
{
    public class AuthOptions
    {
        public static string Issuer { get; set; } // издатель токена
        public static string Audience { get; set; } // потребитель токена
        public static string KeyRefresh { get; set; } // ключ для шифрации refresh
        public static string KeyAccess { get; set; } // ключ для шифрации access
        public static int LifetimeAccess { get; set; } // время жизни токена, минуты 
        public static int LifetimeRefresh { get; set; } // время жизни Refresh токена, минуты (для поторного получения Access токена) 

        public static SymmetricSecurityKey GetSymmetricSecurityKey(string key)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
        }        
    }
}
