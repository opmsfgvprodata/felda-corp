using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using MVC_SYSTEM.SAPPostIntegration;

namespace MVC_SYSTEM.Controllers
{
    public class SAPPostingController : Controller
    {
        // GET: SAPPosting
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string SAPUsername, string SAPPassword, int Month, int Year)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            NetworkCredential Cred = new NetworkCredential();
            BAPIACHE09 InputDataDocHeader = new BAPIACHE09();
            BAPIACPA09 InputDataCustPD = new BAPIACPA09();
            BAPIACGL09 InputDataAccGL_ = new BAPIACGL09();
            BAPIACAP09 InputDataAccPay_ = new BAPIACAP09();
            BAPIACTX09 InputDataAccTax_ = new BAPIACTX09();
            BAPIACCR09 InputDataCurAmt_ = new BAPIACCR09();
            BAPIACCR09 InputDataCurAmt2_ = new BAPIACCR09();
            BAPIRET2 OutputReturn_ = new BAPIRET2();

            EndpointAddress endpoint = new EndpointAddress("http://cifld.felhqr.myfelda:8000/sap/bc/srt/rfc/sap/zwsopmsfiar01/210/zwsopmsfiar01/zwsopmsfiar01");
            ZFMOPMSFIAR01Response SAPPostingResponse = new ZFMOPMSFIAR01Response();
            zwsopmsfiar01Client SAPPosting = new zwsopmsfiar01Client(binding, endpoint);
            ZFMOPMSFIAR01 SAPPostingCollectionData = new ZFMOPMSFIAR01();
            int i = 0;
            try
            {
                Cred.UserName = SAPUsername;
                Cred.Password = SAPPassword;
                SAPPosting.ClientCredentials.UserName.UserName = Cred.UserName;
                SAPPosting.ClientCredentials.UserName.Password = Cred.Password;
                SAPPosting.Open();

                InputDataDocHeader.USERNAME = SAPUsername;
                InputDataDocHeader.HEADER_TXT = "OPMS 1";
                InputDataDocHeader.COMP_CODE = "1000";
                InputDataDocHeader.DOC_DATE = "2018-07-28";
                InputDataDocHeader.PSTNG_DATE = "2018-07-28";
                InputDataDocHeader.DOC_TYPE = "KR";
                InputDataDocHeader.REF_DOC_NO = "REFOPMSHAHA";

                InputDataCustPD.NAME = "SHAH";
                InputDataCustPD.NAME_2 = "";
                InputDataCustPD.POSTL_CODE = "50888";
                InputDataCustPD.CITY = "KUALA LUMPUR";
                InputDataCustPD.COUNTRY = "MY";
                InputDataCustPD.STREET = "KLCC";

                InputDataAccGL_.ITEMNO_ACC = "0000000002";
                InputDataAccGL_.GL_ACCOUNT = "0076510010";
                InputDataAccGL_.ITEM_TEXT = "GL 1";
                InputDataAccGL_.TAX_CODE = "TZ";
                InputDataAccGL_.COSTCENTER = "0113005000";
                InputDataAccGL_.ORDERID = "C113005203";

                BAPIACGL09[] InputDataAccGL = new BAPIACGL09[] { InputDataAccGL_ };
                //
                InputDataAccPay_.ITEMNO_ACC = "0000000001";
                InputDataAccPay_.VENDOR_NO = "FLDA286";
                InputDataAccPay_.PMNTTRMS = "Z030";
                InputDataAccPay_.BLINE_DATE = "2018-07-28";
                InputDataAccPay_.ITEM_TEXT = "PAY TEST 1";
                InputDataAccPay_.ALLOC_NMBR = "OPMS TEST 1";

                BAPIACAP09[] InputDataAccPay = new BAPIACAP09[] { InputDataAccPay_ };
                //
                //InputDataAccTax_.ITEMNO_ACC = "";
                //InputDataAccTax_.GL_ACCOUNT = "";
                //InputDataAccTax_.COND_KEY = "";
                //InputDataAccTax_.TAX_CODE = "";
                //InputDataAccTax_.DIRECT_TAX = "";

                //BAPIACTX09[] InputDataAccTax = new BAPIACTX09[] { InputDataAccTax_ };

                InputDataCurAmt_.ITEMNO_ACC = "0000000001";
                InputDataCurAmt_.CURRENCY = "RM";
                InputDataCurAmt_.AMT_DOCCUR = -2000;
                InputDataCurAmt_.AMT_BASE = 0;

                InputDataCurAmt2_.ITEMNO_ACC = "0000000002";
                InputDataCurAmt2_.CURRENCY = "RM";
                InputDataCurAmt2_.AMT_DOCCUR = 2000;
                InputDataCurAmt2_.AMT_BASE = 0;

                BAPIACCR09[] InputDataCurAmt = new BAPIACCR09[] { InputDataCurAmt_, InputDataCurAmt2_ };

                OutputReturn_.FIELD = null;
                OutputReturn_.ID = null;
                OutputReturn_.LOG_MSG_NO = null;
                OutputReturn_.LOG_NO = null;
                OutputReturn_.MESSAGE = null;
                OutputReturn_.MESSAGE_V1 = null;
                OutputReturn_.MESSAGE_V2 = null;
                OutputReturn_.MESSAGE_V3 = null;
                OutputReturn_.MESSAGE_V4 = null;
                OutputReturn_.NUMBER = null;
                OutputReturn_.PARAMETER = null;
                OutputReturn_.ROW = 0;
                OutputReturn_.SYSTEM = null;
                OutputReturn_.TYPE = null;

                BAPIRET2[] OutputReturn = new BAPIRET2[] { OutputReturn_ };

                SAPPostingCollectionData.DOCUMENTHEADER = InputDataDocHeader;
                SAPPostingCollectionData.CUSTOMERCPD = InputDataCustPD;
                SAPPostingCollectionData.ACCOUNTGL = InputDataAccGL;
                SAPPostingCollectionData.ACCOUNTPAYABLE = InputDataAccPay;
                //SAPPostingCollectionData.ACCOUNTTAX = InputDataAccTax;
                SAPPostingCollectionData.CURRENCYAMOUNT = InputDataCurAmt;
                SAPPostingCollectionData.RETURN = OutputReturn;
                SAPPostingResponse = SAPPosting.ZFMOPMSFIAR01(SAPPostingCollectionData);

                
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SAPPosting.Close();
            }

            return View();
        }
    }
}