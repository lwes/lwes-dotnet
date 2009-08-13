namespace Org.Lwes
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Exception indicating that an attribute was not set.
	/// </summary>
	[Serializable]
	public class AttributeNotSetException : EventSystemException
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance and initializes the error message.
		/// </summary>
		/// <param name="errorMessage">an error message</param>
		public AttributeNotSetException(string errorMessage)
			: base(errorMessage)
		{
		}

		AttributeNotSetException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion Constructors
	}

	/// <summary>
	/// Base class for exceptions thrown by the event system.
	/// </summary>
	[Serializable]
	public class EventSystemException : Exception
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="errorMessage">error message</param>
		public EventSystemException(string errorMessage)
			: base(errorMessage)
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="errorMessage">error message</param>
		/// <param name="innerException">exception that caused this exception</param>
		public EventSystemException(string errorMessage, Exception innerException)
			: base(errorMessage, innerException)
		{
		}

		/// <summary>
		/// Creates a new instance from serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public EventSystemException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion Constructors
	}

	/// <summary>
	/// Exception thrown when an attriubte does not exist in an Event.
	/// </summary>
	[Serializable]
	public class NoSuchAttributeException : EventSystemException
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="errorMessage">error message</param>
		public NoSuchAttributeException(string errorMessage)
			: base(errorMessage)
		{
		}

		/// <summary>
		/// Creates a new instance from serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public NoSuchAttributeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion Constructors
	}

	/// <summary>
	/// Exception thrown when an attribute type does not exist.
	/// </summary>
	[Serializable]
	public class NoSuchAttributeTypeException : EventSystemException
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="errorMessage">error message</param>
		public NoSuchAttributeTypeException(string errorMessage)
			: base(errorMessage)
		{
		}

		/// <summary>
		/// Creates a new instance from serialization.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public NoSuchAttributeTypeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		#endregion Constructors
	}
}