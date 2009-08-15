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