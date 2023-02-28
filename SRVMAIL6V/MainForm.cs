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


        //Liste mail Recrutement du jour
        public List<CMailRecrutement> ListeMailRecrutementDuJour;

        //Liste mail Recrutement du jour
        public List<CMailRecrutement> ListeMailEpurationRecrutement;

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

            bool isSendRecrutementDailyOK = false;
            bool isSendRecrutementEpurationOK = false;


            string msgOK = string.Empty;

            string msgNOK = string.Empty;

            string msgOKRecrutementJour = string.Empty;

            string msgNOKRecrutementJour = string.Empty;

            string msgOKRecrutement = string.Empty;

            string msgNOKRecrutement = string.Empty;

            try
            {
               
                #region Paramètres Email Envoi

                var MyCParams = new CParams();

                MyCParams = mDao.GetParamEmail(Chaine);


                //  // MyCParams.mEmail = " renouvellementSoft@aitek.fr";
                ////  MyCParams.mEmail = "recrutement@aitek.fr";
                //  MyCParams.mEmail = "recrutementci@aitek.fr";

                //  //  MyCParams.mSmtp = "outlook.office365.com";
                //  MyCParams.mSmtp = "smtp-legacy.office365.com";
                //  //MyCParams.mSmtpPassword = "2017Aitek";
                ////  MyCParams.mSmtpPassword = "2022Aitek!!";
                //  MyCParams.mSmtpPassword = "2022Aitek@";

                //  MyCParams.mPort = 587;

                #endregion

                //Ramener la liste des tests qui ont 3 jours en plus sur leur composition==============

                //Critères: avoir négatif sur les 4 tests ou CV<50% et sans commentaires pour préciser 

                ListeMail = mDao.getTESTOK(Chaine).Where(c => c.mCommentaires == string.Empty || c.mCommentaires == null).ToList().Where(p => p.mDateCreationCV <= DateTime.Now.Date.AddDays(-3)).ToList();

                #region MailRefus

                if (ListeMail.Count > 0)
                {
                    var ListeCHOOSE = new List<CTESTOKMAIL>();

                    foreach (var elt in ListeMail)
                    {
                        if ((elt.mNoteTESTLogique <= 0 && elt.mNoteTESTAttentionAuxDetails <= 0 && elt.mNoteTESTResolutionProblemes <= 0) || (elt.mNoteCulVal < 50))
                        {
                            //S'assurer qu'on a pas affaire à un Chauffeur

                            if (elt.mLibelleIdPoste.Trim() != "37")
                            {
                                if (elt.mLibelleIdPoste.Trim() != "43")
                                {
                                    ListeCHOOSE.Add(elt);
                                }

                            }

                        }
                    }


                    //Si on a des candidats éligibles ,on leur balance le email de refus et on met a jour leur statut d'envoi

                    if (ListeCHOOSE.Count > 0)
                    {
                        foreach (var obj in ListeCHOOSE)
                        {
                            isSendOK = false;

                            if (obj.mIdCandidat > 0)
                            {
                                if (obj.mIsMailEnvoyeOK == 0 && obj.mMBTI != string.Empty) isSendOK = mDao.sendMailRefus(MyCParams, obj);

                                EndormirProgramme();
                            }


                            if (isSendOK)
                            {
                                //Mettre a jour le statut Email dans TAL_ASS_CVCandidat
                                bool MAJEmail = false;

                                MAJEmail = mDao.updateEnvoiEmailCVCandidatRefus(obj, Chaine);

                                msgOK += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                            }
                            else
                            {

                                if (obj.mIsMailEnvoyeOK == 0) msgNOK += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                                //  MessageBox.Show("Une erreur est survenue lors de l'envoi mail !"+Environment.NewLine+" Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }

                        }


                        //Fin de l'envoi des mails ,on ecrire le recapitulatif et on ferme l'application
                        if (msgNOK != string.Empty) CLog.Log("L'email automatique de refus n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOK);
                        if (msgOK != string.Empty) CLog.Log("L'email automatique de refus a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOK);

                    }

                }

                #endregion


                ListeMailRecrutementDuJour = mDao.getMailRecrutementDaily(Chaine).Where(c => c.mCommentaires == string.Empty || c.mCommentaires == null).ToList();

                #region Mail Recrutement du jour

                if (ListeMailRecrutementDuJour.Count > 0)
                {

                    //Si on a des candidats éligibles ,on leur balance le email de recrutement du jour et on met a jour leur statut d'envoi

                    foreach (var obj in ListeMailRecrutementDuJour)
                    {
                        isSendRecrutementDailyOK = false;

                        if (obj.mIdCandidat > 0)
                        {
                            if (obj.mIsMailEnvoyeOK == 0 && obj.mIsMailRecrutement == 0)
                            {
                                if (obj.mNoteTESTMBTI != string.Empty && obj.mNoteTESTMBTI != null)
                                {
                                    isSendRecrutementDailyOK = mDao.sendMailRecrutementDuJourAvecMBTI(MyCParams, obj);
                                }
                                else
                                {
                                    isSendRecrutementDailyOK = mDao.sendMailRecrutementDuJourSansMBTI(MyCParams, obj);
                                }
                            }

                            EndormirProgramme();
                        }


                        if (isSendRecrutementDailyOK)
                        {
                            //Mettre a jour le statut mail Recrutement dans TAL_ASS_CVCandidat
                            bool MAJEmail = false;

                            MAJEmail = mDao.updateEnvoiEmailCVCandidatMailRecrutement(obj, Chaine);

                            msgOKRecrutementJour += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                        }
                        else
                        {

                            if (obj.mIsMailRecrutement == 0) msgNOKRecrutementJour += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                            //  MessageBox.Show("Une erreur est survenue lors de l'envoi mail !"+Environment.NewLine+" Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }

                    }


                    //Fin de l'envoi des mails ,on ecrire le recapitulatif et on ferme l'application
                    if (msgNOKRecrutementJour != string.Empty) CLog.Log("L'email automatique de recrutement du jour n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOKRecrutementJour);
                    if (msgOKRecrutementJour != string.Empty) CLog.Log("L'email automatique de recrutement du jour a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOKRecrutementJour);



                }


                #endregion


                //La liste des candidats à épurer depuis le 1er février jusqu'a la veille de l'implementation du programme 
                ListeMailEpurationRecrutement = mDao.getMailRecrutementEpuration(Chaine).Where(c => c.mCommentaires == string.Empty || c.mCommentaires == null).ToList();

                #region Mail Recrutement à partir du 01/02/2023 jusqua la veille de l'implémentation


                if (ListeMailEpurationRecrutement.Count > 0)
                {

                    //Si on a des candidats éligibles ,on leur balance le email de recrutement du jour et on met a jour leur statut d'envoi

                    foreach (var obj in ListeMailEpurationRecrutement)
                    {
                        isSendRecrutementEpurationOK = false;

                        if (obj.mIdCandidat > 0)
                        {
                            if (obj.mIsMailEnvoyeOK == 0 && obj.mIsMailRecrutement == 0)
                            {
                                if (obj.mNoteTESTMBTI != string.Empty && obj.mNoteTESTMBTI != null)
                                {
                                    isSendRecrutementEpurationOK = mDao.sendMailRecrutementDuJourAvecMBTI(MyCParams, obj);
                                }
                                else
                                {
                                    isSendRecrutementEpurationOK = mDao.sendMailRecrutementDuJourSansMBTI(MyCParams, obj);
                                }
                            }

                            EndormirProgramme();
                        }


                        if (isSendRecrutementEpurationOK)
                        {
                            //Mettre a jour le statut mail Recrutement dans TAL_ASS_CVCandidat
                            bool MAJEmail = false;

                            MAJEmail = mDao.updateEnvoiEmailCVCandidatMailRecrutement(obj, Chaine);

                            msgOKRecrutement += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                        }
                        else
                        {

                            if (obj.mIsMailRecrutement == 0) msgNOKRecrutement += obj.mNom + " " + obj.mPrenoms + " TEST passé le : " + obj.mDateCreationCV.Date.ToShortDateString() + Environment.NewLine;

                            //  MessageBox.Show("Une erreur est survenue lors de l'envoi mail !"+Environment.NewLine+" Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }

                    }


                    //Fin de l'envoi des mails ,on ecrire le recapitulatif et on ferme l'application
                    if (msgNOKRecrutement != string.Empty) CLog.Log("L'email automatique de recrutement  n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOKRecrutement);
                    if (msgOKRecrutement != string.Empty) CLog.Log("L'email automatique de recrutement  a été bien envoyé aux candidats suivants : " + Environment.NewLine + msgOKRecrutement);
                    
                }



                #endregion

                

                #region Notification Aurelia BANCHI pour d'éventuelles modifications

                //Envoi Mail Notifications pour m'alerter
                bool isnotificationOK = false;

                string msgNOKALLRecrutement = string.Empty;

                msgNOKALLRecrutement = msgNOKRecrutementJour + Environment.NewLine + msgNOKRecrutement;

                if (msgNOK != string.Empty && msgNOKALLRecrutement.Trim() != string.Empty)
                {
                    string msgnotif = "L'email automatique de refus n'a été envoyé aux candidats suivants : " + Environment.NewLine + msgNOK + Environment.NewLine + "L'email automatique de recrutement n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOKALLRecrutement;
                  
                    isnotificationOK = mDao.sendMailNotification(MyCParams, msgnotif);
                }


                if (msgNOK != string.Empty && msgNOKALLRecrutement.Trim() == string.Empty)
                {

                    isnotificationOK = mDao.sendMailNotification(MyCParams, "L'email automatique de refus n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOK);
                }


                if (msgNOK == string.Empty && msgNOKALLRecrutement.Trim() != string.Empty)
                {

                    isnotificationOK = mDao.sendMailNotification(MyCParams, "L'email automatique de recrutement n'a pas été envoyé aux candidats suivants : " + Environment.NewLine + msgNOKALLRecrutement);
                }

                #endregion

                

                //Fermer l'appli après envoi
                Application.Exit();

            }
            catch (Exception ex)
            {

            }
        }
    }
}
