//
// This file is part of the LWES .NET Binding (LWES.net)
//
// COPYRIGHT© 2009, Phillip Clark (cerebralkungfu[at*g mail[dot*com)
//	 original .NET implementation
//
// LWES.net is free software: you can redistribute it and/or modify
// it under the terms of the Lesser GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// LWES.net is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.Config
{
	/// <summary>
	/// Collection of EventTemplateDB configuration sections.
	/// </summary>
	public class TemplateDBConfigurationElementCollection : AbstractConfigurationElementCollection<TemplateDBConfigurationSection, string>
	{
		#region Methods

		/// <summary>
		/// Gets the element key for a templateDB configuration element. The key is the templateDB's name.
		/// </summary>
		/// <param name="element">the element</param>
		/// <returns>The templateDB's name.</returns>
		protected override string PerformGetElementKey(TemplateDBConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}