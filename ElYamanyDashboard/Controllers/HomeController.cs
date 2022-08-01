using ElYamanyDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class HomeController : Controller
    {
        ElYamanyContext db = new ElYamanyContext();
        public ActionResult Index()
        {
            ViewBag.OrdersCount = db.Orders.Where(i=>i.OrderStatusId==1).Count();
            ViewBag.OrdersTodayAmount = db.Database.SqlQuery<decimal>("select IsNull (sum(TotalPrice),0) from [Order] where OrderStatusId=3 and cast( DeliveryDate as date)=cast((select GETUTCDATE() )as date)").FirstOrDefault();
            ViewBag.OrdersTodayCount = db.Database.SqlQuery<int>("select IsNull (Count(Id),0) from [Order] where OrderStatusId=3 and cast( DeliveryDate as date)=cast((select GETUTCDATE() )as date)").FirstOrDefault();
            ViewBag.OrdersMonthAmount = db.Database.SqlQuery<decimal>("select IsNull (sum(TotalPrice),0) from [Order] where  OrderStatusId=3 and (SELECT DATEPART(month, DeliveryDate))=(SELECT DATEPART(month, (DATEADD(MONTH,0,(select GetutcDate())))))").FirstOrDefault();
            ViewBag.OrdersMonthCount = db.Database.SqlQuery<int>("select IsNull (Count(Id),0) from [Order] where  OrderStatusId=3 and (SELECT DATEPART(month, DeliveryDate))=(SELECT DATEPART(month, (DATEADD(MONTH,0,(select GetutcDate())))))").FirstOrDefault();

            ViewBag.UsersCount = db.Users.Count();
            ViewBag.ItemsCount = db.Products.Count();
            ViewBag.SponsorsCount = db.Users.Where(i=>i.SponsorId== 31).Count();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}