using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Edux.Data;
using Edux.Models;
using Microsoft.AspNetCore.Identity;

namespace Edux.Controllers
{
    public class ApplicationUsersController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicationUsersController(RoleManager<Role> roleManager,UserManager<ApplicationUser> userManager, ApplicationDbContext context):base(context)
        {
            this._roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: ApplicationUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ApplicationUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppTenantId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(applicationUser);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: ApplicationUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Role> Roles = _roleManager.Roles.ToList();
            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }
            ViewBag.Roles = Roles.ToList();
            var selectedRoles = await _userManager.GetRolesAsync(applicationUser);
            ViewBag.SelectedRoles = selectedRoles;

           

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AppTenantId,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] ApplicationUser applicationUser, string[] selectedRoles)
        {
            if (id != applicationUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var applicationUserOld = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
                    applicationUserOld.AppTenantId = applicationUser.AppTenantId;
                    applicationUserOld.UserName = applicationUser.UserName;
                    applicationUserOld.NormalizedUserName = applicationUser.NormalizedUserName;
                    applicationUserOld.Email = applicationUser.Email;
                    applicationUserOld.NormalizedEmail = applicationUser.NormalizedEmail;
                    applicationUserOld.EmailConfirmed = applicationUser.EmailConfirmed;
                    applicationUserOld.PhoneNumber = applicationUser.PhoneNumber;
                    applicationUserOld.PhoneNumberConfirmed = applicationUser.PhoneNumberConfirmed;
                    applicationUserOld.TwoFactorEnabled = applicationUser.TwoFactorEnabled;
                    applicationUserOld.LockoutEnd = applicationUser.LockoutEnd;
                    applicationUserOld.LockoutEnabled = applicationUser.LockoutEnabled;
                    applicationUserOld.AccessFailedCount = applicationUser.AccessFailedCount;

                    await _userManager.RemoveFromRolesAsync(applicationUserOld, await _userManager.GetRolesAsync(applicationUserOld));
                    foreach (var r in selectedRoles) {
                        await _userManager.AddToRoleAsync(applicationUserOld, r);
                       
                    }

                    await _context.SaveChangesAsync();
                }
                
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(applicationUser.Id.ToString()))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");

            }

            return View(applicationUser);
        }

        // GET: ApplicationUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(applicationUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
