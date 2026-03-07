using System;

namespace Saja.Entities
{
    /// <summary>
    /// Represents an administrator in the system.
    /// </summary>
    public class Admin
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PersistentToken { get; set; }
        public DateTime? TokenExpires { get; set; }

        public Admin() { }

        public Admin(int adminId, string username, string fullName, string role)
        {
            AdminId = adminId;
            Username = username;
            FullName = fullName;
            Role = role;
        }

        public override string ToString()
        {
            return $"Admin: {FullName} ({Username}) - Role: {Role}";
        }
    }
}
