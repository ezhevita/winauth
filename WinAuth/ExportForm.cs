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
using System.IO;
using System.Windows.Forms;
using WinAuth.Resources;

namespace WinAuth
{
	/// <summary>
	/// Form for exporting authenticators to a file
	/// </summary>
	public partial class ExportForm : ResourceForm
	{
		/// <summary>
		/// Create the form
		/// </summary>
		public ExportForm()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Protect password
		/// </summary>
		public string Password { get; protected set; }

		/// <summary>
		/// Protect PGP key
		/// </summary>
		public string PGPKey { get; protected set; }

		/// <summary>
		/// Export file
		/// </summary>
		public string ExportFile { get; protected set; }

		/// <summary>
		/// Load the form and pretick checkboxes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportForm_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Form has been shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExportForm_Shown(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Password encryption is ticked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void passwordCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (passwordCheckbox.Checked)
			{
				pgpCheckbox.Checked = false;
			}

			passwordField.Enabled = (passwordCheckbox.Checked);
			verifyField.Enabled = (passwordCheckbox.Checked);
			if (passwordCheckbox.Checked)
			{
				passwordField.Focus();
			}
		}

		/// <summary>
		/// Check the PGP encryption
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pgpCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			if (pgpCheckbox.Checked)
			{
				passwordCheckbox.Checked = false;
			}

			pgpField.Enabled = pgpCheckbox.Checked;
			if (pgpCheckbox.Checked)
			{
				pgpField.Focus();
			}
		}

		/// <summary>
		/// Use the browse button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pgpBrowseButton_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.Filter = "All Files (*.*)|*.*";
			ofd.Title = "Choose PGP Key File";

			if (ofd.ShowDialog(Parent) == DialogResult.OK)
			{
				pgpField.Text = File.ReadAllText(ofd.FileName);
			}
		}

		/// <summary>
		/// Click the browse button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void browseButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.AddExtension = true;
			sfd.CheckPathExists = true;
			if (passwordCheckbox.Checked)
			{
				sfd.Filter = "Zip File (*.zip)|*.zip";
				sfd.FileName = "winauth-" + DateTime.Today.ToString("yyyy-MM-dd") + ".zip";
			}
			else if (pgpCheckbox.Checked)
			{
				sfd.Filter = "PGP File (*.pgp)|*.pgp";
				sfd.FileName = "winauth-" + DateTime.Today.ToString("yyyy-MM-dd") + ".pgp";
			}
			else
			{
				sfd.Filter = "Text File (*.txt)|*.txt|Zip File (*.zip)|*.zip|All Files (*.*)|*.*";
				sfd.FileName = "winauth-" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
			}
			sfd.OverwritePrompt = true;
			if (sfd.ShowDialog(Parent) != DialogResult.OK)
			{
				return;
			}

			fileField.Text = sfd.FileName;
		}

		/// <summary>
		/// OK button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, EventArgs e)
		{
			// check password is set if required
			if (passwordCheckbox.Checked && passwordField.Text.Trim().Length == 0)
			{
				WinAuthForm.ErrorDialog(this, strings.EnterPassword);
				DialogResult = DialogResult.None;
				return;
			}
			if (passwordCheckbox.Checked && string.Compare(passwordField.Text, verifyField.Text) != 0)
			{
				WinAuthForm.ErrorDialog(this, strings.PasswordsDontMatch);
				DialogResult = DialogResult.None;
				return;
			}

			if (pgpCheckbox.Checked && pgpField.Text.Length == 0)
			{
				WinAuthForm.ErrorDialog(this, strings.MissingPGPKey);
				DialogResult = DialogResult.None;
				return;
			}

			if (fileField.Text.Length == 0)
			{
				WinAuthForm.ErrorDialog(this, strings.MissingFile);
				DialogResult = DialogResult.None;
				return;
			}

			// set the valid password type property
			ExportFile = fileField.Text;
			if (passwordCheckbox.Checked && passwordField.Text.Length != 0)
			{
				Password = passwordField.Text;
			}
			if (pgpCheckbox.Checked && pgpField.Text.Length != 0)
			{
				PGPKey = pgpField.Text;
			}
		}

	}

}
