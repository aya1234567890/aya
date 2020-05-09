using emarket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net;
using System.Data;
using emarket.ViewModel;

namespace emarket.Controllers
{
    public class AdminController : Controller
    {
        STOREEntities db = new STOREEntities();
        // GET: Admin     
        // =============== Add new product ==========
        public ActionResult create()
        {
            var c = db.Category.ToList();
            ProductViewmodel p = new ProductViewmodel
            { Category = c };
        

            return View(p);
        }
        [HttpPost]
        public ActionResult create(ProductViewmodel p, HttpPostedFileBase imgfile)
        {
            if (ModelState.IsValid)
            {
                if (imgfile != null)
                {
                    string pic = System.IO.Path.GetFileName(imgfile.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/upload/"),pic);
                    imgfile.SaveAs(path);
                    p.Product.image = "~/Content/upload/" + pic;
                }
                db.Product.Add(p.Product);
                db.SaveChanges();
                var category = db.Category.SingleOrDefault(c=>c.id == p.Product.category_id);
                category.number_of_products++;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("ViewProduct");
        }




       
        // =============== ViewProducts ==========

        public ActionResult ViewProduct(string searchString, string currentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var Product = from s in db.Product
                          select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                Product = Product.Where(s => s.Category.name.Contains(searchString));

            }
            Product = Product.OrderByDescending(s => s.Category.name);
            int pageSize = 1000;
            int pageNumber = (page ?? 1);
            return View(Product.ToPagedList(pageNumber, pageSize));
        }




        // =============== ProductDetails ==========
        public ActionResult ViewProductDetails(int? id)
        {

            Viewmodel v = new Viewmodel();
            Product p = db.Product.Where(x => x.id == id).SingleOrDefault();
            v.id_p = p.id;
            v.name_p = p.name;
            v.image = p.image;
            v.price = p.price;
            v.description = p.description;
            Category cat = db.Category.Where(x => x.id == p.category_id).SingleOrDefault();
            v.name_c = cat.name;
            return View(v);
        }


        // =============== DELETE ==========
        public ActionResult Delete(int? id)
        {
            var p = db.Product.Find(id);

             var category = db.Category.SingleOrDefault(c => c.id == p.category_id);
             category.number_of_products--;
             db.Entry(category).State = EntityState.Modified;
             db.SaveChanges();
            db.Product.Remove(p);
            db.SaveChanges();
            return RedirectToAction("ViewProduct");
        }
        // =============== Update ==========

        [HttpGet]
        public ActionResult update(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product P = db.Product.Find(id);
            Session["imgPath"] = P.image;

            if (P == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.Category, "id", "name", P.category_id);
            return View(P);
        }

       [HttpPost]
        public ActionResult update(HttpPostedFileBase file, Product P)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                    string extension = Path.GetExtension(file.FileName);

                    string path = Path.Combine(Server.MapPath("~/Content/upload/"), _filename);

                    P.image = "~/Content/upload/" + _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 1000000)
                        {
                            db.Entry(P).State = EntityState.Modified;
                           string oldImgPath = Request.MapPath(Session["imgPath"].ToString());

                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                
                                TempData["msg"] = "Data Updated";
                                return RedirectToAction("ViewProduct");
                            }
                        }
                        else
                        {
                            ViewBag.msg = "File Size must be Equal or less than 1mb";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Inavlid File Type";
                    }
                }
                else
                {
                    P.image = Session["imgPath"].ToString();
                    db.Entry(P).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["msg"] = "Data Updated";
                        return RedirectToAction("ViewProduct");
                    }

                }
            }
            return View();
        }
 
    


    }
}