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
// Lesser GNU General Public License for more details.
//
// You should have received a copy of the Lesser GNU General Public License
// along with LWES.net.  If not, see <http://www.gnu.org/licenses/>.
//
namespace Org.Lwes.DB
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using Org.Lwes.ESF;
	using Org.Lwes.Properties;

	/// <summary>
	/// Event template database implementation that uses a file path
	/// and ".esf" files for event definitions.
	/// </summary>
	public class FilePathEventTemplateDB : IEventTemplateDB, ITraceable
	{
		#region Fields

		const string EsfFileSearchPattern = "*.esf";

		bool _initialized;
		Dictionary<string, EventTemplate> _templates = new Dictionary<string, EventTemplate>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// An enumerable containing the names of defined events.
		/// </summary>
		public IEnumerable<string> EventNames
		{
			get
			{
				if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);

				return new List<string>(_templates.Keys);
			}
		}

		/// <summary>
		/// Indicates whether the template db has been initialized.
		/// </summary>
		public bool IsInitialized
		{
			get { return _initialized; }
		}

		#endregion Properties

		#region Methods

		/// <summary>
		/// Gets the named event template.
		/// </summary>
		/// <param name="evName">the event name</param>
		/// <returns>the event template for the named event</returns>
		/// <exception cref="ArgumentOutOfRangeException">thrown if the event is not defined in the DB</exception>
		public EventTemplate GetEventTemplate(string evName)
		{
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (evName == null) throw new ArgumentNullException("evName");
			if (evName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "evName");

			return _templates[evName];
		}

		/// <summary>
		/// Initilizes the template db by reading ESF files at <paramref name="filePath"/>.
		/// </summary>
		/// <param name="filePath">a path containing ESF (*.esf) files</param>
		/// <param name="includeSubdirectories">indicates whether subdirectories should be included
		/// in the search for esf files</param>
		/// <exception cref="InvalidOperationException">thrown if the template db has already been initalized</exception>
		/// <exception cref="ArgumentNullException">thrown if <paramref name="filePath"/> is null</exception>		
		public void InitializeFromFilePath(string filePath, bool includeSubdirectories)
		{
			if (_initialized) throw new InvalidOperationException("Already initialized");
			if (filePath == null) throw new ArgumentNullException("filePath");
			if (!Directory.Exists(filePath))
				throw new IOException(String.Concat("Directory does not exist: ", filePath));

			EsfParser parser = new EsfParser();
			SearchOption option = (includeSubdirectories) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			foreach (var fn in Directory.GetFiles(filePath, EsfFileSearchPattern, option))
			{
				using (FileStream fs = File.Open(fn, FileMode.Open))
				{
					var templates = parser.ParseEventTemplates(fs);
					foreach (var evt in templates)
					{
						if (_templates.ContainsKey(evt.Name))
						{
							// There is already a template by the same name: warn about it
							this.TraceWarning(Resources.Warning_DuplicateEventTemplateFromESF, evt.Name, fn);
							_templates[evt.Name] = evt;
						}
						else
							_templates.Add(evt.Name, evt);
					}
				}
			}
			_initialized = true;
		}

		/// <summary>
		/// Initializes the template db by reading ESF templates from the <paramref name="esfStream"/> (NOT IMPLEMENTED).
		/// </summary>
		/// <param name="esfStream">a stream containing ESF template definitions</param>
		/// <exception cref="NotImplementedException">thrown by this implementation because the method is
		/// not implemented.</exception>
		public void InitializeFromStream(Stream esfStream)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Checks to see if a named event template exists.
		/// </summary>
		/// <param name="eventName">the event name</param>
		/// <returns><em>true</em> if the template with the <paramref name="eventName"/> exists;
		/// otherwise <em>false</em>.</returns>
		public bool TemplateExists(string eventName)
		{
			return (_initialized && _templates.ContainsKey(eventName));
		}

		/// <summary>
		/// Tries to create the named event.
		/// </summary>
		/// <param name="evName">event name</param>
		/// <param name="ev">reference to a variable that contains the event upon success</param>
		/// <param name="performValidation">whether the event should perform validation</param>
		/// <param name="enc">the encoding used for the event</param>
		/// <returns><em>true</em> if the named event is created; otherwise <em>false</em></returns>
		public bool TryCreateEvent(string evName, out Event ev, bool performValidation, SupportedEncoding enc)
		{
			EventTemplate template;
			if (TryGetEventTemplate(evName, out template))
			{
				ev = new Event(template, performValidation, enc);
				return true;
			}
			ev = default(Event);
			return false;
		}

		/// <summary>
		/// Tries to get the named event template.
		/// </summary>
		/// <param name="evName">the event's name</param>
		/// <param name="template">reference to a variable that will contain the template upon success</param>
		/// <returns><em>true</em> if the named event is retreived; otherwise <em>false</em></returns>
		public bool TryGetEventTemplate(string evName, out EventTemplate template)
		{
			if (!_initialized) throw new InvalidOperationException(Resources.Error_NotYetInitialized);
			if (evName == null) throw new ArgumentNullException("evName");
			if (evName.Length == 0) throw new ArgumentException(Resources.Error_EmptyStringNotAllowed, "evName");

			return _templates.TryGetValue(evName, out template);
		}

		#endregion Methods
	}
}