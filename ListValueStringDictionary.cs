/**************************************************************************
Copyright (C) 2023 Rekkonnect

This file is part of CompactView.

CompactView is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

CompactView is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with CompactView.  If not, see <http://www.gnu.org/licenses/>.

CompactView web site <http://sourceforge.net/p/compactview/>.
**************************************************************************/

using Garyon.DataStructures;
using System.Collections.Generic;

namespace CompactView
{
    internal class ListValueStringDictionary<T> : FlexibleInitializableValueDictionary<string, List<T>>, IReadOnlyDictionary<string, IReadOnlyList<T>>
    {
        protected override List<T> GetNewEntryInitializationValue()
        {
            return new List<T>();
        }

        public void Add(string key, T value)
        {
            this[key].Add(value);
        }

        public bool TryGetValue(string key, out IReadOnlyList<T> value)
        {
            throw new System.NotImplementedException();
        }

        #region IReadOnlyDictionary implementation
        IEnumerable<string> IReadOnlyDictionary<string, IReadOnlyList<T>>.Keys => Keys;

        IEnumerable<IReadOnlyList<T>> IReadOnlyDictionary<string, IReadOnlyList<T>>.Values => Values;

        IReadOnlyList<T> IReadOnlyDictionary<string, IReadOnlyList<T>>.this[string key] => this[key];

        IEnumerator<KeyValuePair<string, IReadOnlyList<T>>> IEnumerable<KeyValuePair<string, IReadOnlyList<T>>>.GetEnumerator()
        {
            foreach (var e in this)
                yield return new KeyValuePair<string, IReadOnlyList<T>>(e.Key, e.Value);
        }
        #endregion
    }
}
