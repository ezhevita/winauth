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
	/// Form used to get a password used to protect an authenticator
	/// </summary>
	public partial class SetPasswordForm : ResourceForm
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public SetPasswordForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Current password
		/// </summary>
		public string Password { get; protected set; }

		/// <summary>
		/// Click the Show checkbox to unmask the password fields
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void showCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (showCheckbox.Checked)
			{
				passwordField.UseSystemPasswordChar = false;
				passwordField.PasswordChar = (char)0;
				verifyField.UseSystemPasswordChar = false;
				verifyField.PasswordChar = (char)0;
			}
			else
			{
				passwordField.UseSystemPasswordChar = true;
				passwordField.PasswordChar = '*';
				verifyField.UseSystemPasswordChar = true;
				verifyField.PasswordChar = '*';
			}
		}

		/// <summary>
		/// Click the OK button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, EventArgs e)
		{
			string password = passwordField.Text.Trim();
			string verify = verifyField.Text.Trim();
			if (password != verify)
			{
				//WinAuthForm.ErrorDialog(this, "Your passwords do not match.");
				errorLabel.Text = strings.PasswordsDontMatch;
				errorLabel.Visible = true;
				errorTimer.Enabled = true;
				DialogResult = DialogResult.None;
				return;
			}

			Password = password;
		}

		/// <summary>
		/// Timer fired to clear error message
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void errorTimer_Tick(object sender, EventArgs e)
		{
			errorTimer.Enabled = false;
			errorLabel.Text = string.Empty;
			errorLabel.Visible = false;
		}
	}
}
