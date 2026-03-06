using Carrot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playlist_Radio : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;
    private string s_data_temp = "";
    private List<IDictionary> list_data_play;
    private bool is_auto_retrying = false;
    private const string cache_key_radio = "s_data_offline_radio";

    public void On_Load()
    {
        this.s_data_temp = PlayerPrefs.GetString(cache_key_radio, "");
        this.list_data_play = new List<IDictionary>();
    }

    public void Show()
    {
        app.Create_loading();
        if (this.Is_empty_radio_cache(this.s_data_temp))
            this.Get_data_from_server();
        else
            this.Load_list_by_data(this.s_data_temp);
    }

    private bool Is_empty_radio_cache(string s_data)
    {
        if (string.IsNullOrEmpty(s_data)) return true;
        IList list_radio = Carrot.Json.Deserialize(s_data) as IList;
        if (list_radio == null) return true;
        return list_radio.Count == 0;
    }

    private void Get_data_from_server()
    {
        Dictionary<string, object> filters = new()
        {
            { "page", 1 },
            { "limit", 300 },
            { "order_key", "updated_at" },
            { "order_type", "DESC" }
        };

        app.carrot.hub.ReadTable("radio", filters, act =>
        {
            this.is_auto_retrying = false;
            this.s_data_temp = act;
            PlayerPrefs.SetString(cache_key_radio, act);
            this.Load_list_by_data(act);
        }, err =>
        {
            Debug.LogError(err);
            this.Load_list_by_data("[]");
            if (this.is_auto_retrying) return;
            this.is_auto_retrying = true;
            app.carrot.delay_function(1.5f, () =>
            {
                this.s_data_temp = "";
                this.Show();
            });
        });
    }

    private void Load_list_by_data(string s_data)
    {
        IList list_radio = Carrot.Json.Deserialize(s_data) as IList;
        if (list_radio == null) list_radio = new List<object>();

        app.clear_all_contain();
        Carrot_Box_Item item_title = app.Create_item("title");
        item_title.set_icon(app.sp_icon_radio);
        item_title.set_title(app.carrot.L("m_radio", "Radio"));
        item_title.set_tip(app.carrot.L("m_radio_tip", "List of online radio stations listed by their respective countries"));

        this.list_data_play = new List<IDictionary>();
        if (list_radio.Count > 0)
        {
            int row_index = 0;
            for (int i = 0; i < list_radio.Count; i++)
            {
                IDictionary data_radio = list_radio[i] as IDictionary;
                if (data_radio == null) continue;

                data_radio["index_play"] = this.list_data_play.Count;
                data_radio["type"] = "radio_online";
                Carrot_Box_Item box_item = app.Create_item("item_radio_" + i);
                box_item.set_icon(app.sp_icon_radio_broadcast);
                box_item.set_title(data_radio["name"].ToString());
                box_item.set_tip(data_radio["url"].ToString());
                if (row_index % 2 == 0)
                    box_item.GetComponent<Image>().color = app.color_row_1;
                else
                    box_item.GetComponent<Image>().color = app.color_row_2;
                row_index++;
                box_item.set_act(() => Play(data_radio));

                string s_id_avatar = "pic_radio_" + data_radio["id"].ToString();
                Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                if (sp_pic_avatar != null)
                {
                    box_item.set_icon_white(sp_pic_avatar);
                }
                else
                {
                    string icon_url = "";
                    if (data_radio["icon"] != null) icon_url = data_radio["icon"].ToString();
                    if (!string.IsNullOrEmpty(icon_url))
                        app.carrot.get_img_and_save_playerPrefs(app.carrot.hub.GetUrlFile(icon_url), box_item.img_icon, s_id_avatar);
                }

                Carrot_Box_Btn_Item btn_add_playlist = box_item.create_item();
                btn_add_playlist.set_icon(app.sp_icon_storage_save);
                btn_add_playlist.set_icon_color(Color.white);
                btn_add_playlist.set_color(app.carrot.color_highlight);
                btn_add_playlist.set_act(() =>
                {
                    this.Storage_item(data_radio, btn_add_playlist.gameObject);
                });
                this.list_data_play.Add(data_radio);
                app.Create_btn_add_play(box_item,data_radio);
            }
        }
        else
        {
            app.Create_list_none();
        }
    }

    private void Storage_item(IDictionary data, GameObject obj_btn_storage)
    {
        app.carrot.play_sound_click();
        Destroy(obj_btn_storage);
        data["type"] = "radio_offline";
        app.playlist_offline.Add(data);
        app.carrot.Show_msg(app.carrot.L("playlist", "Playlist"), app.carrot.L("save_song_success", "Successfully stored, you can listen to the song again in the playlist"));
    }

    private void Play(IDictionary data)
    {
        this.app.player_music.Play_by_data(data);
        if (this.list_data_play.Count > 0) this.app.player_music.Set_list_music(this.list_data_play);
    }

}
