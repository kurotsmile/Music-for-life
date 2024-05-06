using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_item_music : MonoBehaviour
{
    public string id_m;
    public Text txt_name;
    public Image icon;
    public string url;
    public string s_color;
    public int index;
    public int index_delete;
    public int type;
    public GameObject btn_delete;
    public GameObject btn_statu_play;
    public GameObject btn_add_playlist;
    public string lyrics;
    public string link_ytb;
    public string link_store;
    public string lang;
    public string artist;
    public string album;
    public string genre;
    public string year;

    public void click()
    {
        if (this.type == 4)
            GameObject.Find("App").GetComponent<Music_online>().show_list_item_in_info(this.txt_name.text, this.id_m,this.lang);
        else
            GameObject.Find("App").GetComponent<App>().play_music(this);
    }

    public void delete()
    {
        if (this.type == 3)
        {
            GameObject.Find("App").GetComponent<Playlist>().delete_song_in_playlist(id_m,lang);
        }
        else
        {
            GameObject.Find("App").GetComponent<Music_offiline>().delete(this.index_delete);
        }
    }

    public void add_playlist()
    {
        GameObject.Find("App").GetComponent<Playlist>().show_add_song_to_playlist(this.id_m, this.lang);
    }
}
