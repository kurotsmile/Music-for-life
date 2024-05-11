using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Music_offiline : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Offline")]
    public Sprite ps_icon_offline;
    int leng=0;
    private Carrot_Window_Input box_inp = null;

    public void On_Load()
    {
        this.leng = PlayerPrefs.GetInt("mo_length",0);
    }

    public void Add(IDictionary data, byte[] data_mp3)
    {
        app.carrot.get_tool().save_file(this.leng+".mp3", data_mp3);
        this.Add(data);
    }

    public void Add(IDictionary data)
    {
        PlayerPrefs.SetString("mo_" + this.leng, Json.Serialize(data));
        this.leng++;
        PlayerPrefs.SetInt("mo_length", this.leng);
    }

    public void Show()
    {
        this.GetComponent<App>().StopAllCoroutines();
        this.GetComponent<App>().clear_all_contain();

        Carrot_Box_Item item_title = app.Create_item("item_title_offline");
        item_title.set_icon(app.sp_icon_storage);
        item_title.set_title(app.carrot.L("playlist", "Playlist"));
        item_title.set_tip(app.carrot.L("playlist_tip", "Playlists you have stored for listening when not connected to the network"));

        Carrot_Box_Item item_add = app.Create_item("item_add");
        item_add.set_icon(app.carrot.icon_carrot_add);
        item_add.set_title(app.carrot.L("create_playlist", "Create a new list"));
        item_add.set_tip(app.carrot.L("create_playlist_tip", "Manage your songs in lists and folders"));
        item_add.set_act(() => Create_folder());

        if (this.leng > 0)
        {
            int index_m = 0;
            for(int i = 0; i < this.leng; i++)
            {
                string s_data = PlayerPrefs.GetString("mo_" + i);
                if (s_data!= "")
                {
                    var index = i;
                    IDictionary data_m = (IDictionary) Json.Deserialize(s_data);
                    Carrot_Box_Item box_item=app.Create_item("mo_item_" + i);
                    data_m["index"] = index_m;
                  
                    box_item.set_title(data_m["name"].ToString());
                    if(data_m["artist"]!=null) box_item.set_tip(data_m["artist"].ToString());

                    Carrot_Box_Btn_Item btn_del = box_item.create_item();
                    btn_del.set_icon(app.carrot.sp_icon_del_data);
                    btn_del.set_icon_color(Color.white);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() => this.Delete(index));

                    if (i % 2 == 0)
                        box_item.GetComponent<Image>().color = app.color_row_1;
                    else
                        box_item.GetComponent<Image>().color = app.color_row_2;

                    if (data_m["id"] != null)
                    {
                        string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                        Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                        if (sp_pic_avatar != null)
                            box_item.set_icon_white(sp_pic_avatar);
                        else
                            if(data_m["avatar"]!=null) app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
                    }

                    if (data_m["type"].ToString() == "folder")
                    {
                        box_item.set_tip(app.carrot.L("playlist", "Playlist"));
                        box_item.set_icon(this.app.sp_icon_music);
                    }


                    if (data_m["type"].ToString() == "music")
                    {
                        box_item.set_tip(app.carrot.L("song", "Song"));
                        box_item.set_icon(this.app.sp_icon_music);
                    }

                    box_item.set_act(() => this.app.player_music.Play_by_data(data_m));
                }
            }
        }
    }

    private void Delete(int index)
    {
        app.carrot.Show_msg(app.carrot.L("delete","Delete song"),"Successfully deleted song from archive!",Msg_Icon.Success);
        PlayerPrefs.DeleteKey("mo_" + index);
        this.Show();
    }

    private void Create_folder()
    {
        app.carrot.play_sound_click();
        if (box_inp != null) box_inp.close();
        box_inp=app.carrot.Show_input(app.carrot.L("create_playlist", "Create a new list"), app.carrot.L("create_playlist_inp", "Enter the name of the list you want to create"));
        box_inp.set_act_done(Create_folder_done);
    }

    private void Create_folder_done(string s_name)
    {
        app.carrot.play_sound_click();
        IDictionary data_folder = (IDictionary)Json.Deserialize("{}");
        data_folder["name"]=s_name;
        data_folder["type"] = "folder";
        this.Add(data_folder);
        if (box_inp != null) box_inp.close();
        this.Show();
    }

}
