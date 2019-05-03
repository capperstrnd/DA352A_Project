using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSQL_0._0
{

    /*
     * ID/personnummer/SSN), address, occupation) and obtaining their personal card with customer specific barcode. 
     */
    class Customer
    {
        public ObjectId Id { get; set; }
        public string SSN { get; set; }
        public string City { get; set; }
        public string Occupation { get; set; }
        public int BonusCounter { get; set; }
        public string MembershipDate { get; set; }

        public Customer(string sSN, string city, string occupation, int bonusCounter, string membershipDate)
        {
            SSN = sSN;
            City = city;
            Occupation = occupation;
            BonusCounter = bonusCounter;
            MembershipDate = membershipDate;
        }
    }
}
