using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace IdSrvExample.Identity.Server.Auth
{
    public class AuthController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            // todo: check if model is valid
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            return await LoginCore(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrWhiteSpace(logoutRequest.PostLogoutRedirectUri))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        private async Task<IActionResult> LoginCore(LoginViewModel vm)
        {
            SignInResult signInResult = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);

            if (signInResult.Succeeded)
            {
                return Redirect(vm.ReturnUrl);
            }
            else if (signInResult.IsLockedOut)
            {
                return BadRequest("Account is locked");
            }
            else
            {
                return BadRequest("Failed to log in");
            }
        }

        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            // todo: check if model is valid
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = new IdentityUser(vm.Username);

            var identityResult = await _userManager.CreateAsync(user, vm.Password);

            if (identityResult.Succeeded)
            {
                return await LoginCore(new LoginViewModel()
                {
                    Username = vm.Username,
                    Password = vm.Password,
                    ReturnUrl = vm.ReturnUrl
                });
            }
            else
            {
                return BadRequest(identityResult.Errors);
            }
        }
    }
}
