namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class ListenerConfigurationElementCollection : AbstractConfigurationElementCollection<ListenerConfigurationSection, string>
	{
		#region Methods

		protected override string PerformGetElementKey(ListenerConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}