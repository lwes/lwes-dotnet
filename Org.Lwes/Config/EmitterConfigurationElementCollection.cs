namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class EmitterConfigurationElementCollection : AbstractConfigurationElementCollection<EmitterConfigurationSection, string>
	{
		#region Methods

		protected override string PerformGetElementKey(EmitterConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}