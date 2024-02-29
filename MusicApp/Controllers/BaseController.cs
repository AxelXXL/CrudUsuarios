using MusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class BaseController : Controller
    {

        #region dbContext
        //Instancia para conexion a la base de datos
        public BD_LOSS_SOUNDS db = new BD_LOSS_SOUNDS();
        #endregion
    }
}