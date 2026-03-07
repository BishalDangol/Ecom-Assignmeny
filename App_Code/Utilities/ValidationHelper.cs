using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Saja.Utilities
{
    /// <summary>
    /// Utility class for various validation tasks.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates if a string is a valid email address.
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Validates Nepal phone numbers (10 digits starting with 9).
        /// </summary>
        public static bool IsValidNepalPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            // Nepal mobile numbers are 10 digits and generally start with 9
            return Regex.IsMatch(phone, @"^9[678][0-9]{7}$");
        }

        /// <summary>
        /// Validates password strength (min 8 chars, 1 upper, 1 lower, 1 digit).
        /// </summary>
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            return password.Length >= 8 &&
                   Regex.IsMatch(password, @"[A-Z]") &&
                   Regex.IsMatch(password, @"[a-z]") &&
                   Regex.IsMatch(password, @"[0-9]");
        }

        /// <summary>
        /// Checks if a string is null or empty.
        /// </summary>
        public static bool IsRequired(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
