using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_offiline : MonoBehaviour
{
    public GameObject prefab_music;

    int leng=0;

    void Start()
    {
        this.leng = PlayerPrefs.GetInt("mfl_leng");
    }

    public void show_list_music_data()
    {
        this.GetComponent<App>().StopAllCoroutines();
        this.GetComponent<App>().clear_all_contain();
        if (this.leng > 0)
        {
            this.GetComponent<App>().list_music = new List<Panel_item_music>();
            int index_m = 0;
            for(int i = 0; i < this.leng; i++)
            {
                if (PlayerPrefs.GetString("mfl_name_"+i) != "")
                {
                    GameObject item_m = Instantiate(this.prefab_music);
                    item_m.name = "item_music";
                    item_m.GetComponent<Panel_item_music>().txt_name.text = PlayerPrefs.GetString("mfl_name_" + i);
                    item_m.GetComponent<Panel_item_music>().s_color = PlayerPrefs.GetString("mfl_color_" + i);
                    item_m.GetComponent<Panel_item_music>().index = index_m;
                    item_m.GetComponent<Panel_item_music>().index_delete = i;
                    item_m.GetComponent<Panel_item_music>().lyrics= PlayerPrefs.GetString("mfl_lyrics_" + i);
                    item_m.GetComponent<Panel_item_music>().link_store= PlayerPrefs.GetString("mfl_link_" +i);
                    item_m.GetComponent<Panel_item_music>().link_ytb = PlayerPrefs.GetString("mfl_ytb_" + i);
                    if (Application.isEditor)
                    {
                        item_m.GetComponent<Panel_item_music>().url = "file://"+Application.dataPath + "/mfl_" + i + ".data";
                    }
                    else
                    {
                        item_m.GetComponent<Panel_item_music>().url = "file://"+Application.persistentDataPath + "/mfl_" + i + ".data";
                    }
                    item_m.GetComponent<Panel_item_music>().type = 3;
                    this.GetComponent<App>().carrot.get_tool().load_file_img("mfl_" + i + ".png", item_m.GetComponent<Panel_item_music>().icon);
                    item_m.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
                    item_m.transform.localPosition = new Vector3(item_m.transform.localPosition.x, item_m.transform.localPosition.y, 0f);
                    item_m.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    item_m.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_m.GetComponent<Panel_item_music>().btn_statu_play.SetActive(false);
                    item_m.GetComponent<Panel_item_music>().btn_add_playlist.SetActive(false);
                    index_m++;
                    this.GetComponent<App>().list_music.Add(item_m.GetComponent<Panel_item_music>());
                }
            }
        }
    }


    public void add_music(Panel_item_music m,byte[] data)
    {
        PlayerPrefs.SetString("mfl_name_" + this.leng, m.txt_name.text);
        PlayerPrefs.SetString("mfl_color_" + this.leng, m.s_color);
        PlayerPrefs.SetString("mfl_url_" + this.leng, m.url);
        PlayerPrefs.SetString("mfl_lyrics_" + this.leng, m.lyrics);
        PlayerPrefs.SetString("mfl_link_" + this.leng, m.link_store);
        PlayerPrefs.SetString("mfl_ytb_" + this.leng, m.link_ytb);

        this.GetComponent<App>().carrot.get_tool().save_file("mfl_" + this.leng + ".data", data);
        this.GetComponent<App>().carrot.get_tool().save_file("mfl_" + this.leng + ".png", m.icon.sprite.texture.EncodeToPNG());
        this.leng++;
        PlayerPrefs.SetInt("mfl_leng", this.leng);
        GameObject.Find("App").GetComponent<App>().carrot.Show_msg(PlayerPrefs.GetString("song_save_success", "Add a song to your favorite playlist !, you can listen to the song without connecting to the internet!"));
    }


    public void delete(int index)
    {
        PlayerPrefs.DeleteKey("mfl_name_" + index);
        PlayerPrefs.DeleteKey("mfl_color_" + index);
        PlayerPrefs.DeleteKey("mfl_url_" + index);
        PlayerPrefs.DeleteKey("mfl_lyrics_" + index);
        PlayerPrefs.DeleteKey("mfl_link_" + index);
        PlayerPrefs.DeleteKey("mfl_ytb_" + index);

        this.GetComponent<App>().carrot.get_tool().delete_file("mfl_" + index + ".data");
        this.GetComponent<App>().carrot.get_tool().delete_file("mfl_" + index + ".png");
        this.show_list_music_data();
    }

}
