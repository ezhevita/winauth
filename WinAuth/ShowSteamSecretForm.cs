/*
 * Copyright (C) 2015 Colin Mackie.
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
using System.Drawing;
using Newtonsoft.Json.Linq;

namespace WinAuth
{
	/// <summary>
	/// Form display initialization confirmation.
	/// </summary>
	public partial class ShowSteamSecretForm : ResourceForm
	{
		/// <summary>
		/// Current authenticator
		/// </summary>
		public WinAuthAuthenticator CurrentAuthenticator { get; set; }

		/// <summary>
		/// Create a new form
		/// </summary>
		public ShowSteamSecretForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Click OK button to close form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Form loaded event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ShowSteamSecretForm_Load(object sender, EventArgs e)
		{
			revocationcodeField.SecretMode = true;
			deviceidField.SecretMode = true;
			steamdataField.SecretMode = false;

			SteamAuthenticator authenticator = CurrentAuthenticator.AuthenticatorData as SteamAuthenticator;

			deviceidField.Text = authenticator.DeviceId;
			if (string.IsNullOrEmpty(authenticator.SteamData) == false && authenticator.SteamData[0] == '{')
			{
				revocationcodeField.Text = JObject.Parse(authenticator.SteamData).SelectToken("revocation_code").Value<string>();
				if (authenticator.SteamData.IndexOf("shared_secret") != -1)
				{
					steamdataField.SecretMode = true;
					steamdataField.Text = authenticator.SteamData;
					steamdataField.ForeColor = SystemColors.ControlText;
        }
			}
			else
			{
				revocationcodeField.Text = authenticator.SteamData;
			}
		}

		/// <summary>
		/// Click the allow copy chekcbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void allowCopyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			revocationcodeField.SecretMode = !allowCopyCheckBox.Checked;
			deviceidField.SecretMode = !allowCopyCheckBox.Checked;
			steamdataField.SecretMode = !allowCopyCheckBox.Checked;
		}

	}
}
