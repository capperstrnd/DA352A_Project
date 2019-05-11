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
        private string ssn;
        public string SSN
        {
            get { return ssn; }
            set
            {
                if (value.Length != 11 || !value.Contains('-'))
                    throw new Exception("SSN is not correct! Must be numeric in the form: xxxxxx-xxxx");
                string[] dateAndLastFour = value.Split('-');
                bool isNumeric1 = int.TryParse(dateAndLastFour[0], out int n1);
                bool isNumeric2 = int.TryParse(dateAndLastFour[1], out int n2);
                bool isCorrectLength = dateAndLastFour[0].Length == 6 && dateAndLastFour[1].Length == 4;
                if (isNumeric1 && isNumeric2 && isCorrectLength)
                {
                    ssn = value;
                }
                else
                    throw new Exception("SSN is not correct! Must be numeric in the form: xxxxxx-xxxx");
            }
        }
        private string city;
        public string City
        {
            get { return city; }
            set
            {
                if (value.Length > 0)
                    city = value;
                else
                    throw new Exception("City must be selected");
            }
        }
        private string occupation;
        public string Occupation
        {
            get { return occupation; }
            set
            {
                if (value.Length > 0)
                    occupation = value;
                else
                    throw new Exception("Occupation must be filled");
            }
        }
        public int BonusCounter { get; set; }
        private string membershipDate;
        public string MembershipDate
        {
            get { return membershipDate; }
            set
            {
                if (value.Length > 0)
                    membershipDate = value;
                else
                    throw new Exception("The date must be filled.");
            } 
        }

        public Customer(string sSN, string city, string occupation, int bonusCounter, string membershipDate)
        {
            SSN = sSN;
            City = city;
            Occupation = occupation;
            BonusCounter = bonusCounter;
            MembershipDate = membershipDate;
        }

        public Customer()
        {
        }
    }
}
