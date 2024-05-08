// Required Unity namespace for working with Unity Engine functionalities
using UnityEngine;

// This attribute ensures that an AudioSource component is attached to the GameObject this script is attached to.
[RequireComponent(typeof(AudioSource))]
public class AudioAnalyser : MonoBehaviour // Inherits from MonoBehaviour, making it a script that can be attached to a GameObject.
{
    private AudioSource audioSource; // Variable to hold the AudioSource component from which we'll analyse the audio data.
    public static float[] freqBands = new float[8]; // Public static array to hold the calculated values for 8 frequency bands, allowing access from other scripts.
    private float[] samples = new float[512]; // Private array to hold audio spectrum data. The size 512 is chosen for detailed analysis.
    public static float amplitude; // Public static variable to hold the calculated overall amplitude of the audio being analyzed.

    void Start() // Start is called before the first frame update.
    {
        audioSource = GetComponent<AudioSource>(); // Gets the AudioSource component attached to the same GameObject as this script.
    }

    void Update() // Update is called once per frame.
    {
        GetSpectrumData(); // Method call to get the spectrum data from the audio source.
        MakeFrequencyBands(); // Method call to divide the spectrum data into frequency bands.
        CalculateAmplitude(); // Method call to calculate the overall amplitude from the frequency bands.
    }

    void GetSpectrumData()
    {
        // Retrieves the spectrum data from the audio source and stores it in the samples array.
        // FFTWindow.Blackman is used as the windowing function for the FFT (Fast Fourier Transform) to reduce leakage.
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    void MakeFrequencyBands()
    {
        /*
         * This method divides the spectrum into 8 distinct frequency bands.
         * The division is not linear; each subsequent band covers a wider frequency range.
         * The 'average' variable holds the sum of the samples in each band, which is then averaged and scaled.
         */
        int count = 0;
        for (int i = 0; i < 8; i++) // Loop through each of the 8 bands.
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2; // Calculate the number of samples in this band.

            if (i == 7) // Special case for the last band to ensure all samples are used.
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++) // Sum the samples for this band.
            {
                average += samples[count] * (count + 1); // Weight the sample value.
                count++;
            }

            average /= count; // Calculate the average for this band.

            freqBands[i] = average * 10; // Scale the average and assign to the frequency bands array.
        }
    }

    void CalculateAmplitude()
    {
        /*
         * This method calculates the overall amplitude of the audio by summing up
         * the values in the frequency bands and then normalizing the sum.
         */
        float currentAmplitude = 0f;
        for (int i = 0; i < freqBands.Length; i++) // Loop through each frequency band.
        {
            currentAmplitude += freqBands[i]; // Sum the values.
        }
        amplitude = currentAmplitude / 8; // Normalize the amplitude value by dividing by the number of bands.
    }
}

