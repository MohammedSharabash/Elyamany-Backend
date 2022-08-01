using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ElYamanyDashboard.Models;
using ElYamanyDashboard.Models.Views;
using System.Data.Entity.Migrations;
using System.Net.Http;

namespace ElYamanyDashboard.Controllers
{
    [Authorize]

    public class OrdersController : Controller
    {
        private ElYamanyContext db = new ElYamanyContext();


        public async Task<ActionResult> GetOrdersSummary()
        {
            var sql = @"select  Area.Name,[User].AreaId,Count([Order].Id) as OrdersCount from [Order] join [User] on [Order].UserId=[User].Id 
left join Area on Area.Id=[User].AreaId where [Order].OrderStatusId=1 group by [User].AreaId,Area.Name";
            var areaOrders = db.Database.SqlQuery<AreaOrders>(sql);
            return View(areaOrders);
        }
        // GET: Orders
        public async Task<ActionResult> Index(long AreaId,long OrderStatusId=0)
        {
            List<Order> orders = new List<Order>();
            ViewBag.AreaId = AreaId;
            if (OrderStatusId>0)
            {
                switch (OrderStatusId)
                {
                    case 1:
                        ViewBag.Color1 = "#519839";
                        break;
                    case 2:
                        ViewBag.Color2 = "#519839";
                        break;
                    case 3:
                        ViewBag.Color3 = "#519839";
                        break;
                    case 4:
                        ViewBag.ColorAll = "#519839";
                        break;
                    default:
                        break;
                }
                orders =await db.Orders.Where(i => i.User.AreaId == AreaId&&i.OrderStatusId==OrderStatusId).OrderByDescending(i=>i.Id).ToListAsync();

            }
            else
            {
                orders =await db.Orders.Where(i => i.User.AreaId == AreaId).OrderByDescending(i => i.Id).ToListAsync();
                ViewBag.ColorAll = "#519839";

            }
            ViewBag.Total = orders.Count;
            ViewBag.Area = db.Areas.Where(i => i.Id == AreaId).FirstOrDefault();
            return View(orders);
        }


        public async Task<ActionResult> OrdersReport(DateTime? FromDate,DateTime? ToDate,long? AreaId,long? UserId,long? OrderStatusId)
        {
            List<Order> orders = new List<Order>();
            ViewBag.AreaId = AreaId;
            ViewBag.FromDate = FromDate != null ? FromDate.Value.ToString("yyyy-MM-dd") : null;
            ViewBag.ToDate = ToDate != null ? ToDate.Value.ToString("yyyy-MM-dd") : null;
            ViewBag.UserId = UserId;
            ViewBag.OrderStatusId = OrderStatusId;
            ViewBag.OrderStatus = db.OrderStatus.ToList();
            ViewBag.Areas = db.Areas.ToList();
            ViewBag.Areas = db.Areas.ToList(); ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");

            var OrdersQuery = db.Orders.Include(i=>i.User);
            int indicator = -1;
            if (OrderStatusId > 0)
            {
                indicator = 1;
                OrdersQuery = OrdersQuery.Where(i => i.OrderStatusId == OrderStatusId);

            }
            if(AreaId>0)
            {
                indicator = 1;

                OrdersQuery = OrdersQuery.Where(i => i.User.AreaId == AreaId);//.OrderByDescending(i => i.Id).ToListAsync();
            }
            if (UserId>0)
            {
                indicator = 1;

                OrdersQuery = OrdersQuery.Where(i => i.UserId == UserId);//.OrderByDescending(i => i.Id).ToListAsync();

            }
            if (OrderStatusId > 0)
            {
                indicator = 1;

                OrdersQuery = OrdersQuery.Where(i => i.OrderStatusId == OrderStatusId);//.OrderByDescending(i => i.Id).ToListAsync();

            }
            if (FromDate!=null&&ToDate!=null)
            {
                indicator = 1;

                OrdersQuery = OrdersQuery.Where(i => i.CreationDate >= FromDate&& i.CreationDate <= ToDate);//.OrderByDescending(i => i.Id).ToListAsync();

            }
            if (indicator == -1)
            {
                ViewBag.Total =0;
                return View(new List<Order>());
            }
            orders=await OrdersQuery.OrderByDescending(i => i.Id).ToListAsync();
            ViewBag.Total = orders.Count;
            return View(orders);
        }

        public async Task<ActionResult> AllOrders()
        {
            List<Order> orders = new List<Order>();
           orders = await db.Orders/*.Include(o => o.OrderStatus).Include(o => o.DeliveryType).Include(o => o.User)*/.OrderByDescending(i => i.Id).ToListAsync();
            return View(orders);
        }


        public async Task<ActionResult> PrintAll(long AreaId, long OrderStatusId = 0)
        {
            List<Order> orders = new List<Order>();
            ViewBag.AreaId = AreaId;
            if (OrderStatusId>0)
            {
                orders = await db.Orders.Include(o => o.OrderItems).Include(o => o.OrderStatus).Include(o => o.DeliveryType).Include(o => o.User).Where(i => i.User.AreaId == AreaId&&i.OrderStatusId==OrderStatusId).OrderByDescending(i => i.Id).ToListAsync();
            }
            else
            {
                orders = await db.Orders.Include(o => o.OrderItems).Include(o => o.OrderStatus).Include(o => o.DeliveryType).Include(o => o.User).Where(i => i.User.AreaId == AreaId).OrderByDescending(i => i.Id).ToListAsync();

            }
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "Name");
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Order order)
        {
            if (order.UserId>0&&order.DeliveryTypeId>0)
            {
                var DeliveryTypeData = db.DeliveryTypes.AsNoTracking().Where(d => d.Id == order.DeliveryTypeId).FirstOrDefault();
                order.CreationDate = DateTime.UtcNow.AddHours(2);
                order.OrderStatusId= 4;//under creation
                order.DeliveryCost = DeliveryTypeData.Cost;
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("EditWithAddItem",new { id=order.Id});
            }

            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "Name", order.DeliveryTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", order.UserId);
            return View(order);
        }

        public async Task<ActionResult> EditWithAddItem(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToListAsync();
            order.OrderItems = OrderItems;
            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "NameAR", order.DeliveryTypeId);
            ViewBag.UserDate = db.Users.AsNoTracking().Where(i => i.Id == order.UserId).FirstOrDefault();
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus.Where(i => i.Id == 1 || i.Id == 4), "Id", "NameAR", order.OrderStatusId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "NameWithPrice");
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditWithAddItem(Order order)
        {
            if (order.ProductCount == null)
            {
                order.ProductCount = 0;
            } 
            var OldData = db.Orders.AsNoTracking().Where(i => i.Id == order.Id).FirstOrDefault();
            if (OldData != null)
            {
                var NewProductData =  db.Products.AsNoTracking().Where(o=>o.Id==order.ProductId).FirstOrDefault();
                if (NewProductData.AvailableCount< order.ProductCount)
                {
                    order.OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToListAsync();

                    ViewBag.ProductId = new SelectList(db.Products, "Id", "NameWithPrice");
                    ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "NameAR", order.DeliveryTypeId);
                    ViewBag.UserDate = db.Users.AsNoTracking().Where(i => i.Id == order.UserId).FirstOrDefault();
                    ViewBag.OrderStatusId = new SelectList(db.OrderStatus.Where(i=>i.Id==1||i.Id==4), "Id", "NameAR", order.OrderStatusId);
                    ViewBag.Message = "الكمية المطلوبة غير متوفرة الكمية المتاحة حاليا هي : " + NewProductData.AvailableCount + "";
                    return View(order);
                }
                OrderItem NewOrderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    ProductId = order.ProductId.Value,
                    ProductCount = (int)order.ProductCount,
                    ProductPrice = NewProductData.CurrentPrice.Value
                };
                db.OrderItems.Add(NewOrderItem);
                db.SaveChanges();

                var OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToListAsync();

                if (NewOrderItem.Id > 0)
                {

                    foreach (var orderItem in OrderItems)
                    {
                        var ReminderCount = orderItem.Product.AvailableCount - orderItem.ProductCount;
                        if (orderItem.ProductCount > 0)
                        {
                            if (orderItem.Product.AvailableCount<=0)
                            {
                                orderItem.ProductCount = 0;
                            }
                            else if (orderItem.ProductCount > orderItem.Product.AvailableCount)
                            {
                                orderItem.ProductCount = (int)orderItem.Product.AvailableCount;
                            }
 
                            orderItem.TotalPrice = orderItem.ProductCount * orderItem.ProductPrice;
                            db.OrderItems.AddOrUpdate(orderItem);
                            db.SaveChanges();
                            await UpdateQuantityOfProduct(orderItem.ProductCount, orderItem.ProductId);
                        }
                    }
                    //
                    var totalItems = OrderItems.Sum(i=>i.TotalPrice);
                    OldData.SubTotal = totalItems;


                    OldData.TotalPrice = OldData.SubTotal + OldData.DeliveryCost;
                    OldData.TotalPoints = (int)(OldData.SubTotal / 8);
                    db.Orders.AddOrUpdate(OldData);
                    db.SaveChanges();

                }
             return RedirectToAction("EditWithAddItem", new { id = OldData.Id });
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "NameWithPrice");

            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "NameAR", order.DeliveryTypeId);
            ViewBag.UserDate = db.Users.AsNoTracking().Where(i => i.Id == order.UserId).FirstOrDefault();
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus.Where(i => i.Id == 1 || i.Id == 4), "Id", "NameAR", order.OrderStatusId);
            return View(order);
        }

        public async Task<ActionResult> DeleteOrderItem(long? id)
        {
            
            var orderItem = await db.OrderItems.FindAsync(id);
            long OrderId = 0;
            if (orderItem!=null)
            {
                var OldData = db.Orders.AsNoTracking().Where(o => o.Id == orderItem.OrderId).FirstOrDefault();
                OrderId = OldData.Id;
                db.OrderItems.Remove(orderItem);
                db.SaveChanges();


                var OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == OrderId).ToListAsync();
                var totalItems = OrderItems.Sum(i => i.TotalPrice);
                OldData.SubTotal = totalItems;
                OldData.TotalPrice = OldData.SubTotal + OldData.DeliveryCost;
                OldData.TotalPoints = (int)(OldData.SubTotal / 8);
                db.Orders.AddOrUpdate(OldData);
                db.SaveChanges();

            }
          return  RedirectToAction("EditWithAddItem", new { id = OrderId });

        }
        [HttpGet]
        public JsonResult GetOrderItembyID(long? orderItemId)
        {
            var data = db.OrderItems.AsNoTracking().Where(t => t.Id == orderItemId).FirstOrDefault();
            // data.Product
            OrderItem orderItem = new OrderItem()
            {
                Id = data.Id,
                OrderId = data.OrderId,
                ProductCount = data.ProductCount,
                ProductPrice = data.ProductPrice,
                TotalPrice = data.TotalPrice,
                ProductId = data.ProductId,
                Product=new Product() {
                    Id=data.Product.Id,
                    NameAR=data.Product.NameAR,
                    Name=data.Product.Name,
                    CurrentPrice=data.Product.CurrentPrice,

                }
            };
            return Json(orderItem, JsonRequestBehavior.AllowGet);
        }
        internal async Task UpdateQuantityOfProduct(int productCount, long? productId)
        {
            string sql = @"update Product set AvailableCount=AvailableCount-" + productCount + " where Id=" + productId + "";
            await db.Database.ExecuteSqlCommandAsync(sql);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateOrderItem(UpdateOrderItemModel updateOrderItemModel)
        {
           
            var OldData = db.Orders.AsNoTracking().Where(i => i.Id == updateOrderItemModel.OrderId).FirstOrDefault();
            var OrderItemData = db.OrderItems.Where(o => o.Id == updateOrderItemModel.Id).FirstOrDefault();
            int OldCount = OrderItemData.ProductCount;
            if (OldData != null)
            {
                var NewProductData = db.Products.AsNoTracking().Where(o => o.Id == OrderItemData.ProductId).FirstOrDefault();
                if (NewProductData.AvailableCount < updateOrderItemModel.ProductCount)
                {
                    updateOrderItemModel.IsError = NewProductData.AvailableCount;
                    return Json(updateOrderItemModel);
                }
                OrderItemData.ProductCount = updateOrderItemModel.ProductCount;
                OrderItemData.TotalPrice = OrderItemData.ProductPrice * updateOrderItemModel.ProductCount;
                db.OrderItems.AddOrUpdate(OrderItemData);
                db.SaveChanges();
                var OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == updateOrderItemModel.OrderId).ToListAsync();
                foreach (var orderItem in OrderItems)
                    {
                        var ReminderCount =  orderItem.ProductCount- OldCount;
                        if (orderItem.ProductCount > 0)
                        {

                            orderItem.TotalPrice = orderItem.ProductCount * orderItem.ProductPrice;
                            db.OrderItems.AddOrUpdate(orderItem);
                            db.SaveChanges();
                            await UpdateQuantityOfProduct(ReminderCount, orderItem.ProductId);
                        }
                    //
                    var totalItems = OrderItems.Sum(i => i.TotalPrice);
                    OldData.SubTotal = totalItems;
                    OldData.TotalPrice = OldData.SubTotal + OldData.DeliveryCost;
                    OldData.TotalPoints = (int)(OldData.SubTotal / 8);
                    db.Orders.AddOrUpdate(OldData);
                    db.SaveChanges();
                }
                return Json(updateOrderItemModel);
            }


            return Json(updateOrderItemModel);
        }
        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            var OrderItems = await db.OrderItems.Include(o => o.Product).Where(o => o.OrderId == order.Id).ToListAsync();

            order.OrderItems = OrderItems;
            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "NameAR", order.DeliveryTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", order.UserId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "NameAR", order.OrderStatusId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Order order)
        {
            var OldData = db.Orders.Where(i => i.Id == order.Id).FirstOrDefault();
            var OrderDateMonth = OldData.CreationDate.Value.Month;
            var OrderDateYear = OldData.CreationDate.Value.Year;
            var CurrentMonth = DateTime.UtcNow.Month;
            var CurrentYear = DateTime.UtcNow.Year;
            
            if (OldData!=null)
            {
                if (order.OrderStatusId==2&& OrderDateMonth == CurrentMonth&&OrderDateYear==CurrentYear)
                {
                    OldData.TotalPoints =0;
                    OldData.CancelationDate = DateTime.UtcNow;
                }
                if (order.OrderStatusId == 2 && OrderDateMonth != CurrentMonth)
                {
                    OldData.TotalPoints = -1*OldData.TotalPoints;
                    OldData.CancelationDate = DateTime.UtcNow;
                }
                if (OldData.OrderStatusId==1&&order.OrderStatusId==3)
                {
                    OldData.DeliveryDate = DateTime.UtcNow.AddHours(2);
                }
                OldData.OrderStatusId = order.OrderStatusId;
                db.Entry(OldData).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Edit",new { id=OldData.Id});
            }
            ViewBag.DeliveryTypeId = new SelectList(db.DeliveryTypes, "Id", "NameAR", order.DeliveryTypeId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "UserCode", order.UserId);
            ViewBag.OrderStatusId = new SelectList(db.OrderStatus, "Id", "NameAR", order.OrderStatusId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<ActionResult> UpdateOrderStatus(long? OrderStatusId,long? OrderId)
        {
            var OrderData = db.Orders.AsNoTracking().Where(i => i.Id == OrderId).FirstOrDefault();

            OrderData.OrderStatusId = OrderStatusId;
            db.Orders.AddOrUpdate(OrderData);
            db.SaveChanges();
            try
            {
                if (OrderStatusId==1)
                {
                    var task = Task.Run(async () =>
                    {
                        var enpointUrl = "http://projectegy-001-site41.gtempurl.com/api/CalculateDailySalary?UserId=" + OrderData.UserId;
                        var client = new HttpClient();
                        var response = await client.GetAsync(enpointUrl);
                        var responseString = await response.Content.ReadAsStringAsync();
                    }
                  );
                   

                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Create");
        }

        public async Task<ActionResult> GetAllUnderCreationOrders()
        {
            List<Order> orders = new List<Order>();
            orders = await db.Orders/*.Include(o => o.OrderStatus).Include(o => o.DeliveryType).Include(o => o.User)*/.Where(i=>i.OrderStatusId==4).OrderByDescending(i => i.Id).ToListAsync();
            return View(orders);
        }
        public async Task<ActionResult> UpdateOrderStatusAndRetunToALL(long? OrderStatusId, long? OrderId)
        {
            var OrderData = db.Orders.AsNoTracking().Where(i => i.Id == OrderId).FirstOrDefault();
            OrderData.OrderStatusId = OrderStatusId;
            db.Orders.AddOrUpdate(OrderData);
            db.SaveChanges();
            try
            {
                if (OrderStatusId == 1)
                {
                    var enpointUrl = "http://projectegy-001-site41.gtempurl.com/api/CalculateDailySalary?UserId=" + OrderData.UserId;
                    var client = new HttpClient();
                    var response = await client.GetAsync(enpointUrl);
                    var responseString = await response.Content.ReadAsStringAsync();

                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("GetAllUnderCreationOrders");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
