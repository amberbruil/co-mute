﻿using CoMute.Web.Data;
using CoMute.Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace CoMute.Web.Controllers.Web
{
    public class HomeController : Controller
    {
        dbCoMuteEntities db = new dbCoMuteEntities();


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        
        /// <summary>
        /// Http response to display view: user's car pools
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserCarPools()
        {
            return View(db.tblUserCarPools.ToList());
        }

        /// <summary>
        /// Method to send login request data
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(LoginRequest loginRequest)
        {

            var user = db.tblRegisters.Where(zz => zz.Email == loginRequest.Email && zz.Password == loginRequest.Password).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credetials");
                return View();
            }
            else
            {
                TempData["ID"] = user.UserID;
                Session["ID"] = user.UserID;

                using (var client = new HttpClient())
                {
                   
                    client.BaseAddress = new Uri("http://localhost:59598/api/authentication");

                    //HTTP POST
                    var postTask = client.PostAsJsonAsync("authentication", loginRequest);
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("UserCarPools");
                    }
                }
                return View(loginRequest);
            
            }
        }

        /// <summary>
        /// Method to send the registration data
        /// </summary>
        /// <param name="registrationRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(RegistrationRequest registrationRequest)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:59598/register");

                var postTask = client.PostAsJsonAsync("register", registrationRequest);
                postTask.Wait();

                var result = postTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            //Error checking
            ModelState.AddModelError("Error", "Server Error. Please contact administrator.");

            return View(registrationRequest);

        }

        

    }
}