namespace Org.Lwes
{
	using System;
	using System.Net;

	using Org.Lwes.ESF;
	using System.Text;

	/// <summary>
	/// Utility class.
	/// </summary>
	public static class EventUtils
	{
		#region Fields

		static Random _rand = new Random(Environment.TickCount);

		#endregion Fields

		#region Methods

		/// <summary>
		/// Generates a random event suitable for use during testing. The event's attributes
		/// are also generated randomly and populated with random data.
		/// </summary>
		/// <param name="name">the new event's name</param>
		/// <param name="maxAttributeCount">maximum number of attributes in the random event.</param>
		/// <param name="enc">an encoding for the event</param>
		/// <returns>a new populated event.</returns>
		public static Event GenerateRandomEvent(string name, int maxAttributeCount, SupportedEncoding enc)
		{
			int attributesToGenerate = _rand.Next(1, maxAttributeCount);
			Event original = new Event(name)
				.SetValue("ID", Guid.NewGuid().ToString("B"));
			for (int i = 1; i < attributesToGenerate; i++)
			{
				String an = String.Concat("Attribute_", i.ToString("X03"));
				TypeToken tt = (TypeToken)_rand.Next((int)TypeToken.MinValue, (int)TypeToken.MaxValue);
				switch (tt)
				{
					case TypeToken.UINT16:
						original = original.SetValue(an, (UInt16)_rand.Next(UInt16.MinValue, UInt16.MaxValue));
						break;
					case TypeToken.INT16:
						original = original.SetValue(an, (Int16)_rand.Next(Int16.MinValue, Int16.MaxValue));
						break;
					case TypeToken.UINT32:
						original = original.SetValue(an, (UInt32)_rand.Next(0, Int32.MaxValue));
						break;
					case TypeToken.INT32:
						original = original.SetValue(an, (Int32)_rand.Next(Int32.MinValue, Int32.MaxValue));
						break;
					case TypeToken.STRING:
						original = original.SetValue(an, GenerateRandomString(_rand.Next(400), enc));
						break;
					case TypeToken.IP_ADDR:
						byte[] addy = new byte[4];
						_rand.NextBytes(addy);
						original = original.SetValue(an, new IPAddress(addy));
						break;
					case TypeToken.INT64:
						original = original.SetValue(an, (Int32)_rand.Next(Int32.MinValue, Int16.MaxValue));
						break;
					case TypeToken.UINT64:
						original = original.SetValue(an, (Int32)_rand.Next(0, Int32.MaxValue));
						break;
					case TypeToken.BOOLEAN:
						original = original.SetValue(an, (_rand.Next() % 2 == 0));
						break;
				}
			}
			return original;
		}

		/// <summary>
		/// Produces a random string of unicode characters except control characters.
		/// </summary>
		/// <param name="len"></param>
		/// <param name="enc">an encoding for the string</param>
		/// <returns></returns>
		public static string GenerateRandomString(int len, SupportedEncoding enc)
		{
			int minCharValue = (enc == SupportedEncoding.ISO_8859_1) ? 32 : 0;
			int maxCharValue = 0xFF;
			char[] ch = new char[len];
			int i = 0;
			while(i < len)
			{

				Char c = Convert.ToChar(_rand.Next(minCharValue, maxCharValue));
				// Leave out the control characters in case the
				// events are printed on a console
				if (!Char.IsControl(c))
				{
					ch[i++] = c;
				}
			}
			return new String(ch);
		}

		#endregion Methods
	}
}