using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Music_online : MonoBehaviour
{
    public Sprite icon_artist;
    public Sprite icon_genre;
    public Sprite icon_year;
    public Sprite icon_album;

    private string s_type_temp;
    public void show_list_artist()
    {
        this.GetComponent<App>().title_name.text = PlayerPrefs.GetString("artist", "Artist");
        this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("artist_tip", "List of songs listed by artist");
        this.show_list_info_by_type("artist");
    }

    public void show_list_genre()
    {
        this.GetComponent<App>().title_name.text = PlayerPrefs.GetString("genre", "Genre");
        this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("genre_tip", "List of songs listed by genre");
        this.show_list_info_by_type("genre");
    }

    public void show_list_year()
    {
        this.GetComponent<App>().title_name.text = PlayerPrefs.GetString("year", "Year");
        this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("year_tip", "List of songs listed by year");
        this.show_list_info_by_type("year");
    }

    private void show_list_info_by_type(string s_type)
    {
        /*
        WWWForm frm = this.GetComponent<App>().carrot.frm_act("list_info");
        frm.AddField("type", s_type);
        this.s_type_temp = s_type;
        this.GetComponent<App>().carrot.send(frm, act_list_info);
        */
    }

    private void act_list_info(string s_data)
    {
        IList all_info = (IList)Carrot.Json.Deserialize(s_data);
        this.GetComponent<App>().clear_all_contain();
        for (int i = 0; i < all_info.Count; i++)
        {
            GameObject obj_info = Instantiate(this.GetComponent<App>().prefab_music_item_list);
            obj_info.name = "item_music";
            obj_info.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
            obj_info.transform.localPosition = new Vector3(obj_info.transform.localPosition.x, obj_info.transform.localPosition.y, 0f);
            obj_info.transform.localScale = new Vector3(1f, 1f, 1f);
            obj_info.transform.localRotation = Quaternion.Euler(Vector3.zero);
            obj_info.GetComponent<Panel_item_music>().id_m = this.s_type_temp;
            obj_info.GetComponent<Panel_item_music>().type = 4;
            if (s_type_temp == "artist") { obj_info.GetComponent<Panel_item_music>().icon.sprite = this.icon_artist; }
            if (s_type_temp == "genre") { obj_info.GetComponent<Panel_item_music>().icon.sprite = this.icon_genre; }
            if (s_type_temp == "year") { obj_info.GetComponent<Panel_item_music>().icon.sprite = this.icon_year; }
            obj_info.GetComponent<Panel_item_music>().txt_name.text = all_info[i].ToString();
            obj_info.GetComponent<Panel_item_music>().btn_statu_play.SetActive(false);
            obj_info.GetComponent<Panel_item_music>().btn_delete.SetActive(false);
            obj_info.GetComponent<Panel_item_music>().btn_add_playlist.SetActive(false);
            if (i % 2 == 0)
                obj_info.GetComponent<Image>().color = this.GetComponent<App>().color_row_1;
            else
                obj_info.GetComponent<Image>().color = this.GetComponent<App>().color_row_2;
        }
    }

    public void show_list_item_in_info(string id_item,string s_type,string s_lang)
    {
        this.GetComponent<App>().title_name.text = id_item;
        if (s_type == "artist") { 
            this.GetComponent<App>().title_icon.sprite = this.icon_artist;
            this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("artist_tip", "List of songs listed by artist");
        }
        if (s_type == "genre") { 
            this.GetComponent<App>().title_icon.sprite = this.icon_genre;
            this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("genre_tip", "List of songs listed by genre");
        }
        if (s_type == "year") { 
            this.GetComponent<App>().title_icon.sprite = this.icon_year;
            this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("year_tip", "List of songs listed by year");
        }
        if (s_type == "album") { 
            this.GetComponent<App>().title_icon.sprite = this.icon_album;
            this.GetComponent<App>().title_tip.text = PlayerPrefs.GetString("album_tip", "List of songs listed by album");
        }

        /*
        WWWForm frm = this.GetComponent<App>().carrot.frm_act("get_list_item_info");
        frm.AddField("type", s_type);
        frm.AddField("s_id", id_item);
        frm.AddField("s_lang", s_lang);
        this.GetComponent<App>().carrot.send(frm, this.get_item_list_info);
        */
    }

    private void get_item_list_info(string s_data)
    {
         IList all_music = (IList)Carrot.Json.Deserialize(s_data);
        this.GetComponent<App>().clear_all_contain();
         this.GetComponent<App>().list_music=new List<Panel_item_music>();
        for (int i = 0; i < all_music.Count; i++)
        {
            IDictionary item_music = (IDictionary)all_music[i];
            GameObject obj_music = create_song_to_playlist(item_music);
            if (i % 2 == 0)
                obj_music.GetComponent<Image>().color = this.GetComponent<App>().color_row_1;
            else
                obj_music.GetComponent<Image>().color = this.GetComponent<App>().color_row_2;

            this.GetComponent<App>().list_music.Add(obj_music.GetComponent<Panel_item_music>());
        }    
    }

    private GameObject create_song_to_playlist(IDictionary item_music)
    {
        GameObject obj_music = Instantiate(this.GetComponent<App>().prefab_music_item_list);
        obj_music.name = "item_music";
        obj_music.transform.SetParent(this.GetComponent<App>().canvas_render.transform);
        obj_music.transform.localPosition = new Vector3(obj_music.transform.localPosition.x, obj_music.transform.localPosition.y, 0f);
        obj_music.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_music.transform.localRotation = Quaternion.Euler(Vector3.zero);
        obj_music.GetComponent<Panel_item_music>().url = item_music["url"].ToString();
        obj_music.GetComponent<Panel_item_music>().id_m = item_music["id"].ToString();
        obj_music.GetComponent<Panel_item_music>().txt_name.text = item_music["name"].ToString();
        obj_music.GetComponent<Panel_item_music>().s_color = item_music["color"].ToString();
        obj_music.GetComponent<Panel_item_music>().lyrics = item_music["lyrics"].ToString();
        obj_music.GetComponent<Panel_item_music>().lang = item_music["lang"].ToString();
        obj_music.GetComponent<Panel_item_music>().genre = item_music["genre"].ToString();
        obj_music.GetComponent<Panel_item_music>().album = item_music["album"].ToString();
        obj_music.GetComponent<Panel_item_music>().year = item_music["year"].ToString();
        obj_music.GetComponent<Panel_item_music>().artist = item_music["artist"].ToString();
        obj_music.GetComponent<Panel_item_music>().type = 0;
        obj_music.GetComponent<Panel_item_music>().btn_delete.SetActive(false);
        obj_music.GetComponent<Panel_item_music>().btn_statu_play.SetActive(false);
        if (item_music["link_ytb"] != null) obj_music.GetComponent<Panel_item_music>().link_ytb = item_music["link_ytb"].ToString();
        obj_music.GetComponent<Panel_item_music>().link_store = this.GetComponent<App>().carrot.mainhost + "/music/" + item_music["id"].ToString() + "/" + item_music["lang"].ToString();
        if (item_music["img_video"] != null) this.GetComponent<App>().carrot.get_img(item_music["img_video"].ToString(), obj_music.GetComponent<Panel_item_music>().icon);
        return obj_music;
    }

    public void get_song_buy_id_and_lang(string s_id,string s_lang)
    {
        /*
        WWWForm frm = this.GetComponent<App>().carrot.frm_act("get_song_by_id");
        frm.AddField("s_id", s_id);
        frm.AddField("s_lang", s_lang);
        this.GetComponent<App>().carrot.send(frm, this.act_play_song);
        */
    }

    public void act_play_song(string s_data)
    {
        if (s_data != "")
        {
            IDictionary data_song = (IDictionary)Carrot.Json.Deserialize(s_data);
            GameObject new_song = this.create_song_to_playlist(data_song);
            this.GetComponent<App>().play_music(new_song.GetComponent<Panel_item_music>());
        }
    }
}
