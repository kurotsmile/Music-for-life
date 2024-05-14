using UnityEngine;

public class Playlist_Search : MonoBehaviour
{
    public App app;

    public void Show()
    {
        this.app.carrot.play_sound_click();
        this.app.carrot.show_search(Act_done, "Enter the name of the song, radio channel or audio you want to search for");
    }

    private void Act_done(string s_key)
    {
        this.app.carrot.play_sound_click();
        Debug.Log("Key search:" + s_key);
    }
}
