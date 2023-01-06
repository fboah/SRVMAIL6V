using SRVMAIL6V.Models;
using SRVMAIL6V.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRVMAIL6V.DAO
{
  public  class DAOASS
    {
        private IDbConnection mConnection;
        private readonly DbProviderFactory mProvider = DbProviderFactories.GetFactory("System.Data.SqlClient");

        private string Appli = "SRVMAIL6V";
        
        public List<CTESTOKMAIL>getTESTOK(string Chaineconnex)
         {
            var listPays = new List<CTESTOKMAIL>();

            string FiltrePoste = string.Empty;

            string FiltreStatut = string.Empty;
            

            using (mConnection = mProvider.CreateConnection())
            {
                if (mConnection == null) return listPays;
                mConnection.ConnectionString = Chaineconnex;
                mConnection.Open();

                using (var command = mConnection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = @"select C.Id as IdCandidat, C.DateCreationCV as DateCV,C.Nom,C.Prenoms,TM.NoteTESTMBTI as MBTI
                                                ,TC.NoteTESTC as Culture,
                                                TV.NoteTESTV as Valeur,L.NoteTESTLogique,R.NoteTESTResolutionProblemes,D.NoteTESTAttentionAuxDetails,CR.NoteTESTPenseeCritique,
                                                C.PretentionSalariale,C.Disponibilite,C.Tel1,C.Email,C.IsFinishTEST,C.IsMailEnvoyeOK,C.Commentaires,C.LibelleIdPoste
                                                from TAL_ASS_TESTLogique L
                                                left join TAL_ASS_CVCandidat C on L.IdCandidat=C.Id
                                                left join TAL_ASS_TESTPenseeCritique CR on CR.IdCandidat=C.Id
                                                left join TAL_ASS_TESTAttentionAuxDetails D on D.IdCandidat=C.Id
                                                left join TAL_ASS_TESTResolutionProblemes R on R.IdCandidat=C.Id
                                                left join TAL_ASS_TESTC TC on TC.IdCandidat=C.Id
                                                left join TAL_ASS_TESTV TV on TV.IdCandidat=C.Id
                                                left join TAL_ASS_TESTMBTI TM on TM.IdCandidat=C.Id

                                                where C.IsDelete=0 AND C.IsFinishTEST =1 and C.IsMailEnvoyeOK=0 ";

                        //command.Parameters.Add(new SqlParameter("dateDebutGen", dateDebutGen));
                        //command.Parameters.Add(new SqlParameter("dateFinGen", dateFinGen));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var pays = new CTESTOKMAIL
                                {
                                    mIdCandidat = reader["IdCandidat"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IdCandidat"]),

                                    mDateCreationCV = reader["DateCV"] == DBNull.Value ? new DateTime() : DateTime.Parse(reader["DateCV"].ToString()),

                                    mNom = reader["Nom"] == DBNull.Value ? string.Empty : reader["Nom"] as string,
                                    mPrenoms = reader["Prenoms"] == DBNull.Value ? string.Empty : reader["Prenoms"] as string,
                                  // mAutresPostes = reader["AutresPostes"] == DBNull.Value ? string.Empty : reader["AutresPostes"] as string,
                                    mMBTI = reader["MBTI"] == DBNull.Value ? string.Empty : reader["MBTI"] as string,
                                  //  mLibelleIdPoste = reader["LibelleIdPoste"] == DBNull.Value ? string.Empty : reader["LibelleIdPoste"] as string,
                                    mNoteCulture = reader["Culture"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Culture"]),
                                    mNoteValeur = reader["Valeur"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Valeur"]),
                                    mNoteTESTLogique = reader["NoteTESTLogique"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NoteTESTLogique"]),
                                    mNoteTESTPenseeCritique = reader["NoteTESTPenseeCritique"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NoteTESTPenseeCritique"]),
                                    mNoteTESTAttentionAuxDetails = reader["NoteTESTAttentionAuxDetails"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NoteTESTAttentionAuxDetails"]),
                                    mNoteTESTResolutionProblemes = reader["NoteTESTResolutionProblemes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["NoteTESTResolutionProblemes"]),
                                    mDisponibilite = reader["Disponibilite"] == DBNull.Value ? string.Empty : reader["Disponibilite"] as string,
                                    mTel1 = reader["Tel1"] == DBNull.Value ? string.Empty : reader["Tel1"] as string,
                                    mEmail = reader["Email"] == DBNull.Value ? string.Empty : reader["Email"] as string,

                                    mPretentionSalariale = reader["PretentionSalariale"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PretentionSalariale"]),
                                    //mIsDelete = reader["IsDelete"] == DBNull.Value ? 0 : Convert.ToInt16(reader["IsDelete"]),
                                   // mStatut = reader["IsFinishTEST"] == DBNull.Value ? 0 : Convert.ToInt16(reader["IsFinishTEST"]),
                                    mIsMailEnvoyeOK = reader["IsMailEnvoyeOK"] == DBNull.Value ? 0 : Convert.ToInt16(reader["IsMailEnvoyeOK"]),

                                    mCommentaires = reader["Commentaires"] == DBNull.Value ? string.Empty : reader["Commentaires"] as string,
                                    mLibelleIdPoste = reader["LibelleIdPoste"] == DBNull.Value ? string.Empty : reader["LibelleIdPoste"] as string,

                                };

                                
                                //Note Culture+Valeur

                                //On va diviser par 12 pour ne pas léser ceux qui ont passere le test avec cette erreur
                                int ret = 0;

                                double Arrond = 0;

                                int notefinale = 0;

                                notefinale = pays.mNoteCulture + pays.mNoteValeur;

                                double res = (double)(notefinale) / 2;
                                //double res = (double)(ret * 100) / 13;

                                Arrond = Math.Round(res, 2);

                                double som = 0;

                                if (notefinale > 0)
                                {
                                    som = 0.5;
                                }
                                else
                                {
                                    som = -0.5;
                                }

                                pays.mNoteCulVal = (int)(Arrond + som);
                                
                                //Tester si le candidat n'existe pas déjà

                                var isExist = listPays.Exists(c => c.mIdCandidat == pays.mIdCandidat);

                                if (!isExist) listPays.Add(pays);
                                
                            }
                            return listPays;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur est survenue! Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        var msg = "DAOASS -> getTESTOK-> TypeErreur: " + ex.Message;
                      //CLog.Log(msg);
                        return listPays;
                    }
                    finally
                    {
                        mConnection.Close();
                    }
                }
            }
        }

        #region EMail

        public string GetStatutMBTI(string mbti)
        {
            string ret = string.Empty;

            try
            {
                if (mbti.ToUpper().Trim().Equals("INTJ"))
                {
                    ret = "INTJ – ARCHITECTE";
                }

                if (mbti.ToUpper().Trim().Equals("INTP"))
                {
                    ret = "INTP – LOGICIEN";
                }

                if (mbti.ToUpper().Trim().Equals("ENTJ"))
                {
                    ret = "ENTJ – COMMANDANT";
                }

                if (mbti.ToUpper().Trim().Equals("ENTP"))
                {
                    ret = "ENTP – INNOVATEUR";
                }

                if (mbti.ToUpper().Trim().Equals("INFJ"))
                {
                    ret = "INFJ – AVOCAT";
                }

                if (mbti.ToUpper().Trim().Equals("INFP"))
                {
                    ret = "INFP – MEDIATEUR";
                }


                if (mbti.ToUpper().Trim().Equals("ENFJ"))
                {
                    ret = "ENFJ – PROTAGONISTE";
                }


                if (mbti.ToUpper().Trim().Equals("ENFP"))
                {
                    ret = "ENFP – INSPIRATEUR";
                }

                if (mbti.ToUpper().Trim().Equals("ISTJ"))
                {
                    ret = "ISTJ – LOGISTICIEN";
                }

                if (mbti.ToUpper().Trim().Equals("ISFJ"))
                {
                    ret = "ISFJ – DEFENSEUR";
                }

                if (mbti.ToUpper().Trim().Equals("ESTJ"))
                {
                    ret = "ESTJ – DIRECTEUR";
                }

                if (mbti.ToUpper().Trim().Equals("ESFJ"))
                {
                    ret = "ESFJ – CONSUL";
                }

                if (mbti.ToUpper().Trim().Equals("ISTP"))
                {
                    ret = "ISTP – VIRTUOSE";
                }

                if (mbti.ToUpper().Trim().Equals("ISFP"))
                {
                    ret = "ISFP – AVENTURIER";
                }

                if (mbti.ToUpper().Trim().Equals("ESTP"))
                {
                    ret = "ESTP – ENTREPRENEUR";
                }

                if (mbti.ToUpper().Trim().Equals("ESFP"))
                {
                    ret = "ESFP – AMUSEUR";
                }


                return ret;
            }
            catch (Exception ex)
            {
                return ret;
            }
        }
        
        //Refus===============================================================
        public string getEmailBodyRefus(CTESTOKMAIL LCINF)
        {
            string ret = string.Empty;

            try
            {
                ret = @" " +
                     "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                     "<head>" +
                     "<title>Email</title>" +
                     "</head>" +
                    "<body style=\"font-family:'Calibri'\">" +

                     " <p> Cher " + LCINF.mNom + " " + LCINF.mPrenoms + " , </p>" +
                     "<p>Vous avez participé aux sessions d’évaluation d’AITEK le " + LCINF.mDateCreationCV.ToShortDateString() + " et nous vous remercions de l’intérêt porté à notre structure.  </p>" +
                     "<p>Nous n’avons actuellement pas de poste disponible correspondant à votre profil mais ne manquerons pas de vous contacter en cas d’opportunité susceptible de vous intéresser.</p>" +
                     "<p>Dans l’attente votre profil MBTI est  " + GetStatutMBTI(LCINF.mMBTI) + " dont vous trouverez un descriptif en cliquant sur le lien suivant : https://www.16personalities.com/fr/la-personnalite-" + LCINF.mMBTI + ".  </p>" +
                     "<p>Cordialement,</p>" +
                     "<p>Team RH AITEK</p>";


                return ret;
            }
            catch (Exception ex)
            {
                return ret;
            }
        }

        public bool sendMailRefus(CParams ParamEmail, CTESTOKMAIL CAFS)
        {
            bool ret = false;
            try
            {
                if (ParamEmail.mEmail != string.Empty && ParamEmail.mSmtp != string.Empty && ParamEmail.mSmtpPassword != string.Empty && ParamEmail.mPort > 0)
                {
                    //Si on a pas de mail renseigné,on n'envoie pas de mail

                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(ParamEmail.mEmail),
                        Subject = "Vos résultats aux Sessions d'Evaluation AITEK",
                        Body = getEmailBodyRefus(CAFS),
                        IsBodyHtml = true

                    };

                    //Destinataire
                    mail.To.Add(CAFS.mEmail);


                    var smtpClient = new SmtpClient
                    {
                        Host = ParamEmail.mSmtp,
                        Port = ParamEmail.mPort,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(ParamEmail.mEmail, ParamEmail.mSmtpPassword),
                        EnableSsl = true,

                    };

                    smtpClient.Send(mail);

                    ret = true;

                }

                return ret;
            }
            catch (Exception ex)
            {
                var msg = "ServiceAITASS -> sendMailRefus->TypeErreur: " + ex.Message;
                CLog.Log(msg);

                ret = false;

                return ret;
            }

        }


        public bool sendMailNotification(CParams ParamEmail, string msgmail)
        {
            bool ret = false;
            try
            {
                if (ParamEmail.mEmail != string.Empty && ParamEmail.mSmtp != string.Empty && ParamEmail.mSmtpPassword != string.Empty && ParamEmail.mPort > 0)
                {
                    //Si on a pas de mail renseigné,on n'envoie pas de mail

                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(ParamEmail.mEmail),
                        Subject = "Notification Envoi Email REFUS",
                        Body = msgmail,
                        IsBodyHtml = true

                    };

                    //Destinataire
                    mail.To.Add("franck.boah@aitek.fr");


                    var smtpClient = new SmtpClient
                    {
                        Host = ParamEmail.mSmtp,
                        Port = ParamEmail.mPort,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(ParamEmail.mEmail, ParamEmail.mSmtpPassword),
                        EnableSsl = true,

                    };

                    smtpClient.Send(mail);

                    ret = true;

                }

                return ret;
            }
            catch (Exception ex)
            {
                var msg = "ServiceAITASS -> sendMailNotification->TypeErreur: " + ex.Message;
                CLog.Log(msg);

                ret = false;

                return ret;
            }

        }


        #endregion


        public bool updateEnvoiEmailCVCandidatRefus(CTESTOKMAIL client, string chaineconx)
        {
            bool res = false;

            using (mConnection = mProvider.CreateConnection())
            {
                mConnection.ConnectionString = chaineconx;
                mConnection.Open();
                using (var command = mConnection.CreateCommand())
                {
                    try
                    {
                        command.CommandText = @"UPDATE TAL_ASS_CVCandidat SET                      
                       IsMailEnvoyeOK=1 WHERE Id = @Id ";

                        command.Parameters.Add(new SqlParameter("Id", client.mIdCandidat));

                        command.ExecuteNonQuery();
                        res = true;

                        return res;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Une erreur est survenue! Veuillez contacter votre Administrateur!", Appli, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        var msg = "DAOASS -> updateIsFinishTESTOKCVCandidat-> TypeErreur: " + ex.Message;
                        CLog.Log(msg);
                        return res;
                    }
                    finally
                    {
                        mConnection.Close();
                    }
                }
            }
        }

    }
}
