/**********************************************************************
 * 
 * Update Controls .NET
 * Copyright 2010 Michael L Perry
 * MIT License
 * 
 * http://updatecontrols.net
 * http://www.codeplex.com/updatecontrols/
 * 
 **********************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KnockoutCS
{
    /// <summary>
    /// Base class for <see cref="Dynamic"/> and <see cref="Computed"/> sentries.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    /// <remarks>
    /// This class is for internal use only.
    /// </remarks>
    public abstract class Precedent
    {
		internal class ComputedNode
        {
            public WeakReference Computed;
            public ComputedNode Next;
        }

        internal ComputedNode _firstComputed = null;

        /// <summary>
        /// Method called when the first computed references this field. This event only
        /// fires when HasComputeds goes from false to true. If the field already
        /// has computeds, then this event does not fire.
        /// </summary>
        protected virtual void GainComputed()
        {
        }

        /// <summary>
        /// Method called when the last computed goes out-of-date. This event
        /// only fires when HasComputeds goes from true to false. If the field has
        /// other computeds, then this event does not fire. If the computed is
        /// currently updating and it still depends upon this field, then the
        /// GainComputed event will be fired immediately.
        /// </summary>
        protected virtual void LoseComputed()
        {
        }

        /// <summary>
        /// Establishes a relationship between this precedent and the currently
        /// updating computed.
        /// </summary>
        internal void RecordComputed()
        {
            // Get the current computed.
            Computed update = Computed.GetCurrentUpdate();
            if (update != null && !Contains(update) && update.AddPrecedent(this))
            {
                if (Insert(update))
                    GainComputed();
            }
            else if (!Any())
            {
                // Though there is no lasting dependency, someone
                // has shown interest.
                GainComputed();
                LoseComputed();
            }
        }

        /// <summary>
        /// Makes all direct and indirect computeds out of date.
        /// </summary>
        internal void MakeComputedsOutOfDate()
        {
            // When I make a computed out-of-date, it will
            // call RemoveComputed, thereby removing it from
            // the list.
            Computed first;
            while ((first = First()) != null)
            {
                first.MakeOutOfDate();
            }
        }

        internal void RemoveComputed(Computed computed)
        {
            if (Delete(computed))
                LoseComputed();
        }

        /// <summary>
        /// True if any other fields depend upon this one.
        /// </summary>
        /// <remarks>
        /// If any computed field has used this observable field while updating,
        /// then HasComputeds is true. When that computed becomes out-of-date,
        /// however, it no longer depends upon this field.
        /// <para/>
        /// This property is useful for caching. When all computeds are up-to-date,
        /// check this property for cached fields. If it is false, then nothing
        /// depends upon the field, and it can be unloaded. Be careful not to
        /// unload the cache while computeds are still out-of-date, since
        /// those computeds may in fact need the field when they update.
        /// </remarks>
        public bool HasComputeds
		{
            get { return Any(); }
		}

        private bool Insert(Computed update)
        {
            lock (this)
            {
                bool first = _firstComputed == null;
                _firstComputed = new ComputedNode { Computed = new WeakReference(update), Next = _firstComputed };
                return first;
            }
        }

        private static int _referenceCount = 0;

        private bool Delete(Computed computed)
        {
            lock (this)
            {
                int count = 0;
                ComputedNode prior = null;
                for (ComputedNode current = _firstComputed; current != null; current = current.Next)
                {
                    object target = current.Computed.Target;
                    if (target == null || target == computed)
                    {
                        if (target == null)
                            System.Diagnostics.Debug.WriteLine(String.Format("Dead reference {0}", _referenceCount++));
                        if (target == computed)
                            ++count;
                        if (prior == null)
                            _firstComputed = current.Next;
                        else
                            prior.Next = current.Next;
                    }
                    else
                        prior = current;
                }
				if (count != 1) Debug.Assert(false, String.Format("Expected 1 computed, found {0}.", count));
                return _firstComputed == null;
            }
        }

        private bool Contains(Computed update)
        {
            lock (this)
            {
                for (ComputedNode current = _firstComputed; current != null; current = current.Next)
                    if (current.Computed.Target == update)
                        return true;
                return false;
            }
        }

        private bool Any()
        {
            lock (this)
            {
                return _firstComputed != null;
            }
        }

        private Computed First()
        {
            lock (this)
            {
                while (_firstComputed != null)
                {
                    Computed computed = (Computed)_firstComputed.Computed.Target;
                    if (computed != null)
                        return computed;
                    else
                        _firstComputed = _firstComputed.Next;
                }
                return null;
            }
        }

		public override string ToString()
		{
			return VisualizerName(true);
		}

		#region Debugger Visualization

		/// <summary>Gets or sets a flag that allows extra debug features.</summary>
		/// <remarks>
		/// This flag currently just controls automatic name detection for untitled
		/// NamedObservables, and other precedents that were created without a name 
		/// by calling <see cref="Observable.New"/>() or <see cref="Computed.New"/>(),
		/// including computeds created implicitly by <see cref="GuiUpdateHelper"/>.
		/// <para/>
		/// DebugMode should be enabled before creating any UpdateControls sentries,
		/// otherwise some of them may never get a name. For example, if 
		/// Indepedent.New() is called (without arguments) when DebugMode is false, 
		/// a "regular" <see cref="Observable"/> is created that is incapable of 
		/// having a name.
		/// <para/>
		/// DebugMode may slow down your program. In particular, if you use named 
		/// observables (or <see cref="Observable{T}"/>) but do not explicitly 
		/// specify a name, DebugMode will cause them to compute their names based 
		/// on a stack trace the first time OnGet() is called; this process is
		/// expensive if it is repeated for a large number of Observables.
		/// </remarks>
		public static bool DebugMode { get; set; }
		
		public virtual string VisualizerName(bool withValue)
		{
			return VisNameWithOptionalHash(GetType().Name, withValue);
		}
		protected string VisNameWithOptionalHash(string name, bool withHash)
		{
			if (withHash) {
				// Unless VisualizerName has been overridden, we have no idea what 
				// value is associated with the Precedent. Include an ID code so 
				// that the user has a chance to detect duplicates (that is, when
				// he sees two Observables with the same code, they are probably 
				// the same Observable.)
				return string.Format("{0} #{1:X5}", name, GetHashCode() & 0xFFFFF);
			} else
				return name;
		}

		protected class ComputedVisualizer
		{
			Precedent _self;
			public ComputedVisualizer(Precedent self) { _self = self; }
			public override string ToString() { return _self.VisualizerName(true); }

			[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
			public ComputedVisualizer[] Items
			{
				get {
					var list = new List<ComputedVisualizer>();
					lock (_self)
					{
						for (ComputedNode current = _self._firstComputed; current != null; current = current.Next)
						{
							var dep = current.Computed.Target as Computed;
							if (dep != null)
								list.Add(new ComputedVisualizer(dep));
						}

						list.Sort((a, b) => a.ToString().CompareTo(b.ToString()));

						// Return as array so that the debugger doesn't offer a useless "Raw View"
						return list.ToArray();
					}
				}
			}
		}

		#endregion
	}
}
