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

    public void On_Load()
    {
        if (app.carrot.is_offline()) this.s_data_temp = PlayerPrefs.GetString("s_data_offline_radio");
        this.list_data_play=new List<IDictionary>();
    }

    public void Show()
    {
        app.Create_loading();
        if (s_data_temp == "")
        {
            StructuredQuery q = new("radio");
            app.carrot.server.Get_doc(q.ToJson(), this.Act_get_done);
        }
        else
        {
            this.Load_list_by_data(s_data_temp);
        }

    }

    void Act_get_done(string s_data)
    {
        this.s_data_temp = s_data;
        PlayerPrefs.SetString("s_data_offline_radio", s_data);
        this.Load_list_by_data(s_data);
    }

    private void Load_list_by_data(string s_data)
    {
        Fire_Collection fc = new(s_data);

        app.clear_all_contain();
        Carrot_Box_Item item_title = app.Create_item("title");
        item_title.set_icon(app.sp_icon_radio);
        item_title.set_title(app.carrot.L("m_radio", "Radio"));
        item_title.set_tip(app.carrot.L("m_radio_tip", "List of online radio stations listed by their respective countries"));

        if (!fc.is_null)
        {
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_radio = fc.fire_document[i].Get_IDictionary();
                data_radio["index_play"] = i;
                data_radio["type"] = "radio_online";
                Carrot_Box_Item box_item = app.Create_item("item_radio_" + i);
                box_item.set_icon(app.sp_icon_radio_broadcast);
                box_item.set_title(data_radio["name"].ToString());
                box_item.set_tip(data_radio["url"].ToString());
                if (i % 2 == 0)
                    box_item.GetComponent<Image>().color = app.color_row_1;
                else
                    box_item.GetComponent<Image>().color = app.color_row_2;
                box_item.set_act(() => Play(data_radio));

                Carrot_Box_Btn_Item btn_add_playlist = box_item.create_item();
                btn_add_playlist.set_icon(app.sp_icon_storage_save);
                btn_add_playlist.set_icon_color(Color.white);
                btn_add_playlist.set_color(app.carrot.color_highlight);
                btn_add_playlist.set_act(() =>
                {
                    this.Storage_item(data_radio, btn_add_playlist.gameObject);
                });
                this.list_data_play.Add(data_radio);
            }
        }
        else
        {
            app.Create_list_none();
        }
    }

    private void Storage_item(IDictionary data,GameObject obj_btn_storage)
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
        if(this.list_data_play.Count>0) this.app.player_music.Set_list_music(this.list_data_play);
    }

}
