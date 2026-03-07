using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents a customer/member of the e-commerce platform.
    /// </summary>
    public class Member
    {
        public int MemberId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PersistentToken { get; set; }
        public DateTime? TokenExpires { get; set; }

        public Member() { }

        public Member(int memberId, string username, string email, string fullName)
        {
            MemberId = memberId;
            Username = username;
            Email = email;
            FullName = fullName;
        }

        public override string ToString()
        {
            return $"Member: {FullName} ({Username}) - {Email}";
        }
    }
}
