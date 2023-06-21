/*
 * Copyright (C) 2013 Colin Mackie.
 * This software is distributed under the terms of the GNU General Public License.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Windows.Forms;
using WinAuth.Resources;

namespace WinAuth
{
	/// <summary>
	/// Class for form that prompts for password and unprotects authenticator
	/// </summary>
	public partial class UnprotectPasswordForm : ResourceForm
	{
		/// <summary>
		/// Create new form
		/// </summary>
		public UnprotectPasswordForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Authenticator to unprotect
		/// </summary>
		public WinAuthAuthenticator Authenticator { get; set; }

		/// <summary>
		/// Load the form and make it topmost
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UnprotectPasswordForm_Load(object sender, EventArgs e)
		{
			// window text is "{0} Password" 
			Text = string.Format(Text, Authenticator.Name);

			// force this window to the front and topmost
			// see: http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
			var oldtopmost = TopMost;
			TopMost = true;
			TopMost = oldtopmost;
			Activate();
		}

		/// <summary>
		/// Click the OK button to unprotect the authenticator with given password
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, EventArgs e)
		{
			// it isn't empty
			string password = passwordField.Text;
			if (password.Length == 0)
			{
				invalidPasswordLabel.Text = strings.EnterPassword;
				invalidPasswordLabel.Visible = true;
				invalidPasswordTimer.Enabled = true;
				DialogResult = DialogResult.None;
				return;
			}

			// try to unprotect
			try
			{
				if (Authenticator.AuthenticatorData.Unprotect(password))
				{
					Authenticator.MarkChanged();
				}
			}
			catch (BadYubiKeyException)
			{
				invalidPasswordLabel.Text = "Please insert your YubiKey";
				invalidPasswordLabel.Visible = true;
				invalidPasswordTimer.Enabled = true;
				DialogResult = DialogResult.None;
			}
			catch (BadPasswordException)
			{
				invalidPasswordLabel.Text = strings.InvalidPassword;
				invalidPasswordLabel.Visible = true;
				invalidPasswordTimer.Enabled = true;
				DialogResult = DialogResult.None;
			}
		}

		/// <summary>
		/// Display error message for couple seconds
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void invalidPasswordTimer_Tick(object sender, EventArgs e)
		{
			invalidPasswordTimer.Enabled = false;
			invalidPasswordLabel.Visible = false;
		}

	}
}
