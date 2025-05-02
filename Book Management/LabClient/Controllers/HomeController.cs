using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LabClient.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using Microsoft.Extensions.Configuration;


namespace LabClient.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;
        private string serviceUrl;

        // Constructor to initialize configuration and service URL
        public HomeController(IConfiguration config)
        {
            _configuration = config;
            serviceUrl = _configuration.GetValue<string>("ServerURL");
            if (string.IsNullOrEmpty(serviceUrl))
            {
                throw new InvalidOperationException("ServerURL is not configured in appsettings.json.");
            }
        }
        // Action for the Index page. Retrieves a list of books from the API.
        public IActionResult Index()
        {
            List<Book> bookList = new List<Book>(); 
            using (var httpClient = new HttpClient())
            {
                string apiUrl = $"{serviceUrl}"; 
                var response = httpClient.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    bookList = JsonSerializer.Deserialize<List<Book>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    return View("Error");
                }
            }
            return View(bookList);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Book book = null;
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($"{serviceUrl}/{id}").Result; 
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    book = JsonSerializer.Deserialize<Book>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    return NotFound();
                }
            }

            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            using (var httpClient = new HttpClient())
            {
                var jsonContent = JsonSerializer.Serialize(book, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = httpClient.PutAsync($"{serviceUrl}/{book.BookID}", contentString).Result; //ooks API
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error");
                }
            }
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(Book book)
        {
            if (!ModelState.IsValid)
            {
                return View(book);
            }

            using (var httpClient = new HttpClient())
            {
                var jsonContent = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync($"{serviceUrl}", jsonContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the data.");
                    return View(book);
                }
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.DeleteAsync($"{serviceUrl}/{id.Value}").Result;  

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

       
 


         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

}
