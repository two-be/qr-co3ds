using System;
using QrCo3ds.Models;

namespace QrCo3ds.Extensions
{
    public static class ExceptionExtensions
    {
        public static ExceptionInfo ToInfo(this Exception ex)
        {
            return new ExceptionInfo(ex.Message, ex.StackTrace);
        }
    }
}
