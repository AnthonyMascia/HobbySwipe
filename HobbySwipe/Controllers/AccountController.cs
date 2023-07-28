using HobbySwipe.Models;
using HobbySwipe.ViewModels.Account;
using HobbySwipe.ViewModels.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HobbySwipe.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfigurationRoot _config;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPasswordValidator<IdentityUser> _passwordValidator;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IConfigurationRoot config,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IPasswordValidator<IdentityUser> passwordValidator,
            ILogger<AccountController> logger)
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _passwordValidator = passwordValidator;
            _logger = logger;

        }

        /******************************************************************************************************************/
        /*** LOGIN                                                                                                      ***/
        /******************************************************************************************************************/
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            _logger.LogInformation("Login process initiated");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                // Clear the existing external cookie to ensure a clean login process.
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                // Setup the view model and return the view.
                return View(new LoginViewModel(returnUrl, null));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Login process: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login POST process initiated");
            try
            {
                // Check to see if values are valid.
                if (ModelState.IsValid)
                {
                    // Validate that user exists.
                    var user = await _userManager.FindByNameAsync(model.Email);
                    if (user == null)
                    {
                        _logger.LogWarning("User does not exist");
                        throw new Exception("The email or password is incorrect.");
                    }

                    // Keep track of the access failed to get the first time user is locked.
                    var accessFailedCountBefore = user.AccessFailedCount;

                    // Try to log the user in.
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                    // Handle the result.
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User successfully logged in.");
                        return LocalRedirect(model.ReturnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out");

                        // Check to see if this is the first time user is locked out.
                        // If so, send email to user letting them know and for option to reset password.
                        if (accessFailedCountBefore > 0 && user.AccessFailedCount == 0)
                        {
                            var callbackUrl = Url.Action(
                              nameof(PasswordResetRequest),
                              ControllerContext.ActionDescriptor.ControllerName,
                              new { },
                              Request.Scheme);

                            //_emailHelper.SendLockedOutEmail(model.Email, callbackUrl);
                        }

                        return RedirectToAction(nameof(Lockout));
                    }
                    if (result.IsNotAllowed)
                    {
                        _logger.LogWarning("User account email not confirmed.");
                        throw new Exception("Please confirm your email before logging in.");
                    }
                    else
                    {
                        _logger.LogError($"Password is incorrect.");

                        ModelState.AddModelError("ArgumentException", "The email or password is incorrect.");
                        throw new Exception($"You have {(5 - user.AccessFailedCount)} attempts remaining before lockout.");
                    }
                }

                return LocalRedirect(model.ReturnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Login POST process: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            _logger.LogInformation("ExternalLogin process initiated");
            try
            {
                // Request a redirect to the external login provider.
                var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during ExternalLogin process: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            _logger.LogInformation("ExternalLoginCallback process initiated");
            if (remoteError != null)
            {
                _logger.LogError($"Error from external provider: {remoteError}");
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", new LoginViewModel(returnUrl, null));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Error loading external login information.");
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("Login", new LoginViewModel(returnUrl, null));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("User successfully logged in with external provider.");
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction("Lockout");
            }
            else
            {
                _logger.LogInformation("Handling case where user's email might already be registered.");

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    // The email is already registered
                    var hasPassword = await _userManager.HasPasswordAsync(user);
                    if (hasPassword)
                    {
                        // The user has a local account
                        var addLoginResult = await _userManager.AddLoginAsync(user, info);
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User successfully logged in with external provider and local account.");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            AddErrors(addLoginResult);
                            return View("Login", new LoginViewModel(returnUrl, null));
                        }
                    }
                    else
                    {
                        // The user is already registered only with an external login
                        // We assume it's okay to add another external login
                        var addLoginResult = await _userManager.AddLoginAsync(user, info);
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("User successfully logged in with multiple external providers.");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            AddErrors(addLoginResult);
                            return View("Login", new LoginViewModel(returnUrl, null));
                        }
                    }
                }
                else
                {
                    // No user is registered with this email
                    user = new IdentityUser { UserName = email, Email = email };
                    var identityResult = await _userManager.CreateAsync(user);
                    if (identityResult.Succeeded)
                    {
                        identityResult = await _userManager.AddLoginAsync(user, info);
                        if (identityResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            _logger.LogInformation("New user created and logged in with external provider.");
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    AddErrors(identityResult);
                    return View("Login", new LoginViewModel(returnUrl, null));
                }
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            _logger.LogInformation("ExternalLoginConfirmation process initiated");
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogError("Error loading external login information during confirmation.");
                    return View("Error", new ErrorViewModel(errorMessage: "Error loading external login information during confirmation."));
                }
                var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true }; // todo: change 'EmailConfirmed' when email works
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("New user created and logged in with external provider.");
                        return RedirectToAction("Index", "Home");
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet("login/2fa")]
        public async Task<IActionResult> LoginTwoFactor(string emailAddress, bool rememberMe, string returnUrl = null)
        {
            _logger.LogInformation("2FA Login process initiated");
            try
            {
                // Variable declaration.
                var errorMessage = "You are not authorized to view this content at this time";

                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                // Validate that user exists.
                var user = await _userManager.FindByNameAsync(emailAddress);
                if (user == null)
                {
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                // Look to see if email two factor provider exists.
                var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
                if (!providers.Contains("Email"))
                {
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                // Generate the two factor token to be sent to user.
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                // Generate the callback URL to request a password reset.
                var callbackUrl = Url.Action(
                    nameof(PasswordResetRequest),
                    ControllerContext.ActionDescriptor.ControllerName,
                    new { },
                    Request.Scheme);

                // Send out the 2FA email.
                //_emailHelper.Send2FAEmail(emailAddress, token, callbackUrl);
                _logger.LogInformation("2FA email sent to user.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during 2FA Login process: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            // Redirect to view that tells user to check their email to input one-time code.
            return View(new LoginTwoFactorViewModel(emailAddress, rememberMe, returnUrl));
        }

        [HttpPost("login/2fa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginTwoFactor(LoginTwoFactorViewModel model)
        {
            _logger.LogInformation("2FA Login POST process initiated");
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate that user exists.
                    var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                    if (user == null)
                    {
                        _logger.LogError("Error with the two factor authentication process");
                        throw new Exception("There was an error with the two factor authentication process");
                    }

                    // Attempt to log the user in.
                    _logger.LogInformation($"Attempting to do 2FA log in user with email: {model.EmailAddress}");
                    var result = await _signInManager.TwoFactorSignInAsync("Email", model.OneTimeCode, model.RememberMe, rememberClient: false);

                    // Handle the result.
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User successfully logged in with 2FA.");
                        return LocalRedirect(model.ReturnUrl);
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        return RedirectToAction(nameof(Lockout));
                    }
                    else
                    {
                        _logger.LogError("Login failed. Invalid one-time code.");
                        throw new Exception("You have entered an invalid passcode. Either try entering it again or request a new code below.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during 2FA Login POST process: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            // Redirect to view.
            return View(model);
        }




        /******************************************************************************************************************/
        /*** REGISTER                                                                                                   ***/
        /******************************************************************************************************************/

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            _logger.LogInformation("Registration process initiated");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                return View(new RegisterViewModel(returnUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Registration process: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Registration POST process initiated");
            try
            {
                // Validate the registration model is correct.
                if (ModelState.IsValid)
                {
                    // Check if the entered password is valid.
                    var passwordValidation = await _passwordValidator.ValidateAsync(_userManager, null, model.Password);
                    if (!passwordValidation.Succeeded)
                    {
                        foreach (var error in passwordValidation.Errors)
                        {
                            ModelState.AddModelError("ArgumentException", error.Description);
                        }
                        return View(model);
                    }

                    // Password valid, try creating the user.
                    var user = new IdentityUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        EmailConfirmed = true, // todo: change 'EmailConfirmed' when email works
                        TwoFactorEnabled = false, // todo: change this to true when email works
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User created a new account with email: {model.Email}");

                        // Get the unique token to confirm the registration.
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                        // Set up the confirmation email and send it to the new user.
                        var callbackUrl = Url.Action(
                            nameof(ConfirmEmail),
                            ControllerContext.ActionDescriptor.ControllerName,
                            new { userId = user.Id, token = token, returnUrl = model.ReturnUrl },
                            Request.Scheme);

                        // Send out the confirmation email.
                        //_emailHelper.SendConfirmEmailEmail(model.Email, callbackUrl);
                        _logger.LogInformation("Confirmation email sent to user.");

                        // Return to the email confirmation page.
                        return RedirectToAction(nameof(RegisterConfirmation), new { userId = user.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Registration POST process: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            return View(model);
        }

        [HttpGet("register/confirmation")]
        public IActionResult RegisterConfirmation(string userId)
        {
            _logger.LogInformation("Registration confirmation process initiated");
            try
            {
                // Validate that user not already logged in.
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return LocalRedirect("/");
                }

                // Validate the user ID was provided.
                if (userId == null)
                {
                    _logger.LogError("Unauthorized attempt to view registration confirmation");
                    throw new Exception("You are not authorized to view this content at this time");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during Registration confirmation process: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }




        /******************************************************************************************************************/
        /*** CONFIRM EMAIL                                                                                              ***/
        /******************************************************************************************************************/

        [HttpGet("[action]")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token, string returnUrl = null)
        {
            _logger.LogInformation("Email confirmation process initiated");
            try
            {
                // Variable declaration.
                var errorMessage = "You are not authorized to confirm your email time";

                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                // Validate the both user ID and token were provided.
                if (userId == null || token == null)
                {
                    _logger.LogError("User ID or token is missing during email confirmation.");
                    throw new Exception(errorMessage);
                }

                // Validate user exists.
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError("User doesn't exist during email confirmation.");
                    throw new Exception(errorMessage);
                }

                // Validate that the email was not already confirmed.
                if (user.EmailConfirmed)
                {
                    _logger.LogError("Email is already confirmed.");
                    throw new Exception("This account has already been registered. Please sign in or reset your password");
                }

                // Try to verify the token against the user.
                var success = await _userManager.VerifyUserTokenAsync(
                    user,
                    _userManager.Options.Tokens.EmailConfirmationTokenProvider,
                    "EmailConfirmation",
                    token);

                // If token verification fails, allow user to generate a new token.
                if (!success)
                {
                    _logger.LogWarning("Token verification failed. Redirecting to generate new token.");
                    return RedirectToAction(nameof(ConfirmEmailExpired), new { userId, returnUrl });
                }

                // Try to confirm the user's email address via the provided token.
                var result = await _userManager.ConfirmEmailAsync(user, token);

                // Unsuccessful.
                if (!result.Succeeded)
                {
                    _logger.LogError("Error confirming email address.");
                    throw new Exception("There was an error confirming your email address");
                }

                _logger.LogInformation($"User email confirmed.");

                // Registration was successful so now we can add claims.
                await _userManager.AddClaimAsync(user, new Claim(_config["Auth:Claims:UserID"], user.Id));
                await _userManager.AddClaimAsync(user, new Claim(_config["Auth:Claims:Email"], user.Email));
                if (user.Email.ToLower().Contains(_config["Auth:AkrfDomain"])) { await _userManager.AddClaimAsync(user, new Claim(_config["Auth:Claims:Employee"], "true")); }
                _logger.LogInformation("Claims added to user.");

                // Set the viewdata.
                returnUrl = returnUrl ?? "/";
                ViewData["ReturnUrl"] = returnUrl;

                // Successful. Show view to let user know.
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during email confirmation process: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpGet("confirmemail/expired")]
        public IActionResult ConfirmEmailExpired(string userId, string returnUrl = null)
        {
            _logger.LogInformation("Handling expired email confirmation token");
            try
            {
                // Validate the user ID was provided.
                if (userId == null)
                {
                    _logger.LogError("Unauthorized attempt to view expired email confirmation page");
                    throw new Exception("You are not authorized to view this content at this time");
                }
                returnUrl = returnUrl ?? "/";

                return View(new ConfirmEmailExpiredViewModel(userId, returnUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling expired email confirmation token: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpPost("confirmemail/expired")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmailExpired(ConfirmEmailExpiredViewModel model)
        {
            _logger.LogInformation("Handling POST request for expired email confirmation token");
            try
            {
                if (ModelState.IsValid)
                {
                    // Validate user exists.
                    var user = await _userManager.FindByIdAsync(model.UserID);
                    if (user == null)
                    {
                        _logger.LogError("User doesn't exist during handling of expired email confirmation token.");
                        throw new Exception("There was an error generating a new email confirmation link");
                    }

                    // Generate the unique token to confirm the registration.
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Set up the confirmation email and send it to the new user.
                    var callbackUrl = Url.Action(
                        nameof(ConfirmEmail),
                        ControllerContext.ActionDescriptor.ControllerName,
                        new { userId = user.Id, token = token, returnUrl = model.ReturnUrl },
                        Request.Scheme);

                    // Send out the confirmation email.
                    //_emailHelper.SendConfirmEmailEmail(user.Email, callbackUrl);

                    // Return to the email confirmation page.
                    return RedirectToAction(nameof(RegisterConfirmation), ControllerContext.ActionDescriptor.ControllerName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during POST request for expired email confirmation token: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            return View(model);
        }



        /******************************************************************************************************************/
        /*** LOGOUT                                                                                                     ***/
        /******************************************************************************************************************/

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            _logger.LogInformation("User logout initiated");
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                _logger.LogInformation("Redirecting to Home after logout");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                _logger.LogInformation("Redirecting after logout");
                return RedirectToAction();
            }
        }

        /******************************************************************************************************************/
        /*** PASSWORD RESET REQUEST                                                                                     ***/
        /******************************************************************************************************************/

        [HttpGet("passwordreset/request")]
        public IActionResult PasswordResetRequest(string returnUrl = null)
        {
            _logger.LogInformation("Password reset request view accessed");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                return View(new PasswordResetRequestViewModel(returnUrl));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in password reset request view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpPost("passwordreset/request")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordResetRequest(PasswordResetRequestViewModel model)
        {
            _logger.LogInformation("Handling POST request for password reset");
            try
            {
                _logger.LogInformation($"Attempting to request a password reset for user with email: {model.Email}");

                // Validate that user exists.
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    _logger.LogError("User doesn't exist during password reset request.");
                    throw new Exception("This account does not exist");
                }

                // Validate that the user's email is confirmed.
                if (!user.EmailConfirmed)
                {
                    _logger.LogError("User's email is not confirmed during password reset request.");
                    throw new Exception("You must first confirm your email address to reset your password");
                }

                // Generate the reset token.
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Set up the reset email and send it to the user.
                var callbackUrl = Url.Action(
                    nameof(PasswordReset),
                    ControllerContext.ActionDescriptor.ControllerName,
                    new { userId = user.Id, token = token, returnUrl = model.ReturnUrl },
                    Request.Scheme);

                // Send out the reset email.
                //_emailHelper.SendResetPasswordEmail(model.Email, callbackUrl);
                _logger.LogInformation("Reset email sent to user.");

                // Return to the password reset confirmation page.
                return RedirectToAction(nameof(PasswordResetRequestConfirmation));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during password reset request: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            return View(model);
        }

        [HttpGet("passwordreset/request/confirmation")]
        public IActionResult PasswordResetRequestConfirmation()
        {
            _logger.LogInformation("Password reset request confirmation view accessed");
            try
            {
                // Validate that user not already logged in.
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return LocalRedirect("/");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in password reset request confirmation view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }




        /******************************************************************************************************************/
        /*** PASSWORD RESET                                                                                             ***/
        /******************************************************************************************************************/

        [HttpGet("[action]")]
        public async Task<IActionResult> PasswordReset(string userId, string token, string returnUrl = null)
        {
            _logger.LogInformation("Password reset view accessed");
            try
            {
                // Variable declaration.
                var errorMessage = "You are not authorized to reset your password at this time";

                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                // Validate the both user ID and token were provided.
                if (userId == null || token == null)
                {
                    _logger.LogError("User ID or token is not provided.");
                    throw new Exception(errorMessage);
                }

                // Validate user exists.
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogError("User doesn't exist during password reset.");
                    throw new Exception(errorMessage);
                }

                // Try to verify the token against the user.
                var success = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
                if (!success)
                {
                    _logger.LogError("Token verification failed during password reset.");
                    return RedirectToAction(nameof(PasswordResetExpired));
                }

                return View(new PasswordResetViewModel(userId, token));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in password reset view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PasswordReset(PasswordResetViewModel model)
        {
            _logger.LogInformation("Handling POST request for password reset");
            try
            {
                _logger.LogInformation($"Attempting to reset password for user with ID: {model.UserID}");

                // Check to see if values are valid.
                if (ModelState.IsValid)
                {
                    // Validate user exists.
                    var user = await _userManager.FindByIdAsync(model.UserID);
                    if (user == null)
                    {
                        _logger.LogError("User doesn't exist during password reset.");
                        throw new Exception("There was an error resetting your password");
                    }

                    // Attempt to reset the user's password.
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                    // If it did not succeed, log the errors and let user know it was unsuccessful.
                    if (!result.Succeeded)
                    {
                        _logger.LogError("Password reset failed.");
                        throw new Exception("There was an issue resetting your password");
                    }

                    _logger.LogInformation("Password successfully reset.");

                    // Reset any lockout on the account.
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(user);

                    // Send email letting user know password was reset.
                    var resetRequestCallbackUrl = Url.Action(
                        nameof(PasswordResetRequest),
                        ControllerContext.ActionDescriptor.ControllerName,
                        new { },
                        Request.Scheme);

                    //_emailHelper.SendPasswordWasChangedEmail(user.Email, resetRequestCallbackUrl);
                    _logger.LogInformation("Notification email sent to user.");

                    // Return to the password reset confirmation page.
                    return RedirectToAction(nameof(PasswordResetConfirmation), ControllerContext.ActionDescriptor.ControllerName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during password reset: {ex.Message}");
                ModelState.AddModelError("ArgumentException", ex.Message);
            }

            return View(model);
        }

        [HttpGet("passwordreset/confirmation")]
        public IActionResult PasswordResetConfirmation(string returnUrl = null)
        {
            _logger.LogInformation("Password reset confirmation view accessed");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in password reset confirmation view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        [HttpGet("passwordreset/expired")]
        public IActionResult PasswordResetExpired(string returnUrl = null)
        {
            _logger.LogInformation("Password reset expired view accessed");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in password reset expired view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        /******************************************************************************************************************/
        /*** LOCKOUT                                                                                                    ***/
        /******************************************************************************************************************/

        [HttpGet("[action]")]
        public IActionResult Lockout(string returnUrl = null)
        {
            _logger.LogInformation("Lockout view accessed");
            try
            {
                // Validate that user not already logged in.
                returnUrl = returnUrl ?? Url.Content("/");
                if (User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("User is already authenticated. Redirecting to home.");
                    return RedirectToAction("Index", "Home");
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in lockout view: {ex.Message}");
                return View("Error", new ErrorViewModel(errorMessage: $"{ex.Message}"));
            }
        }

        /******************************************************************************************************************/
        /*** CONTROLLER HELPER METHODS                                                                                  ***/
        /******************************************************************************************************************/

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                _logger.LogError($"Error occurred in controller helper methods: {error.Description}");
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

    }
}
