using MusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class ManageUserController : BaseController
    {
        // GET: ManageUser
        public ActionResult Index()
        {
            ViewBag.Users = db.tb_Usuario.ToList();

            return View();
        }


    }
}