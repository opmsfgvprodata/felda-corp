//--Author:		< Faeza >
//--Create date: < 16.07.2023 >
//--Description:	< Maybank RCMS Gen>
using MVC_SYSTEM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateFile
    {
        private static ChangeTimeZone timezone = new ChangeTimeZone();
        private GetTriager GetTriager = new GetTriager();
        private static GetNSWL GetNSWL = new GetNSWL();
        public static string GenFileMaybank(List<ModelsSP.sp_MaybankRcms_Result> maybankrcmsList, tbl_Wilayah tbl_Wilayah, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? WilayahID, string CompCode, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalGaji = 0;
            int CountData = 0;
            int rowno = 1;
            int SalaryInt = 0;
            int SalaryHash = 0;
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            int AccountHash = 0;
            int TotalHash = 0;
            int SumAllTotalHash = 0;
            int onedigit = 0;
            string statusmsg = "";
            string PaymentCode = "";
            string ResidentIInd = "";           
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            char onechar;
            //DateTime? date = timezone.gettimezone();
            //DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            //DateTime? PaymentDate = new DateTime(Today.Year, Today.Month, 5);
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" + WilayahID.ToString() + "/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            filename = "M2E BURUH (" + InitialName +") " + tbl_Wilayah.fld_WlyhName.ToUpper() + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                    CountData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 28;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else if (i == 2)
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        //else if (i == 4)
                        //{
                        //    Header.Insert(i, "B|");
                        //}
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";
                        string WorkerName2 = "";
                        string WorkerName3 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 40)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }
                        if (WorkerNameLength > 40 && WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, WorkerNameLength - 40);
                        }
                        if (WorkerNameLength > 80 && WorkerNameLength <= 120)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, 40);
                            WorkerName3 = maybankrcms.fld_Nama.Substring(80, WorkerNameLength - 80);
                        }

                        //***SalaryHashing***
                        SalaryInt = (int)(maybankrcms.fld_GajiBersih * 100);
                        SalaryHash = (SalaryInt % 2000) + rowno;

                        //**AccountHashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_NoAkaun;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno; 
                        }
                        else if(AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if(AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2 ) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }

                                //SixDigitAccNoInt = int.Parse(maybankrcms.fld_NoAkaun.Substring(maybankrcms.fld_NoAkaun.Length - 6, 6));
                                //while (SixDigitAccNoInt > 0)
                                //{
                                //    reminder = SixDigitAccNoInt % 10;
                                //    AccountHash = AccountHash + reminder;
                                //    SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                //}
                                //AccountHash = (AccountHash * 2) + rowno;
                            }
                        }                       

                        //**TotalHash***
                        TotalHash = SalaryHash + AccountHash;
                        SumAllTotalHash = SumAllTotalHash + TotalHash;

                        //start write body
                        int BodyLoop = 337;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 1) //2
                            {
                                PaymentCode = maybankrcms.fld_RcmsBankCode == "MBBEMYKL" ? "IT" : "IG";
                                Body.Insert(i, PaymentCode + "|");
                            }
                            else if (i == 2) //3
                            {
                                Body.Insert(i, "Domestic Payments (MY)|");
                            }
                            else if (i == 4) //5
                            {
                                Body.Insert(i, string.Format("{0:ddMMyyyy}", PaymentDateFormat) + "|");
                            }
                            else if (i == 7) //8
                            {
                                Body.Insert(i, maybankrcms.fld_Nokp.ToUpper() + "|");
                            }
                            else if (i == 8) //9
                            {
                                Body.Insert(i, "MAPA " + bulan + tahun + "|");
                                //Body.Insert(i, "MAPA " + bulan + tahun.Substring(2, 2) + "|");
                            }
                            else if (i == 9) //10
                            {
                                Body.Insert(i, maybankrcms.fld_LdgName + "|");
                            }
                            else if (i == 10) //11
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 11) //12
                            {
                                Body.Insert(i, maybankrcms.fld_GajiBersih + "|");
                            }
                            else if (i == 12) //13
                            {
                                Body.Insert(i, "Y|");
                            }
                            else if (i == 13) //14
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 14) //15
                            {
                                Body.Insert(i, AccNo + "|");
                            }
                            else if (i == 15) //16
                            {
                                Body.Insert(i, maybankrcms.fld_NoAkaun + "|");
                            }
                            else if (i == 18) //19
                            {
                                ResidentIInd = maybankrcms.fld_Kdrkyt == "MA" ? "Y" : "N";
                                Body.Insert(i, ResidentIInd + "|");
                            }
                            else if (i == 19) //20
                            {
                                if (WorkerName1 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName1 + "|");
                                }
                            }
                            else if (i == 20) //21
                            {
                                if (WorkerName2 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName2 + "|");
                                }
                            }
                            else if (i == 21) //22
                            {
                                if (WorkerName3 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName3 + "|");
                                }
                            }
                            else if (i == 24) //25
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 27) //28
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                            }
                            else if (i == 36) //37
                            {
                                Body.Insert(i, maybankrcms.fld_RcmsBankCode + "|");
                            }
                            else if (i == 102) //103
                            {
                                if (CompCode == "1000")
                                {
                                    Body.Insert(i, "FELDA " + tbl_Wilayah.fld_WlyhName.ToUpper() + "|");
                                }
                                else if (CompCode == "8800")
                                {
                                    Body.Insert(i, "FPMSB " + tbl_Wilayah.fld_WlyhName.ToUpper() + "|");
                                }
                            }
                            else if (i == 109) //110
                            {
                                Body.Insert(i, "01|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, CountData + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalGaji + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public static string GenFileTNG(List<ModelsSP.sp_MaybankTNG_Result> maybankrcmsList, tbl_Wilayah tbl_Wilayah, string bulan, string tahun, int? NegaraID, int? SyarikatID, int? WilayahID, string CompCode, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalGaji = 0;
            int CountData = 0;
            int rowno = 1;
            int SalaryInt = 0;
            int SalaryHash = 0;
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            int AccountHash = 0;
            int TotalHash = 0;
            int SumAllTotalHash = 0;
            int onedigit = 0;
            string statusmsg = "";
            string PaymentCode = "";
            string ResidentIInd = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            char onechar;
            //DateTime? date = timezone.gettimezone();
            //DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            //DateTime? PaymentDate = new DateTime(Today.Year, Today.Month, 5);
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" + WilayahID.ToString() + "/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            filename = "TNG BURUH (" + InitialName + ") " + tbl_Wilayah.fld_WlyhName.ToUpper() + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                    CountData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 28;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else if (i == 2)
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }
                        }
                        //else if (i == 4)
                        //{
                        //    Header.Insert(i, "B|");
                        //}
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";
                        string WorkerName2 = "";
                        string WorkerName3 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 40)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }
                        if (WorkerNameLength > 40 && WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, WorkerNameLength - 40);
                        }
                        if (WorkerNameLength > 80 && WorkerNameLength <= 120)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, 40);
                            WorkerName3 = maybankrcms.fld_Nama.Substring(80, WorkerNameLength - 80);
                        }

                        //***SalaryHashing***
                        SalaryInt = (int)(maybankrcms.fld_GajiBersih * 100);
                        SalaryHash = (SalaryInt % 2000) + rowno;

                        //**AccountHashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_NoAkaun;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }

                                //SixDigitAccNoInt = int.Parse(maybankrcms.fld_NoAkaun.Substring(maybankrcms.fld_NoAkaun.Length - 6, 6));
                                //while (SixDigitAccNoInt > 0)
                                //{
                                //    reminder = SixDigitAccNoInt % 10;
                                //    AccountHash = AccountHash + reminder;
                                //    SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                //}
                                //AccountHash = (AccountHash * 2) + rowno;
                            }
                        }

                        //**TotalHash***
                        TotalHash = SalaryHash + AccountHash;
                        SumAllTotalHash = SumAllTotalHash + TotalHash;

                        //start write body
                        int BodyLoop = 337;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 1) //2
                            {
                                //PaymentCode = maybankrcms.fld_RcmsBankCode == "MBBEMYKL" ? "IT" : "IG";
                                PaymentCode = "CN";
                                Body.Insert(i, PaymentCode + "|");
                            }
                            else if (i == 2) //3
                            {
                                Body.Insert(i, "Domestic Payments (MY)|");
                            }
                            else if (i == 4) //5
                            {
                                Body.Insert(i, string.Format("{0:ddMMyyyy}", PaymentDateFormat) + "|");
                            }
                            else if (i == 7) //8
                            {
                                Body.Insert(i, maybankrcms.fld_Nokp.ToUpper() + "|");
                            }
                            else if (i == 8) //9
                            {
                                Body.Insert(i, "MAPA " + bulan + tahun + "|");
                                //Body.Insert(i, "MAPA " + bulan + tahun.Substring(2, 2) + "|");
                            }
                            else if (i == 9) //10
                            {
                                Body.Insert(i, maybankrcms.fld_LdgName + "|");
                            }
                            else if (i == 10) //11
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 11) //12
                            {
                                Body.Insert(i, maybankrcms.fld_GajiBersih + "|");
                            }
                            else if (i == 12) //13
                            {
                                Body.Insert(i, "Y|");
                            }
                            else if (i == 13) //14
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 14) //15
                            {
                                Body.Insert(i, AccNo + "|");
                            }
                            else if (i == 15) //16
                            {
                                Body.Insert(i, maybankrcms.fld_NoAkaun + "|");
                            }
                            else if (i == 18) //19
                            {
                                ResidentIInd = maybankrcms.fld_Kdrkyt == "MA" ? "Y" : "N";
                                Body.Insert(i, ResidentIInd + "|");
                            }
                            else if (i == 19) //20
                            {
                                if (WorkerName1 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName1 + "|");
                                }
                            }
                            else if (i == 20) //21
                            {
                                if (WorkerName2 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName2 + "|");
                                }
                            }
                            else if (i == 21) //22
                            {
                                if (WorkerName3 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName3 + "|");
                                }
                            }
                            else if (i == 24) //25
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                                else
                                {
                                    Body.Insert(i, "|");
                                }
                            }
                            else if (i == 27) //28
                            {
                                if (maybankrcms.fld_Kdrkyt == "MA")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, maybankrcms.fld_Nokp + "|");
                                }
                            }
                            else if (i == 36) //37
                            {
                                Body.Insert(i, maybankrcms.fld_RcmsBankCode + "|");
                            }
                            else if (i == 102) //103
                            {
                                if (CompCode == "1000")
                                {
                                    Body.Insert(i, "FELDA " + tbl_Wilayah.fld_WlyhName.ToUpper() + "|");
                                }
                                else if (CompCode == "8800")
                                {
                                    Body.Insert(i, "FPMSB " + tbl_Wilayah.fld_WlyhName.ToUpper() + "|");
                                }
                            }
                            else if (i == 109) //110
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 137) //138
                            {
                                ResidentIInd = maybankrcms.fld_Kdrkyt == "MA" ? "Y" : "N";
                                Body.Insert(i, ResidentIInd + "|");
                            }
                            else if (i == 138) //139
                            {
                                Body.Insert(i, maybankrcms.fld_Kdrkytbank + "|");
                            }
                            else if (i == 139) //140
                            {
                                Body.Insert(i, "02|");
                            }
                            else if (i == 187) //188
                            {
                                Body.Insert(i, "FT|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, CountData + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalGaji + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}