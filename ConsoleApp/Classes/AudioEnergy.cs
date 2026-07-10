using System;
using System.Collections.Generic;
using NAudio.Wave;

namespace Classes
{
    public class AudioEnergy
    {
        public AudioEnergy(string wavPath, int videoFrameCount, int videoFramesPerSecond)
        {
            _wavPath = wavPath;
            _videoFrameCount = videoFrameCount;
            _videoFramesPerSecond = videoFramesPerSecond;

            int sampleRate = 0;
            List<float> samples = new();

            using (AudioFileReader audioFileReader = new AudioFileReader(_wavPath))
            {
                sampleRate = audioFileReader.WaveFormat.SampleRate;

                float[] buffer = new float[sampleRate];
                int read = 0;

                while ((read = audioFileReader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        samples.Add(buffer[i]);
                    }
                }
            }

            int windowSize = sampleRate / 10;
            List<float> rmsEnergy = new();

            for (int i = 0; i + windowSize < samples.Count; i += windowSize)
            {
                float sum = 0f;

                for (int j = 0; j < windowSize; j++)
                {
                    float sample = samples[i + j];

                    sum += sample * sample;
                }

                float value = MathF.Sqrt(sum / windowSize);

                rmsEnergy.Add(value);
            }

            // envelope smoothing
            List<float> smoothed = new();
            float previous = 0f;
            float attack = 0.05f;
            float release = 0.02f;

            foreach (float value in rmsEnergy)
            {
                float rate = value > previous ? attack : release;
                
                previous += (value - previous) * rate;
                smoothed.Add(previous);
            }

            // normalize values to 0..1
            float max = 0f;

            foreach (float value in smoothed)
            {
                if (value > max)
                {
                    max = value;
                }
            }

            if (max < 1e-6f) // 0.000001
            {
                max = 1f;
            }

            for (int i = 0; i < smoothed.Count; i++)
            {
                smoothed[i] /= max;
            }

            // resample to video frame rate
            _audioEnergy = new float[_videoFrameCount];
            
            float audioDuration = samples.Count / ((float)sampleRate);

            for (int i = 0; i < _videoFrameCount; i++)
            {
                float time = i / ((float)_videoFramesPerSecond);
                float audioPosition = time / audioDuration;
                int index = ((int)(audioPosition * smoothed.Count));

                index = Math.Clamp(index, 0, smoothed.Count - 1);
                _audioEnergy[i] = smoothed[index];
            }
        }

        private string _wavPath { get; set; }
        private int _videoFrameCount { get; set; }
        private int _videoFramesPerSecond { get; set; }
        private float[] _audioEnergy { get; set; }
    }
}