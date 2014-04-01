/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2011 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;

namespace KnockoutCS.Collections
{
    public class ComputedList<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerable<T>> _computeCollection;

        private List<T> _list = new List<T>();
        private Computed _dependentSentry;

        public ComputedList(Func<IEnumerable<T>> computeCollection)
        {
            _computeCollection = computeCollection;

            _dependentSentry = new NamedComputed(MemoizedTypeName<ComputedList<T>>.GenericName(),
			delegate {
                using (var bin = new RecycleBin<T>(_list))
                {
                    _list.Clear();

                    foreach (T item in computeCollection())
                        _list.Add(bin.Extract(item));
                }
            });
        }

        public int IndexOf(T item)
        {
            _dependentSentry.OnGet();
            return _list.IndexOf(item);
        }

        public T this[int index]
        {
            get
            {
                _dependentSentry.OnGet();
                return _list[index];
            }
        }

        public bool Contains(T item)
        {
            _dependentSentry.OnGet();
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dependentSentry.OnGet();
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { _dependentSentry.OnGet(); return _list.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            _dependentSentry.OnGet();
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            _dependentSentry.OnGet();
            return ((System.Collections.IEnumerable)_list).GetEnumerator();
        }

        public Computed ComputedSentry
        {
            get { return _dependentSentry; }
        }
    }
}
