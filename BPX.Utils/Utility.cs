using System;
using System.Linq;
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

            return sb.ToString();             
        }

        public static string Hypenate2124(string input)
        {
            string result = input;

            if (result != null && result.Length == 21)
            {
                // formats a 21 character string to a 24 character string with hypens
                // 1234-12345678-1234-12345
                return result.Insert(4, "-").Insert(13, "-").Insert(18, "-");
            }

            return result;
        }

        public static string GetLToken(string pToken)
        {
            return new string(pToken.ToCharArray().Reverse().ToArray());
        }

        public static string GetUToken(string pToken)
        {
            string result = string.Empty;

            if (pToken != null)
            {
                for (int i = 0; i < pToken.Length; i += 2)
                {
                    result += pToken[i];
                }

                for (int i = 1; i < pToken.Length; i += 2)
                {
                    result += pToken[i];
                }
            }

            return result;
        }
    }

    public static class RecordStatus
    {
        // variables for the record status
        public const string Active = "A";
        public const string Inactive = "I";
        public const string Archived = "R";
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
        public const string CAC = "C";
        public const string PIV = "P";
        public const string Username = "U";        
    }
}