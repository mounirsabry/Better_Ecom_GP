using BC = BCrypt.Net.BCrypt;

namespace Better_Ecom_Backend.Helpers
{


    public class SecurityUtilities
    {
        public SecurityUtilities() { }

        /// <summary>
        /// takes use password and hash it.
        /// </summary>
        /// <param name="pass">user password</param>
        /// <returns>hashed password</returns>
        public static string HashPassword(string pass)
        {

            return BC.HashPassword(pass);
        }
        /// <summary>
        /// verify given password against a hash
        /// </summary>
        /// <param name="normalPass">the user pass.</param>
        /// <param name="hashedPass">the stored hash.</param>
        /// <returns></returns>
        public static bool Verify(string normalPass, string hashedPass)
        {
            return BC.Verify(normalPass, hashedPass);
        }
    }
}
