using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace PPJoy
{
    /// <summary>
    ///   A <see cref = "MappingCollection" /> is a specialized collection 
    ///   that can store related <see cref = "Mapping" /> objects together.  This provides for
    ///   ease of handling when many different <see cref = "Mapping" />s must
    ///   be manipulated as a group.
    ///   A <see cref = "MappingCollection" /> exposes
    ///   several sub-collections, from which all <see cref = "Mapping" />s of a particular Type can be retrieved (for instance, all <see cref = "ButtonMapping" />s 
    ///   in the <see cref = "MappingCollection" /> can be retrieved from the <see cref = "MappingCollection.ButtonMappings" /> property.
    /// </summary>
    [Serializable]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public sealed class MappingCollection : IEnumerable, ICollection, IList, ICloneable
    {
        private readonly ArrayList _controls = new ArrayList();

        /// <summary>
        ///   Gets a <see cref = "MappingCollection" /> containing all the <see cref = "ButtonMapping" /> objects contained in this <see cref = "MappingCollection" />.
        /// </summary>
        public MappingCollection ButtonMappings
        {
            get { return GetAllOfType(typeof (ButtonMapping)); }
        }

        /// <summary>
        ///   Gets a <see cref = "MappingCollection" /> containing all the <see cref = "PovMapping" />s contained in this <see cref = "MappingCollection" />.
        /// </summary>
        public MappingCollection PovMappings
        {
            get { return GetAllOfType(typeof (PovMapping)); }
        }

        /// <summary>
        ///   Gets a <see cref = "MappingCollection" /> containing all the <see cref = "AxisMapping" />s contained in this <see cref = "MappingCollection" />.
        /// </summary>
        public MappingCollection AxisMappings
        {
            get { return GetAllOfType(typeof (AxisMapping)); }
        }

        /// <summary>
        ///   Gets an item from the <see cref = "MappingCollection" />, given its index number.
        /// </summary>
        /// <param name = "index">The zero-based index of the item to retrieve.</param>
        /// <returns>The <see cref = "Mapping" /> object corresponding to the given index in the <see cref = "MappingCollection" />.</returns>
        public Mapping this[int index]
        {
            get { return (Mapping) ((IList) _controls)[index]; }
            set { ((IList) _controls)[index] = value; }
        }

        #region ICloneable Members

        /// <summary>
        ///   Creates a shallow copy of the <see cref = "MappingCollection" />.
        /// </summary>
        /// <returns>A shallow copy of the <see cref = "MappingCollection" />.</returns>
        public object Clone()
        {
            return _controls.Clone();
        }

        #endregion

        #region ICollection Members

        /// <summary>
        ///   Gets the number of <see cref = "Mapping" />s in the <see cref = "MappingCollection" />.
        /// </summary>
        public int Count
        {
            get { return _controls.Count; }
        }

        /// <summary>
        ///   Copies the members of the <see cref = "MappingCollection" /> to an array.
        /// </summary>
        /// <param name = "array">An Array to copy the <see cref = "MappingCollection" />'s members to.</param>
        /// <param name = "index">The zero-based index into the array at which copying should begin at.</param>
        public void CopyTo(Array array, int index)
        {
            if (!(array is Mapping[]))
            {
                throw new ArgumentException("Argument value is not of type " + typeof (Mapping).Name);
            }
            _controls.CopyTo(array, index);
        }

        /// <summary>
        ///   Gets a value indicating whether access to the <see cref = "MappingCollection" /> is synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return _controls.IsSynchronized; }
        }

        /// <summary>
        ///   Gets an object that can be used to synchronize access to the <see cref = "MappingCollection" />.
        /// </summary>
        public object SyncRoot
        {
            get { return _controls.SyncRoot; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        ///   Gets an <see cref = "IEnumerator" /> that can be used to iterate over the <see cref = "MappingCollection" />.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _controls.GetEnumerator();
        }

        #endregion

        #region IList Members

        /// <summary>
        ///   Adds a <see cref = "System.Object" /> (referencing a <see cref = "Mapping" /> object) to the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">an <see cref = "System.Object" /> (referencing a <see cref = "Mapping" /> object) to add to the <see cref = "MappingCollection" />.</param>
        /// <returns>The index of the <see cref = "System.Object" /> in the <see cref = "MappingCollection" />.</returns>
        public int Add(object value)
        {
            if (value != null)
            {
                if (value is Mapping)
                {
                    return _controls.Add(value);
                }
                else
                {
                    throw new ArgumentException("Argument value is not of type " + typeof (Mapping).Name);
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        ///   Removes all elements from the <see cref = "MappingCollection" />.
        /// </summary>
        public void Clear()
        {
            _controls.Clear();
        }

        /// <summary>
        ///   Determines whether an element is in the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">The <see cref = "System.Object" /> (referencing a <see cref = "Mapping" />) to locate in the <see cref = "MappingCollection" />. <paramref name = "value" /> can be <see langword = "null" />.</param>
        /// <returns><see langword = "true" /> if the <see cref = "MappingCollection" /> contains the specified <see cref = "System.Object" />, or <see langword = "false" /> if it does not.</returns>
        public bool Contains(object value)
        {
            return _controls.Contains(value);
        }

        /// <summary>
        ///   Searches for the specified <see cref = "System.Object" /> and returns the zero-based index of
        ///   the first occurence within the entire <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">The <see cref = "System.Object" /> to locate in the <see cref = "MappingCollection" />.  <paramref name = "value" /> can be <see langword = "null" />.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name = "value" /> 
        ///   within the entire <see cref = "MappingCollection" />, if found; otherwise, -1.</returns>
        public int IndexOf(object value)
        {
            return _controls.IndexOf(value);
        }

        /// <summary>
        ///   Inserts an element into the <see cref = "MappingCollection" /> at the specified index.
        /// </summary>
        /// <param name = "index">The zero-based index at which <paramref name = "value" /> should be inserted.</param>
        /// <param name = "value">The <see cref = "System.Object" /> to insert. <paramref name = "value" /> can be <see langword = "null" />.</param>
        public void Insert(int index, object value)
        {
            if (value is Mapping)
            {
                _controls.Insert(index, value);
            }
            else
            {
                throw new ArgumentException("Argument value is not of type " + typeof (Mapping).Name);
            }
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref = "MappingCollection" /> has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return _controls.IsFixedSize; }
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref = "MappingCollection" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return _controls.IsReadOnly; }
        }

        /// <summary>
        ///   Removes the first occurrence of a specific <see cref = "System.Object" /> from the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">The <see cref = "System.Object" /> to remove from the <see cref = "MappingCollection" />.  <paramref name = "value" /> can be <see langword = "null" />.</param>
        public void Remove(object value)
        {
            _controls.Remove(value);
        }

        /// <summary>
        ///   Removes the element at the specified index of the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _controls.RemoveAt(index);
        }

        ///<summary>
        ///  Gets or sets the element at the specified index.
        ///</summary>
        ///<param name = "index">The zero-based index of the element to get or set.</param>
        ///<returns>The element at the specified index.</returns>
        ///<exception cref = "System.ArgumentOutOfRangeException"><paramref name = "index" /> is not a valid index in the <see cref = "System.Collections.IList" />.</exception>
        ///<exception cref = "System.NotSupportedException">The property is set and the <see cref = "System.Collections.IList" /> is read-only.</exception>
        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                if (value != null)
                {
                    if (value is Mapping)
                    {
                        this[index] = (Mapping) value;
                    }
                    else
                    {
                        throw new ArgumentException("Argument value is not of type " + typeof (Mapping).Name);
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }
        }

        #endregion

        /// <summary>
        ///   Returns all <see cref = "Mapping" />s from the <see cref = "MappingCollection" /> that are of a specific <see cref = "System.Type" />.
        /// </summary>
        /// <param name = "proto"><see cref = "System.Type" /> of <see cref = "Mapping" /> to return.  All <see cref = "Mapping" />s in the <see cref = "MappingCollection" /> that are of this <see cref = "System.Type" /> will be returned.</param>
        /// <returns>A <see cref = "MappingCollection" /> that is a subset of this <see cref = "MappingCollection" />, containing only <see cref = "Mapping" />s of the specified <see cref = "System.Type" />.</returns>
        private MappingCollection GetAllOfType(Type proto)
        {
            var toReturn = new MappingCollection();
            foreach (Mapping mapping in this)
            {
                if (proto.IsInstanceOfType(mapping))
                {
                    toReturn.Add(mapping);
                }
            }
            return toReturn;
        }

        /// <summary>
        ///   Adds a <see cref = "Mapping" /> to the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">A <see cref = "Mapping" /> to add to the <see cref = "MappingCollection" />.</param>
        /// <returns>The index at which the <see cref = "Mapping" /> was added to the <see cref = "MappingCollection" />.</returns>
        public int Add(Mapping value)
        {
            return ((IList) _controls).Add(value);
        }

        /// <summary>
        ///   Removes a <see cref = "Mapping" /> from the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "obj">A <see cref = "Mapping" /> to remove from the <see cref = "MappingCollection" />.</param>
        public void Remove(Mapping obj)
        {
            ((IList) _controls).Remove(obj);
        }

        /// <summary>
        ///   Sorts the <see cref = "MappingCollection" />.
        /// </summary>
        public void Sort()
        {
            _controls.Sort(new MappingComparer());
        }

        /// <summary>
        ///   Copies the members of the <see cref = "MappingCollection" /> to an array.
        /// </summary>
        /// <param name = "array">A strongly-typed Array (of type <see cref = "Mapping" />[]), to which
        ///   the <see cref = "MappingCollection" />'s members will be copied.</param>
        /// <param name = "index">The zero-based index into the array where copying should begin at.</param>
        public void CopyTo(Mapping[] array, int index)
        {
            _controls.CopyTo(array, index);
        }

        /// <summary>
        ///   Determines whether a specific <see cref = "Mapping" /> is in the <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">The <see cref = "Mapping" /> to locate in the <see cref = "MappingCollection" />. <paramref name = "value" /> can be <see langword = "null" />.</param>
        /// <returns><see langword = "true" /> if the <see cref = "MappingCollection" /> contains the specified <see cref = "Mapping" />, or <see langword = "false" /> 
        ///   if it does not.</returns>
        public bool Contains(Mapping value)
        {
            return _controls.Contains(value);
        }

        /// <summary>
        ///   Searches for the specified <see cref = "Mapping" /> and returns the zero-based index of
        ///   the first occurence within the entire <see cref = "MappingCollection" />.
        /// </summary>
        /// <param name = "value">The <see cref = "Mapping" /> to locate in the <see cref = "MappingCollection" />.  <paramref name = "value" /> can be <see langword = "null" />.</param>
        /// <returns>The zero-based index of the first occurrence of <paramref name = "value" />
        ///   within the entire <see cref = "MappingCollection" />, if found; otherwise, -1.</returns>
        public int IndexOf(Mapping value)
        {
            return _controls.IndexOf(value);
        }

        /// <summary>
        ///   Inserts a <see cref = "Mapping" /> into the <see cref = "MappingCollection" /> at the specified index.
        /// </summary>
        /// <param name = "index">The zero-based index at which <paramref name = "value" /> should be inserted.</param>
        /// <param name = "value">The <see cref = "Mapping" /> to insert.  <paramref name = "value" /> can be <see langword = "nll" />.</param>
        public void Insert(int index, Mapping value)
        {
            _controls.Insert(index, value);
        }
    }
}