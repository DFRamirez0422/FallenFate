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

namespace NPA_RhythmBonusPrefabs
{
    /*
        DESCRIPTION: MusicBarUI
        AUTHOR: Jose Escobedo

        Code modified from the original PulseToBeat.cs file created by:
            MIKE HERNANDEZ

        Probably will drastically change once we get into talks with the UI designers as to
        what they want to see out of the music bar as well as what we would like to incorporate
        from Ratatan. Prototyping!

        Anyways...

        This script performs movement and updates of the music bar UI elements. If we were to base
        the UI on Ratatan, we would need a reference to all beat markers. To keep things simple for now,
        we just need a reference to the cursor. To start out this such simple task, the cursor will
        just teleport once it reaches the end. Rather gnarly, perhaps, but we will fix it in accordance
        to the game vision.

        TODO: talk with the UI designers and play the Ratatan demo to find out how to implement this behavior.
        TODO: fix up the code proper instead of a shoddy copy of PulseToBeat.cs

        UPDATE (as of September 27th, 2025):
        The new game to use as a base reference is High-Fi Rush. After playing through the game for about 90 or so
        minutes and collecting gameplay videos through my own inputs, I can safely say that the game
        was well worth what I paid for as it sat in my steam library. I couldn't put it down, do you see? The
        engaging gameplay paired with my xbox controller made it quite fun in a way that couldn't be
        replicted by online videos. I defeated the big robot hands guy and decided to stop for now so I
        can actually sleep hahaha. Anyways, we'll try to replicate that UI now in accordanace to the
        Rhythm System Document document.
    */
    public class MusicBarUI : MonoBehaviour
    {
        [Tooltip("The instance of the rhythm music player.")]
        [SerializeField] private RhythmMusicPlayer music;  // Music clock

        [Tooltip("Music staff boundary box.")]
        [SerializeField] private RectTransform m_StaffObject;

        [Tooltip("Prefab for the beat marker object.")]
        [SerializeField] private GameObject m_BeatMarkerPrefab;

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

        // Current index within the queue to the head.
        private int m_CurrentHeadIndex = 0;

        // Current index within the queue to the tail.
        private int m_CurrentTailIndex = 0;

        private void Start()
        {
            // As stated in the note above, we initialize all the needed markers right away and make them inactive
            // immediately to only be made active when needed.
            Vector3[] staff_corners = new Vector3[4];
            m_StaffObject.GetLocalCorners(staff_corners);

            m_LeftmostEnd = Vector3.Lerp(staff_corners[0], staff_corners[1], 0.5f);//Vector3.zero;
            m_RightmostEnd = Vector3.Lerp(staff_corners[2], staff_corners[3], 0.5f);//new Vector3(m_StaffObject.rect.width, 0, 0);
            var rotation = Quaternion.identity;

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
            m_ElapsedTime += (float)music.SongStartDSP + (music.BeatSec * 0.25f);
        }

        // Helper function for initializating new beat markers.
        private void AddNewMarker(Vector3 position)
        {
            // All new beat markers start life deactivated and only activated upon request.
            var new_marker = Instantiate(m_BeatMarkerPrefab);
            var marker_script = new_marker.GetComponent<UIElementFader>();
            marker_script.deactivate(0.0f);

            // WHY IS UNITY SO STUPID WITH THIS? EVERY TIME WITH UI PREFABS.
            // If I'm giving you a direct position, why place it at some random point on the screen?
            // Then the X component is right while the Y is wrong. WHY????
            //
            // ...above, the reaction of spending about an hour on this issue. Writing down on paper the
            // entire heirarchy isn't enough it seems.
            //new_marker.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.5f, 0.5f);
            new_marker.transform.SetParent(m_StaffObject, false);
            new_marker.transform.localPosition = position;

            m_BeatMarkersList.Add(new_marker);
        }

        private void Update()
        {
            handleBeatAppearance();
            updateMarkerMovement();
            handleBeatOnCenter();
            m_ElapsedTime += Time.deltaTime;
        }

        // Handles the general cycle for beats in terms of appearance, not related to their actual movement.
        private void handleBeatAppearance()
        {
            if (m_ElapsedTime > music.BeatSec)
            {
                // For the first couple of beats, we have to activate them manually.
                if (m_CurrentHeadIndex < s_NumBeatMarkers)
                {
                    var marker_left_script = getBeatMarkerOnLeft(m_CurrentHeadIndex).GetComponent<UIElementFader>();
                    var marker_right_script = getBeatMarkerOnRight(m_CurrentHeadIndex).GetComponent<UIElementFader>();

                    marker_left_script.activate(music.BeatSec);
                    marker_right_script.activate(music.BeatSec);
                }

                m_CurrentHeadIndex++;
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

            // We can assume the beats are moving at exactly the same pace so no need to check the other end.
            if (marker_left.transform.localPosition.x > 0.0f)
            {
                var marker_left_script = marker_left.GetComponent<UIElementFader>();
                var marker_right_script = marker_right.GetComponent<UIElementFader>();

                marker_left_script.deactivate(0.0f);
                marker_right_script.deactivate(0.0f);

                marker_left.transform.localPosition = m_LeftmostEnd;
                marker_right.transform.localPosition = m_RightmostEnd;

                marker_left_script.activate(music.BeatSec * 2.0f);
                marker_right_script.activate(music.BeatSec * 2.0f);

                m_CurrentTailIndex++;
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
            float staff_dimension = m_StaffObject.rect.width / 2.0f;
            float delta_speed = (staff_dimension / (float)num_beats_per_measure) / music.BeatSec * Time.deltaTime;
            int num_beat_markers = Math.Min(s_NumBeatMarkers, m_CurrentHeadIndex);

            for (int i = 0; i < num_beat_markers; i++)
            {
                getBeatMarkerOnLeft(i).transform.Translate(delta_speed, 0.0f, 0.0f);
            }

            for (int i = 0; i < num_beat_markers; i++)
            {
                getBeatMarkerOnRight(i).transform.Translate(-delta_speed, 0.0f, 0.0f);
            }
        }

        // Returns the beat marker to the left of the center by its index. The lower the number, the closer to
        // the center.
        private GameObject getBeatMarkerOnLeft(int index)
        {
            return m_BeatMarkersList[(index + m_CurrentTailIndex) % s_NumBeatMarkers];
        }

        // Returns the beat marker to the right of the center by its index. The lower the number, the closer to
        // the center.
        private GameObject getBeatMarkerOnRight(int index)
        {
            return m_BeatMarkersList[m_BeatMarkersList.Count - 1 - (index + m_CurrentTailIndex) % s_NumBeatMarkers];
        }
    }
}