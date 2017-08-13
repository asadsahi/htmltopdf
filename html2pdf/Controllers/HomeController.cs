using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Microsoft.AspNetCore.NodeServices;
using html2pdf.Services;
using html2pdf.Model;

namespace html2pdf.Controllers
{
    public class HomeController : Controller
    {
        private readonly INodeServices nodeServices;
        private readonly IViewRenderService viewRenderService;

        public HomeController(INodeServices nodeServices, IViewRenderService viewRenderService)
        {
            this.nodeServices = nodeServices;
            this.viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CshtmlPdf()
        {
            List<Person> model = GetSampleData();
            var htmlContent = await viewRenderService.RenderToStringAsync("Home/Contact", model);

            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            HttpContext.Response.ContentType = "application/pdf";

            HttpContext.Response.Headers.Add("x-filename", "report.pdf");
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

       [HttpGet]
        public async Task<IActionResult> HtmlPdf()
        {
            HttpClient hc = new HttpClient();
            var htmlContent = await hc.GetStringAsync($"http://{Request.Host}/report.html");

            var result = await nodeServices.InvokeAsync<byte[]>("./pdf", htmlContent);

            HttpContext.Response.ContentType = "application/pdf";

            HttpContext.Response.Headers.Add("x-filename", "report.pdf");
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            HttpContext.Response.Body.Write(result, 0, result.Length);
            return new ContentResult();
        }

        private List<Person> GetSampleData()
        {
            var people = new List<Person>();
            for (int i = 1; i < 10000; i++)
            {
                people.Add(new Person { Id = i, Name = $"Test name {i.ToString()}", Age = 1, DOB = DateTime.Now.AddYears(-30).AddMonths(i) });
            }
            return people;
        }


    }
}
