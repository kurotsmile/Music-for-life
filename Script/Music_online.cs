using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Music_online : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    public GameObject prefab_item_music;

    private string s_data_temp = "";

    [Header("Obj Online")]
    public Sprite icon_artist;
    public Sprite icon_genre;
    public Sprite icon_year;
    public Sprite icon_album;

    public void On_load()
    {
        if(app.carrot.is_offline()) this.s_data_temp = PlayerPrefs.GetString("s_data_music_");
    }

    public void show_list_artist()
    {
        this.Show_list_info_by_type("artist");
    }

    public void show_list_genre()
    {
        this.Show_list_info_by_type("genre");
    }

    public void show_list_year()
    {
        this.Show_list_info_by_type("year");
    }

    private void Show_list_info_by_type(string s_type)
    {

    }

    public void Get_song_by_id(string s_id)
    {

    }

    public void Show(string s_lang)
    {
        if (this.s_data_temp == "")
            this.Get_data_from_server(s_lang);
        else
            this.Load_list_by_data(this.s_data_temp);
    }

    private void Get_data_from_server(string s_lang)
    {
        app.Create_loading();
        StructuredQuery q = new("song");
        q.Add_select("name");
        q.Add_select("genre");
        q.Add_select("artist");
        q.Add_select("year");
        q.Add_select("album");
        q.Add_select("lang");
        q.Add_select("mp3");
        q.Add_select("avatar");
        q.Add_select("link_ytb");
        q.Add_where("lang", Query_OP.EQUAL, s_lang);
        q.Add_order("publishedAt");
        q.Set_limit(30);
        this.app.carrot.server.Get_doc(q.ToJson(), (s_data) =>
        {
            this.s_data_temp = s_data;
            PlayerPrefs.SetString("s_data_music_", s_data);
            this.Load_list_by_data(s_data);
        });
    }

    private void Load_list_by_data(string s_data)
    {
        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            app.clear_all_contain();
            this.s_data_temp = s_data;

            Carrot_Box_Item item_title = app.Create_item("item_title");
            item_title.set_icon(app.sp_icon_music);
            item_title.set_title(app.carrot.L("m_music", "Music"));
            item_title.set_tip(app.carrot.L("m_music_tip", "Online playlists are listed by respective countries"));

            Carrot_Box_Item item_sel_lang = app.Create_item("item_sel_lang");
            item_sel_lang.set_icon(app.carrot.lang.icon);
            item_sel_lang.set_title(app.carrot.L("m_music_country", "Listen to music by country"));
            item_sel_lang.set_tip(app.carrot.L("m_music_country_tip", "Show music list by country")+" ("+PlayerPrefs.GetString("lang_music", "en")+")");

            Carrot_Box_Btn_Item btn_list_lang = item_sel_lang.create_item();
            btn_list_lang.set_icon(this.app.carrot.icon_carrot_all_category);
            btn_list_lang.set_icon_color(Color.white);
            btn_list_lang.set_color(app.carrot.color_highlight);
            btn_list_lang.set_act(() => app.carrot.lang.Show_list_lang(this.Act_show_list_by_lang,false));
            item_sel_lang.set_act(()=> app.carrot.lang.Show_list_lang(this.Act_show_list_by_lang, false));

            IList list_music = (IList)Json.Deserialize("[]");
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_m = fc.fire_document[i].Get_IDictionary();
                data_m["type"] = "music_online";
                data_m["index"] = i;
                Carrot_Box_Item box_item = app.Create_item("item_m_" + i);
                box_item.set_icon(app.carrot.game.icon_play_music_game);
                box_item.set_title(data_m["name"].ToString());
                if (data_m["artist"] != null) box_item.set_tip(data_m["artist"].ToString());
                box_item.set_act(() => { app.player_music.Play_by_data(data_m); });

                if (i % 2 == 0)
                    box_item.GetComponent<Image>().color = app.color_row_1;
                else
                    box_item.GetComponent<Image>().color = app.color_row_2;

                string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                    box_item.set_icon_white(sp_pic_avatar);
                else
                    app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);

                list_music.Add(data_m);
            }
            this.app.player_music.Set_list_music(list_music);
        }
    }

    private void Act_show_list_by_lang(string key)
    {
        PlayerPrefs.SetString("lang_music", key);
        this.s_data_temp = "";
        this.Show(key);
    }
}
