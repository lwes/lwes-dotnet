namespace Org.Lwes.DB
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	using Org.Lwes.ESF;

	/// <summary>
	/// Interface for event template databases.
	/// </summary>
	public interface IEventTemplateDB
	{
		#region Properties

		/// <summary>
		/// An enumerable containing the names of defined events.
		/// </summary>
		IEnumerable<string> EventNames
		{
			get;
		}

		/// <summary>
		/// Indicates whether the template db has been initialized.
		/// </summary>
		bool IsInitialized
		{
			get;
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Gets the named event template.
		/// </summary>
		/// <param name="evName">the event name</param>
		/// <returns>the event template for the named event</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the event is not defined in the DB</exception>
		EventTemplate GetEventTemplate(string evName);

		/// <summary>
		/// Initilizes the template db by reading ESF files at <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath">a path containing ESF (*.esf) files</param>
		/// <param name="includeSubdirectories">indicates whether subdirectories should be included</param>
		/// <exception cref="InvalidOperationException">thrown if the template db has already been initalized</exception>
		/// <exception cref="ArgumentNullException">thrown if <paramref name="filePath"/> is null</exception>		
		void InitializeFromFilePath(string filePath, bool includeSubdirectories);

		/// <summary>
		/// Initializes the template db by reading ESF templates from the <paramref name="esfStream"/>.
		/// </summary>
		/// <param name="esfStream">a stream containing ESF template definitions</param>
		/// <exception cref="InvalidOperationException">thrown if the template db has already been initalized</exception>
		/// <exception cref="ArgumentNullException">thrown if <paramref name="esfStream"/> is null</exception>
		void InitializeFromStream(Stream esfStream);

		/// <summary>
		/// Checks to see if a named event template exists.
		/// </summary>
		/// <param name="eventName">the event name</param>
		/// <returns><em>true</em> if the template with the <paramref name="eventName"/> exists;
		/// otherwise <em>false</em>.</returns>
		bool TemplateExists(string eventName);

		/// <summary>
		/// Tries to create the named event.
		/// </summary>
		/// <param name="evName">event name</param>
		/// <param name="ev">reference to a variable that contains the event upon success</param>
		/// <param name="performValidation">whether the event should perform validation</param>
		/// <param name="enc">the encoding used for the event</param>
		/// <returns><em>true</em> if the named event is created; otherwise <em>false</em></returns>
		bool TryCreateEvent(string evName, out Event ev, bool performValidation, SupportedEncoding enc);

		/// <summary>
		/// Tries to get the named event template.
		/// </summary>
		/// <param name="evName">the event's name</param>
		/// <param name="template">reference to a variable that will contain the template upon success</param>
		/// <returns><em>true</em> if the named event is retreived; otherwise <em>false</em></returns>
		bool TryGetEventTemplate(string evName, out EventTemplate template);

		#endregion Methods
	}
}