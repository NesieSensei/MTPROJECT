using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class CustomerR
    {
        public string Movie;
        public string RNum;
        public string SNum;
        

        public CustomerR(string movie_, string RNum_, string SNum_)
        {
            Movie = movie_;
            RNum = RNum_;
            SNum = SNum_;
        }
    }
}
