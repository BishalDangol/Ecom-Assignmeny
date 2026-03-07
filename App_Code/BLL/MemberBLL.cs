using System;
using Saja.DAL;
using Saja.Entities;
using Saja.Utilities;

namespace Saja.BLL
{
    public class MemberBLL
    {
        private MemberDAL memberDAL;

        public MemberBLL()
        {
            memberDAL = new MemberDAL();
        }

        /// <summary>
        /// Authenticates a member and returns the member object if successful.
        /// </summary>
        public OperationResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return OperationResult.Failed("Username and password are required.");

            Member member = memberDAL.GetByUsername(username);

            if (member == null)
                return OperationResult.Failed("User does not exist.");

            // Verify hashed password
            if (SecurityHelper.VerifyPassword(password, member.Password))
            {
                // Clear password for security before putting in session
                member.Password = string.Empty;
                return OperationResult.Succeeded("Login successful.", member);
            }

            return OperationResult.Failed("Incorrect password.");
        }

        /// <summary>
        /// Registers a new member with validation.
        /// </summary>
        public OperationResult Register(Member member)
        {
            // Validation
            if (!ValidationHelper.IsRequired(member.Username))
                return OperationResult.Failed("Username is required.");
            
            if (!ValidationHelper.IsValidEmail(member.Email))
                return OperationResult.Failed("Invalid email address.");

            if (!ValidationHelper.IsValidNepalPhone(member.Phone))
                return OperationResult.Failed("Please enter a valid 10-digit Nepal phone number.");

            if (memberDAL.GetByUsername(member.Username) != null)
                return OperationResult.Failed("Username is already taken.");

            // Hash Password
            member.Password = SecurityHelper.HashPassword(member.Password);

            int newId = memberDAL.Insert(member);
            if (newId > 0)
                return OperationResult.Succeeded("Registration successful. You can now login.", newId);

            return OperationResult.Failed("Registration failed. Please try again.");
        }
    }
}
