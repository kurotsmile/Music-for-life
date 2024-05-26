using Carrot;
using System.Collections;
using UnityEngine;

public class List_Backgrounds: MonoBehaviour
{

    public App app;
    private Carrot_Box box;
    private string s_data_temp = "";

    public void On_Load()
    {
        Texture2D data_pic_bk = this.app.carrot.get_tool().get_texture2D_to_playerPrefs("bk_app");
        if (data_pic_bk != null) this.set_skybox_Texture(data_pic_bk);
    }

    public void Show()
    {
        app.carrot.play_sound_click();
        if (s_data_temp == "")
            this.Get_data_from_server();
        else
            this.Load_list_by_data(this.s_data_temp);
    }

    private void Get_data_from_server()
    {
        app.carrot.show_loading();
        StructuredQuery q = new("background");
        app.carrot.server.Get_doc(q.ToJson(), (data) =>
        {
            app.carrot.hide_loading();
            this.Load_list_by_data(data);
        }, app.Act_server_fail);
    }

    private void Load_list_by_data(string data)
    {
        this.s_data_temp = data;
        Fire_Collection fc = new(data);
        if (!fc.is_null)
        {
            this.box = app.carrot.Create_Box();
            this.box.set_title(app.carrot.L("bk", "Background"));
            this.box.set_icon(app.icon_background);
            this.box.set_type(Carrot_Box_Type.Grid_Box);

            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data_bk = fc.fire_document[i].Get_IDictionary();
                Carrot_Box_Item bk_item = this.box.create_item("item_bk_" + i);
                string s_id_sp_bk = "bk_sp_"+data_bk["id"].ToString();
                Sprite sp_bk=app.carrot.get_tool().get_sprite_to_playerPrefs(s_id_sp_bk);
                if (sp_bk != null)
                    bk_item.set_icon_white(sp_bk);
                else
                    app.carrot.get_img_and_save_playerPrefs(data_bk["icon"].ToString(), bk_item.img_icon, s_id_sp_bk);

                bk_item.set_act(() => this.Set_bk_for_app(s_id_sp_bk));
            }
        }
    }

    private void Set_bk_for_app(string id_bk_app)
    {
        app.carrot.play_sound_click();
        if (this.box != null) this.box.close();
        Texture2D texture = app.carrot.get_tool().get_texture2D_to_playerPrefs(id_bk_app);
        this.app.carrot.get_tool().PlayerPrefs_Save_texture2D("bk_app",texture);
        this.set_skybox_Texture(texture);
        this.app.panel_footer.hide_menu_full();
    }

    private void set_skybox_Texture(Texture textT)
    {
        Material result = new Material(Shader.Find("RenderFX/Skybox"));
        result.SetTexture("_FrontTex", textT);
        result.SetTexture("_BackTex", textT);
        result.SetTexture("_LeftTex", textT);
        result.SetTexture("_RightTex", textT);
        result.SetTexture("_UpTex", textT);
        result.SetTexture("_DownTex", textT);
        this.app.bk.material = result;
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             