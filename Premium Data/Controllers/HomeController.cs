using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Premium_Data.Models;
using PagedList;
using System.Web.Security;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity.Validation;
using System.IO;

namespace Premium_Data.Controllers
{
    public class HomeController : BaseController
    {
        Cyclus_DataEntities db = new Cyclus_DataEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (Session["id"] != null)
            {
                return RedirectToAction("OverzichtWeken");
            }
            else
            {
                return View();
            }
                
        }
        [HttpPost]
        public ActionResult Login(tbl_Medewerkers works)
        {
            var checklog = (from e in db.tbl_Medewerkers where e.INLOG_GEBRUIKERSNAAM == works.INLOG_GEBRUIKERSNAAM && e.INLOG_WACHTWOORD == works.INLOG_WACHTWOORD select e).FirstOrDefault();
            if (checklog != null)
            {
                if (checklog.INLOG_ACTIEF==true)
                {
                    Session["id"] = checklog.MEDEWERKER_ID;
                    Session.Timeout =60;
                    return RedirectToAction("OverzichtWeken");
                }
                else
                {
                    ViewBag.error = "Uw gebruikersaccount is niet actief";
                    return View();
                }
                



            }
            else
            {
                ViewBag.error = "Onjuiste gebruikersnaam of wachtwoord";
                return View();
            }

        }
        public ActionResult OverzichtWeken(string sortOrder, string currentFilter, string searchString, int? page, Search searchModel)
        {
            if (Session["id"] != null)
            {
                ViewBag.changep = TempData["change"];
                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                ViewBag.Role = find.INLOG_LEVEL;
                ViewBag.mede = find.MEDEWERKER_ID;
                ViewBag.status = searchModel.Status;
                ViewBag.Medewerker = searchModel.Medewerker;
                ViewBag.Werkzaam = searchModel.Werkzaam;
                ViewBag.Jaar = searchModel.Jaar;
                ViewBag.week = searchModel.week;
                ViewBag.CurrentSort = sortOrder;
                TempData["Status"] = searchModel.Status;

                TempData["med1"] = searchModel.Medewerker;
                TempData["Werkzaam"] = searchModel.Werkzaam;
                TempData["Jaar"] = searchModel.Jaar;
                TempData["week"] = searchModel.week;
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                var getone = (from e in db.tbl_Medewerkers_Weken where e.MEDEWERKER_ID == find.MEDEWERKER_ID select e).FirstOrDefault();
                ViewBag.name = getone.MEDEWERKER_NAAM;
                ViewBag.organization = getone.MEDEWERKER_ORGANISATIE;
                if (find.INLOG_LEVEL == 1)
                {
                    var select = (from e in db.tbl_Medewerkers_Weken where e.MEDEWERKER_ID == find.MEDEWERKER_ID select e);
                    var result = select.GroupBy(x => x.WEEK_STATUS)
                  .Select(group => group.FirstOrDefault());
                    var result1 = select.GroupBy(x => x.MEDEWERKER_NAAM)
                        .Select(group => group.FirstOrDefault());
                    var result2 = select.GroupBy(x => x.MEDEWERKER_ORGANISATIE)
                            .Select(group => group.FirstOrDefault());
                    var result3 = select.GroupBy(x => x.JAAR)
                        .Select(group => group.FirstOrDefault());
                    var result4 = select.GroupBy(x => x.PERIODE_WEEK)
                        .Select(group => group.FirstOrDefault());

                    List<SelectListItem> li = new List<SelectListItem>();
                    li.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result)
                    {


                        li.Add(new SelectListItem { Text = m.WEEK_STATUS, Value = m.WEEK_STATUS });
                        ViewBag.relativeName = li;

                    }
                    List<SelectListItem> li1 = new List<SelectListItem>();
                    li1.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result1)
                    {


                        li1.Add(new SelectListItem { Text = m.MEDEWERKER_NAAM, Value = m.MEDEWERKER_NAAM });
                        ViewBag.PERIODE_MAAND = li1;

                    }
                    List<SelectListItem> li2 = new List<SelectListItem>();
                    li2.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result2)
                    {


                        li2.Add(new SelectListItem { Text = m.MEDEWERKER_ORGANISATIE, Value = m.MEDEWERKER_ORGANISATIE });
                        ViewBag.AM_KLANT_REFERENTIE = li2;

                    }
                    List<SelectListItem> li3 = new List<SelectListItem>();
                    li3.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result3)
                    {


                        li3.Add(new SelectListItem { Text = m.JAAR, Value = m.JAAR });
                        ViewBag.AM_CATEGORIE = li3;

                    }
                    List<SelectListItem> li4 = new List<SelectListItem>();
                    li4.Add(new SelectListItem { Text = "<Alle>", Value = "" });
                    foreach (var m in result4)
                    {


                        li4.Add(new SelectListItem { Text = m.PERIODE_WEEK, Value = m.PERIODE_WEEK });
                        ViewBag.PERIODE_WEEK = li4;

                    }

                    if (!string.IsNullOrEmpty(searchModel.Status))
                    {

                        
                        select = select.Where(x => x.WEEK_STATUS.Contains(searchModel.Status));
                    }
                    if (searchModel.Medewerker != null || searchModel.Medewerker == "0")

                    {

                        
                        select = select.Where(x => x.MEDEWERKER_NAAM.Contains(searchModel.Medewerker));
                    }
                    if (searchModel.Werkzaam != null || searchModel.Werkzaam == "0")

                    {

                       
                        select = select.Where(x => x.MEDEWERKER_ORGANISATIE.Contains(searchModel.Werkzaam));
                    }
                    if (searchModel.Jaar != null || searchModel.Jaar == "0")

                    {

                        
                        select = select.Where(x => x.JAAR.Contains(searchModel.Jaar));
                    }



                    if (!string.IsNullOrEmpty(searchModel.week))
                    {
                        
                        select = select.Where(x => x.PERIODE_WEEK.Contains(searchModel.week));
                    }

                    switch (sortOrder)
                    {
                        case "date":
                            select = select.OrderByDescending(s => s.PERIODE_WEEK);
                            break;
                        default:
                            select = select.OrderByDescending(s => s.MEDEWERKER_NAAM);
                            break;

                    }
                    int pageSize = 25;
                    int pageNumber = (page ?? 1);
                    return View(select.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var select = (from e in db.tbl_Medewerkers_Weken where e.MANAGER_NAAM==find.MEDEWERKER_NAAM || e.MEDEWERKER_NAAM==find.MEDEWERKER_NAAM select e);
                    var result = select.GroupBy(x => x.WEEK_STATUS)
                  .Select(group => group.FirstOrDefault());
                    var result1 = select.OrderBy(x => x.MEDEWERKER_NAAM).GroupBy(x => x.MEDEWERKER_NAAM)
                        .Select(group => group.FirstOrDefault());
                    var result2 = select.OrderBy(x => x.MEDEWERKER_ORGANISATIE).GroupBy(x => x.MEDEWERKER_ORGANISATIE)
                            .Select(group => group.FirstOrDefault());
                    var result3 = select.OrderBy(x => x.JAAR).GroupBy(x => x.JAAR)
                        .Select(group => group.FirstOrDefault());
                    var result4 = select.OrderBy(x => x.PERIODE_WEEK).GroupBy(x => x.PERIODE_WEEK)
                        .Select(group => group.FirstOrDefault());

                    List<SelectListItem> li = new List<SelectListItem>();
                    li.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result)
                    {


                        li.Add(new SelectListItem { Text = m.WEEK_STATUS, Value = m.WEEK_STATUS });
                        ViewBag.relativeName = li;

                    }

                    List<SelectListItem> li1 = new List<SelectListItem>();
                    li1.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result1)
                    {


                        li1.Add(new SelectListItem { Text = m.MEDEWERKER_NAAM, Value = m.MEDEWERKER_NAAM });
                       
                        ViewBag.PERIODE_MAAND = li1;

                    }
                   
                    List<SelectListItem> li2 = new List<SelectListItem>();
                    li2.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result2)
                    {


                        li2.Add(new SelectListItem { Text = m.MEDEWERKER_ORGANISATIE, Value = m.MEDEWERKER_ORGANISATIE });
                        ViewBag.AM_KLANT_REFERENTIE = li2;

                    }
                    List<SelectListItem> li3 = new List<SelectListItem>();
                    li3.Add(new SelectListItem { Text = "<Alle>", Value = "" });

                    foreach (var m in result3)
                    {


                        li3.Add(new SelectListItem { Text = m.JAAR, Value = m.JAAR });
                        ViewBag.AM_CATEGORIE = li3;

                    }
                    List<SelectListItem> li4 = new List<SelectListItem>();
                    li4.Add(new SelectListItem { Text = "<Alle>", Value = "" });
                    foreach (var m in result4)
                    {


                        li4.Add(new SelectListItem { Text = m.PERIODE_WEEK, Value = m.PERIODE_WEEK });
                        ViewBag.PERIODE_WEEK = li4;

                    }

                    if (!string.IsNullOrEmpty(searchModel.Status))
                    {

                       
                        select = select.Where(x => x.WEEK_STATUS.Contains(searchModel.Status));
                    }
                    if (searchModel.Medewerker != null || searchModel.Medewerker == "0")

                    {

                       
                        select = select.Where(x => x.MEDEWERKER_NAAM.Contains(searchModel.Medewerker));
                    }
                    if (searchModel.Werkzaam != null || searchModel.Werkzaam == "0")

                    {
                        

                        select = select.Where(x => x.MEDEWERKER_ORGANISATIE.Contains(searchModel.Werkzaam));
                    }
                    if (searchModel.Jaar != null || searchModel.Jaar == "0")

                    {
                        

                        select = select.Where(x => x.JAAR.Contains(searchModel.Jaar));
                    }



                    if (!string.IsNullOrEmpty(searchModel.week))
                    {
                        
                        select = select.Where(x => x.PERIODE_WEEK.Contains(searchModel.week));
                    }

                    switch (sortOrder)
                    {
                        case "date":
                            select = select.OrderByDescending(s => s.PERIODE_WEEK);
                            break;
                        default:
                            select = select.OrderByDescending(s => s.MEDEWERKER_NAAM);
                            break;

                    }
                    int pageSize = 10;
                    int pageNumber = (page ?? 1);
                    return View(select.ToPagedList(pageNumber, pageSize));
                }


            }
            else
            {
                return RedirectToAction("Login");
            }


        }
        public ActionResult Week(int? page, string index, string sortOrder, Search searchModel)
        {
            if (Session["id"] != null)
            {
                string medid = Convert.ToString(Session["id"]);
                if (index != null)
                {
                    string[] abcd = Regex.Split(index, @"\s+");
                    ViewBag.index1 = abcd[0];
                    ViewBag.index2 = abcd[1];
                    ViewBag.index = index;
                    ViewBag.CurrentSort = sortOrder;
                    ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                    ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                    var tbl1 = (from e in db.tbl_Medewerkers_Weken where e.INDEX == index select e).FirstOrDefault();
                    //get data for dropdown
                    var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                    ViewBag.Role = find.INLOG_LEVEL;
                  
                    ViewBag.medew = find.MEDEWERKER_ID;
                    var unicode = (from e in db.tbl_Medewerkers_Urencodes where e.CHECKED == true && e.MEDEWERKER_ID == tbl1.MEDEWERKER_ID select e);


                    var result1 = unicode.GroupBy(x => x.PROJECT_NAAM)
                     .Select(group => group.FirstOrDefault());

                    List<SelectListItem> li1 = new List<SelectListItem>();

                    li1.Add(new SelectListItem { Text = "<Kies een omschrijving>", Value = "" });
                    foreach (var m in result1)
                    {


                        li1.Add(new SelectListItem { Text = m.PROJECT_NAAM, Value = m.PROJECT_NAAM });
                        ViewBag.project_name = li1;

                    }
                    //end get data for dropdown
                   
                    ViewBag.id = tbl1.VOLGNUMMER;
                    ViewBag.budget = tbl1.WEEK_BUDGET;
                    ViewBag.total = tbl1.TOTAAL_UREN_OVERUREN;

                    var getovr = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == tbl1.MEDEWERKER_ID select e).FirstOrDefault();
                    ViewBag.OVERURENs = getovr.OVERUREN_SCHRIJVEN;

                    var Getdata = (from e in db.tbl_Medewerkers_Uren where e.INDEX == index select e);
                    var tbl2 = (from e in db.tbl_Instellingen_Periodes where e.PERIODE_WEEK == tbl1.PERIODE_WEEK select e).FirstOrDefault();
                    DateTime mindtl= Convert.ToDateTime(tbl2.DATUM);
                    var mindate = mindtl.ToShortDateString(); 
                    ViewBag.mindate = mindate;
                    DateTime maxdate = Convert.ToDateTime(tbl2.DATUM);
                    DateTime maxdtl = maxdate.AddDays(6);
                    var maxd = maxdtl.ToShortDateString();
                    ViewBag.maxdate = maxd;
                    ViewBag.mede2 = tbl1.MEDEWERKER_ID;


                    int pageSize = 25;
                    int pageNumber = (page ?? 1);
                    ViewBag.Week = tbl1.PERIODE_WEEK;
                    string getwd = tbl1.PERIODE_WEEK;
                    string[] getwdl = getwd.Split('-');
                    ViewBag.yr = getwdl[0];
                    ViewBag.wk = getwdl[1];


                    ViewBag.MedeWerker = tbl1.MEDEWERKER_NAAM;
                    ViewBag.Week_Status = tbl1.WEEK_STATUS;
                    ViewBag.Uren = tbl1.TOTAAL_UREN;
                    ViewBag.Overuren = tbl1.TOTAAL_OVERUREN;
                    ViewBag.Totaal = tbl1.TOTAAL_UREN_OVERUREN;
                    ViewBag.Opmerking = tbl1.OPMERKINGEN;
                    TempData["Opmer"]= tbl1.OPMERKINGEN;
                    ViewBag.id = tbl1.VOLGNUMMER;
                   
                    switch (sortOrder)
                    {
                        case "":
                            Getdata = Getdata.OrderBy(s => s.VOLGORDE);
                            break;
                        default:
                            Getdata = Getdata.OrderBy(s => s.VOLGORDE);
                            break;

                    }
                    return View(Getdata.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    return RedirectToAction("OverzichtWeken");
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult approve(int? id)
        {
            var Med = TempData["med1"];
            var Stat = TempData["Status"];


            var Werk = TempData["Werkzaam"];
            var Jaarr = TempData["Jaar"];
            var weekk = TempData["week"];
            if (id != null)
            {
                var find = (from e in db.tbl_Medewerkers_Weken where e.VOLGNUMMER == id select e).FirstOrDefault();
                find.WEEK_STATUS = "Goedgekeurd";
                db.SaveChanges();
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });

            }
            else
            {
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });
            }

        }
        public ActionResult disapprove(int? id)
        {
            var Med = TempData["med1"];
            var Stat = TempData["Status"];


            var Werk = TempData["Werkzaam"];
            var Jaarr = TempData["Jaar"];
            var weekk = TempData["week"];
            if (id != null)
            {
                var find = (from e in db.tbl_Medewerkers_Weken where e.VOLGNUMMER == id select e).FirstOrDefault();
                var getmail = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == find.MEDEWERKER_ID select e).FirstOrDefault();
                find.WEEK_STATUS = "Afgekeurd";
                db.SaveChanges();
                
                SendMail2(getmail.E_MAILADRES_ZAKELIJK);
               
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });

            }
            else
            {
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });
            }

        }
        public bool SendMail2(string email)
        {
            string smtpAddress = "smtp.office365.com";
            int portNumber = 587;
            bool enableSSL = true;
            bool result = false;

            string emailFrom = "noreply-uren-online@cyclusnv.nl";
            string password = "Cyclus@9881";
            string emailTo = email.ToString();
            string subject = "Afkeurde uren Uren-Online";
            // string body = "Geachte collega,<br><br>Er staan uren in Uren-Online klaar die zijn afgekeurd door uw leidinggevende.<br>Wilt u zo vriendelijk zijn om deze afgekeurde uren waar nodig aan te passen en opnieuw in te dienen ?<br><br>Met vriendelijke groet,<br><br><b>Bedrijfsadministratie</b>";
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Views/Home/EmailTemplate.cshtml")))
            {
                body = reader.ReadToEnd();
            }
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;


                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                    result = true;
                }
            }
            return result;
        }
        public ActionResult Indienen(int? id)
        {
            var Med = TempData["med1"];
            var Stat = TempData["Status"];


            var Werk = TempData["Werkzaam"];
            var Jaarr = TempData["Jaar"];
            var weekk = TempData["week"];
            if (id != null)
            {
                var find = (from e in db.tbl_Medewerkers_Weken where e.VOLGNUMMER == id select e).FirstOrDefault();
                find.WEEK_STATUS = "Ingediend";
                db.SaveChanges();
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });

            }
            else
            {
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });
            }

        }

        public ActionResult Week_Verwijderen(int? id)
        {
            var Med = TempData["med1"];
            var Stat = TempData["Status"];


            var Werk = TempData["Werkzaam"];
            var Jaarr = TempData["Jaar"];
            var weekk = TempData["week"];
            if (id != null)
            {
                var find = (from e in db.tbl_Medewerkers_Weken where e.VOLGNUMMER == id select e).FirstOrDefault();
                find.WEEK_STATUS = "Verwijderen";
                db.SaveChanges();
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });

            }
            else
            {
                return RedirectToAction("OverzichtWeken", "Home", new { Medewerker = Med, Status = Stat, Werkzaam = Werk, Jaar = Jaarr, week = weekk });
            }

        }


        [HttpPost]
        public ActionResult AddWeek(string ind, DateTime? datum, string Project_Name, string Start, string Eind, string Uren, string Overuren_Check, string Opmerking)
        {
            if (Session["id"] != null)
            {
                try
                {
                    string medid = Convert.ToString(Session["id"]);
                    var getn = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                tbl_Medewerkers_Uren ab = new tbl_Medewerkers_Uren();
                var find = (from e in db.tbl_Medewerkers_Weken where e.INDEX == ind select e).FirstOrDefault();
                ab.JAAR = find.JAAR;
                ab.PERIODE_WEEK = find.PERIODE_WEEK;
                ab.MEDEWERKER_ID = find.MEDEWERKER_ID;
                ab.MEDEWERKER_NAAM = find.MEDEWERKER_NAAM;
                ab.MANAGER_ID = find.MANAGER_ID;
                ab.MANAGER_NAAM = find.MANAGER_NAAM;
                ab.MEDEWERKER_ORGANISATIE = find.MEDEWERKER_ORGANISATIE;
                
                string Uren1 = Uren.Replace(",", ".");
                ab.UREN = decimal.Parse(Uren1, CultureInfo.InvariantCulture);
                ab.PROJECT_NAAM = Project_Name;
                var getid = (from e in db.tbl_Medewerkers_Urencodes where e.PROJECT_NAAM == Project_Name select e).FirstOrDefault();
                ab.PROJECT_ID = getid.PROJECT_ID;
                ab.DATUM = datum;
                ab.DAG = datum;
              
                ab.OPMERKINGEN = Opmerking;
                DateTime vol = Convert.ToDateTime(datum);
                
                
                ab.INGEVOERD_DATUM = DateTime.Now;
                ab.INGEVOERD_TIJD = DateTime.Now.TimeOfDay;
                ab.INDEX = ind;
                if (Overuren_Check == "Uren")
                {
                        ab.VOLGORDE = vol.ToString("yyyyMMdd") + "1";
                        ab.OVERUREN_CHECK = false;
                }
                if (Overuren_Check == "Overuren")
                {
                        ab.VOLGORDE = vol.ToString("yyyyMMdd") + "2";
                        ab.OVERUREN_CHECK = true;
                }
                if (Overuren_Check == "Uren")
                {
                    ab.UREN_NORMAAL = ab.UREN;
                    ab.UREN_OVERUREN = 0;
                }
                if (Overuren_Check == "Overuren")
                {
                    ab.UREN_OVERUREN = ab.UREN;
                    ab.UREN_NORMAAL = 0;
                }
                ab.GEWIJZIGD_CHECK = true;
                db.tbl_Medewerkers_Uren.Add(ab);
                db.SaveChanges();

                var get = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == false && e.INDEX == ind select e).ToList();
                find.TOTAAL_UREN = 0;

                foreach (var item in get)
                {
                    find.TOTAAL_UREN = find.TOTAAL_UREN + item.UREN_NORMAAL;
                }
                var forOver = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == true && e.INDEX == ind select e).ToList();
                find.TOTAAL_OVERUREN = 0;
                foreach (var item in forOver)

                {
                    find.TOTAAL_OVERUREN = find.TOTAAL_OVERUREN + item.UREN_OVERUREN;
                }
                find.TOTAAL_UREN_OVERUREN = find.TOTAAL_UREN + find.TOTAAL_OVERUREN;
                find.GEWIJZIGD_CHECK = true;
                db.SaveChanges();


                return RedirectToAction("Week", new { index = ind });
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw e;
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Login");
        }
       
       public ActionResult Editpage(int? id,string index1,string index2)
        {
            if (Session["id"] != null)
            {

                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                ViewBag.OVERUREN = find.OVERUREN_SCHRIJVEN;
                //datetimepicker
                string index = index1 + " " + index2;
                ViewBag.index = index;
                var tbl1 = (from e in db.tbl_Medewerkers_Weken where e.INDEX == index select e).FirstOrDefault();
                var tbl2 = (from e in db.tbl_Instellingen_Periodes where e.PERIODE_WEEK == tbl1.PERIODE_WEEK select e).FirstOrDefault();
                DateTime mindtl = Convert.ToDateTime(tbl2.DATUM);
                var mindate = mindtl.ToShortDateString();
                ViewBag.mindate = mindate;
                DateTime maxdate = Convert.ToDateTime(tbl2.DATUM);
                DateTime maxdtl = maxdate.AddDays(6);
                var maxd = maxdtl.ToShortDateString();
                ViewBag.maxdate = maxd;
                //end datetimepicker
                var unicode = (from e in db.tbl_Medewerkers_Urencodes where e.CHECKED == true && e.MEDEWERKER_ID == find.MEDEWERKER_ID select e);


               
                var result1 = unicode.GroupBy(x => x.PROJECT_NAAM)
                 .Select(group => group.FirstOrDefault());

                List<SelectListItem> li1 = new List<SelectListItem>();

               
                foreach (var m in result1)
                {


                    li1.Add(new SelectListItem { Text = m.PROJECT_NAAM, Value = m.PROJECT_NAAM });
                    ViewBag.project_name = li1;

                }
                var getp = (from e in db.tbl_Medewerkers_Uren where e.VOLGNUMMER == id select e).FirstOrDefault();
                return PartialView(getp);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult Editpage(int? id,string ind, DateTime? DATUM, string PROJECT_NAAM, string Start, string Eind, string Uren, string Overuren_Check, string Opmerking)
        {
            if (Session["id"] != null)
            {
                var ab = (from e in db.tbl_Medewerkers_Uren where e.VOLGNUMMER == id select e).FirstOrDefault();

                string Uren1 = Uren.Replace(",", ".");
                ab.UREN = decimal.Parse(Uren1, CultureInfo.InvariantCulture);
                ab.PROJECT_NAAM = PROJECT_NAAM;
                var getid = (from e in db.tbl_Medewerkers_Urencodes where e.PROJECT_NAAM == PROJECT_NAAM select e).FirstOrDefault();
                ab.PROJECT_ID = getid.PROJECT_ID;
                ab.DATUM = DATUM;
                ab.DAG = DATUM;
               
                ab.OPMERKINGEN = Opmerking;
                DateTime vol = Convert.ToDateTime(DATUM);
                
                

                ab.INDEX = ind;
                if (Overuren_Check == "Uren")
                {
                    ab.VOLGORDE = vol.ToString("yyyyMMdd") + "1";
                    ab.OVERUREN_CHECK = false;
                }
                if (Overuren_Check == "Overuren")
                {
                    ab.VOLGORDE = vol.ToString("yyyyMMdd") + "2";
                    ab.OVERUREN_CHECK = true;
                }
                if (Overuren_Check == "Uren")
                {
                   
                    ab.UREN_NORMAAL = decimal.Parse(Uren1, CultureInfo.InvariantCulture);
                    ab.UREN_OVERUREN = 0;
                }
                if (Overuren_Check == "Overuren")
                {
                    ab.UREN_OVERUREN = decimal.Parse(Uren1, CultureInfo.InvariantCulture);
                    ab.UREN_NORMAAL = 0;
                }
                ab.GEWIJZIGD_CHECK = true;
                db.SaveChanges();
                var find = (from e in db.tbl_Medewerkers_Weken where e.INDEX == ind select e).FirstOrDefault();
                var get = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == false && e.INDEX == ind select e).ToList();
                find.TOTAAL_UREN = 0;

                foreach (var item in get)
                {
                    find.TOTAAL_UREN = find.TOTAAL_UREN + item.UREN_NORMAAL;
                }
                var forOver = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == true && e.INDEX == ind select e).ToList();
                find.TOTAAL_OVERUREN = 0;
                foreach (var item in forOver)

                {
                    find.TOTAAL_OVERUREN = find.TOTAAL_OVERUREN + item.UREN_OVERUREN;
                }
                find.TOTAAL_UREN_OVERUREN = find.TOTAAL_UREN + find.TOTAAL_OVERUREN;
                find.GEWIJZIGD_CHECK = true;
                db.SaveChanges();
                return RedirectToAction("Week", new { index = ind });
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Kopieren(int? id, string index1, string index2)
        {
            if (Session["id"] != null)
            {
                //copy item
                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                ViewBag.OVERUREN = find.OVERUREN_SCHRIJVEN;
                var getp = (from e in db.tbl_Medewerkers_Uren where e.VOLGNUMMER == id select e).FirstOrDefault();
                tbl_Medewerkers_Uren ab = new tbl_Medewerkers_Uren();
                string index = index1 + " " + index2;
                ViewBag.index = index;
                ab.JAAR = getp.JAAR;
                ab.PERIODE_WEEK = getp.PERIODE_WEEK;
                ab.MEDEWERKER_ID = getp.MEDEWERKER_ID;
                ab.MEDEWERKER_NAAM = getp.MEDEWERKER_NAAM;
                ab.MANAGER_ID = getp.MANAGER_ID;
                ab.MANAGER_NAAM = getp.MANAGER_NAAM;
                ab.MEDEWERKER_ORGANISATIE = getp.MEDEWERKER_ORGANISATIE;


                ab.UREN = getp.UREN;
                ab.PROJECT_NAAM = getp.PROJECT_NAAM;
             
                ab.PROJECT_ID = getp.PROJECT_ID;
                ab.DATUM = getp.DATUM;
                ab.DAG = getp.DAG;

                ab.START = getp.START;

                ab.EINDE = getp.EINDE;
                ab.OPMERKINGEN = getp.OPMERKINGEN;

                ab.VOLGORDE = getp.VOLGORDE;
                ab.INGEVOERD_DATUM = DateTime.Now;
                ab.INGEVOERD_TIJD = DateTime.Now.TimeOfDay;
                ab.INDEX = getp.INDEX;
               
                    ab.OVERUREN_CHECK = getp.OVERUREN_CHECK;
               
               
                    ab.UREN_NORMAAL = getp.UREN_NORMAAL;
                    ab.UREN_OVERUREN = getp.UREN_OVERUREN;
                
                db.tbl_Medewerkers_Uren.Add(ab);
                db.SaveChanges();

                var get = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == false && e.INDEX == index select e).ToList();
                var getweken = (from e in db.tbl_Medewerkers_Weken where e.INDEX == index select e).FirstOrDefault();
                getweken.TOTAAL_UREN = 0;

                foreach (var item in get)
                {
                    getweken.TOTAAL_UREN = getweken.TOTAAL_UREN + item.UREN_NORMAAL;
                }
                var forOver = (from e in db.tbl_Medewerkers_Uren where e.OVERUREN_CHECK == true && e.INDEX == index select e).ToList();
                getweken.TOTAAL_OVERUREN = 0;
                foreach (var item in forOver)

                {
                    getweken.TOTAAL_OVERUREN = getweken.TOTAAL_OVERUREN + item.UREN_OVERUREN;
                }
                getweken.TOTAAL_UREN_OVERUREN = getweken.TOTAAL_UREN + getweken.TOTAAL_OVERUREN;
                db.SaveChanges();
                //end copy item
                //datetimepicker
                
                var tbl1 = (from e in db.tbl_Medewerkers_Weken where e.INDEX == index select e).FirstOrDefault();
                var tbl2 = (from e in db.tbl_Instellingen_Periodes where e.PERIODE_WEEK == tbl1.PERIODE_WEEK select e).FirstOrDefault();
                DateTime mindtl = Convert.ToDateTime(tbl2.DATUM);
                var mindate = mindtl.ToShortDateString();
                ViewBag.mindate = mindate;
                DateTime maxdate = Convert.ToDateTime(tbl2.DATUM);
                DateTime maxdtl = maxdate.AddDays(6);
                var maxd = maxdtl.ToShortDateString();
                ViewBag.maxdate = maxd;
                //end datetimepicker
                var unicode = (from e in db.tbl_Medewerkers_Urencodes where e.CHECKED == true && e.MEDEWERKER_ID == find.MEDEWERKER_ID select e);


                var result1 = unicode.GroupBy(x => x.PROJECT_NAAM)
                 .Select(group => group.FirstOrDefault());

                List<SelectListItem> li1 = new List<SelectListItem>();
                li1.Add(new SelectListItem { Text = "<Kies een omschrijving>", Value = "" });

                foreach (var m in result1)
                {


                    li1.Add(new SelectListItem { Text = m.PROJECT_NAAM, Value = m.PROJECT_NAAM });
                    ViewBag.project_name = li1;

                }
                int urenid = db.tbl_Medewerkers_Uren.OrderByDescending(p => p.VOLGNUMMER).First().VOLGNUMMER;
                var geth = (from e in db.tbl_Medewerkers_Uren where e.VOLGNUMMER == urenid select e).FirstOrDefault();
                return PartialView(getp);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult dltWeek(int? id, string weekid)
        {
            if (id != null)
            {
                var find = (from e in db.tbl_Medewerkers_Uren where e.VOLGNUMMER == id select e).FirstOrDefault();
                var getdata = (from e in db.tbl_Medewerkers_Weken where e.INDEX == find.INDEX select e).FirstOrDefault();
                if (find.OVERUREN_CHECK==true)
                {
                    getdata.TOTAAL_UREN_OVERUREN = getdata.TOTAAL_UREN_OVERUREN - find.UREN_OVERUREN;
                    getdata.TOTAAL_UREN = getdata.TOTAAL_UREN - find.UREN_NORMAAL;
                    getdata.TOTAAL_OVERUREN = getdata.TOTAAL_OVERUREN - find.UREN_OVERUREN;
                    getdata.GEWIJZIGD_CHECK = true;
                    db.SaveChanges();
                    db.tbl_Medewerkers_Uren.Remove(find);
                    db.SaveChanges();
                    return RedirectToAction("week", new { index = weekid });
                }
                else
                {
                    getdata.TOTAAL_UREN_OVERUREN = getdata.TOTAAL_UREN_OVERUREN - find.UREN_NORMAAL;
                    getdata.TOTAAL_UREN = getdata.TOTAAL_UREN - find.UREN_NORMAAL;
                    getdata.TOTAAL_OVERUREN = getdata.TOTAAL_OVERUREN - find.UREN_OVERUREN;
                    getdata.GEWIJZIGD_CHECK = true;
                    db.SaveChanges();
                    db.tbl_Medewerkers_Uren.Remove(find);
                    db.SaveChanges();
                    return RedirectToAction("week", new { index = weekid });
                }

               
            }
            else
            {
                return RedirectToAction("week", new { index = weekid });
            }

        }
        public ActionResult Informatie()
        {
            if (Session["id"] != null)
            {
                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();

                var getdata = (from e in db.tbl_Medewerkers_Info where e.MEDEWERKER_ID == find.MEDEWERKER_ID select e).FirstOrDefault();
                return View(getdata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Handleiding()
        {
            if (Session["id"] != null)
            {
                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();

                var getdata = (from e in db.tbl_Medewerkers_Info where e.MEDEWERKER_ID == find.MEDEWERKER_ID select e).FirstOrDefault();
                return View(getdata);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult weeksubmit(int id, string Opmerking)
        {
            string opm =Convert.ToString( TempData["Opmer"]);
            if (!string.IsNullOrEmpty(Opmerking) || !string.IsNullOrEmpty(opm)) 
            {
                var find = (from e in db.tbl_Medewerkers_Weken where e.VOLGNUMMER == id select e).FirstOrDefault();
                find.OPMERKINGEN = Opmerking;
                find.GEWIJZIGD_CHECK = true;
                db.SaveChanges();
            }
            
            var Med = TempData["med1"];
            var Stat = TempData["Status"];

           
          var Werk=  TempData["Werkzaam"];
          var Jaarr=  TempData["Jaar"];
          var weekk=   TempData["week"];
            return RedirectToAction("OverzichtWeken","Home",new {  Medewerker = Med, Status = Stat, Werkzaam= Werk, Jaar= Jaarr, week= weekk });
        }
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(string email)
        {
           

                string mailError = "";

            var verifyEmail = (from v in db.tbl_Medewerkers
                               where v.E_MAILADRES_ZAKELIJK == email
                               select v).SingleOrDefault();

            if (verifyEmail != null)
                    {
                       
                        string newPassword = GetUniqueKey(9);
                        bool check = SendMail(verifyEmail.E_MAILADRES_ZAKELIJK, newPassword);
                        if (!check)
                        {
                            mailError = "De e-mail kan niet verzonden worden, probeer het opnieuw!";

                        }
                        else
                        {
                            change_passward(email,newPassword);

                        }


                    }
                    else
                    {
                        mailError = "E-mailadres is onjuist.";

                    }
            ViewBag.mailError = mailError;


                return View();
            
        }
        public bool SendMail(string email, string newPassword)
        {
            string smtpAddress = "smtp.office365.com";
            int portNumber = 587;
            bool enableSSL = true;
            bool result = false;

            string emailFrom = "noreply-uren-online@cyclusnv.nl";
            string password = "Cyclus@9881";
            string emailTo = email.ToString();
            string subject = "Nieuw wachtwoord Uren-Online";
            string body = "Geachte collega,<br><br>We hebben een nieuw wachtwoord voor u aangemaakt. <br><br>Uw nieuwe wachtwoord is :  <B>" + newPassword + "</B><br><br>Met vriendelijke groet,<br><br><B>Bedrijfsadministratie</B>";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;


                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                    result = true;
                }
            }
            return result;
        }
        public void change_passward(string email, string newPassword)
        {

            var find = (from v in db.tbl_Medewerkers
                               where v.E_MAILADRES_ZAKELIJK == email
                               select v).SingleOrDefault();



            find.INLOG_WACHTWOORD = newPassword;

                int x = db.SaveChanges();
            
        }
        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                //crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetBytes(data); //GetNonZeroBytes(data);
            }
            System.Text.StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                //result.Append((UInt32)(BitConverter.ToUInt32(data, 4 * maxSize) % (chars.Length)));
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public ActionResult ChangePassword()
        {
            if (Session["id"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(string oldpassword, string newpassword)
        {
            if (Session["id"] != null)
            {
                string medid = Convert.ToString(Session["id"]);
                var find = (from e in db.tbl_Medewerkers where e.MEDEWERKER_ID == medid select e).FirstOrDefault();
                var select = (from e in db.tbl_Medewerkers where e.INLOG_WACHTWOORD == oldpassword && e.E_MAILADRES_ZAKELIJK == find.E_MAILADRES_ZAKELIJK select e).FirstOrDefault();
                if (select != null)
                {
                    select.INLOG_WACHTWOORD = newpassword;
                    db.SaveChanges();
                    TempData["change"] = "Uw wachtwoord is succesvol gewijzigd.";
                    return RedirectToAction("OverzichtWeken");

                }
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}