using ExcelDataReader;
using ExcelFileReader.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ExcelFileReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ExcelFileReader()
        { return View(); }

        [HttpPost]
        public async Task<IActionResult> ExcelFileReader(IFormFile file)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Upload File
            if (file != null && file.Length > 0)
            {
                var uploadDirectory = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";

                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var filePath = Path.Combine(uploadDirectory, file.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //Read File
                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    var excelData = new List<List<object>>();
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                var rowData = new List<object>();
                                for (int column = 0; column < reader.FieldCount; column++)
                                {
                                    rowData.Add(reader.GetValue(column));
                                }
                                excelData.Add(rowData);
                            }
                        } while (reader.NextResult());

                        ViewBag.ExcelData = excelData;
                    }
                }
            }

            return View();
        }
    }
}