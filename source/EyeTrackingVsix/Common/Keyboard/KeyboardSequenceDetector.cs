using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace EyeTrackingVsix.Common
{
    public class KeyboardSequenceDetector
    {
        private readonly IEnumerator<KeyFrame> _frames;
        private readonly IMeassureTime _clock;
        private readonly Action _detected;

        private bool _started;

        public KeyboardSequenceDetector(IMeassureTime clock, Action detected, IEnumerable<KeyFrame> frames)
        {
            _frames = frames.GetEnumerator();
            _frames.MoveNext();
            _clock = clock;
            _detected = detected;
        }

        public void Update(Key key, bool isDown)
        {
            double elapsed = 0;
            if (_started)
            {
                elapsed = _clock.Elapsed.TotalMilliseconds;
            }

            _clock.Start();

            var expected = _frames.Current;
            if (expected.Key == key && expected.Down == isDown)
            {
                if (expected.Time < elapsed)
                {
                    // timeout...
                    _frames.Reset();
                    _frames.MoveNext();
                    _started = false;

                    // or this might be a new attempt
                    Update(key, isDown);
                    return;
                }

                if (_frames.MoveNext() != false)
                {
                    _started = true;
                    return;
                }

                // we successfully reached the end of the sequence
                _started = false;
                _detected();
            }

            _frames.Reset();
            _frames.MoveNext();
            _started = false;
        }
    }
}
