namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

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