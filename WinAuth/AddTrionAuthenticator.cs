/*
 * Copyright (C) 2010 Colin Mackie.
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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinAuth
{
	/// <summary>
	/// Form class for create a new Battle.net authenticator
	/// </summary>
	public partial class AddTrionAuthenticator : ResourceForm
	{
		/// <summary>
		/// Form instantiation
		/// </summary>
		public AddTrionAuthenticator()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Current authenticator
		/// </summary>
		public WinAuthAuthenticator Authenticator { get; set; }

#region Form Events

		/// <summary>
		/// Load the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddTrionAuthenticator_Load(object sender, EventArgs e)
		{
			nameField.Text = Authenticator.Name;

			newSerialNumberField.SecretMode = true;
			newLoginCodeField.SecretMode = true;
			newRestoreCodeField.SecretMode = true;
		}

		/// <summary>
		/// Allow copy of authenticator codes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void allowCopyNewButton_CheckedChanged(object sender, EventArgs e)
		{
			newSerialNumberField.SecretMode = !allowCopyNewButton.Checked;
			//newLoginCodeField.SecretMode = !allowCopyNewButton.Checked;
			newRestoreCodeField.SecretMode = !allowCopyNewButton.Checked;
		}

		/// <summary>
		/// Ticker event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newAuthenticatorTimer_Tick(object sender, EventArgs e)
		{
			if (Authenticator != null && Authenticator.AuthenticatorData != null)
			{
				int time = (int)(Authenticator.AuthenticatorData.ServerTime / 1000L) % 30;
				if (time == 0)
				{
					newLoginCodeField.Text = Authenticator.AuthenticatorData.CurrentCode;
				}
			}
		}

		/// <summary>
		/// Click to get they security questions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void restoreGetQuestionsButton_Click(object sender, EventArgs e)
		{
			string email = restoreEmailField.Text.Trim();
			string password = restorePasswordField.Text.Trim();
			if (email.Length == 0 || password.Length == 0)
			{
				WinAuthForm.ErrorDialog(Owner, "Please enter your account email and password");
				return;
			}

			try
			{
				string question1, question2;
				TrionAuthenticator.SecurityQuestions(email, password, out question1, out question2);
				restoreQuestion1Label.Text = question1 ?? string.Empty;
				restoreQuestion2Label.Text = question2 ?? string.Empty;
			}
			catch (InvalidRestoreResponseException irre)
			{
				WinAuthForm.ErrorDialog(Owner, irre.Message, irre);
			}
			catch (Exception ex)
			{
				WinAuthForm.ErrorDialog(Owner, "Unable to access account: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Press the form's cancel button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelButton_Click(object sender, EventArgs e)
		{
			if (Authenticator.AuthenticatorData != null)
			{
				DialogResult result = WinAuthForm.ConfirmDialog(Owner,
					"You have created a new authenticator. "
					+ "If you have attached this authenticator to your account, you might not be able to login in the future." + Environment.NewLine + Environment.NewLine
					+ "Do you want to save this authenticator?", MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Yes)
				{
					DialogResult = DialogResult.OK;
					return;
				}
				if (result == DialogResult.Cancel)
				{
					DialogResult = DialogResult.None;
				}
			}
		}

		/// <summary>
		/// Click the OK button to verify and add the authenticator
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, EventArgs e)
		{
			if (verifyAuthenticator() == false)
			{
				DialogResult = DialogResult.None;
			}
		}

		/// <summary>
		/// Select one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void iconRift_Click(object sender, EventArgs e)
		{
			riftIconRadioButton.Checked = true;
		}

		/// <summary>
		/// Select one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void iconGlyph_Click(object sender, EventArgs e)
		{
			glyphIconRadioButton.Checked = true;
		}

		/// <summary>
		/// Select one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void iconArcheAge_Click(object sender, EventArgs e)
		{
			archeageIconRadioButton.Checked = true;
		}

		/// <summary>
		/// Select one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void trionIcon_Click(object sender, EventArgs e)
		{
			trionAuthenticatorRadioButton.Checked = true;
		}

		/// <summary>
		/// Set the authenticaotr icon
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void iconRadioButton_CheckedChanged(object sender, EventArgs e)
		{
			if (((RadioButton)sender).Checked)
			{
				Authenticator.Skin = (string)((RadioButton)sender).Tag;
			}
		}

		/// <summary>
		/// Draw the tabs of the tabcontrol so they aren't white
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
		{
			TabPage page = tabControl1.TabPages[e.Index];
			e.Graphics.FillRectangle(new SolidBrush(page.BackColor), e.Bounds);

			Rectangle paddedBounds = e.Bounds;
			int yOffset = (e.State == DrawItemState.Selected) ? -2 : 1;
			paddedBounds.Offset(1, yOffset);
			TextRenderer.DrawText(e.Graphics, page.Text, Font, paddedBounds, page.ForeColor);
		}

#endregion

#region Private methods

		/// <summary>
		/// Verify and create the authenticator if needed
		/// </summary>
		/// <returns>true is successful</returns>
		private bool verifyAuthenticator()
		{
			if (tabControl1.SelectedIndex == 0)
			{
				if (Authenticator.AuthenticatorData == null)
				{
					WinAuthForm.ErrorDialog(Owner, "You need to create an authenticator and attach it to your account");
					return false;
				}
			}
			else
			{				
				if (restoreQuestion1Label.Text.Length == 0)
				{
					restoreGetQuestionsButton_Click(null, null);
					return false;
				}

				string email = restoreEmailField.Text.Trim();
				string password = restorePasswordField.Text.Trim();
				string deviceId = restoreDeviceIdField.Text.Trim();
				string answer1 = restoreAnswer1Field.Text;
				string answer2 = restoreAnswer2Field.Text;
				if (deviceId.Length == 0 || (restoreQuestion2Label.Text.Length != 0 && answer1.Length == 0) || (restoreQuestion2Label.Text.Length != 0 && answer2.Length == 0))
				{
					WinAuthForm.ErrorDialog(Owner, "Please enter the device ID and answers to your secret questions");
					return false;
				}

				try
				{
					TrionAuthenticator authenticator = new TrionAuthenticator();
					authenticator.Restore(email, password, deviceId, answer1, answer2);
					Authenticator.AuthenticatorData = authenticator;
				}
				catch (InvalidRestoreResponseException irre)
				{
					WinAuthForm.ErrorDialog(Owner, "Unable to restore the authenticator: " + irre.Message, irre);
					return false;
				}
				catch (Exception ex)
				{
					WinAuthForm.ErrorDialog(Owner, "An error occured restoring the authenticator: " + ex.Message, ex);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Clear the authenticator and any associated fields
		/// </summary>
		/// <param name="showWarning"></param>
		private void clearAuthenticator(bool showWarning = true)
		{
			if (Authenticator.AuthenticatorData != null && showWarning)
			{
				DialogResult result = WinAuthForm.ConfirmDialog(Owner,
					"This will clear the authenticator you have just created. "
					+ "If you have attached this authenticator to your account, you might not be able to login in the future." + Environment.NewLine + Environment.NewLine
					+ "Are you sure you want to continue?");
				if (result != DialogResult.Yes)
				{
					return;
				}

				Authenticator.AuthenticatorData = null;
			}

			newSerialNumberField.Text = string.Empty;
			newSerialNumberField.SecretMode = true;
			newLoginCodeField.Text = string.Empty;
			newLoginCodeField.SecretMode = true;
			newRestoreCodeField.Text = string.Empty;
			newRestoreCodeField.SecretMode = true;
			allowCopyNewButton.Checked = false;

			restoreEmailField.Text = string.Empty;
			restorePasswordField.Text = string.Empty;
			restoreDeviceIdField.Text = string.Empty;
			restoreQuestion1Label.Text = string.Empty;
			restoreQuestion2Label.Text = string.Empty;
			restoreAnswer1Field.Text = string.Empty;
			restoreAnswer2Field.Text = string.Empty;
		}

		/// <summary>
		/// Click to create a enroll a new authenticator
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void enrollAuthenticatorButton_Click(object sender, EventArgs e)
		{
			do
			{
				try
				{
					newSerialNumberField.Text = "creating...";

					TrionAuthenticator authenticator = new TrionAuthenticator();
#if DEBUG
					authenticator.Enroll(Debugger.IsAttached);
#else
					authenticator.Enroll();
#endif
					Authenticator.AuthenticatorData = authenticator;
					newSerialNumberField.Text = authenticator.Serial;
					newLoginCodeField.Text = authenticator.CurrentCode;
					newRestoreCodeField.Text = authenticator.DeviceId;

					return;
				}
				catch (InvalidEnrollResponseException iere)
				{
					if (WinAuthForm.ErrorDialog(Owner, "An error occured while registering a new authenticator", iere, MessageBoxButtons.RetryCancel) != DialogResult.Retry)
					{
						break;
					}
				}
			} while (true);

			clearAuthenticator(false);
		}

#endregion


	}
}
