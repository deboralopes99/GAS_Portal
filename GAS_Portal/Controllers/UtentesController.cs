using GAS_Portal.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System;
using System.Text;


namespace GAS_Portal.Controllers
{
	public class UtentesController : Controller
	{
        private static List<Utente> Utentes = new List<Utente>();
        private static string token;

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
            //Fazer pesquisa para obter recordId 
            /*var clientGetRecordId = new HttpClient();
            var requestGetRecordId = new HttpRequestMessage(HttpMethod.Post, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/_find?recordId=1");
            requestGetRecordId.Headers.Add("Authorization", "Bearer " + token);

            var contentGetRecordId = new StringContent("{\r\n    \"query\" :\r\n    [\r\n        {\r\n          \"UTE_nrID\": \"" + id + "\"\r\n        }\r\n    ]\r\n}", null, "application/json");
            //var content = new StringContent("{\r\n    \"query\" :\r\n    [\r\n        {\r\n          \"UTE_nrID\": \"0001\"\r\n        }\r\n    ]\r\n}", null, "application/json");
            requestGetRecordId.Content = contentGetRecordId;

            var responseGetRecordId = await clientGetRecordId.SendAsync(requestGetRecordId);
            responseGetRecordId.EnsureSuccessStatusCode();
            if (!responseGetRecordId.IsSuccessStatusCode)
            {
                var errorMessage = await responseGetRecordId.Content.ReadAsStringAsync();
            }
           


            string jsonStrT = responseGetRecordId.Content.ReadAsStringAsync().Result;
            JObject jsonObj = JObject.Parse(jsonStrT);

            var recordId = jsonObj["response"]["data"]["recordId"];*/


            if (ModelState.IsValid)
            {
                //Fazer update a partir do recordId
                var clientUpdate = new HttpClient();
                //var requestUpdate = new HttpRequestMessage(HttpMethod.Patch, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records/" + recordId + "");
                var requestUpdate = new HttpRequestMessage(HttpMethod.Patch, "https://rcsa.seekdata.pt/fmi/data/v2/databases/GAS DEV/layouts/LD_Utentes_API/records/481");
                requestUpdate.Headers.Add("Authorization", "Bearer " + token);


                var contentUpdate = new StringContent("{\r\n\"fieldData\":{ \"UTE_cpostal\": \"" + utente.CodigoPostal + "\",\r\n              \"UTE_nif\": \"" + utente.NIF + "\"\r\n            }\r\n}", null, "application/json");
                /*var jsonObject = new JObject(
                     new JProperty("fieldData", new JObject(
                         new JProperty("UTE_cpostal", utente.CodigoPostal),
                         new JProperty("UTE_nif", utente.NIF)
                     ))
                );
                var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");*/
                requestUpdate.Content = contentUpdate;
                var responseUpdate = await clientUpdate.SendAsync(requestUpdate);
                responseUpdate.EnsureSuccessStatusCode();

                if (!responseUpdate.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to retrieve data from the API");
                }

                return RedirectToAction(nameof(Index));
            }



            return View(utente);

        }
    }
}
