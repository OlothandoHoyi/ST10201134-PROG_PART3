using POE_part2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace POE_part2.Controllers
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
            //get the connection string from the connection class
            connection conn = new connection();
            //check the connection
            try
            {
                //THEN CHECK
                using (SqlConnection connect = new SqlConnection(conn.Connecting()))
                {
                    connect.Open();
                    Console.WriteLine("Connected");
                    connect.Close();

                }
            }
            catch (Exception error)
            {
                //error message
                Console.WriteLine("Error :" + error.Message);
            }

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
        //http post for register
        //form the register form
        public IActionResult Register_user(register add_user)
        {
            //collect user's value
            string name = add_user.username;
            string email = add_user.email;
            string password = add_user.password;
            string role = add_user.role;

            //check if all are collected
            //Console.WriteLine("Name: " + name + "\nEmail: " + email + "Role: " + role);
            //pass all the values to insert method
            string message = add_user.insert_user(name, email, role, password);

            //then check if the user is inserted
            if (message == "done")
            {
                //redirect
                return RedirectToAction("Login", "Home");
            }
            else
            {
                //track error output
                Console.Write(message);
                //redirect
                return RedirectToAction("Login", "Home");
            }
            //redirect
            return RedirectToAction("Index", "Home");
        }
        //for login page
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpPost]
        public IActionResult login_user(check_login user)
        {
            string email = user.email;
            string role = user.role;
            string password = user.Password;

            string message = user.login_user(email, role, password);

            if (message == "found")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public IActionResult Claim_sub(IFormFile file, Claim insert)
        {
            //assign
            string module_name = insert.user_email;
            string hours_worked = insert.hours_worked;
            string hour_rate = insert.hour_rate;
            string description = insert.description;

            //file info
            string filename = "No file";
            if (file != null && file.Length > 0)
            {
                // Get the file name
                filename = Path.GetFileName(file.FileName);

                // Define the folder path (pdf folder)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");

                // Ensure the pdf folder exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Define the full path where the file will be saved
                string filePath = Path.Combine(folderPath, filename);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);



                }
            }


            string message = insert.insert_claim(module_name, hours_worked, hour_rate, description, filename);

            if (message == "done")
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
            else
            {
                Console.WriteLine(message);
                return RedirectToAction("Dashboard", "Home");
            }
        }
        public IActionResult view_claims()
        {
            get_claims collect = new get_claims();

            return View(collect);
        }

        public IActionResult approve_claims()
        {
            get_claims collect = new get_claims();


            return View(collect);
        }

        public async Task<IActionResult> ReviewClaims()
        // Implement approval workflows to streamline the verification and approval process.
        {
            var claims = await _context.Claims.Where(c => !c.IsApproved).ToListAsync();
            return View(claims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int id, bool isApproved, string comments)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim != null)
            {
                claim.IsApproved = isApproved;
                claim.Comments = comments;
                _context.Update(claim);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ReviewClaims");
        }

        [Authorize(Roles = "HR")]
        public ActionResult ExportToPdf()
        {
            // Fetch approved claims
            var claims = _context.Claims.Where(c => c.Status == "Approved").ToList();

            // Create a PDF document in memory
            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document();
                PdfWriter.GetInstance(document, memoryStream).CloseStream = false;

                document.Open();
                document.Add(new iTextSharp.text.Paragraph("Approved Claims Report"));

                // Create a table with headers
                var table = new iTextSharp.text.pdf.PdfPTable(5);
                table.AddCell("Lecturer Name");
                table.AddCell("Hours Worked");
                table.AddCell("Hourly Rate");
                table.AddCell("Final Payment");
                table.AddCell("Date Approved");

                // Populate the table with claim data
                foreach (var claim in claims)
                {
                    table.AddCell(claim.LecturerName);
                    table.AddCell(claim.HoursWorked.ToString());
                    table.AddCell(claim.HourlyRate.ToString("C"));
                    table.AddCell(claim.FinalPayment.ToString("C"));
                    table.AddCell(claim.DateApproved?.ToString("yyyy-MM-dd") ?? "");
                }

                document.Add(table);
                document.Close();

                // Return the PDF as a downloadable file
                memoryStream.Position = 0;
                return File(memoryStream, "application/pdf", "ApprovedClaimsReport.pdf");
            }
        }
    }
}

