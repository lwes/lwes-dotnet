namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	/// <summary>
	/// Collection of Emitter configuration sections.
	/// </summary>
	public class EmitterConfigurationElementCollection : AbstractConfigurationElementCollection<EmitterConfigurationSection, string>
	{
		#region Methods

		/// <summary>
		/// Gets the element key for a emitter configuration element. The key is the emitter's name.
		/// </summary>
		/// <param name="element">the element</param>
		/// <returns>The emitter's name.</returns>
		protected override string PerformGetElementKey(EmitterConfigurationSection element)
		{
			return element.Name;
		}

		#endregion Methods
	}
}