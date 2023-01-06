using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRVMAIL6V.Models
{
   public class CParams
    {
        public int mId { get; set; }
        public string mEmail { get; set; }
        public string mSmtp { get; set; }
        public string mSmtpPassword { get; set; }
        public int mPort { get; set; }
        public string mCheminDocServeur { get; set; }


        public CParams()
        {
            mId = 0;
            mEmail = string.Empty;
            mSmtp = string.Empty;
            mPort = 0;
            mSmtpPassword = string.Empty;
            mCheminDocServeur = string.Empty;
        }

    }
}
