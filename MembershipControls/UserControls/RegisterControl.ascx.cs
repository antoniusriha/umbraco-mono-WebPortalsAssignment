//
// RegisterControl.ascx.cs
//
// Author:
//       Antonius Riha <antoniusriha@gmail.com>
//
// Copyright (c) 2012 Antonius Riha
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Web.Security;
using System.Web.UI;
using umbraco;

namespace MembershipControls.UserControls
{
	public partial class RegisterControl : UserControl
	{
		// Macro Parameters
		public int SuccessfulLoginPage { get; set; }
		
		// Member Group
		const string MembersGroup = "SiteMembers";
		
		// Error format
		const string ErrorFormat = "<p class=\"formerror\">{0}</p>";
		
		protected void RegisterPlayer (object sender, EventArgs e)
		{
			// Don't register if user currently logged in
			if (Context.User.Identity.IsAuthenticated) {
				litError.Text = string.Format (ErrorFormat, "Please log the current user out" +
					"before registering another user.");
				return;
			}
			
			var email = tbEmail.Text.Trim ();
			var password = tbPassword.Text.Trim ();
			var username = tbUsername.Text.Trim ();
			// Do some server side checks just to be on the safe side
			if (string.IsNullOrWhiteSpace (email) ||
				string.IsNullOrWhiteSpace (password) ||
				string.IsNullOrWhiteSpace (username)) {
				litError.Text = string.Format (ErrorFormat, "Please complete all fields");
				return;
			}
			
			// Check the user isn't already registered
			if (Membership.FindUsersByName (username).Count == 0) {
				
				//create the member, and set the password and email
				MembershipCreateStatus createStatus;
				Membership.CreateUser (username, password, email, "question",
				                       "answer", true, out createStatus);
				if (createStatus == MembershipCreateStatus.Success) {
					// Set the member group
					Roles.AddUserToRole (username, MembersGroup);
					FormsAuthentication.SetAuthCookie (username, false);
					Response.Redirect (library.NiceUrl (SuccessfulLoginPage));
				} else {
					litError.Text = string.Format (ErrorFormat,
					                               ErrorCodeToString (createStatus));
				}
			} else {
				// Error, member already exists with email or username used
				litError.Text = string.Format (ErrorFormat, "User already exists");
			}
		}
		
		static string ErrorCodeToString (MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus) {
			case MembershipCreateStatus.DuplicateUserName:
				return "User name already exists. Please enter a different user name.";
			case MembershipCreateStatus.DuplicateEmail:
				return "A user name for that e-mail address already exists. Please enter a " +
					"different e-mail address.";
			case MembershipCreateStatus.InvalidPassword:
				return "The password provided is invalid. Please enter a valid password value.";
			case MembershipCreateStatus.InvalidEmail:
				return "The e-mail address provided is invalid. Please check the value and " +
					"try again.";
			case MembershipCreateStatus.InvalidAnswer:
				return "The password retrieval answer provided is invalid. Please check the" +
					"value and try again.";
			case MembershipCreateStatus.InvalidQuestion:
				return "The password retrieval question provided is invalid. Please check the " +
					"value and try again.";
			case MembershipCreateStatus.InvalidUserName:
				return "The user name provided is invalid. Please check the value and try" +
					"again.";
			case MembershipCreateStatus.ProviderError:
				return "The authentication provider returned an error. Please verify your" +
					"entry and try again. If the problem persists, please contact your" +
					"system administrator.";
			case MembershipCreateStatus.UserRejected:
				return "The user creation request has been canceled. Please verify your" +
					"entry and try again. If the problem persists, please contact your" +
					"system administrator.";
			default:
				return "An unknown error occurred. Please verify your entry and try again." +
					"If the problem persists, please contact your system administrator.";
			}
		}
	}
}
