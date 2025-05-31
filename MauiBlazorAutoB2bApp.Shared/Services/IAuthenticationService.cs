using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiBlazorAutoB2bApp.Shared.Services
{
	public interface IAuthenticationService
	{
		Task<AuthenticationResult> SignInAsync();
		Task SignOutAsync();
	}
}
