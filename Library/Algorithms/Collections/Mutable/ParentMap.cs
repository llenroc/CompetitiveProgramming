#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the express permission of Wesner Moise.
//
// Copyright (C) 2002-2004, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Softperson.Collections
{
    /// <summary>
    /// Summary description for ParentMap.
    /// </summary>
    public class ParentMap<K, V>
        where K : class, IComposite<K>
    {
        #region Variables

        private readonly Dictionary<K, V> hashtable = new Dictionary<K, V>();

        #endregion

        #region Construction

        #endregion

        #region Methods

        /// <summary>
        /// Gets the object for the given type
        /// </summary>
        public V this[K node]
        {
            [DebuggerStepThrough]
            get
            {
                while (true)
                {
                    V result;

                    // Check for a direct result
                    if (hashtable.TryGetValue(node, out result))
                        return result;

                    node = node.Parent;
                    if (node == null)
                        return default(V);
                }
            }

            [DebuggerStepThrough]
            set { hashtable[node] = value; }
        }

        public void Remove(K node)
        {
            hashtable.Remove(node);
        }

        #endregion
    }
}