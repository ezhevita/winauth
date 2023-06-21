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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WinAuth
{
	/// <summary>
	/// Form class for create a new Battle.net authenticator
	/// </summary>
	public partial class AddBattleNetAuthenticator : ResourceForm
	{
		/// <summary>
		/// Form instantiation
		/// </summary>
		public AddBattleNetAuthenticator()
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
		private void AddBattleNetAuthenticator_Load(object sender, EventArgs e)
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

			if (Authenticator != null && Authenticator.AuthenticatorData != null)
			{
				// Issue#122: remove dashes if copyable so can be pasted into Battle.net form
				if (allowCopyNewButton.Checked)
				{
					newSerialNumberField.Text = ((BattleNetAuthenticator)Authenticator.AuthenticatorData).Serial.Replace("-", "");
				}
				else
				{
					newSerialNumberField.Text = ((BattleNetAuthenticator)Authenticator.AuthenticatorData).Serial;
				}
			}
			newLoginCodeField.SecretMode = !allowCopyNewButton.Checked;
			newRestoreCodeField.SecretMode = !allowCopyNewButton.Checked;
		}

		/// <summary>
		/// Ticker event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newAuthenticatorTimer_Tick(object sender, EventArgs e)
		{
			if (Authenticator.AuthenticatorData != null && newAuthenticatorProgress.Visible)
			{
				int time = (int)(Authenticator.AuthenticatorData.ServerTime / 1000L) % 30;
				newAuthenticatorProgress.Value = time + 1;
				if (time == 0)
				{
					newLoginCodeField.Text = Authenticator.AuthenticatorData.CurrentCode;
				}
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

		/// <summary>
		/// Click one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void icon1_Click(object sender, EventArgs e)
		{
			icon1RadioButton.Checked = true;
		}

		/// <summary>
		/// Click one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void icon2_Click(object sender, EventArgs e)
		{
			icon2RadioButton.Checked = true;
		}

		/// <summary>
		/// Click one of the icons
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void icon3_Click(object sender, EventArgs e)
		{
			icon3RadioButton.Checked = true;
		}

		/// <summary>
		/// Select one of the icons
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

#endregion

#region Private methods

		/// <summary>
		/// Verify and create the authenticator if needed
		/// </summary>
		/// <returns>true is successful</returns>
		private bool verifyAuthenticator()
		{
			Authenticator.Name = nameField.Text;

			if (tabControl1.SelectedIndex == 0)
			{
				if (Authenticator.AuthenticatorData == null)
				{
					WinAuthForm.ErrorDialog(Owner, "You need to create an authenticator and attach it to your account");
					return false;
				}
			}
			else if (tabControl1.SelectedIndex == 1)
			{
				string serial = restoreSerialNumberField.Text.Trim();
				string restore = restoreRestoreCodeField.Text.Trim();
				if (serial.Length == 0 || restore.Length == 0)
				{
					WinAuthForm.ErrorDialog(Owner, "Please enter the Serial number and Restore code");
					return false;
				}

				try
				{
					BattleNetAuthenticator authenticator = new BattleNetAuthenticator();
					authenticator.Restore(serial, restore);
					Authenticator.AuthenticatorData = authenticator;
				}
				catch (InvalidRestoreResponseException irre)
				{
					WinAuthForm.ErrorDialog(Owner, "Unable to restore the authenticator: " + irre.Message, irre);
					return false;
				}
			}
			else if (tabControl1.SelectedIndex == 2)
			{
				string privatekey = importPrivateKeyField.Text.Trim();
				if (privatekey.Length == 0)
				{
					WinAuthForm.ErrorDialog(Owner, "Please enter the Private key");
					return false;
				}
				// just get the hex chars
				privatekey = Regex.Replace(privatekey, @"0x", "", RegexOptions.IgnoreCase);
				privatekey = Regex.Replace(privatekey, @"[^0-9abcdef]", "", RegexOptions.IgnoreCase);
				if (privatekey.Length == 0 || privatekey.Length < 40)
				{
					WinAuthForm.ErrorDialog(Owner, "The private key must be a sequence of at least 40 hexadecimal characters, e.g. 7B0BFA82... or 0x7B, 0x0B, 0xFA, 0x82, ...");
					return false;
				}
				try
				{
					BattleNetAuthenticator authenticator = new BattleNetAuthenticator();
					if (privatekey.Length == 40) // 20 bytes which is key only
					{
						authenticator.SecretKey = WinAuth.Authenticator.StringToByteArray(privatekey);
						authenticator.Serial = "US-Imported";
					}
					else
					{
						authenticator.SecretData = privatekey;
						if (string.IsNullOrEmpty(authenticator.Serial))
						{
							authenticator.Serial = "US-Imported";
						}
					}
					authenticator.Sync();
					Authenticator.AuthenticatorData = authenticator;
				}
				catch (Exception irre)
				{
					WinAuthForm.ErrorDialog(Owner, "Unable to import the authenticator. The private key is probably invalid.", irre);
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

			newAuthenticatorProgress.Visible = false;
			newAuthenticatorTimer.Enabled = false;
			newSerialNumberField.Text = string.Empty;
			newSerialNumberField.SecretMode = true;
			newLoginCodeField.Text = string.Empty;
			newLoginCodeField.SecretMode = true;
			newRestoreCodeField.Text = string.Empty;
			newRestoreCodeField.SecretMode = true;
			allowCopyNewButton.Checked = false;

			restoreSerialNumberField.Text = string.Empty;
			restoreRestoreCodeField.Text = string.Empty;

			importPrivateKeyField.Text = string.Empty;
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

					BattleNetAuthenticator authenticator = new BattleNetAuthenticator();
#if DEBUG
					authenticator.Enroll(Debugger.IsAttached);
#else
					authenticator.Enroll();
#endif
					Authenticator.AuthenticatorData = authenticator;
					newSerialNumberField.Text = authenticator.Serial;
					newLoginCodeField.Text = authenticator.CurrentCode;
					newRestoreCodeField.Text = authenticator.RestoreCode;

					newAuthenticatorProgress.Visible = true;
					newAuthenticatorTimer.Enabled = true;

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
