using UnityEngine;

public class PulseComparison : MonoBehaviour
{
    [SerializeField] private AudioProcessor audioProcessor;
    [SerializeField] private Transform beatObject;      // Pulses on beat
    [SerializeField] private Transform spectrumObject;  // Pulses on spectrum
    
    [Header("Beat Settings")]
    [SerializeField] private float beatPulseSize = 1.3f;
    [SerializeField] private float beatReturnSpeed = 5f;
    
    [Header("Spectrum Settings")]
    [SerializeField] private float spectrumMultiplier = 5f;
    [Tooltip("The frequency band to use for the spectrum. 0=bass, 5=mid, 11=treble")]
    [SerializeField] private int frequencyBand = 0; // 0=bass, 5=mid, 11=treble
    [SerializeField] private float spectrumReturnSpeed = 5f;
    
    private Vector3 beatStartSize;
    private Vector3 spectrumStartSize;

    private void Start()
    {
        if (beatObject != null)
            beatStartSize = beatObject.localScale;
        if (spectrumObject != null)
            spectrumStartSize = spectrumObject.localScale;
        
        if (audioProcessor != null)
        {
            audioProcessor.onBeat.AddListener(OnBeatDetected);
            audioProcessor.onSpectrum.AddListener(OnSpectrumUpdate);
        }
    }

    private void Update()
    {
        // Return beat object to normal size
        if (beatObject != null)
        {
            beatObject.localScale = Vector3.Lerp(
                beatObject.localScale, 
                beatStartSize, 
                Time.deltaTime * beatReturnSpeed
            );
        }
        
        // Return spectrum object to normal size
        if (spectrumObject != null)
        {
            spectrumObject.localScale = Vector3.Lerp(
                spectrumObject.localScale, 
                spectrumStartSize, 
                Time.deltaTime * spectrumReturnSpeed
            );
        }
    }

    private void OnBeatDetected()
    {
        if (beatObject != null)
        {
            beatObject.localScale = beatStartSize * beatPulseSize;
        }
    }

    private void OnSpectrumUpdate(float[] spectrumData)
    {
        if (spectrumObject != null)
        {
            float intensity = spectrumData[frequencyBand];
            spectrumObject.localScale = spectrumStartSize * (1f + intensity * spectrumMultiplier);
        }
    }

    private void OnDestroy()
    {
        if (audioProcessor != null)
        {
            audioProcessor.onBeat.RemoveListener(OnBeatDetected);
            audioProcessor.onSpectrum.RemoveListener(OnSpectrumUpdate);
        }
    }
}

