using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;

namespace Better_Ecom_Backend.Helpers
{
    public class SecurityUtilities
    {
        public SecurityUtilities() { }
        public static string HashPassword(string pass)
        {
            return BC.HashPassword(pass);
        }

        public static bool Verify(string normalPass, string hashedPass)
        {
            return BC.Verify(normalPass, hashedPass);
        }
    }
}
