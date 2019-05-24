using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dax.Scrapping.Appraisal.Core
{
    public class Order
    {
        public string OrderDetailId { get; set; }
        public string ProductType { get; set; }
        public string TransactionType { get; set; }
        public string OrderType { get; set; }
        public string DueDate { get; set; }
        public string ClientLoanNumber { get; set; }
        public string Feed { get; set; }
        public string Address { get; set; }
        public string City { get; set;}
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Client { get; set; }
        public string AssignedDate { get; set; }


    }
}
