using System;
using System.Collections.Generic;
using System.Text;
using Backgammon.Utils;

namespace Backgammon.Models
{
    /// <summary>
    /// Class that models the dice.
    /// </summary>
    public class Dice : BaseNotifier
    {
        protected static Random? rand;
        private static readonly object randLock = new object();
        protected int? value;
        protected int max;
        protected bool used;

        // Last thrown value.
        public int? Value { get => value; }

        /// <summary>
        /// Create a new dice.
        /// </summary>
        /// <param name="max">Maximum value on the dice.</param>
        public Dice(int max)
        {
            this.max = max;

            // Create only one random generator.
            if (rand == null)
            {
                lock (randLock)
                {
                    if (rand == null)
                        rand = new Random();
                }

            }
            used = true;
        }

        public void Clear()
        {
            SetProperty(ref value, null);
        }

        /// <summary>
        /// Dice was used and should be rethrown before using again.
        /// </summary>
        public bool Used
        {
            get => used;
            private set => SetProperty(ref used, value);
        }

        /// <summary>
        /// Mark the dice as used. Can no longer be used until it is rethrown.
        /// </summary>
        public void MarkAsUsed()
        {
            Used = true;
        }

        /// <summary>
        /// Thrown the dice and return the new value.
        /// </summary>
        /// <returns></returns>
        public int ThrowNext()
        {
            if (rand == null)
            {
                throw new InvalidOperationException("Random is not set");
            }
            int v = rand.Next(1, max +1);
            SetProperty(ref value, v);
            Used = false;
            return value.Value;
        }
    }
}
