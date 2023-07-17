using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JV_Imprest_Payment.Data;
using JV_Imprest_Payment.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Globalization;
using System.Security.Claims;
using JV_Imprest_Payment.Services;
using OfficeOpenXml;
using JV_Imprest_Payment.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace JV_Imprest_Payment.Controllers
{
    [Authorize]
    public class PayRequestsController : Controller
    {
        
        private readonly IEmailSender _mailSender;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public PayRequestsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IEmailSender mailSender, UserManager<IdentityUser> userManager)
        {
            _context = context;

            _mailSender = mailSender;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }


       

        public async Task<IActionResult> Index()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            // Create a view model to pass the data to the view
            var viewModel = new PayRequestViewModel();

            switch (userRole)
            {
                case "Partner":
                    // Retrieve the requests raised by the signed-in user
                    var myRequests = await _context.PayRequest
                        .Include(p => p.Category)
                        .Where(p => p.CreatedBy == userName)
                        .ToListAsync();


                    var inProgressRequests = await _context.PayRequest
                        .Where(p => p.CreatedBy == userName && p.Status == "In Progress")
                        .ToListAsync();

                    var approvedRequests = await _context.PayRequest
                        .Where(p => p.CreatedBy == userName && p.Status == "Approved")
                        .ToListAsync();

                    var rejectedRequests = await _context.PayRequest
                        .Where(p => p.CreatedBy == userName && p.Status == "Rejected")
                        .ToListAsync();
                    viewModel.PayRequests = myRequests;
                    viewModel.MyRequestsCount = myRequests;
                    viewModel.InProgressCount = inProgressRequests;
                    viewModel.ApprovedCount = approvedRequests;
                    viewModel.RejectedCount = rejectedRequests;
                    break;

                case "JVA":
                    // Retrieve the total number of requests in the system
                    var totalRequests = await _context.PayRequest.CountAsync();

                    var accountantRequests = await _context.PayRequest
                        .Include(p => p.Category)
                        .ToListAsync();
                    var pendingRequests = await _context.PayRequest
                        .Where(p => p.Status == "JVA")
                        .ToListAsync();

                    var actionedRequests = await _context.PayRequest
                        .Where(p => p.Status == "AFE")
                        .ToListAsync();

                    // Calculate accountant's performance
                    var accountantPerformance = (double)actionedRequests.Count / totalRequests * 100;

                    viewModel.PayRequests = accountantRequests;
                    viewModel.TotalRequestCount = totalRequests;
                    viewModel.PendingRequestCount = pendingRequests;
                    viewModel.ActionedRequestCount = actionedRequests;
                    viewModel.AccountantPerformance = accountantPerformance;
                    break;
                case "AFE":

                    var user = await _userManager.FindByNameAsync(userName);
                    var userId = user?.Id;

                    var filteredContracts = await _context.PayRequest
                        .Include(p => p.Category)
                        .Where(p => p.Assignedto == userId)
                        .ToListAsync(); ;

                    // Retrieve the contracts assigned to the signed-in AFE
                    var myContracts = await _context.PayRequest
                        .Include(p => p.Category)
                        .Where(p => p.Assignedto == userId)
                        .ToListAsync();

                    var approvedContracts = await _context.PayRequest
                        .Where(p => p.Assignedto == userId && p.Status == "Approved")
                        .ToListAsync();

                    var rejectedContracts = await _context.PayRequest
                        .Where(p => p.Assignedto == userId && p.Status == "Rejected")
                        .ToListAsync();

                    // Calculate AFE's performance
                    var afePerformance = (double)approvedContracts.Count / (approvedContracts.Count + rejectedContracts.Count) * 100;

                    viewModel.PayRequests = myContracts;
                    viewModel.AFEContracts = myContracts;
                    viewModel.AFEApproved = approvedContracts;
                    viewModel.AFERejected = rejectedContracts;
                    viewModel.AFEPerformance = afePerformance;
                    break;

                case "Admin":
                    // Retrieve the total number of requests in the system
                    var adminTotalRequests = await _context.PayRequest.CountAsync();

                    var pendingAccountants = await _context.PayRequest
                        .Where(p => p.Status == "JVA" || p.Status == "AFE" && !string.IsNullOrEmpty(p.Assignedto))
                        .ToListAsync();

                    var pendingPartners = await _context.PayRequest
                        .Where(p => p.Status == "In Progress" && !string.IsNullOrEmpty(p.ActionedBy))
                        .ToListAsync();

                    var adminApprovedRequests = await _context.PayRequest
                        .Where(p => p.Status == "Approved")
                        .ToListAsync();

                    var adminRejectedRequests = await _context.PayRequest
                        .Where(p => p.Status == "Rejected")
                        .ToListAsync();
                    var payRequests = await _context.PayRequest
                        .Include(p => p.Category)
                        .ToListAsync();
                    var users = await _context.Users.ToListAsync();

                    // Calculate overall performance
                    var overallPerformance = (double)adminApprovedRequests.Count / (adminApprovedRequests.Count + adminRejectedRequests.Count) * 100;

                    viewModel.PayRequests = payRequests;
                    viewModel.TotalRequestCount = adminTotalRequests;
                    viewModel.OverAllPendingAccountant = pendingAccountants;
                    viewModel.OverAllPendingAFE = pendingPartners;
                    viewModel.OverAllApproved = adminApprovedRequests;
                    viewModel.OverAllRejected = adminRejectedRequests;
                    viewModel.OverAllPerformance = overallPerformance;
                    viewModel.Users = users;

                    break;

                default:
                    // Handle the case when the user role is unknown or not specified
                    // You can return an error view or redirect to an appropriate page
                    return RedirectToAction("Index", "Home");
            }

            // Pass the user role to the view
            ViewBag.UserRole = userRole;

            return View(viewModel);
        }



        // GET: PayRequests/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payRequest = await _context.PayRequest.FirstOrDefaultAsync(m => m.Id == id);
            if (payRequest == null)
            {
                return NotFound();
            }

            return PartialView("_DetailsModalPartial", payRequest);
        }


        // GET: PayRequests/Create
        public async Task<IActionResult> CreateAsync()
        {


            var afeStructures = _context.AfeStructure.AsNoTracking();

            var payRequests = _context.PayRequest.AsNoTracking();

            var viewModel = new PayRequestViewModel
            {
                //PayRequestModel = (PayRequest)payRequests,
                GetAfeStructures = new List<AfeStructure>(await afeStructures.ToListAsync()) { },

            };
            return View(viewModel);
        }



        // POST: PayRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Created([Bind("Id,Description,TransactionDate,Category,CreatedBy,ApprovedBy,ActionedBy,ProcessDate,ProcessStatus,EvidenceOfPayment")] PayRequestViewModel payRequest, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                
                _context.Add(payRequest);
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    payRequest.PayRequestModel.EvidenceOfPayment = fileName;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayRequestViewModel model, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var userName = User.FindFirstValue(ClaimTypes.Name);
                PayRequest newPayrequest = new PayRequest
                {
                    CategoryId = model.PayRequestModel.CategoryId,
                    Description = model.PayRequestModel?.Description,
                    TransactionDate = model.PayRequestModel.TransactionDate,
                    ProcessDate = model.PayRequestModel.ProcessDate,
                    //EvidenceOfPayment
                    ProcessStatus = model.PayRequestModel.ProcessStatus,
                    CreatedBy = userName,
                    Status = "JVA",
                    Requester = userName,
                    RequesterDate = DateTime.Today

                };
                

                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    newPayrequest.EvidenceOfPayment = fileName;
                }
                _context.Add(newPayrequest);
                await _context.SaveChangesAsync();
                // The Notification
                var emailBody = @"<div style=""font-family: 'Helvetica Neue', Arial, sans-serif; color: #434383; max-width: 600px; margin: 0 auto; padding: 20px;""> 
                    <p>Hello,</p>
                    <p>A new record(s) have been uploaded.</p>
                    <p><u>See details below:</u></p>
                    <p>Partner: <b>" + userName + @"</b></p>
                    <p>Date: <b>" + DateTime.Now.ToString("dd MMM yyyy") + @"</b></p>
                    <p>Kindly <a href=""https://jvaimpreststest.oandoplc.com:4472"">click here</a> to view on the application.</p>
                    <p><b>Regards.</b></p>
                    </div>";


                var to = "oajibade@oandoenergyresources.com";
                var cc = userName;

                _mailSender.SendEmailAsync(to, "JV Analysis Updated", emailBody);
                return RedirectToAction(nameof(Index));

            }
            TempData["WrongMessage"] = "Something Went Wrong";
            return RedirectToAction("Create");


        }

        // GET: PayRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PayRequest == null)
            {
                return NotFound();
            }

            var payRequest = await _context.PayRequest.FindAsync(id);
            if (payRequest == null)
            {
                return NotFound();
            }

            // Retrieve user data from the database
            var users = await _context.Users.ToListAsync();

            // Pass the user data to the view
            ViewBag.Users = users;

            return View(payRequest);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(PayRequest payRequest)
        {
            if (!ModelState.IsValid)
            {
                // Handle invalid form data
                // You can display an error message or redirect back to the form
                return RedirectToAction(nameof(Index));
            }

            var userName = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                var existingPayRequest = await _context.PayRequest.FindAsync(payRequest.Id);
                if (existingPayRequest == null)
                {
                    return NotFound();
                }

                existingPayRequest.Assignedto = payRequest.Assignedto;
                existingPayRequest.ActionedBy = userName;
                existingPayRequest.Status = "AFE";

                // Mark the specific fields to be updated
                _context.Entry(existingPayRequest).Property(x => x.Assignedto).IsModified = true;
                _context.Entry(existingPayRequest).Property(x => x.ActionedBy).IsModified = true;

                // Save changes to the database
                await _context.SaveChangesAsync();

                // The Notification
                var emailBody = @"<div style=""font-family: 'Helvetica Neue', Arial, sans-serif; color: #434383; max-width: 600px; margin: 0 auto; padding: 20px;""> 
            <p>Hello,</p>
            <p> A record has been assigned to you.</p>
            <p><u>See details below:</u></p>
            <p>Description: <b>" + payRequest.Description + @"</b></p>
            <p>Category: <b>" + payRequest.Category + @"</b></p>
            <p>Date: <b>" + DateTime.Now.ToString("dd MMM yyyy") + @"</b></p>
            <p>Kindly <a href=""https://imprestpayment.oandoplc.com/"">click here</a> to review the application.</p>
            <p><b>Regards.</b></p>
            </div>";

                var to = "oajibade@oandoenergyresources.com";
                var cc = userName;

                _mailSender.SendEmailAsync(to, "JV Analysis Updated", emailBody);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayRequestExists(payRequest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var payRequest = await _context.PayRequest.FindAsync(id);
            if (payRequest == null)
            {
                return NotFound();
            }

            var userName = User.FindFirstValue(ClaimTypes.Name);

            payRequest.Status = "Approved";
            payRequest.JVA = userName;
            payRequest.JVADate = DateTime.Today;

            _context.Update(payRequest);
            await _context.SaveChangesAsync();

            // The Notification
            var emailBody = @"<div style=""font-family: 'Helvetica Neue', Arial, sans-serif; color: #434383; max-width: 600px; margin: 0 auto; padding: 20px;""> 
        <p>Hello,</p>
        <p>A Payment Imprest has been approved for action.</p>
        <p><u>See details below:</u></p>
        <p>Approved By: <b>" + userName + @"</b></p>
        <p>Description: <b>" + payRequest.Description + @"</b></p>
        <p>Date: <b>" + DateTime.Now.ToString("dd MMM yyyy") + @"</b></p>
        <p>Kindly <a href=""https://imprestpayment.oandoplc.com/"">click here</a> to view on the application.</p>
        <p><b>Regards.</b></p>
        </div>";

            var to = "oajibade@oandoenergyresources.com";
            var cc = userName;

            _ = _mailSender.SendEmailAsync(to, "JV Imprest Approved", emailBody);

            return RedirectToAction(nameof(Index));
        }
         [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var payRequest = await _context.PayRequest.FindAsync(id);
            if (payRequest == null)
            {
                return NotFound();
            }

            var userName = User.FindFirstValue(ClaimTypes.Name);

            payRequest.Status = "Rejected";
            payRequest.JVA = userName;
            payRequest.JVADate = DateTime.Today;

            _context.Update(payRequest);
            await _context.SaveChangesAsync();

            // The Notification
            var emailBody = @"<div style=""font-family: 'Helvetica Neue', Arial, sans-serif; color: #434383; max-width: 600px; margin: 0 auto; padding: 20px;""> 
        <p>Hello,</p>
        <p>A Payment Imprest has been rejected.</p>
        <p><u>See details below:</u></p>
        <p>Rejected By: <b>" + userName + @"</b></p>
        <p>Description: <b>" + payRequest.Description + @"</b></p>
        <p>Date: <b>" + DateTime.Now.ToString("dd MMM yyyy") + @"</b></p>
        <p>Kindly <a href=""https://imprestpayment.oandoplc.com/"">click here</a> to view on the application.</p>
        <p><b>Regards.</b></p>
        </div>";

            var to = "oajibade@oandoenergyresources.com";
            var cc = userName;

            _ = _mailSender.SendEmailAsync(to, "JV Imprest Rejected", emailBody);

            return RedirectToAction(nameof(Index));
        }


      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,TransactionDate,Category,CreatedBy,ApprovedBy,ActionedBy,ProcessDate,ProcessStatus")] PayRequest payRequest, IFormFile file)
        {
            if (id != payRequest.Id)
            {
                return NotFound();
            }

            var users = await _context.Users.ToListAsync();
            var userName = User.FindFirstValue(ClaimTypes.Name);

            try
            {
                // Retrieve the existing payRequest from the database
                var existingPayRequest = await _context.PayRequest.FindAsync(payRequest.Id);
                if (existingPayRequest == null)
                {
                    return NotFound();
                }

                // Update the fields with new values
                existingPayRequest.CreatedBy = userName;
                existingPayRequest.Status = "JVA";

                if (file != null && file.Length > 0)
                {
                    // Save new file
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", file.FileName);
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Update file path in payRequest when a new file is uploaded
                    existingPayRequest.EvidenceOfPayment = file.FileName;
                }

                // Update payRequest in database
                _context.Update(existingPayRequest);
                await _context.SaveChangesAsync();

                ViewBag.Users = users;

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayRequestExists(payRequest.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }



        //To post by uploading an excel sheet
        public async Task<IActionResult> Import(IFormFile file, int categoryId)
        {
            if (file != null)
            {
                if (categoryId == 0)
                {
                    TempData["WrongMessage"] = "Please select a category.";
                    return RedirectToAction("Create", "PayRequests");
                }
                var list = new List<PayRequest>();
                var userName = User.FindFirstValue(ClaimTypes.Name);

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        using (var package = new ExcelPackage(stream))
                        {
                            var howMany = 0;
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                            var rowcount = worksheet.Dimension.Rows;
                            var cultureInfo = new CultureInfo("en-US");
                            for (int row = 2; row <= rowcount; row++)
                            {
                                if (ModelState.IsValid)
                                {
                                    if (DateTime.TryParse(worksheet.Cells[row, 2].Value.ToString().Trim(), out DateTime transactionDate))
                                    {
                                        // The string was successfully parsed to a DateTime object
                                        _context.Add(new PayRequest
                                        {
                                            Description = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                            TransactionDate = transactionDate,
                                            CreatedBy = userName,
                                            CategoryId = categoryId,
                                            Status = "JVA"
                                            // ...
                                        });

                                        howMany++; // Increment the counter for successful imports
                                    }
                                    else
                                    {
                                        // The string was not in the correct format
                                        TempData["WrongMessage"] = "Something Went Wrong: Invalid date format.";
                                        return RedirectToAction("Create", "PayRequests");
                                    }
                                }
                                else
                                {
                                    TempData["WrongMessage"] = "Something Went Wrong: Check the data types and for empty spaces.";
                                    return RedirectToAction("Index", "PayRequests");
                                }
                            }

                            // Save changes outside the loop after all the rows have been processed
                            await _context.SaveChangesAsync();

                            TempData["AlertMessage"] = "Database Updated Successfully";

                            // The Notification
                            var emailBody = @"<div style=""font-family: 'Helvetica Neue', Arial, sans-serif; color: #434383; max-width: 600px; margin: 0 auto; padding: 20px;""> 
                            <p>Hello,</p>
                            <p>" + howMany + @" record(s) have been submitted.</p>
                            <p><u>See details below:</u></p>
                            <p>Created By: <b>" + userName + @"</b></p>
                            <p>Number of imprest payment: <b>" + howMany + @"</b></p>
                            <p>Category: <b>" + categoryId + @"</b></p>
                            <p>Date: <b>" + DateTime.Now.ToString("dd MMM yyyy") + @"</b></p>
                            <p>Kindly <a href=""https://imprestpayment.oandoplc.com/"">click here</a> to view on the application.</p>
                            <p><b>Regards.</b></p>
                            </div>";


                            var to = "oajibade@oandoenergyresources.com";
                            var cc = userName;

                            _ = _mailSender.SendEmailAsync(to, "JV Imprest Payment Submitted", emailBody);

                            return RedirectToAction("Index", "PayRequests");
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["SuccessMessage"] = "An error occurred during import: " + ex.Message;
                    return RedirectToAction("Index", "PayRequests");
                }
            }
            else
            {
                //This allows them to create if they don't want to upload from excel
                return RedirectToAction(nameof(Create));
            }
        }



        public IActionResult ExportExcel()
        {
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                var headers = new string[] { "Description", "Tansaction Date (Format = Date)" }; //replace with actual header names
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"ImprestsRecord-{DateTime.Now.ToString("yyyymmddhhmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }







        [HttpPost]
        public async Task<IActionResult> EditMultiples(int[] selectedItems, [Bind("Id,Description,TransactionDate,Category,CreatedBy,ApprovedBy,ActionedBy,ProcessDate,ProcessStatus,EvidenceOfPayment")] PayRequest[] payRequests, List<IFormFile> files)
        {
            if (selectedItems == null || selectedItems.Length == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var items = _context.PayRequest.Where(i => selectedItems.Contains(i.Id)).ToList();

            if (items == null || items.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            for (int i = 0; i < items.Count; i++)
            {
                var payRequest = items[i];
                var newPayRequest = payRequests[i];

                payRequest.Description = newPayRequest.Description;
                payRequest.TransactionDate = newPayRequest.TransactionDate;
                payRequest.Category = newPayRequest.Category;
                payRequest.CreatedBy = newPayRequest.CreatedBy;
                payRequest.ApprovedBy = newPayRequest.ApprovedBy;
                payRequest.ActionedBy = newPayRequest.ActionedBy;
                payRequest.ProcessDate = newPayRequest.ProcessDate;
                payRequest.ProcessStatus = newPayRequest.ProcessStatus;

                if (files[i] != null && files[i].Length > 0)
                {
                    var fileName = Path.GetFileName(files[i].FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await files[i].CopyToAsync(fileStream);
                    }
                    payRequest.EvidenceOfPayment = fileName;
                }

                _context.Update(payRequest);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // POST: PayRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edits(int id, [Bind("Id,Description,TransactionDate,Category,CreatedBy,ApprovedBy,ActionedBy,ProcessDate,ProcessStatus,EvidenceOfPayment")] PayRequest payRequest)
        {
            if (id != payRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayRequestExists(payRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payRequest);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMultiple(List<PayRequest> payRequests, List<IFormFile> files)
        {
            if (payRequests.Count != files.Count)
            {
                ModelState.AddModelError("", "The number of PayRequest objects must match the number of files.");
                return View(payRequests);
            }

            for (int i = 0; i < payRequests.Count; i++)
            {
                var payRequest = payRequests[i];
                var file = files[i];

                if (ModelState.IsValid)
                {
                    _context.Add(payRequest);
                    if (file != null && file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        payRequest.EvidenceOfPayment = fileName;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: PayRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PayRequest == null)
            {
                return NotFound();
            }

            var payRequest = await _context.PayRequest
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payRequest == null)
            {
                return NotFound();
            }

            return View(payRequest);
        }

        // POST: PayRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PayRequest == null)
            {
                return Problem("Entity set 'ApplicationDbContext.PayRequest'  is null.");
            }
            var payRequest = await _context.PayRequest.FindAsync(id);
            if (payRequest != null)
            {
                _context.PayRequest.Remove(payRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayRequestExists(int id)
        {
          return (_context.PayRequest?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
