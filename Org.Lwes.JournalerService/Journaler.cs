namespace Org.Lwes.JournalerService
{
	using System.ServiceProcess;

	public partial class Journaler : ServiceBase
	{
		#region Constructors

		public Journaler()
		{
			InitializeComponent();
		}

		#endregion Constructors

		#region Methods

		protected override void OnStart(string[] args)
		{
		}

		protected override void OnStop()
		{
		}

		#endregion Methods
	}
}