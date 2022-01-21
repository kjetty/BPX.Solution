using System;

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
    }

    public static class RecordStatus
    {
        // Variables for the record status
        public const string Active = "A";
        public const string Inactive = "I";
    }

    public static class SortOrder
    {
        // Variables for the record status
        public const string Ascending = "asc";
        public const string Descending = "desc";
    }
}