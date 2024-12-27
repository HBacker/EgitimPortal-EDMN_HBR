using EgitimPortalFinal.Models;
using EgitimPortalFinal.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EgitimPortalFinal.Repositories
{
    public class AuthRepository : GenericRepository<AppUser>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthRepository(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
            : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterModel model)
        {
            var user = new AppUser { UserName = model.UserName, Email = model.Email, FullName = model.FullName, City = model.City };
            return await _userManager.CreateAsync(user, model.Password);
        }

        public async Task<SignInResult> LoginUserAsync(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return SignInResult.Failed;

            return await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
        }

        public async Task LogoutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<List<AppRole>> GetRolesAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityResult> AddRoleAsync(AppRole model)
        {
            return await _roleManager.CreateAsync(model);
        }

        public async Task<IdentityResult> AddRoleToUserAsync(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());

            return await _userManager.AddToRolesAsync(user, selectedRoles);
        }

        public async Task<AppUser> FindUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<AppRole> FindRoleByNameAsync(string roleName)
        {
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task AddToRoleAsync(AppUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }
    }
}
