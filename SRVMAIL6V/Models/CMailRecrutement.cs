using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRVMAIL6V.Models
{
  public  class CMailRecrutement
    {
        
        public int mIdCandidat { get; set; }
        public DateTime mDateCreationCV { get; set; }
        public string mNom { get; set; }
        public string mPrenoms { get; set; }
        public string mNoteTESTMBTI { get; set; }

        public int mIsPasseTESTATTENTIONDETAILS { get; set; }

        public int mIsPasseTESTC { get; set; }

        public int mIsPasseTESTLOGIQUE { get; set; }

        public int mIsPasseTESTPENSEECRITIQUE { get; set; }

        public int mIsPasseTESTPROBSOLUTION { get; set; }

        public int mIsPasseTESTV { get; set; }

        public int mIsPasseTESTMBTI { get; set; }
        public int mIsMailRecrutement { get; set; }

        public int mIsFinishTEST { get; set; }
        
        public string mEmail { get; set; }

        public int mIsDelete { get; set; }//1 = oui / 0=NON C'est pour savoir si c'est "supprimé" ou pas

        public int mIsMailEnvoyeOK { get; set; }//0: Non envoyé et 1: Refus et 2:POSITIF EN ATTENTE et 3:POSITIF A CONVOQUER POUR TEST TECHNIQUE

        public string mCommentaires { get; set; }
       

        


        public CMailRecrutement()
        {
            mIdCandidat = 0;
            mDateCreationCV = new DateTime();
            mNom = string.Empty;
            mPrenoms = string.Empty;
          
            mNoteTESTMBTI = string.Empty;
          
            mEmail = string.Empty;

            mIsDelete = 0;

            mIsMailEnvoyeOK = 0;

            mCommentaires = string.Empty;
            
            mIsPasseTESTATTENTIONDETAILS = 0;
            mIsPasseTESTC = 0;
            mIsPasseTESTLOGIQUE = 0;
            mIsPasseTESTPENSEECRITIQUE = 0;
            mIsPasseTESTPROBSOLUTION = 0;
            mIsPasseTESTV = 0;
            mIsPasseTESTMBTI = 0;
            mIsMailRecrutement = 0;
            mIsFinishTEST = 0;
       
            mIsMailEnvoyeOK = 0;
      

        }


    }
}
