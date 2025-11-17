using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    private bool leftFoot = true;

    public void PlayFootstep()
    {
        if (footstepClips.Length == 0 || audioSource == null)
            return;

        int randomIndex = Random.Range(0, footstepClips.Length);

        // Alternar pie
        audioSource.panStereo = leftFoot ? -0.9f : 0.9f;
        leftFoot = !leftFoot;

        // Variación natural del paso
        audioSource.pitch = Random.Range(0.5f, 1.1f);

        audioSource.PlayOneShot(footstepClips[randomIndex]);
    }
}
