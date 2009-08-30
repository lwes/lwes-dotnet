namespace Org.Lwes.JournalerService
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.ServiceProcess;
	using System.Text;

	static class Program
	{
		#region Methods

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
				new Journaler()
			};
			ServiceBase.Run(ServicesToRun);
		}

		#endregion Methods
	}
}