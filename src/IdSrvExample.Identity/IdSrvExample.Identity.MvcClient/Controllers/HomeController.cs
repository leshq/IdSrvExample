using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdSrvExample.Identity.TestMvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        [Authorize]
        [Route("private")]
        public async Task<IActionResult> Authorized()
        {
            return View();
        }

        [Authorize]
        [Route("apisecret")]
        public async Task<IActionResult> ApiPrivateTask()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            string result = await GetApiData(accessToken, "https://localhost:5010/authtest/private");

            ViewBag.Result = result;

            return View("Secret");
        }

        [Authorize]
        [Route("apiadmin")]
        public async Task<IActionResult> ApiAdmin()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            string result = await GetApiData(accessToken, "https://localhost:5010/authtest/admin");

            ViewBag.Result = result;

            return View("Secret");
        }

        [Authorize(Roles = "admin")]
        [Route("admin")]
        public async Task<IActionResult> Admin()
        {
            return View();
        }

        [HttpGet]
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return Ok("You shall not pass");
        }

        private async Task<string> GetApiData(string accessToken, string url)
        {
            var apiClient = _httpClientFactory.CreateClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}