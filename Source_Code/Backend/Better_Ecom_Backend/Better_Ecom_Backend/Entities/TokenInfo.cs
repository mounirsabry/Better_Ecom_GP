using System;

namespace Better_Ecom_Backend.Entities
{
    public class TokenInfo
    {
        public int UserID { get; set; }
        public string Type { get; set; }

        public TokenInfo(int userID, string type)
        {
            UserID = userID;
            Type = type;
        }

        public TokenInfo() : this(-1, "invalid") { }

        public void Print()
        {
            Console.WriteLine($"Token Info :{{UserID :{ UserID }, Type :{ Type }}}.");
        }
    }
}
