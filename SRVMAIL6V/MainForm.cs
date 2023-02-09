using SRVMAIL6V.DAO;
using SRVMAIL6V.Models;
using SRVMAIL6V.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRVMAIL6V
{
    public partial class MainForm : Form
    {
       // string Chaine = @"Initial Catalog=AITSOFTWARE;Data Source=FRANCK\SAGE300;Integrated Security=SSPI";
          private string Chaine = @"Initial Catalog=AITSOFTWARE;Data Source=NATSQL02\SAGE100C;user=SA;password=$AGE100";


        public List<CTESTOKMAIL> ListeMail;

        private readonly DAOASS mDao = new DAOASS();
        public MainForm()
        {
            InitializeComponent();
        }


        private void EndormirProgramme()
        {
            try
            {
                TimeSpan interval = new TimeSpan(0, 0, 50);
                Thread.Sleep(interval);
               
            }
            catch(Exception ex)
            {

            }
        }




        private void MainForm_Load(object sender, EventArgs e)
        {
            ListeMail = new List<CTESTOKMAIL>();

            bool isSendOK = false;

            string msgOK = string.Empty;

            string msgNOK = string.Empty;

            try
            {
                #region Paramètres Email Envoi

                var   MyCParams = new CParams();

                // MyCParams.mEmail = " renouvellementSoft@aitek.fr";
              //  MyCParams.mEmail = "recrutement@aitek.fr";
                MyCParams.mEmail = "recrutementci@aitek.fr";

                //  MyCParams.mSmtp = "outlook.office365.com";
                MyCParams.mSmtp = "smtp-legacy.office365.com";
                //MyCParams.mSmtpPassword = "2017Aitek";
              //  MyCParams.mSmtpPassword = "2022Aitek!!";
                MyCParams.mSmtpPassword = "2022Aitek@";
                
                MyCParams.mPort = 587;

                #endregion
                
                //Ramener la liste des tests qui ont 3 jours en plus sur leur composition==============

                //Critères: avoir négatif sur les 4 tests ou CV<50% et sans commentaires pour préciser 

                ListeMail = mDao.getTESTOK(Chaine).Where(c=>c.mCommentaires==string.Empty || c.mCommentaires==null).ToList().Where(p=>p.mDateCreationCV<=DateTime.Now.Date.AddDays(-3)).ToList();
                
                if(ListeMail.Count>0)
                {
                    var ListeCHOOSE = new List<CTESTOKMAIL>();

                    foreach(var elt in ListeMail)
                    {
                        if((elt.mNoteTESTLogique<=0 && elt.mNoteTESTAttentionAuxDetails<=0 && elt.mNoteTESTResolutionProblemes<=0)||(elt.mNoteCulVal<50))
                        {
                            //S'assurer qu'on a pas affaire à un Chauffeur

                            if(elt.mLibelleIdPoste.Trim()!= "37")
                            {
                                if (elt.mLibelleIdPoste.Trim() != "43")
                                {
                                    ListeCHOOSE.Add(elt);
                                }
                              
                            }
                            
                        }
                    }


                    //Si on a des candidats éligibles ,on leur balance le email de refus et on met a jour leur statut d'envoi

                    if(ListeCHOOSE.Count>0)
                    {
                        foreach (var obj in ListeCHOOSE)
                        {
                            isSendOK = false;
                            
                            if (obj.mIdCandidat>0)
                            {
                                if (obj.mIsMailEnvoyeOK == 0 && obj.mMBTI != string.Empty) isSendOK = mDao.sendMailRefus(MyCParams, obj);

                                EndormirProgramme();
                            }


                            if (isSendOK)
                            {
                                //Mettre a jour le statut Email dans TAL_ASS_CVCandidat
                                bool MAJEmail = false;

                                MAJEmail = mDao.updateEnvoiEmailCVCandidatRefus(obj, Chaine);
                                
                                msgOK += obj.mNom + " " + obj.mPrenoms+" TEST passé le : "+obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                            }
                            else
                            {

                                if (obj.mIsMailEnvoyeOK == 0) msgNOK += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString()  + Environment.NewLine;

                                //  MessageBox.Show("Une erreur est survenue lors de l'envoi mail !"+Environment.NewLine+" Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }

                        }

                  
                    //Fin de l'envoi des mails ,on ecrire le recapitulatif et on ferme l'application
                    if (msgNOK!=string.Empty) CLog.Log("L'email de réponse n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOK);
                    if (msgOK!=string.Empty) CLog.Log("L'email de réponse a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOK);

                        //Envoi Mail Notifications pour m'alerter
                        bool isnotificationOK = false;

                        if(msgNOK != string.Empty && msgOK != string.Empty)
                        {
                            string msgnotif = "L'email de réponse a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOK + Environment.NewLine + "L'email de réponse n'a pas été envoyé aux candidats suivants : " + Environment.NewLine+msgNOK;

                            isnotificationOK = mDao.sendMailNotification(MyCParams, msgnotif);
                        }


                        if (msgNOK != string.Empty && msgOK == string.Empty)
                        {

                            isnotificationOK = mDao.sendMailNotification(MyCParams, "L'email de réponse n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOK);
                        }


                        if (msgNOK == string.Empty && msgOK != string.Empty)
                        {

                            isnotificationOK = mDao.sendMailNotification(MyCParams, "L'email de réponse a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOK);
                        }



                    }

                }

                //Fermer l'appli après envoi
                Application.Exit();
                
            }
            catch(Exception ex)
            {

            }
        }
    }
}
