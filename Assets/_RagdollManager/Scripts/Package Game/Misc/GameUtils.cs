// © 2015 Mario Lelas
using UnityEngine;

namespace MLSpace
{
    /// <summary>
    /// Game utilities class.
    /// </summary>
    public static class GameUtils 
    {


        /// <summary>
        /// Chosses random clip from passed array and play at position.
        /// </summary>
        /// <param name="clips">audioclip array</param>
        /// <param name="position">world position</param>
        public static void PlayRandomClipAtPosition(AudioClip[] clips, Vector3 position)
        {
#if DEBUG_INFO
            if (clips == null)
            {
                Debug.LogError("object can be null");
                return;
            }
#endif
            if (clips.Length > 0)
            {
                int len = clips.Length;
                int rnd = Random.Range(0, len);
                AudioClip clip = clips[rnd];
#if DEBUG_INFO
                if(!clip)
                {
                    Debug.LogError("object cannot be null! " );
                    return;
                }
#endif
                AudioSource.PlayClipAtPoint(clip, position);
            }
        }

        /// <summary>
        /// Chosses random clip from passed array and play.
        /// </summary>
        /// <param name="audioSource">audio source</param>
        /// <param name="clips">audioclip array</param>
        public static void PlayRandomClip(AudioSource audioSource, AudioClip[] clips)
        {
#if DEBUG_INFO
            if (clips == null)
            {
                Debug.LogError("object can be null");
                return;
            }
            if(!audioSource )
            {
                Debug.LogError("Object cannot be null ( AudioSource )");
                return;
            }
#endif
            
            if (clips.Length > 0)
            {
                int len = clips.Length;
                int rnd = Random.Range(0, len);
                AudioClip clip = clips[rnd];
#if DEBUG_INFO
                if (!clip)
                {
                    Debug.LogError("object cannot be null ( AudioClip ) on: " + audioSource.gameObject .name);
                    return;
                }
#endif
                audioSource.PlayOneShot(clip);
            }
        }
    } 
}
