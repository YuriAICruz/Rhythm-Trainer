using UnityEngine;
using UnityEngine.UI;

namespace Graphene.Rhythm.Presentation
{
    public class VignetePulse : MonoBehaviour
    {
        private Material _mat;

        private float[] samples;
        private float[] spectrum;
        private int qSamples = 512;
        private int fSample;
        private float refValue = 0.1f;
        private float threshold = 0.02f;
        private float _lastPitch;

        public float Smooth = 1;
        [SerializeField]private float _amp = 0.05f;

        private void Awake()
        {
            _mat = GetComponent<Image>().material;
            
            samples = new float[qSamples];
            spectrum = new float[qSamples];
        }

        private void Start()
        {
            fSample = AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            var s = AnalyzeSound();
            _lastPitch = Mathf.Lerp(_lastPitch, s, Time.deltaTime * Smooth);
            // _mat.SetFloat("_Intensity", _lastPitch);
        }

        float AnalyzeSound()
        {
            AudioListener.GetOutputData(samples, 0);
            
            _mat.SetFloatArray("_FFT", samples);
            var sum = 0f;
            for (var i = 0; i < qSamples; i++)
            {
                sum += samples[i] * samples[i]; // sum squared samples
            }
            var rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
            var dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
            if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
            // get sound spectrum
            AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
            var maxV = 0f;
            var maxN = 0;
            for (var i = 0; i < qSamples; i++)
            {
                // find max 
                if (spectrum[i] > maxV && spectrum[i] > threshold)
                {
                    maxV = spectrum[i];
                    maxN = i; // maxN is the index of max
                }
            }
            var freqN = (float)maxN; // pass the index to a float variable
            if (maxN > 0 && maxN < qSamples - 1)
            {
                // interpolate index using neighbours
                var dL = spectrum[maxN - 1] / spectrum[maxN];
                var dR = spectrum[maxN + 1] / spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            return freqN * (fSample / 2f) / qSamples * _amp; // convert index to frequency
        }
    }
}