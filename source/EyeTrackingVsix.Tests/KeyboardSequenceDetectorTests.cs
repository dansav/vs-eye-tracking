using System;
using System.Windows.Input;
using EyeTrackingVsix.Common;
using NUnit.Framework;

namespace EyeTrackingVsix.Tests
{
    public class KeyboardSequenceDetectorTests
    {
        [Test]
        public void  UpdateSingleFrame_CorrectKey_CompletesSequence()
        {
            var done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
            });

            sequence.Update(Key.RightCtrl, true);

            Assert.That(done, Is.True);
        }

        [Test]
        public void UpdateSingleFrame_OthertKey_DoesNotCompletesSequence()
        {
            var done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
            });

            sequence.Update(Key.Space, true);

            Assert.That(done, Is.False);
        }

        [Test]
        public void UpdateSingleFrame_CalledManyTimes_CanBeReused()
        {
            int done = 0;
            var clock = new FakeClock();
            Action callback = () => done++;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
            });

            sequence.Update(Key.RightCtrl, true);
            sequence.Update(Key.RightCtrl, true);
            sequence.Update(Key.RightCtrl, true);
            sequence.Update(Key.RightCtrl, true);

            Assert.That(done, Is.EqualTo(4));
        }

        [Test]
        public void UpdateMultipleFrames_CorrectInput_CompletesSequence()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 0),
            });

            sequence.Update(Key.RightCtrl, true);
            sequence.Update(Key.RightCtrl, false);

            Assert.That(done, Is.True);
        }
        [Test]
        public void UpdateMultipleFrames_CorrectInputWithinTime_CompletesSequence()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(99);
            sequence.Update(Key.RightCtrl, false);

            Assert.That(done, Is.True);
        }

        [Test]
        public void UpdateMultipleFrames_TooSlow_SequenceFailes()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(200);
            sequence.Update(Key.RightCtrl, false);

            Assert.That(done, Is.False);
        }

        [Test]
        public void UpdateMultipleFrames_TooSlowThenNewValidAtempt_SequenceCompletes()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(200);
            sequence.Update(Key.RightCtrl, false);

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(99);
            sequence.Update(Key.RightCtrl, false);


            Assert.That(done, Is.True);
        }

        [Test]
        public void UpdateMultipleFrames_DoubleTap_SequenceCompletes()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
                new KeyFrame(Key.RightCtrl, true, 200),
                new KeyFrame(Key.RightCtrl, false, 100),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(100);
            sequence.Update(Key.RightCtrl, false);

            clock.Elapsed = TimeSpan.FromMilliseconds(200);
            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(99);
            sequence.Update(Key.RightCtrl, false);


            Assert.That(done, Is.True);
        }

        [Test]
        public void UpdateMultipleFrames_DoubleTap_FailsIfNotReleasingFastEnough()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
                new KeyFrame(Key.RightCtrl, true, 200),
                new KeyFrame(Key.RightCtrl, false, 100),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(100);
            sequence.Update(Key.RightCtrl, false);

            clock.Elapsed = TimeSpan.FromMilliseconds(200);
            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(300);
            sequence.Update(Key.RightCtrl, false);


            Assert.That(done, Is.False);
        }

        [Test]
        public void UpdateMultipleFrames_DoubleTapAndHold_SequenceCompletes()
        {
            bool done = false;
            var clock = new FakeClock();
            Action callback = () => done = true;

            var sequence = new KeyboardSequenceDetector(
            clock,
            callback,
            new[]
            {
                new KeyFrame(Key.RightCtrl, true, 0),
                new KeyFrame(Key.RightCtrl, false, 100),
                new KeyFrame(Key.RightCtrl, true, 200),
                new KeyFrame(Key.RightCtrl, true, 300),
            });

            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(100);
            sequence.Update(Key.RightCtrl, false);

            clock.Elapsed = TimeSpan.FromMilliseconds(200);
            sequence.Update(Key.RightCtrl, true);

            clock.Elapsed = TimeSpan.FromMilliseconds(300);
            sequence.Update(Key.RightCtrl, true);


            Assert.That(done, Is.True);
        }
    }

    public class FakeClock : IMeassureTime
    {
        public TimeSpan Elapsed { get; set; }

        public void Start()
        {
            Elapsed = TimeSpan.Zero;
        }
    }
}
