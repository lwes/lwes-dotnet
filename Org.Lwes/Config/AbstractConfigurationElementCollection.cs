namespace Org.Lwes.Config
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Text;

	public abstract class AbstractConfigurationElementCollection<TElement, TKey> : ConfigurationElementCollection
		where TElement : ConfigurationElement, new()
	{
		#region Constructors

		protected AbstractConfigurationElementCollection()
		{
		}

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

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		public new int Count
		{
			get { return base.Count; }
		}

		#endregion Properties

		#region Indexers

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

		public TElement this[TKey key]
		{
			get { return (TElement)BaseGet(key); }
		}

		#endregion Indexers

		#region Methods

		public void Add(TElement item)
		{
			BaseAdd(item);
		}

		public void Clear()
		{
			BaseClear();
		}

		public int IndexOf(TElement item)
		{
			return BaseIndexOf(item);
		}

		public void Remove(TElement item)
		{
			BaseRemove(GetElementKey(item));
		}

		public void Remove(TKey key)
		{
			BaseRemove(key);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		protected override void BaseAdd(ConfigurationElement element)
		{
			BaseAdd(element, false);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new TElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return PerformGetElementKey((TElement)element);
		}

		protected abstract TKey PerformGetElementKey(TElement element);

		#endregion Methods
	}
}