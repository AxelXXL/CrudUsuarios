using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class SongsController : BaseController
    {
        // GET: Songs
        public ActionResult Index()
        {
            return View();
        }

        #region Canciones

        [HttpGet]
        public JsonResult GetCanciones()
        {

            var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                                      .Take(50) // Tomar las primeras 50 filas
                                      .Select(c => new
                                      {
                                          IdCancion = c.ID_CANCION,
                                          NombreCancion = c.Nombre_Cancion,
                                          NombreArtista = c.tb_Artista.Nombre_Artista,
                                          NombreAlbum = c.tb_Album.Nombre_album,
                                          Caratula = c.Caratula_Cancion,
                                          Ruta_Audio = c.Ruta_Audio
                                      })
                                          .ToList();

            return Json(datos, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetNovedades()
        {
            var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                                       .Take(6) // Tomar las primeras 50 filas
                                       .Select(c => new
                                       {
                                           IdCancion = c.ID_CANCION,
                                           NombreCancion = c.Nombre_Cancion,
                                           NombreArtista = c.tb_Artista.Nombre_Artista,
                                           NombreAlbum = c.tb_Album.Nombre_album,
                                           Caratula = c.Caratula_Cancion,
                                           Ruta_Auido = c.Ruta_Audio,
                                       })
                                           .ToList();



            return Json(datos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRecomendaciones(int idUser)
        {
            var random = new Random();

            var datalikes = db.tb_LikeMusic.Include(t => t.tb_Cancion).Include(t => t.tb_Usuario)
                .Where(a => a.ID_USUARIO == idUser)
                .Select(c => new
                {
                    Gen = c.tb_Cancion.tb_Album.Genero
                })
                .FirstOrDefault();

            if (datalikes != null)
            {
                var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                    .Where(c => c.tb_Album.Genero.Contains(datalikes.Gen))
                    .ToList(); // Obtén los datos de la base de datos

                // Ordena alfabéticamente en C# y selecciona todas las canciones
                var sortedData = datos
                    .OrderBy(c => c.Nombre_Cancion, StringComparer.CurrentCulture)
                    .Select(c => new
                    {
                        IdCancion = c.ID_CANCION,
                        NombreCancion = c.Nombre_Cancion,
                        NombreArtista = c.tb_Artista.Nombre_Artista,
                        NombreAlbum = c.tb_Album.Nombre_album,
                        Caratula = c.Caratula_Cancion
                    })
                    .ToList();

                return Json(sortedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var datos = db.tb_Cancion.Include(t => t.tb_Album).Include(t => t.tb_Artista)
                    .ToList(); // Obtén los datos de la base de datos

                // Ordena alfabéticamente en C# y selecciona todas las canciones
                var sortedData = datos
                    .OrderBy(c => c.Nombre_Cancion, StringComparer.CurrentCulture)
                    .Select(c => new
                    {
                        IdCancion = c.ID_CANCION,
                        NombreCancion = c.Nombre_Cancion,
                        NombreArtista = c.tb_Artista.Nombre_Artista,
                        NombreAlbum = c.tb_Album.Nombre_album,
                        Caratula = c.Caratula_Cancion
                    })
                    .ToList();

                return Json(sortedData, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetCancionList(string txt)
        {
            var items = db.tb_Cancion
                .Where(x => x.Nombre_Cancion.Contains(txt)
                || x.tb_Album.Nombre_album.Contains(txt)
                || x.tb_Album.Genero.Contains(txt)
                || x.tb_Artista.Nombre_Artista.Contains(txt))
                .Select(x => new
                {
                    Text = x.Nombre_Cancion,
                    Value = x.ID_CANCION
                })
                .ToList();
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCancionesArtist(int idArtist)
        {
            var canciones = db.tb_Cancion.Include(t => t.tb_Album)
                              .Where(b => b.ID_ARTISTA == idArtist)
                              .Select(t => new
                              {
                                  t.ID_CANCION,
                                  t.Nombre_Cancion,
                                  t.tb_Album.Nombre_album,
                                  t.Caratula_Cancion,

                              }).ToList();

            return Json(canciones, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetAlbumsArtist(int idArtist)
        {
            var canciones = db.tb_Album
                              .Where(b => b.ID_ARTISTA == idArtist)
                              .Select(t => new
                              {
                                  t.ID_ALBUM,
                                  t.Nombre_album,
                                  t.Genero,
                                  t.Año_Album

                              }).ToList();

            return Json(canciones, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public FileResult GetImagen(byte[] dataImg)
        {
            byte[] fileBytes = dataImg;// aqui el array de bytes
            return File(fileBytes, "image/png", "nombre.png"); //3er argumento es opcional
        }

        #endregion
    }
}