using GAS_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using FMData;
using RestSharp;

namespace GAS_Portal.Controllers
{
	public class UtentesController : Controller
	{
        private static List<Utente> Utentes = new List<Utente>();
        private static string token;
        private static string recordId;

        /*public IActionResult Index()
		{
			return View();
		}*/

        public async Task<IActionResult> Index()
        {
            //obter token
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/sessions");
            request.Headers.Add("Authorization", "Basic d2ViOiNwYXJhZGlzZUA=");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Lê o conteúdo da resposta como uma string JSON
            string jsonStr = response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse(jsonStr);
            token = json["response"]["token"].ToString();
            GetToken model = new GetToken { Token = token };

            //obter tabelas
            var clientT = new HttpClient();
            var requestT = new HttpRequestMessage(HttpMethod.Get, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records");
            requestT.Headers.Add("Authorization", "Bearer " + token);
            var responseT = await clientT.SendAsync(requestT);
            responseT.EnsureSuccessStatusCode();

            string jsonStrT = responseT.Content.ReadAsStringAsync().Result;
            JObject jsonObj = JObject.Parse(jsonStrT);

            Utentes.Clear();

            foreach (var ut in jsonObj["response"]["data"])
            {
                Utente utente = new Utente();

                utente.Nome = ut["fieldData"]["UTE_nome"].ToString();
                utente.CodigoUtente = ut["fieldData"]["UTE_codUTE"].ToString();
                utente.ID = ut["fieldData"]["UTE_nrID"].ToString();
                utente.Data = ut["fieldData"]["UTE_data"].ToString();
                utente.Localidade = ut["fieldData"]["UTE_localidade"].ToString();
                utente.Telemovel = ut["fieldData"]["UTE_telemovel"].ToString();
                utente.NIF = ut["fieldData"]["UTE_nif"].ToString();
                utente.SNS = ut["fieldData"]["UTE_nr_SNS"].ToString();
                utente.Morada = ut["fieldData"]["UTE_morada"].ToString();
                utente.CodigoPostal = ut["fieldData"]["UTE_cpostal"].ToString();
                utente.DataNascimento = ut["fieldData"]["UTE_datanascimento"].ToString();
                utente.Sexo = ut["fieldData"]["UTE_sexo"].ToString();
                utente.PreLog = ut["fieldData"]["UTE_prelog"].ToString();
                utente.RecordId = ut["recordId"].ToString();

                Utentes.Add(utente);

            }

            return View(Utentes);
        }

        // GET: Utentes/Details/5
        public async Task<IActionResult> Details(string? id)
        {


            var matchingUtente = Utentes.FirstOrDefault(u => u.ID == id);

            if (matchingUtente != null)
            {
                // If a matching object is found, return a view of the object
                return View(matchingUtente);
            }
            else
            {
                // If no matching objects are found, return a NotFound result
                return NotFound();
            }


        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || Utentes == null)
            {
                return NotFound();
            }

            var utentes = Utentes.FirstOrDefault(u => u.ID == id);
            if (utentes == null)
            {
                return NotFound();
            }
            return View(utentes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, Utente utente)
        {

            if (id != utente.ID)
            {
                return NotFound();
            }

            string recordId = "";

            foreach (var i in Utentes)
            {
                if (i.ID == id)
                {
                    recordId = i.RecordId;
                    break;
                }

            }

            if (ModelState.IsValid)
            {
                var options = new RestClientOptions("https://rcsa.seekdata.pt")
                {
                    MaxTimeout = -1,
                };
                //Fazer update a partir do recordId
                var clientUpdate = new RestClient(options);
                var requestUpdate = new RestRequest("/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records/" + recordId + "", Method.Patch);
                requestUpdate.AddHeader("Content-Type", "application/json");
                requestUpdate.AddHeader("Authorization", "Bearer " + token);

                var body = @"{
                        " + "\n" +
                        @"""fieldData"":{ ""UTE_cpostal"": """ + utente.CodigoPostal + @""",
                        " + "\n" +
                        @"              ""UTE_nif"": """+ utente.NIF + @"""
                        " + "\n" +
                        @"            }
                        " + "\n" +
                        @"}";

                //var contentUpdate = new StringContent("{\r\n\"fieldData\":{ \"UTE_cpostal\": \"" + utente.CodigoPostal + "\",\r\n              \"UTE_nif\": \"" + utente.NIF + "\"\r\n            }\r\n}", null, "application/json");

                requestUpdate.AddStringBody(body, DataFormat.Json);
                RestResponse responseUpdate = await clientUpdate.ExecuteAsync(requestUpdate);

                //return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
            //return View(utente);

        }


        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, Utente utente)
        {

            if (id != utente.ID)
            {
                return NotFound();
            }

            string recordId="";

            foreach(var i in Utentes)
            {
                if (i.ID == id)
                {
                    recordId = i.RecordId;
                    break;
                }
                    
            }

            if (ModelState.IsValid)
            {
                //Fazer update a partir do recordId
                var clientUpdate = new HttpClient();
                var requestUpdate = new HttpRequestMessage(HttpMethod.Patch, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records/" + recordId + "");
                requestUpdate.Headers.Add("Authorization", "Bearer " + token);


                //var contentUpdate = new StringContent("{\r\n\"fieldData\":{ \"UTE_cpostal\": \"" + utente.CodigoPostal + "\",\r\n              \"UTE_nif\": \"" + utente.NIF + "\"\r\n            }\r\n}", null, "application/json");
                var jsonObject = new JProperty("fieldData", new JObject(
                                        new JProperty("UTE_cpostal", utente.CodigoPostal),
                                        new JProperty("UTE_nif", utente.NIF)
                                             )
                );
                //contentUpdate = new StringContent(contentUpdate.ToString(), Encoding.UTF8, "application/json");
                var contentUpdate = new StringContent(jsonObject.ToString(), null, "application/json");
                requestUpdate.Content = contentUpdate;
                var responseUpdate = await clientUpdate.SendAsync(requestUpdate);
                responseUpdate.EnsureSuccessStatusCode();

           

                //return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
            //return View(utente);

        }*/


    }
}

/* [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string? id, Utente utente)
        {
            if (id != utente.ID)
            {
                return NotFound();
            }

            // Get the record ID associated with the ID
            string recordId = Utentes.First(u => u.ID == id).RecordId;

            if (ModelState.IsValid)
            {
                // Connect to the FileMaker Data API
                var client = new FMSClient(new Uri("https://rcsa.seekdata.pt/fmi/data/v1/databases/GAS DEV/layouts/LD_Utentes_API/records"));

                // Set the authorization token
                client.SetToken(token);

                // Create a new record object with the updated field values
                var record = new FMRecord
                {
                    Layout = "LD_Utentes_API",
                    RecordId = recordId,
                    FieldData = new Dictionary<string, object>
            {
                { "UTE_cpostal", utente.CodigoPostal },
                { "UTE_nif", utente.NIF }
            }
                };

                try
                {
                    // Update the record
                    await client.EditAsync(record);
                }
                catch (Exception ex)
                {
                    // Handle any errors
                    return BadRequest(ex.Message);
                }
            }

            return RedirectToAction(nameof(Index));
        }*/