using System;
using BC = BCrypt.Net.BCrypt;

namespace HashingTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "A11111";
            string hashedPassword = BC.HashPassword(password);
            Console.WriteLine($"Hashed Password :{ hashedPassword }");

            Console.ReadLine();
        }
    }
}
