using UnityEngine;
using UnityEngine.Audio;
public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenu");
        }
    }
    public void SetVolume(float volume) {
        Debug.Log(volume);
    }
    public void Fullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }
    public void ChangeVolume(float AudioVolume) {
        audioMixer.SetFloat("AudioVolume", AudioVolume);
    }
}
