using Carrot;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Music_offiline : MonoBehaviour
{
    [Header("Obj Main")]
    public App app;

    [Header("Obj Offline")]
    public Sprite ps_icon_offline;
    int leng=0;

    private Carrot_Window_Input box_inp = null;
    private Carrot_Box box = null;
    private IDictionary data_cur = null;

    public void On_Load()
    {
        this.leng = PlayerPrefs.GetInt("mo_length",0);
    }

    public void Add(IDictionary data, byte[] data_mp3)
    {
        app.carrot.get_tool().save_file(this.leng+".data", data_mp3);
        this.Add(data);
    }

    public void Add(IDictionary data)
    {
        PlayerPrefs.SetString("mo_" + this.leng, Json.Serialize(data));
        this.leng++;
        PlayerPrefs.SetInt("mo_length", this.leng);
    }

    private void Update_data(int index,IDictionary data)
    {
        PlayerPrefs.SetString("mo_" + index, Json.Serialize(data));
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
                    index_m++;
                    var index = i;
                    IDictionary data_m = (IDictionary) Json.Deserialize(s_data);
                    Carrot_Box_Item box_item=app.Create_item("mo_item_" + i);
                    data_m["index"] = i;
                  
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

                    if (data_m["type"].ToString() == "folder")
                    {
                        box_item.set_tip(app.carrot.L("playlist", "Playlist"));
                        box_item.set_icon(this.app.sp_icon_playlist);
                        box_item.set_act(() => Show_menu_folder(data_m));
                    }

                    if (data_m["type"].ToString() == "music_offline")
                    {
                        box_item.set_tip(app.carrot.L("m_music", "Music"));
                        box_item.set_icon(this.app.sp_icon_music);

                        if (data_m["id"] != null)
                        {
                            string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                            Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                            if (sp_pic_avatar != null)
                                box_item.set_icon_white(sp_pic_avatar);
                            else
                                if (data_m["avatar"] != null) app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
                        }
                        box_item.set_act(() => this.app.player_music.Play_by_data(data_m));
                    }

                    if (data_m["type"].ToString() == "radio_offline")
                    {
                        box_item.set_tip(app.carrot.L("m_radio", "Radio"));
                        box_item.set_icon(this.app.sp_icon_radio);

                        if (data_m["id"] != null)
                        {
                            string s_id_avatar = "pic_avatar_" + data_m["id"].ToString();
                            Sprite sp_pic_avatar = app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_avatar);
                            if (sp_pic_avatar != null)
                                box_item.set_icon_white(sp_pic_avatar);
                            else
                                if (data_m["avatar"] != null) app.carrot.get_img_and_save_playerPrefs(data_m["avatar"].ToString(), box_item.img_icon, s_id_avatar);
                        }
                        box_item.set_act(() => this.app.player_music.Play_by_data(data_m));
                    }


                    if (data_m["type"].ToString() == "sound_offline")
                    {
                        box_item.set_tip(app.carrot.L("m_sound", "Sound"));
                        box_item.set_icon(this.app.sp_icon_audio);
                        box_item.set_act(() => this.app.player_music.Play_by_data(data_m));
                    }

                    this.Create_btn_menu(box_item).set_act(()=>this.Show_menu_folder(data_m));
                }
            }
        }
    }

    private Carrot_Box_Btn_Item Create_btn_menu(Carrot_Box_Item item)
    {
        Carrot_Box_Btn_Item btn_menu=item.create_item();
        btn_menu.set_icon(app.carrot.icon_carrot_all_category);
        btn_menu.set_icon_color(Color.white);
        btn_menu.set_color(app.carrot.color_highlight);
        return btn_menu;
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

    private void Show_menu_folder(IDictionary data)
    {
        this.data_cur = data;
        app.carrot.play_sound_click();
        if (box != null) this.box.close();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.carrot.icon_carrot_advanced);
        this.box.set_title(app.carrot.L("menu","Menu")+" - " + data["name"].ToString());

        var index = int.Parse(data["index"].ToString());
        Carrot_Box_Item item_rename = box.create_item("item_rename");
        item_rename.set_icon(app.carrot.icon_carrot_write);
        item_rename.set_title("Rename");
        item_rename.set_tip("Change the name of this item");
        item_rename.set_act(() => Rename(data));

        if (data["type"].ToString()=="music_offline"|| data["type"].ToString() == "radio_offline"|| data["type"].ToString() == "sound_offline")
        {
            Carrot_Box_Item item_move = box.create_item("item_move");
            item_move.set_icon(app.sp_icon_move);
            item_move.set_title("Move");
            item_move.set_tip("Move this item to another list");
        }
 
        Carrot_Box_Item item_del = box.create_item("item_del");
        item_del.set_icon(app.carrot.sp_icon_del_data);
        item_del.set_title("Delete");
        item_del.set_tip("Remove this item from the list");
        item_del.set_act(() => this.Delete(index));
    }

    private void Rename(IDictionary data)
    {
        app.carrot.play_sound_click();
        this.data_cur = data;
        this.box_inp= app.carrot.Show_input("Rename", "Change the name of this item", data["name"].ToString());
        box_inp.set_act_done(Act_done_name);
    }

    private void Act_done_name(string s_name)
    {
        this.data_cur["name"] = s_name;
        if (box_inp != null) box_inp.close();
        if (box != null) box.close();
        int index=int.Parse(this.data_cur["index"].ToString());
        this.Update_data(index,this.data_cur);
        app.carrot.Show_msg("Update name item success!");
        this.Show();
    }
}
