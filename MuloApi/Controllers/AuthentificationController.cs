﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MuloApi.Classes;
using MuloApi.DataBase;
using MuloApi.DataBase.Control;
using MuloApi.DataBase.Control.Interfaces;
using MuloApi.Interfaces;
using MuloApi.Models;

namespace MuloApi.Controllers
{
    [ApiController]
    public class AuthentificationController : ControllerBase
    {
        public ICheckData CheckDataUser = new CheckDataUser();
        public IActionUser ControlDataBase = new ActionUserDataBase();
        public IControlDataBase DataBase = new AppDbContent().Current;

        [HttpPost]
        [Route("/authorization")]
        public async Task<ActionResult> ConnectUser(ModelConnectingUser dataUser)
        {
            try
            {
                if (dataUser?.Login != null && dataUser.Password != null)
                    if (CheckDataUser.CheckLoginRegular(dataUser.Login) &&
                        CheckDataUser.CheckPasswordRegular(dataUser.Password))
                    {
                        if (!await DataBase.TestConnection())
                            return new JsonResult(new
                                {
                                    error = "ERRORSERVER"
                                })
                                {StatusCode = 521};

                        var idUser = await ControlDataBase.GetUserId(dataUser.Login);

                        if (idUser != -1)
                        {
                            var hashUser = await ControlDataBase.SaveHashUser(idUser, Request.Headers);
                            var newSettingCookie = new CookieOptions
                            {
                                HttpOnly = true
                            };
                            Response.Cookies.Append("session", hashUser, newSettingCookie);
                            return new JsonResult(new
                                {
                                    user_id = idUser,
                                    login = dataUser.Login
                                })
                                {StatusCode = 200};
                        }
                    }
            }
            catch (Exception e)
            {
                if (Startup.LoggerApp != null)
                    await Task.Run(() => Startup.LoggerApp.LogWarning(e.ToString()));
            }

            return new JsonResult(new
                {
                    errors = new
                    {
                        message = "INCORRECT_PASSWORD_OR_LOGIN"
                    }
                })
                {StatusCode = 401};
        }

        [HttpPost]
        [Route("/registration")]
        public async Task<ActionResult> CreateUser(ModelConnectingUser dataUser)
        {
            try
            {
                if (dataUser?.Login != null && dataUser.Password != null)
                {
                    if (!CheckDataUser.CheckLoginRegular(dataUser.Login))
                        return new JsonResult(new
                            {
                                errors = new
                                {
                                    message = "INCORRECT_LOGIN"
                                }
                            })
                            {StatusCode = 401};

                    if (!CheckDataUser.CheckPasswordRegular(dataUser.Password))
                        return new JsonResult(new
                            {
                                errors = new
                                {
                                    message = "INCORRECT_PASSWORD"
                                }
                            })
                            {StatusCode = 401};

                    if (!await DataBase.TestConnection())
                        return new JsonResult(new
                            {
                                error = "ERRORSERVER"
                            })
                            {StatusCode = 521};

                    var resultExist = await ControlDataBase.ExistUser(dataUser.Login);
                    if (resultExist)
                        return new JsonResult(new
                            {
                                errors = new
                                {
                                    message = "EXISTING_USER"
                                }
                            })
                            {StatusCode = 401};

                    var resultAdd = await ControlDataBase.AddUser(dataUser.Login, dataUser.Password);
                    if (!resultAdd)
                        return new JsonResult(new
                            {
                                error = "ERRORSERVER"
                            })
                            {StatusCode = 521};

                    var idUser = await ControlDataBase.GetUserId(dataUser.Login);
                    if (idUser != -1)
                    {
                        IActionDirectory addDirectoryUser = new UserDirectory();
                        addDirectoryUser.CreateDirectoryUser(idUser);
                        var hashUser = await ControlDataBase.SaveHashUser(idUser, Request.Headers);
                        var newSettingCookie = new CookieOptions
                        {
                            HttpOnly = true
                        };
                        Response.Cookies.Append("session", hashUser, newSettingCookie);
                        return new JsonResult(new
                            {
                                user_id = idUser,
                                login = dataUser.Login
                            })
                            {StatusCode = 200};
                    }
                }
            }
            catch (Exception e)
            {
                if (Startup.LoggerApp != null)
                    await Task.Run(() => Startup.LoggerApp.LogWarning(e.ToString()));
            }

            return new JsonResult(new
                {
                    error = "ERRORSERVER"
                })
                {StatusCode = 500};
        }

        [HttpGet]
        [Route("/user/{idUser:min(0)}/soundtracks")]
        public async Task<ActionResult> GetSoundTracksUser(int idUser)
        {
            if (!Request.Cookies.ContainsKey("session"))
                return RedirectToRoute(new {controller = "Authentification", action = "ConnectUser"});
            if (!await ControlDataBase.CheckUserSession(Request.Cookies["session"], idUser, Request.Headers))
                return RedirectToRoute(new {controller = "Authentification", action = "ConnectUser"});

            IActionDirectory userDirectory = new UserDirectory();
            var listTracks = await userDirectory.GetRootTracksUser(idUser);
            if (listTracks == null)
                return new JsonResult(new
                    {
                        error = "ERRORSERVER"
                    })
                    {StatusCode = 500};
            if (listTracks.Length == 0)
                return new JsonResult(new
                    {
                        tracks = "empty"
                    })
                    {StatusCode = 200};
            return new JsonResult(new
            {
                tracks = listTracks
            });
        }
    }
}