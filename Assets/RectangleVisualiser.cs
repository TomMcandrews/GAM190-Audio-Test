// Required Unity namespace for working with Unity Engine functionalities
using UnityEngine;

public class RectangleVisualiser : MonoBehaviour // Inherits from MonoBehaviour, making it a script that can be attached to a GameObject.
{
    // Public fields to control the behavior of the sphere visualizer
    public bool reactToBass; // Determines if the sphere should react specifically to bass frequencies.
    public float bounceSpeed = 2.0f; // Controls the speed of the bouncing animation.
    public float scaleSensitivity = 2f; // Controls how sensitive the scaling is to the amplitude of the music.
    private Material material; // Variable to hold the material of the sphere for color changing.
    private Vector3 originalScale; // Stores the original scale of the sphere for reference in scaling.
    private float originalY; // Stores the original Y position for the bouncing animation.

    // Base colors for the sphere that can be changed in the Unity Editor. These are used to influence the sphere's color dynamically.
    public Color baseColorBar1= Color.blue; // Example base color for a sphere reacting to bass.
    public Color baseColorBar2 = Color.red; // Example base color for a sphere reacting to treble.

    void Start() // Start is called before the first frame update.
    {
        material = GetComponent<Renderer>().material; // Gets the material component of the sphere.
        originalScale = transform.localScale; // Stores the original scale of the sphere.
        originalY = transform.position.y; // Stores the original Y position of the sphere.

        // Assigns a base color to the sphere based on whether it is set to react to bass or not.
        material.color = reactToBass ? baseColorBar1 : baseColorBar2;
    }

    void Update() // Update is called once per frame.
    {
        // Accesses the appropriate frequency band value and amplitude from the AudioAnalyser script.
        float bandValue = reactToBass ? AudioAnalyser.freqBands[0] : AudioAnalyser.freqBands[7];
        float amplitude = AudioAnalyser.amplitude;

        // Calls methods responsible for modifying the sphere's behavior based on the audio data.
        Bounce(amplitude);
        Scale(amplitude);
        Rotate();
        ChangeColor(amplitude, bandValue);
    }

    void Bounce(float amplitude)
    {
        // Calculates a new position for the sphere to create a bouncing effect.
        Vector3 newPosition = new Vector3(transform.position.x, originalY + Mathf.Sin(Time.time * bounceSpeed) * amplitude, transform.position.z);
        transform.position = newPosition; // Applies the new position.
    }

    void Scale(float amplitude)
    {
        // Dynamically scales the sphere based on the amplitude of the music.
        //transform.localScale = originalScale + originalScale * amplitude * scaleSensitivity;
        
        transform.localScale = new Vector3(1f, transform.localScale.y * amplitude * scaleSensitivity + 1f, 1f);
        
    }

    void Rotate()
    {
        // Rotates the sphere. The direction of rotation is determined by whether the sphere reacts to bass or not.
        float rotationSpeed = 30f; // Defines the speed of rotation.
        transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed * (reactToBass ? 1 : -1)); // Applies the rotation.
    }

    void ChangeColor(float amplitude, float bandValue)
    {
        // Changes the color of the sphere dynamically based on the amplitude and the specific frequency band value.
        Color targetColor = (reactToBass ? baseColorBar1 : baseColorBar2) * (1 + bandValue * amplitude);
        material.color = Color.Lerp(material.color, targetColor, Time.deltaTime); // Smoothly transitions to the target color.
    }
}
