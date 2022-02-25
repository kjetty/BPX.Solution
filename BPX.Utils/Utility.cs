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

            char[] chars = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
            byte[] bytes = new byte[size];

            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(bytes);
            }

            foreach (byte b in bytes)
            {
                sb.Append(chars[b % (chars.Length)]);
            }

            string temp = sb.ToString();
            string temp16 = temp.Insert(16, "-");
            string temp12 = temp16.Insert(12, "-");
            string temp8 = temp12.Insert(8, "-");
            string temp4 = temp8.Insert(4, "-");

            return temp4;
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
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }

    public static class LoginCategory
    {
        // variables for the record status
        public const string Username = "U";
        public const string PIV = "P";
    }
}