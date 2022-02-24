using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BPX.Utils
{
    public static class Utility
    {
        public static string GetErrorMessageFromException(Exception ex)
        {
            Exception exception = ex;

            // extract message from the innermost exception
            while (exception.InnerException != null)
                exception = exception.InnerException;

            return exception.Message;
        }

        public static string RemoveSpecialCharacters(string inputString)
        {
            return Regex.Replace(inputString, @"[\W-[_]]+", string.Empty).Trim();
        }

        public static string GetUUID(int size)
        {
            StringBuilder sb = new StringBuilder(size);

            char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] bytes = new byte[size];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(bytes);
            }

            foreach (byte b in bytes)
            {
                sb.Append(chars[b % (chars.Length)]);
            }

            return sb.ToString();
        }
    }

    public static class RecordStatus
    {
        // variables for the record status
        public const string Active = "A";
        public const string Inactive = "I";
    }

    public static class SortOrder
    {
        // variables for the record status
        public const string Ascending = "asc";
        public const string Descending = "desc";
    }

    public static class LoginCategory
    {
        // variables for the record status
        public const string Username = "U";
        public const string PIV = "P";
    }
}