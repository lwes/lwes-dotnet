namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class TemplateDBConfigurationElementCollection : AbstractConfigurationElementCollection<TemplateDBConfigurationSection, string>
	{
		#region Methods

		protected override string PerformGetElementKey(TemplateDBConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}