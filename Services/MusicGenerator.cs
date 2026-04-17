using System;
using System.IO;
using NAudio.Wave;
namespace MusicAlbums.Services
{
    public class MusicGenerator
    {
        public byte[] Generate(int seed, int durationSeconds = 10)
        {
            double[] frequencies = new double[]
            {
            261.63, // C
            293.66, // D
            329.63, // E
            349.23, // F
            392.00, // G
            440.00, // A
            493.88  // B
            };
            int sampleRate = 44100;
            int totalSamples = sampleRate * durationSeconds;

            var random = new Random(seed);

            float[] samples = new float[totalSamples];

            int currentSample = 0;

            while (currentSample < totalSamples)
            {
                double freq1 = frequencies[random.Next(frequencies.Length)];

                // вторая нота (иногда есть, иногда нет)
                bool useSecond = random.NextDouble() > 0.5;
                double freq2 = frequencies[random.Next(frequencies.Length)];

                int noteLength = random.Next(
                    (int)(sampleRate * 0.2),
                    (int)(sampleRate * 0.8)
                );

                for (int i = 0; i < noteLength && currentSample < totalSamples; i++)
                {
                    double t = (double)i / sampleRate;

                    // плавное затухание
                    double fade = 1.0 - (double)i / noteLength;

                    double sample = Math.Sin(2 * Math.PI * freq1 * t);

                    if (useSecond)
                    {
                        sample += 0.5 * Math.Sin(2 * Math.PI * freq2 * t);
                    }

                    samples[currentSample] = (float)(0.3 * fade * sample);

                    currentSample++;
                }
            }

            return ConvertToWav(samples, sampleRate);
        }
        private byte[] ConvertToWav(float[] samples, int sampleRate)
        {
            using var ms = new MemoryStream();
            var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 1);

            using (var writer = new WaveFileWriter(ms, waveFormat))
            {
                writer.WriteSamples(samples, 0, samples.Length);
            }

            return ms.ToArray();
        }
    }
}
    