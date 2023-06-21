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

using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;

namespace WinAuth
{
	/// <summary>
	/// Subclass of TextBox that does not set the Text value based on the ShowBitmap flag and we
	/// draw it ourselves, so that window spys cannot get the contents
	/// </summary>
	public class SecretTextBox : TextBox, ISecretTextBox
	{
		/// <summary>
		/// Our hidden txet value
		/// </summary>
		private string m_text;

		/// <summary>
		/// Number of chars to space out when in secret mode
		/// </summary>
		private int m_spaceOut;

		/// <summary>
		/// Flag to draw ourselves
		/// </summary>
		private bool m_secretMode;

		/// <summary>
		/// Save the font as when we switch secret mode the old font needs resetting
		/// </summary>
		private string m_fontFamily;
		private float m_fontSize;

		/// <summary>
		/// Create a new SecretTextBox
		/// </summary>
		public SecretTextBox()
		{
		}

		/// <summary>
		/// Get/set the flag to draw as a bitmap
		/// </summary>
		public bool SecretMode
		{
			get
			{
				return m_secretMode;
			}
			set
			{
				m_secretMode = value;
				Enabled = !value; // we disable so cannot select/copy 
				SetStyle(ControlStyles.UserPaint, value);
				Text = m_text;
				//
				// when we disable secret mode we need to reset the font else sometimes it doesn't show corretly
				if (m_fontFamily == null)
				{
					m_fontFamily = Font.FontFamily.Name;
					m_fontSize = Font.Size;
				}
				if (value == false)
				{
					Font = new Font(m_fontFamily, m_fontSize);
				}
				//
				Invalidate(); // force it to redraw
			}
		}

		public int SpaceOut
		{
			get
			{
				return m_spaceOut;
			}
			set
			{
				m_spaceOut = value;
			}
		}

		/// <summary>
		/// Value of the textbox, but if in secret mode then will return ***** as value
		/// </summary>
		public override string Text
		{
			get
			{
				return (SecretMode ? m_text : base.Text);
			}
			set
			{
				m_text = value;
				base.Text = (SecretMode ? (string.IsNullOrEmpty(value) == false ? new string('*', value.Length) : value) : value);
				Invalidate();
			}
		}

		/// <summary>
		/// Overriden Paint method to draw the text based on our secret value rather than Text, which might be ****'s
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			using (Brush brush = new SolidBrush(base.ForeColor))
			{
				StringFormat sf = StringFormat.GenericTypographic;
				sf.Alignment = StringAlignment.Center;
				sf.LineAlignment = StringAlignment.Center;
				g.TextRenderingHint = TextRenderingHint.AntiAlias;

				string text = m_text;

				// if we have spacing, we add a space in between each set of chars
				if (m_spaceOut != 0 && m_text != null)
				{
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < m_text.Length; i += m_spaceOut)
					{
						if (i >= m_text.Length)
						{
							break;
						}
            if (i + m_spaceOut >= m_text.Length)
            {
              sb.Append(m_text.Substring(i));
            }
            else
            {
              sb.Append(m_text.Substring(i, m_spaceOut)).Append(" ");
            }
					}
					text = sb.ToString().Trim();
				}

				// draw the whole string
				g.DrawString((text != null ? text : string.Empty), base.Font, brush, new RectangleF(0, 0, Width, Height), sf);
			}
		}

	}
}
