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
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
                ViewBag.Role = roleClaim?.Value; 
            }
            catch
            {
               
                return RedirectToAction("Login");
            }


            List<Book> bookList = new List<Book>(); 
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                string apiUrl = $"{serviceUrl}"; 
                var response = httpClient.GetAsync(apiUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    bookList = JsonSerializer.Deserialize<List<Book>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return RedirectToAction("Login");
                    }
                    return View("Error");
                }
            }
            return View(bookList);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();


            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            Book book = null;
            using (var httpClient = new HttpClient())
            {
              
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
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

            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid) return View(book);

            
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            using (var httpClient = new HttpClient())
            {
             
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonSerializer.Serialize(book, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = httpClient.PutAsync($"{serviceUrl}/{book.BookID}", contentString).Result; 
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
     
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");
            
            return View();
        }

        [HttpPost]
        public IActionResult New(Book book)
        {
            if (!ModelState.IsValid) return View(book);

          
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            using (var httpClient = new HttpClient())
            {
            
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var jsonContent = new StringContent(JsonSerializer.Serialize(book), Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync($"{serviceUrl}", jsonContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No permission or an error occurred while saving data");
                    return View(book);
                }
            }
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

         
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            try
            {
                using (var httpClient = new HttpClient())
                {
            
                    httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            using var _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var loginData = new { Username = username, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://api:8080/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseString);
                var token = result.GetProperty("token").GetString();

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddHours(2)
                };
                HttpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

                return RedirectToAction("Index");
            }
            ViewBag.Error = " Invalid username or password";
            return View();
        }

 
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}