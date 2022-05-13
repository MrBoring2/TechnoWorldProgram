using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Helpers.Services
{
    public class MD5EncoderService
    {
        public static string EncodePassword(string login, string password)
        {
            using (var md5 = MD5.Create())
            {
                byte[] passBytes = System.Text.Encoding.ASCII.GetBytes(password + login);
                byte[] hash = md5.ComputeHash(passBytes);


                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }
    }
}
