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
namespace Org.Lwes.Config
{
	using System.Configuration;

	/// <summary>
	/// Base class for configuration element collections.
	/// </summary>
	/// <typeparam name="TElement">Element type</typeparam>
	/// <typeparam name="TKey">Key type</typeparam>
	public abstract class AbstractConfigurationElementCollection<TElement, TKey> : ConfigurationElementCollection
		where TElement : ConfigurationElement, new()
	{
		#region Constructors

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		protected AbstractConfigurationElementCollection()
		{
		}

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="addElmName">name used to add an element to the collection (default is 'add')</param>
		/// <param name="clearElmName">name used when clearing elements from the collection (default is 'clear')</param>
		/// <param name="removeElmName">name used to delete an element from the collection (default is 'remove')</param>
		protected AbstractConfigurationElementCollection(string addElmName
			, string clearElmName
			, string removeElmName)
		{
			base.AddElementName = addElmName;
			base.ClearElementName = clearElmName;
			base.RemoveElementName = removeElmName;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// CollectionType
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		/// <summary>
		/// Number of elements.
		/// </summary>
		public new int Count
		{
			get { return base.Count; }
		}

		#endregion Properties

		#region Indexers

		/// <summary>
		/// Accesses an element by index.
		/// </summary>
		/// <param name="index">element index</param>
		/// <returns>the element at <paramref name="index"/></returns>
		public TElement this[int index]
		{
			get { return (TElement)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		/// <summary>
		/// Accesses a element by key.
		/// </summary>
		/// <param name="key">an element's key</param>
		/// <returns>the element with the given key</returns>
		public TElement this[TKey key]
		{
			get { return (TElement)BaseGet(key); }
		}

		#endregion Indexers

		#region Methods

		/// <summary>
		/// Adds an element.
		/// </summary>
		/// <param name="item"></param>
		public void Add(TElement item)
		{
			BaseAdd(item);
		}

		/// <summary>
		/// Clears the elements.
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}

		/// <summary>
		/// Finds the index of an element.
		/// </summary>
		/// <param name="item">the element</param>
		/// <returns>the index of the element</returns>
		public int IndexOf(TElement item)
		{
			return BaseIndexOf(item);
		}

		/// <summary>
		/// Removes an element.
		/// </summary>
		/// <param name="item">the element</param>
		public void Remove(TElement item)
		{
			BaseRemove(GetElementKey(item));
		}

		/// <summary>
		/// Removes an element by key.
		/// </summary>
		/// <param name="key">the element's key</param>
		public void Remove(TKey key)
		{
			BaseRemove(key);
		}

		/// <summary>
		/// Removes an element at the given index.
		/// </summary>
		/// <param name="index">the element's index</param>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		/// <summary>
		/// Creates a new element of type TElement.
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TElement();
		}

		/// <summary>
		/// Gets the element's key.
		/// </summary>
		/// <param name="element">the element</param>
		/// <returns>the element's key</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return PerformGetElementKey((TElement)element);
		}

		/// <summary>
		/// Abstract method; gets the element's key.
		/// </summary>
		/// <param name="element">the element</param>
		/// <returns>the element's key</returns>
		protected abstract TKey PerformGetElementKey(TElement element);

		#endregion Methods
	}
}