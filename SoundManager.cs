using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Threading;
using NAudio;
using NAudio.Wave;

namespace Snake
{
    // This class manages background music and sound effects
    // I wasted 2 days on this
    class SoundManager
    {
        public WaveOutEvent outputDevice;
        public AudioFileReader audioFile;

        // Play a background track in loop
        public void PlayLoop(string filePath)
        {
            Stop();  // stop previous track if any

            audioFile = new AudioFileReader(filePath);
            var loop = new LoopStream(audioFile);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(loop);
            outputDevice.Play();
        }

        // Play a one-shot sound effect
        public void PlayOnce(string filePath)
        {
            var reader = new AudioFileReader(filePath);
            var oneShot = new WaveOutEvent();
            oneShot.Init(reader);
            oneShot.Play();

            oneShot.PlaybackStopped += (s, e) =>
            {
                reader.Dispose();
                oneShot.Dispose();
            };
        }

        // Stop background music
        public void Stop()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }
    }

    // Helper class to loop a WaveStream
    class LoopStream : WaveStream
    {
        private readonly WaveStream sourceStream;

        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
        }

        public override WaveFormat WaveFormat => sourceStream.WaveFormat;
        public override long Length => long.MaxValue;
        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int read = sourceStream.Read(buffer, offset, count);
            if (read == 0)
            {
                sourceStream.Position = 0;
                read = sourceStream.Read(buffer, offset, count);
            }
            return read;
        }
    }
}

