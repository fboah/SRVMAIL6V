using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRVMAIL6V.Models
{
  public  class CTESTOKMAIL
    {
        public int mIdCandidat { get; set; }
        public DateTime mDateCreationCV { get; set; }
        public string mNom { get; set; }
        public string mPrenoms { get; set; }
        public string mMBTI { get; set; }
 
        public int mNoteCulture { get; set; }
       
        public int mNoteValeur { get; set; }
       
        public int mNoteCulVal { get; set; }
      
        public int mNoteTESTLogique { get; set; }
       
        public int mNoteTESTResolutionProblemes { get; set; }
       
        public int mNoteTESTAttentionAuxDetails { get; set; }
       
        public int mNoteTESTPenseeCritique { get; set; }
       
        public int mPretentionSalariale { get; set; }
      
        public string mDisponibilite { get; set; }
        public string mTel1 { get; set; }
        public string mEmail { get; set; }

        public int mIsDelete { get; set; }//1 = oui / 0=NON C'est pour savoir si c'est "supprimé" ou pas
        
        public int mIsMailEnvoyeOK { get; set; }//0: Non envoyé et 1: Refus et 2:POSITIF EN ATTENTE et 3:POSITIF A CONVOQUER POUR TEST TECHNIQUE

        public string mCommentaires { get; set; }

        public string mLibelleIdPoste { get; set; }

        

        public CTESTOKMAIL()
        {
            mIdCandidat = 0;
            mDateCreationCV = new DateTime();
            mNom = string.Empty;
            mPrenoms = string.Empty;
            mMBTI = string.Empty;

            mNoteCulture = 0;
            mNoteValeur = 0;
            mNoteTESTLogique = 0;
            mNoteTESTResolutionProblemes = 0;
            mNoteTESTAttentionAuxDetails = 0;
            mNoteTESTPenseeCritique = 0;

            mNoteCulVal = 0;
            
            mPretentionSalariale = 0;
         
            mDisponibilite = string.Empty;
            mTel1 = string.Empty;
            mEmail = string.Empty;

            mIsDelete = 0;
            
            mIsMailEnvoyeOK = 0;

            mCommentaires = string.Empty;

            mLibelleIdPoste = string.Empty;

        }




    }
}
