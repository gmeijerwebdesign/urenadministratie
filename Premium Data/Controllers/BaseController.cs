using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Premium_Data.Controllers
{
    public class BaseController : Controller
    {
        Cyclus_DataEntities db = new Cyclus_DataEntities();
        public BaseController()
        {

            if (System.Web.HttpContext.Current.Session["id"] != null)
            {
                string medid = System.Web.HttpContext.Current.Session["id"].ToString();

                tbl_Medewerkers data = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                ViewBag.employeeName = data.MEDEWERKER_NAAM;
                ViewBag.organizationName = data.MEDEWERKER_ORGANISATIE;
                DateTime date1 = new DateTime(2021, 1, 1);
                
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dfi.Calendar;
             var currentweek=  cal.GetWeekOfYear(date1, dfi.CalendarWeekRule,
                                          dfi.FirstDayOfWeek);
                var cw = currentweek + 1;
                if (cw<10)
                {
                    var current=2021 + "-" +0+""+cw;
                    ViewBag.currentweek = current;

                }
                else
                {
                    var current = 2021 + "-" + cw;
                    ViewBag.currentweek = current;
                }


            }


        }
        public int getUserId(string usr)
        {

            var usnm = (from e in db.tbl_Medewerkers where e.INLOG_GEBRUIKERSNAAM == usr select e.VOLGNUMMER).FirstOrDefault();
            return usnm;


        }
    }
}