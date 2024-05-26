using Carrot;
using System.Collections;
using UnityEngine;

public class List_Backgrounds: MonoBehaviour
{

    public App app;
    private Carrot_Box box;
    private string s_data_temp = "";
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

                bk_item.set_act(() => this.Set_bk_for_app());
            }
        }
    }

    private void Set_bk_for_app()
    {
        app.carrot.Show_msg("sdsd");
    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             