using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MuloApi.Controllers;
using MuloApi.Models;
using Xunit;

namespace XUnitTestMuloApi
{
    public class UnitTestAuthentificationController
    {
        [Fact]
        public async void ConnectUserTest()
        {
            //Arrange
            var setTestDataUser = new[]
            {
                new ModelConnectingUser
                {
                    Login = "layon18@yandex.ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser(),
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru",
                    Password = ""
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru",
                    Password = "1234"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex,ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex,ru",
                    Password = "1234"
                }
            };
            var resultsConnectUser = new List<JsonResult>();
            var authentificationController = new AuthentificationController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            foreach (var dataUser in setTestDataUser)
            {
                var result = await authentificationController.ConnectUser(dataUser);
                resultsConnectUser.Add(result);
            }

            var settingDB = new StreamWriter(@"dbsettings.json");
            var setConnect =
                "{\"ConnectionStrings\": { \"DefaultConnection\": \"server=localhost;database=muloplaye;user=mulobd;password=051291+Mulobd\" } }";
            await settingDB.WriteLineAsync(setConnect);
            settingDB.Close();

            foreach (var dataUser in setTestDataUser)
            {
                var result = await authentificationController.ConnectUser(dataUser);
                resultsConnectUser.Add(result);
            }

            settingDB = new StreamWriter(@"dbsettings.json");
            var setConnectOK =
                "{\"ConnectionStrings\": { \"DefaultConnection\": \"server=localhost;database=muloplayer;user=mulobd;password=051291+Mulobd\" } }";
            await settingDB.WriteLineAsync(setConnectOK);
            settingDB.Close();

            //Assert
            foreach (var result in resultsConnectUser)
            {
                Assert.NotNull(result);
                if (result.Value.ToString().Contains("user_id") && result.Value.ToString().Contains("login"))
                {
                    Assert.Matches(@"^\{\s(user_id = )[0-9]+\,\s(login = ).*\s\}$",
                        result.Value.ToString());
                    continue;
                }

                result.AssertContains(ListResults.GetListResults().MethodConnectUser());
            }
        }

        [Fact]
        public async void CreateUserTest()
        {
            //Arrange
            var setTestDataUser = new[]
            {
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser(),
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru",
                    Password = ""
                },
                new ModelConnectingUser
                {
                    Login = "layon55@yandex.ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser
                {
                    Login = "layon18@yandex.ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex.ru",
                    Password = "1234"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex,ru",
                    Password = "asdfqr"
                },
                new ModelConnectingUser
                {
                    Login = "layon16@yandex,ru",
                    Password = "1234"
                }
            };
            var resultsCreateUser = new List<JsonResult>();
            var authentificationController = new AuthentificationController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            foreach (var dataUser in setTestDataUser)
            {
                var result = await authentificationController.CreateUser(dataUser);
                resultsCreateUser.Add(result);
            }

            var settingDB = new StreamWriter(@"dbsettings.json");
            var setConnect =
                "{\"ConnectionStrings\": { \"DefaultConnection\": \"server=localhost;database=muloplaye;user=mulobd;password=051291+Mulobd\" } }";
            await settingDB.WriteLineAsync(setConnect);
            settingDB.Close();

            foreach (var dataUser in setTestDataUser)
            {
                var result = await authentificationController.CreateUser(dataUser);
                resultsCreateUser.Add(result);
            }

            settingDB = new StreamWriter(@"dbsettings.json");
            var setConnectOK =
                "{\"ConnectionStrings\": { \"DefaultConnection\": \"server=localhost;database=muloplayer;user=mulobd;password=051291+Mulobd\" } }";
            await settingDB.WriteLineAsync(setConnectOK);
            settingDB.Close();

            // Assert
            foreach (var result in resultsCreateUser)
            {
                Assert.NotNull(result);
                if (result.Value.ToString().Contains("user_id") && result.Value.ToString().Contains("login"))
                {
                    Assert.Matches(@"^\{\s(user_id = )[0-9]+\,\s(login = ).*\s\}$",
                        result.Value.ToString());
                    continue;
                }

                result.AssertContains(ListResults.GetListResults().MethodCreateUser());
            }
        }

        [Fact]
        public async void GetSoundTracksUserTest()
        {
            //Arrange
            var resultsGetTrackUser = new List<JsonResult>();
            var authentificationController = new AuthentificationController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            //Act
            for (var i = 0; i < 10; i++)
            {
                var result = await authentificationController.GetSoundTracksUser(i);
                resultsGetTrackUser.Add(result);
            }

            // Assert
            foreach (var result in resultsGetTrackUser)
            {
                Assert.NotNull(result);
                if (result.Value.ToString().Contains("MuloApi.Models.ModelUserTracks"))
                {
                    Assert.Matches(@"^\{\s(tracks = )(MuloApi.Models.ModelUserTracks\[)([0-9]+|)\]\s\}$",
                        result.Value.ToString());
                    continue;
                }

                result.AssertContains(ListResults.GetListResults().MethodGetSoundTracksUser());
            }
        }
    }
}