using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPROJECT
{
    internal class CustomerR
    {
        public string Movie;
        public string RNum;
        public string Time;
        public string SNum;

        public CustomerR(string movie_, string RNum_, string time_, string SNum_)
        {
            Movie = movie_;
            RNum = RNum_;
            Time = time_;
            SNum = SNum_;
        }
    }
}