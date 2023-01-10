using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CardboardBread.WorkLater.Utilities
{
    // A type to replace recording a numerical tick delay in a class. Attempts
    // to always keep the delay as a distance from the current/latest tick,
    // and therefore be save/load resilient. Recording a tick delay using this
    // type should allow you to save the tick delay into a game's save data
    // reliably and accurately.
    //
    // If you are going to update a tick delay every tick (like in a Component
    // class) then it is best to not use this class, as it is designed for
    // infrequent updates/accesses.
    // 
    // As an aside - if a tick delay is recorded once - saving that value into
    // game save data is not accurate, as the ticks elapsed between the
    // original recording of the tick delay and the point of the save usually
    // differ. The outcome of this is that after loading this inaccurate tick
    // delay, it represents a point in 'time' that is further than the
    // original delay.
    public struct ResumableTickDelay : ILoadReferenceable
    {
        // Implicit operators to make using this type cleaner, as it will appear like an analog for a long
        public static implicit operator long(ResumableTickDelay resumable) => resumable.Delay;

        public static implicit operator ResumableTickDelay(long value) => new ResumableTickDelay(value);

        // Easy saving/loading by making it compatible with Scribe_Values.
        public static ResumableTickDelay ParseResumableTickDelay(string str)
        {
            if (!long.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                throw new ArgumentException($"Failed to parse '{result}'");
            }
            return result;
        }

        static ResumableTickDelay()
        {
            ParseHelper.Parsers<ResumableTickDelay>.Register(ParseResumableTickDelay);
        }


        private long _delay;
        private long _lastAccessTick;
        public readonly long CreationTick;

        public ResumableTickDelay(long delay)
        {
            _delay = delay;
            _lastAccessTick = Find.TickManager.TicksGame;
            CreationTick = Find.TickManager.TicksGame;
        }

        public long Delay
        {
            get
            {
                UpdateDelay();
                return _delay;
            }
            set
            {
                _delay = value;
                _lastAccessTick = Find.TickManager.TicksGame;
            }
        }

        public long LastAccessTick => _lastAccessTick;

        // Effectively recreate this type at the current tick, updating the old delay value by how long it's been since last access.
        private void UpdateDelay()
        {
            var currentTick = Find.TickManager.TicksGame;
            if (_lastAccessTick != currentTick)
            {
                var distance = currentTick - _lastAccessTick;
                _delay -= distance;
            }
            _lastAccessTick = currentTick;
        }

        public override string ToString() => Delay.ToString();

        string ILoadReferenceable.GetUniqueLoadID() => $"{GetType().Name}_{this.GetHashCode()}";
    }
}
