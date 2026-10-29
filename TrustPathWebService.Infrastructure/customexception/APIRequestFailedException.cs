using System;
using System.Linq;

namespace TrustPathWebService.Infrastructure.CustomException
{
    [Serializable]
    public class APIRequestFailedException : Exception
    {
        public APIRequestFailedException(string message)
            : base(message) { }
    }
}
