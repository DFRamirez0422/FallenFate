/*
    Author: Jose Escobedo
    Created on: Friday, September 22, 2025 13:26 for UNITY ENGINE 6000.1.9f1

    Description:
    UI behavior script for the Music Bar to visualize the rhythm combat to the player.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Timeline;

namespace NPA_RhythmBonusPrefabs
{
    
    /*
        DESCRIPTION: MusicBarUI
        AUTHOR: David Glazier
        CREATED ON: 2025-11-20
        DESCRIPTION:
        This script performs the movment of music bar staff elements on object transform allowing us to use this on any object instead of just UI elements. 
    */
    public class DG_MusicBarUI : MonoBehaviour
    {
        // ========================= Public Fields =========================//

        /*
            Tells the music staff to start animating and appear on screen. To be used when the
            music staff will be needed to show on screen.
        */
        public void Activate()
        {
            onActivate();
        }

        /*
            Tells the music staff to hide from the screen and stop processing. To be used when
            the music staff is no longer needed to be on screen.
        */
        public void Deactivate()
        {
            onDeactivate();
        }

        // ========================= Private Fields =========================//

        [Tooltip("The instance of the rhythm music player.")]
        [SerializeField] private RhythmMusicPlayer music;  // Music clock

        [Tooltip("The key to press to make the music staff appear or disappear from view.")]
        [SerializeField] private KeyCode m_StaffAppearKey = KeyCode.Tab;

        [Tooltip("Center GameObject that serves as the reference point for the music bar.")]
        [SerializeField] private Transform m_CenterObject;

        [Tooltip("Width of the music staff (distance from center to each end).")]
        [SerializeField] private float m_StaffWidth = 10.0f;

        [Tooltip("Prefab for the beat marker object.")]
        [SerializeField] private GameObject m_BeatMarkerPrefab;

        [Tooltip("UI element fader instance from the staff center image.")]
        [SerializeField] private UIElementFader m_StaffCenterUI;

        [Tooltip("Speed to fade in the beats from the ends of the staff.")]
        [SerializeField] private float m_BeatAppearanceSpeed = 2.0f;

        [Tooltip("Speed for the music staff to pop in and pop out when activate() or deactivate() are called.")]
        [SerializeField] private float m_StaffPopInOutSpeed = 2.0f;

        [Tooltip("Flag on whether or not to start the music staff as soon as it is initialized.")]
        [SerializeField] private bool m_IsActiveImmediately = false;

        /*
            How do we implement the moving beat makers?
            Well, we keep a list of all the instances of the prefab that we initialize on Start(). This list
            all start as inactive at first at both extremes of the staff. On each update, activate one of
            the markers on both ends and move them closer to the center. On activation, they fade from
            transparent to opaque over time. On each beat, we again cycle by activating more markers from
            both ends. Once some markers reach the center, we deactivate and move them to the ends.

            I envision this working as an ad-hoc FIFO queue structure. The first to go in are the first to be
            swapped out.

            Now internally, the list is ordered such that either end of the staff correspond to either end
            of the list. Thus retrieving by index is relatively simple. On the other hand, a variable
            is needed to keep track on which position in the queue is the tail to send swapped out markers to.

            To keep things simple, about four beats will be on the staff, so eight entries total.
        */
        private List<GameObject> m_BeatMarkersList = new List<GameObject>();

        // Extreme positions of the music staff.
        private Vector3 m_LeftmostEnd;
        private Vector3 m_RightmostEnd;

        // Amount of time since initialization or activation.
        private float m_ElapsedTime = 0.0f;

        // As mentioned above, in order to keep things simple, we will initialize to 4 beats on the staff.
        private const int s_NumBeatMarkers = 4;

        // Current index within the queue to the head, aka where the closest markers to the center are.
        private int m_CurrentHeadIndex = 0;

        // Current index within the queue to the tail, aka where the farthest away markers are located.
        private int m_CurrentTailIndex = 0;

        // Flag to be set only when deactivate() was called but the UI is still visible on screen.
        private bool m_IsFading = false;

        // Active flag. The music staff will only work when this flag is set.
        private bool m_IsActive = false;

        private void Start()
        {
            m_IsActive = false;
            m_IsFading = false;

            if (m_IsActiveImmediately)
            {
                Activate();
            }
        }

        // Called when the UI is activated via activate()
        private void onActivate()
        {
            if (m_StaffCenterUI != null)
            {
                m_StaffCenterUI.activate(m_StaffPopInOutSpeed);
            }
            
            // As stated in the note above, we initialize all the needed markers right away and make them inactive
            // immediately to only be made active when needed.
            // Calculate leftmost and rightmost positions relative to the center object
            Vector3 centerPosition = m_CenterObject != null ? m_CenterObject.position : Vector3.zero;
            Vector3 rightDirection = m_CenterObject != null ? m_CenterObject.right : Vector3.right;
            
            m_LeftmostEnd = centerPosition - rightDirection * m_StaffWidth;
            m_RightmostEnd = centerPosition + rightDirection * m_StaffWidth;

            for (int i = 0; i < s_NumBeatMarkers; i++)
            {
                AddNewMarker(m_LeftmostEnd);
            }

            for (int i = 0; i < s_NumBeatMarkers; i++)
            {
                AddNewMarker(m_RightmostEnd);
            }

            // GIANT FIXME:
            //
            // There is a nasty non-determinstic bug occuring because of Unity start up lag.
            // This makes the UI not match up properly to the music and not match up properly to the
            // beat entering the center of the screen. This line is suppoosed to help fix it
            // but it's only a bit of a band-aid. It doesn't always work either...
            //m_ElapsedTime += (float)music.SongStartDSP + (music.BeatSec * 0.25f);
            //
            // Update: September 29, 2025 13:13
            // Might be fixed with this line and with a little experimenting with the offset in PulseToBeat.cs
            m_ElapsedTime -= (float)Time.realtimeSinceStartupAsDouble % music.BeatSec;

            m_IsActive = true;
            m_IsFading = false;
        }

        // Called when the UI is stopped via deactivate()
        private void onDeactivate()
        {
            m_IsActive = false;
            m_IsFading = true;
            
            if (m_StaffCenterUI != null)
            {
                m_StaffCenterUI.deactivate(music.BeatSec);
            }

            foreach (GameObject marker in m_BeatMarkersList)
            {
                var fader = marker.GetComponent<UIElementFader>();
                if (fader != null)
                {
                    fader.deactivate(m_StaffPopInOutSpeed);
                }
            }
        }

        // Helper function for initializating new beat markers.
        private void AddNewMarker(Vector3 position)
        {
            // All new beat markers start life deactivated and only activated upon request.
            var new_marker = Instantiate(m_BeatMarkerPrefab);
            var marker_script = new_marker.GetComponent<UIElementFader>();
            if (marker_script != null)
            {
                marker_script.deactivate(0.0f);
            }

            // Set parent to center object if available, otherwise use world space
            if (m_CenterObject != null)
            {
                new_marker.transform.SetParent(m_CenterObject, false);
                // Convert world position to local position relative to center object
                new_marker.transform.localPosition = m_CenterObject.InverseTransformPoint(position);
            }
            else
            {
                new_marker.transform.position = position;
            }

            m_BeatMarkersList.Add(new_marker);
        }

        private void FixedUpdate()
        {
            // Update on 2025-11-03 13:54: Requested to have the music staff key check on this script/prefab.
            if (Input.GetKeyDown(m_StaffAppearKey))
            {
                if (m_IsActive)
                {
                    Deactivate();
                }
                else
                {
                    Activate();
                }
            }

            // If the UI is either active or in the process of fading out in deactivation,
            // update all movement variables. Otherwise, destroy the entire list.
            if (m_IsActive)
            {
                handleBeatAppearance();
                updateMarkerMovement();
                handleBeatOnCenter();
                m_ElapsedTime += Time.deltaTime;
            }
            else if (m_IsFading)
            {
                updateMarkerMovement();
                handleBeatOnCenter();
                handleBeatDeactivate();
                m_ElapsedTime += Time.deltaTime;
            }
        }

        // Handles disappearing the beat markers when deactivate() was called.
        // Should only be called when m_IsFading is true and m_IsActive is false!
        private void handleBeatDeactivate()
        {
            if (m_IsActive) return;
            if (!m_IsFading) return;

            // Check only the first beat since chances are if one is finished, the
            // rest are finished as well. If it is finished, destroy the entire list
            // reset the entire object as new, ready for activation.
            var fader = getBeatMarkerOnLeft(1).GetComponent<UIElementFader>();
            if (fader != null && fader.isDoneDeactivate())
            {
                m_IsActive = false;
                m_IsFading = false;

                foreach (GameObject marker in m_BeatMarkersList)
                {
                    Destroy(marker);
                }

                m_BeatMarkersList = new List<GameObject>();
                m_ElapsedTime = 0.0f;
                m_CurrentHeadIndex = 0;
                m_CurrentTailIndex = 0;
            }

/*
            for (int i = 0; i < m_BeatMarkersList.Count; i++)
            {
                var marker_fader = m_BeatMarkersList[i].GetComponent<UIElementFader>();
                if (marker_fader.isDoneDeactivate())
                {
                    Destroy(m_BeatMarkersList[i]);
                    m_BeatMarkersList[i] = null;
                }
            }

            // Walk over the list again and count down any dead entries.
            m_BeatMarkersList.RemoveAll(marker => marker == null);

            // If no beat markers remain, we're no longer fading.
            if (m_BeatMarkersList.Count == 0)
            {
                m_IsFading = false;
                m_ElapsedTime = 0.0f;
                m_CurrentHeadIndex = 0;
                m_CurrentTailIndex = 0;
                m_BeatMarkersList = new List<GameObject>();
            }
            */
        }

        // Handles the general cycle for beats in terms of appearance, not related to their actual movement.
        private void handleBeatAppearance()
        {
            if (m_ElapsedTime > music.BeatSec)
            {
                // For the first couple of beats, we have to activate them manually.
                if (!m_IsFading && m_CurrentTailIndex < s_NumBeatMarkers)
                {
                    var marker_left_script = getBeatMarkerOnLeft(m_CurrentTailIndex).GetComponent<UIElementFader>();
                    var marker_right_script = getBeatMarkerOnRight(m_CurrentTailIndex).GetComponent<UIElementFader>();

                    if (marker_left_script != null)
                    {
                        marker_left_script.activate(music.BeatSec);
                    }
                    if (marker_right_script != null)
                    {
                        marker_right_script.activate(music.BeatSec);
                    }
                }

                m_CurrentTailIndex++;
                m_ElapsedTime = 0.0f;
            }
        }

        // Handles the situation when the beat markers move to the center of the screen in order to reset
        // them.
        private void handleBeatOnCenter()
        {
            // Once the beat markers reach the center of the screen:
            // - Deactivate them, effectively making them invisible to be moved and reset.
            // - Move them to the extremes again.
            // - Activate them to start the fade in process via its own script.
            GameObject marker_left = getBeatMarkerOnLeft(0);
            GameObject marker_right = getBeatMarkerOnRight(0);

            // Calculate center position for comparison
            Vector3 centerPosition = m_CenterObject != null ? m_CenterObject.position : Vector3.zero;
            Vector3 rightDirection = m_CenterObject != null ? m_CenterObject.right : Vector3.right;
            
            // Check if left marker has passed the center (using world position)
            Vector3 leftToCenter = centerPosition - marker_left.transform.position;
            float dotProduct = Vector3.Dot(leftToCenter, rightDirection);
            
            // We can assume the beats are moving at exactly the same pace so no need to check the other end.
            if (dotProduct < 0.0f) // Left marker has passed center
            {
                var marker_left_script = marker_left.GetComponent<UIElementFader>();
                var marker_right_script = marker_right.GetComponent<UIElementFader>();

                if (marker_left_script != null)
                {
                    marker_left_script.deactivate(0.0f);
                }
                if (marker_right_script != null)
                {
                    marker_right_script.deactivate(0.0f);
                }

                if (!m_IsFading)
                {
                    // Update end positions in case center object moved
                    m_LeftmostEnd = centerPosition - rightDirection * m_StaffWidth;
                    m_RightmostEnd = centerPosition + rightDirection * m_StaffWidth;
                    
                    // Set positions (convert to local if parented to center object)
                    if (m_CenterObject != null)
                    {
                        marker_left.transform.localPosition = m_CenterObject.InverseTransformPoint(m_LeftmostEnd);
                        marker_right.transform.localPosition = m_CenterObject.InverseTransformPoint(m_RightmostEnd);
                    }
                    else
                    {
                        marker_left.transform.position = m_LeftmostEnd;
                        marker_right.transform.position = m_RightmostEnd;
                    }

                    if (marker_left_script != null)
                    {
                        marker_left_script.activate(music.BeatSec * m_BeatAppearanceSpeed);
                    }
                    if (marker_right_script != null)
                    {
                        marker_right_script.activate(music.BeatSec * m_BeatAppearanceSpeed);
                    }
                }

                m_CurrentHeadIndex++;
            }
        }

        // Main update for the beat markers handling transforms.
        private void updateMarkerMovement()
        {
            // Implement the movement speed based on the beats per second and the staff dimensions.
            // If the cursor moves past the end, snap it back to the beginning.
            // Of course this carries the assumption that the cursor is on the correct position to
            // begin with, so not very ideal.
            int num_beats_per_measure = 4;
            float staff_dimension = m_StaffWidth;
            float delta_speed = (staff_dimension / (float)num_beats_per_measure) / music.BeatSec * Time.deltaTime;
            int num_beat_markers = Math.Min(s_NumBeatMarkers, m_CurrentTailIndex);

            // Get the right direction from center object, or use world right
            Vector3 rightDirection = m_CenterObject != null ? m_CenterObject.right : Vector3.right;

            for (int i = 0; i < num_beat_markers; i++)
            {
                getBeatMarkerOnLeft(i).transform.Translate(rightDirection * delta_speed, Space.World);
            }

            for (int i = 0; i < num_beat_markers; i++)
            {
                getBeatMarkerOnRight(i).transform.Translate(-rightDirection * delta_speed, Space.World);
            }
        }

        // Returns the beat marker to the left of the center by its index. The lower the number, the closer to
        // the center.
        private GameObject getBeatMarkerOnLeft(int index)
        {
            Assert.IsTrue(index >= 0 && index < s_NumBeatMarkers);
            return m_BeatMarkersList[(index + m_CurrentHeadIndex) % s_NumBeatMarkers];
        }

        // Returns the beat marker to the right of the center by its index. The lower the number, the closer to
        // the center.
        private GameObject getBeatMarkerOnRight(int index)
        {
            Assert.IsTrue(index >= 0 && index < s_NumBeatMarkers);
            return m_BeatMarkersList[m_BeatMarkersList.Count - 1 - (index + m_CurrentHeadIndex) % s_NumBeatMarkers];
        }
    }
}