using LabClient.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Xml.Schema;
using System.Xml;
using LabClient.Models;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace LabClient.Controllers
{
    public class UploadController : Controller
    {
        private readonly ILogger<UploadController> _logger;
        private readonly IConfiguration _configuration;

        public UploadController(ILogger<UploadController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration; 
        }

        public IActionResult Index()
        {
            return View();
        }

        // Action for handling errors, returns the Error view with error details
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // POST action to handle XML and Schema file upload
        [HttpPost]
        public async Task<IActionResult> Index(XMLandSchemaFileUpload xmlAndSchemaFiles)
        {
            // Extracting files from the form submission
            IFormFile schemaFile = xmlAndSchemaFiles.SchemaFile;
            IFormFile xmlFile = xmlAndSchemaFiles.XmlFile;
            // Setting up XML schema validation
            XmlSchemaSet sc = new XmlSchemaSet();
            using (XmlReader schemaReader = XmlReader.Create(schemaFile.OpenReadStream()))
            {
                sc.Add(null, schemaReader);
            }

            // Configuring XML reader settings for validation
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            // List to store validation errors
            List<XmlValidationError> validationResults = new List<XmlValidationError>();
            settings.ValidationEventHandler += (s, e) =>
            {
                validationResults.Add(new XmlValidationError
                {
                    Element = ((XmlReader)s).Name,
                    ErrorType = e.Severity.ToString(),
                    Line = e.Exception.LineNumber,
                    Column = e.Exception.LinePosition,
                    Message = e.Message
                });
            };

            // 
            try
            {
                using (XmlReader xmlReader = XmlReader.Create(xmlFile.OpenReadStream(), settings))
                {
                    while (xmlReader.Read()) ;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reading XML.");
            }

            
            if (!validationResults.Any())
            {
                ViewData["XmlFileName"] = xmlFile.FileName;
                ViewData["SchemaFileName"] = schemaFile.FileName;
                XDocument xmlDoc = XDocument.Load(xmlFile.OpenReadStream());

                string serverUrl = _configuration.GetValue<string>("ServerURL");
                var xBook = xmlDoc.Descendants("Book").FirstOrDefault();
                if (xBook != null)
                {
                    // Creating book object from XML data
                    var book = new Book
                    {
                        
                        Title = xBook.Element("Title")?.Value,
                        Author = xBook.Element("Author")?.Value,
                        PublicationYear = DateTime.Parse(xBook.Element("PublicationYear")?.Value),
                        IsCheckedOut = bool.Parse(xBook.Element("IsCheckedOut")?.Value)
                    };

                    // 
                    string jsonContent = JsonSerializer.Serialize(book);
                    _logger.LogInformation("JSON content to be sent: {JsonContent}", jsonContent);

                    // 
                    using (var httpClient = new HttpClient())
                    {
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                        var response = await httpClient.PostAsync(serverUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            return View("Success");
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            _logger.LogError("API fail: StatusCode: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                            return View("Error");
                        }
                    }
                }
                else
                {
                    //
                    _logger.LogError("No book found in the XML.");
                    return View("Error");
                }
            }
            else
            {
                // // Returning view with validation errors
                ViewData["XmlFileName"] = xmlFile.FileName;
                ViewData["SchemaFileName"] = schemaFile.FileName;
                return View("Error", validationResults);
            }
        }
    }
}
