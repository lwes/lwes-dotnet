// 
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT (C) 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//   original .NET implementation
// 
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Collection of Listener configuration sections.
	/// </summary>
	public class ListenerConfigurationElementCollection : AbstractConfigurationElementCollection<ListenerConfigurationSection, string>
	{
		#region Methods

		/// <summary>
		/// Gets the element key for a listener configuration element. The key is the listener's name.
		/// </summary>
		/// <param name="element">the element</param>
		/// <returns>The listener's name.</returns>
		protected override string PerformGetElementKey(ListenerConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}