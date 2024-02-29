using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using MusicApp.Models;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace MusicApp.Controllers
{
    public class LoginController : BaseController
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(tb_Usuario user)
        {
            user.Contrasena = Encrypt(user.Contrasena);

            using (SqlConnection cn = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("User", user.Nombre_Usuario);
                cmd.Parameters.AddWithValue("Password", user.Contrasena);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                user.ID_USUARIO = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (user.ID_USUARIO != 0)
            {
                Session["user"] = user;
                return RedirectToAction("Index", "Songs");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }
        }


        [HttpPost]
        public ActionResult Register(tb_Usuario user)
        {
            bool registrado;
            string message;

            if(user.Contrasena == user.ConfirmPassword)
            {
                user.Contrasena = Encrypt(user.Contrasena);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using(SqlConnection con = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_registerUser", con);
                cmd.Parameters.AddWithValue("User", user.Nombre_Usuario);
                cmd.Parameters.AddWithValue("Password", user.Contrasena);
                cmd.Parameters.AddWithValue("ID_Rol", 3);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                message = cmd.Parameters["Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = message;

            if (registrado)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                return View();
            }

        }

        public ActionResult CerrarSesion()
        {
            Session["user"] = null;
            return RedirectToAction("Login", "Login");
        }

        #region Encrypt

        //private static string EncryptSha256(string txt)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    using (SHA256 hash = SHA256Managed.Create())
        //    {
        //        Encoding enc = Encoding.UTF8;
        //        byte[] result = hash.ComputeHash(enc.GetBytes(txt));

        //        foreach (byte b in result)
        //        {
        //            sb.Append(b.ToString("x2"));
        //        }

        //        return sb.ToString();
        //    }
        //}


        public static string Encrypt(string clearText, string key = "abc123")
        {
            string encryptionKey = key;
            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key = "abc123")
        {
            string encryptionKey = key;
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        #endregion

    }
}