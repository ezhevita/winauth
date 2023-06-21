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
using System.Collections.Generic;
using System.Windows.Forms;
using MetroFramework.Controls;

namespace WinAuth
{
	public class GroupMetroRadioButton : MetroRadioButton
	{
		public GroupMetroRadioButton()
		{
		}

		public GroupMetroRadioButton(string group)
		{
			Group = group;
		}

		public string Group { get; set; }

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);

			string group = Group;
			if (string.IsNullOrEmpty(group))
			{
				return;
			}

			bool check = Checked;
			Form form = FindParentControl<Form>();
			GroupMetroRadioButton[] radios = FindAllControls<GroupMetroRadioButton>(form);
			foreach (GroupMetroRadioButton grb in radios)
			{
				if (grb != this && check && grb.Group == group && grb.Checked)
				{
					grb.Checked = false;
				}
			}
		}

		private T FindParentControl<T>() where T : Control
		{
			Control parent = Parent;
			while (parent != null && !(parent is T))
			{
				parent = parent.Parent;
			}
			return (T)parent;
		}

		private static T[] FindAllControls<T>(Control parent) where T : Control
		{
			List<T> controls = new List<T>();
			foreach (Control c in parent.Controls)
			{
				if (c is T)
				{
					controls.Add((T)c);
					if (c.Controls != null && c.Controls.Count != 0)
					{
						controls.AddRange(FindAllControls<T>(c));
					}
				}
			}

			return controls.ToArray();
		}
	}

	public class GroupRadioButton : RadioButton
	{
		public GroupRadioButton()
		{
		}

		public GroupRadioButton(string group)
		{
			Group = group;
		}

		public string Group { get; set; }

		protected override void OnCheckedChanged(EventArgs e)
		{
			base.OnCheckedChanged(e);

			string group = Group;
			if (string.IsNullOrEmpty(group))
			{
				return;
			}

			bool check = Checked;
			Form form = FindParentControl<Form>();
			GroupRadioButton[] radios = FindAllControls<GroupRadioButton>(form);
			foreach (GroupRadioButton grb in radios)
			{
				if (grb != this && check && grb.Group == group && grb.Checked)
				{
					grb.Checked = false;
				}
			}
		}

		private T FindParentControl<T>() where T : Control
		{
			Control parent = Parent;
			while (parent != null && !(parent is T))
			{
				parent = parent.Parent;
			}
			return (T)parent;
		}

		private static T[] FindAllControls<T>(Control parent) where T : Control
		{
			List<T> controls = new List<T>();
			foreach (Control c in parent.Controls)
			{
				if (c is T)
				{
					controls.Add((T)c);
					if (c.Controls != null && c.Controls.Count != 0)
					{
						controls.AddRange(FindAllControls<T>(c));
					}
				}
			}

			return controls.ToArray();
		}
	}
}
