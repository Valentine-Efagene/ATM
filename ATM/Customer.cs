using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM
{
    public class Customer
    {
        private string FirstName = null;
        private string MiddleName = null;
        private string LastName = null;
        private string AccountNumber = null;
        private string Email = null;
        private string PhoneNumber = null;
        private int[] Fingerprints = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        public string GetFirstName()
        {
            return FirstName;
        }

        public void SetFirstName(string s)
        {
            FirstName = s;
        }

        public string GetAccountNumber()
        {
            return AccountNumber;
        }

        public void SetAccountNumber(string s)
        {
            AccountNumber = s;
        }

        public string GetMiddleName()
        {
            return MiddleName;
        }

        public void SetMiddleName(string s)
        {
            MiddleName = s;
        }

        public void SetLastName(string s)
        {
            LastName = s;
        }

        public string GetLastName()
        {
            return LastName;
        }

        public void SetEmail(string s)
        {
            Email = s;
        }

        public string GetPhoneNumber()
        {
            return PhoneNumber;
        }

        public void SetPhoneNumber(string s)
        {
            PhoneNumber = s;
        }

        public string GetEmail()
        {
            return Email;
        }

        public void SetFingerPrints(int[] f)
        {

        }
        public int[] GetFingerPrints()
        {
            return Fingerprints;
        }
    }
}
