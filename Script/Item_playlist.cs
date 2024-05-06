using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_playlist : MonoBehaviour
{
    public Text txt_name;
    public string s_data;
    public string id_playlist;
    public Text txt_length_song;
    public int type = 0; //0-show 1-add
    public GameObject button_edit;
    public GameObject button_delete;
    public GameObject button_share;
    public void click()
    {
        if (this.type == 0)
        {
            GameObject.Find("App").GetComponent<Playlist>().show_playlist_by_id(this.id_playlist, this.s_data, this.txt_name.text);
        }
        else
        {
            GameObject.Find("App").GetComponent<Playlist>().add_song_playlist(this.id_playlist, this.s_data);
        }
    }

    public void edit()
    {
        GameObject.Find("App").GetComponent<Playlist>().edit_playlist(this.id_playlist,this.txt_name.text);
    }

    public void show_new_playlist()
    {
        GameObject.Find("App").GetComponent<Playlist>().show_new_playlist();
    }

    public void delete()
    {
        GameObject.Find("App").GetComponent<Playlist>().delete_playlist(this.id_playlist);
    }

    public void show_share()
    {
        string s_link_playlist = GameObject.Find("App").GetComponent<App>().carrot.mainhost+"/playlist/"+this.id_playlist+"/"+ GameObject.Find("App").GetComponent<App>().carrot.user.get_lang_user_login();
        GameObject.Find("App").GetComponent<App>().carrot.show_share(s_link_playlist, PlayerPrefs.GetString("playlist_share_tip", "Share this playlist with your friends so everyone can hear it!"));
    }
}
