using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;

namespace CorpMessengerBackend.Pages
{
    public class adminModel : PageModel
    {
        private IAuthService _authService;

        public adminModel(IAuthService authService, AppDataContext context)
        {
            _authService = authService;
            //_db = context;
        }

        public string AdminKey { get; set; }

        public void OnGet()
        {
            AdminKey = Request.Cookies["adminKey"];
        }

        public async Task<IActionResult> OnPostAsync(string? adminKey)
        {
            if (adminKey != null)
            {
                var options = new CookieOptions();
                //options.

                Response.Cookies.Append("adminKey", adminKey, options);
            }

            return RedirectToPage("/admin");
        }
    }
}
