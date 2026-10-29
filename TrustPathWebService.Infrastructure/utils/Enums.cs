using System;
using System.Linq;

namespace TrustPathWebService.Infrastructure.Utils
{
    public static class Enums
    {
        public enum UserType
        {
            System_Administrator = 1,
            Business_Admin = 2,
            Staff_Member = 3,
            Dyeing_Supervisor = 4,
            Collection_Agent = 5,
            Client = 6
        }

        public enum InvoiceStatus
        {
            Pending,
            PartiallyPaid,
            Paid,
            Overpaid
        }

        public enum CommuncationMode
        {
            Whatsapp = 1,
            Message = 2,
            Call = 3,
            One_to_One = 4,
        }

    }

}
