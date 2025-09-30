using UnityEngine;
using UnityEngine.UI;
using System;

public class UIElementFader : MonoBehaviour
{
    [Tooltip("Image component of the UI element with which to modify over time.")]
    [SerializeField] private Image m_UIElement;
    private float m_FadeInTime = 0.0f;
    private float m_FadeOutTime = 0.0f;
    private float m_FadeTimeRemaining = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // If m_UIElement is not assigned, try to get it from the same GameObject
        if (m_UIElement == null)
        {
            m_UIElement = GetComponent<Image>();
        }
        
        // If still null, try to get it from children
        if (m_UIElement == null)
        {
            m_UIElement = GetComponentInChildren<Image>();
        }
        
        if (m_UIElement != null)
        {
            m_UIElement.enabled = false;
        }
        else
        {
            Debug.LogError("UIElementFader: No Image component found! Please assign m_UIElement in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Early exit if UI element is not available
        if (m_UIElement == null)
            return;
            
        // Linear interpolation between the remaining time and the total fade time to control opacity.
        // A personal fun exercise on complex branching, a source of many bugs.
        // Anyways, in the case that a proper fade time was given, we interpolate to give the appropriate opacity
        // value. In all cases where fading is done except for when fade out is finished, the Image is enabled.
        // In that one case, fading is disabled instead since no point in rendering an invisble object. Otherwise,
        // nothing is changed.
        //
        // Opacity changing code from:
        // https://discussions.unity.com/t/having-some-serious-trouble-changing-image-alpha/690952

        if (m_FadeOutTime > 0.0f)
        {
            if (m_FadeTimeRemaining > 0.0f)
            {
                m_FadeTimeRemaining -= Time.deltaTime;

                m_UIElement.enabled = true;
                setImageOpacity(m_FadeTimeRemaining / m_FadeOutTime);
            }
            else
            {
                m_UIElement.enabled = false;
            }
        }
        else if (m_FadeInTime > 0.0f)
        {
            m_UIElement.enabled = true;

            if (m_FadeTimeRemaining > 0.0f)
            {
                m_FadeTimeRemaining -= Time.deltaTime;
                setImageOpacity((m_FadeInTime - m_FadeTimeRemaining) / m_FadeInTime);
            }
        }
    }

    // Deactivates the object, which makes the UI element fade out over time, expressed in seconds.
    public void deactivate(float time)
    {
        if (m_UIElement == null)
        {
            Debug.LogWarning("UIElementFader.deactivate: m_UIElement is null, cannot deactivate.");
            return;
        }

        m_FadeInTime = 0.0f;
        m_FadeOutTime = time;
        m_FadeTimeRemaining = time;

        // If the given time is too short, don't bother fading or rendering anything.
        float threshold = 0.01f;

        if (time < threshold)
        {
            m_UIElement.enabled = false;
        }
    }

    // Activates this object, which makes the UI element fade in over time, expressed in seconds.
    public void activate(float time)
    {
        if (m_UIElement == null)
        {
            Debug.LogWarning("UIElementFader.activate: m_UIElement is null, cannot activate.");
            return;
        }
        
        m_FadeInTime = time;
        m_FadeOutTime = 0.0f;
        m_FadeTimeRemaining = time;
    }

    // Returns true if the UI element is finished fading in.
    public bool isDoneActivate()
    {
        return (m_FadeInTime > 0.0f) && (m_FadeTimeRemaining <= 0.0f);
    }

    // Returns true if the UI element is finished fading out.
    public bool isDoneDeactivate()
    {
        return (m_FadeOutTime > 0.0f) && (m_FadeTimeRemaining <= 0.0f);
    }

    // Helper function for the song and dance involving changing the alpha value of an Image component.
    private void setImageOpacity(float opacity)
    {
        var color = m_UIElement.color;
        color.a = opacity;
        m_UIElement.color = color;
    }
}
