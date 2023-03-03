using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using GAS_Portal.Models;

namespace GAS_Portal.Controllers
{
    public class LogInController : Controller
    {
        public async Task<IActionResult> LogIn()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> LogIn(Utilizador user)
        {
            bool isUserAuthenticated = false;

            //Pedido token
            var clientToken = new HttpClient();
            var requestToken = new HttpRequestMessage(HttpMethod.Post, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/sessions");
            requestToken.Headers.Add("Authorization", "Basic d2ViOiNwYXJhZGlzZUA=");
            var contentToken = new StringContent(string.Empty);
            contentToken.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestToken.Content = contentToken;
            var responseToken = await clientToken.SendAsync(requestToken);
            responseToken.EnsureSuccessStatusCode();

            // Lê o conteúdo da resposta como uma string JSON
            string jsonStr = responseToken.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(jsonStr);
            string token = json["response"]["token"].ToString();
            //GetToken model = new GetToken { Token = token };

            //Obtêm data dos utilizadores
            var clientUsers = new HttpClient();
            var requestUsers = new HttpRequestMessage(HttpMethod.Get, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utilizadores_API/records");
            requestUsers.Headers.Add("Authorization", "Bearer " + token);
            var responseUsers = await clientUsers.SendAsync(requestUsers);
            responseUsers.EnsureSuccessStatusCode();

            string jsonStrT = responseUsers.Content.ReadAsStringAsync().Result;
            JObject jsonObj = JObject.Parse(jsonStrT);

            foreach (var u in jsonObj["response"]["data"])
            {
                string name = u["fieldData"]["UTL_nome"].ToString();
                string pass = u["fieldData"]["UTL_senha"].ToString();

                if (name == user.Username && pass == user.Password)
                {
                    isUserAuthenticated = true;
                    break;
                }

            }

            if (isUserAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(user);
            }

        }
    }
}
