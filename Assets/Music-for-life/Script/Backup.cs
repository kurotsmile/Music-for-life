using Carrot;
using Crosstales.Common.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backup : MonoBehaviour
{
    [Header("Main Obj")]
    public App app;
    private Carrot_Box box;

    public void Show()
    {
        app.carrot.play_sound_click();
        this.box = app.carrot.Create_Box();
        this.box.set_icon(app.sp_icon_sync);
        this.box.set_title(app.carrot.L("backup", "Backup"));

        Carrot_Box_Item item_export_data = this.box.create_item("item_export_data");
        item_export_data.set_icon(app.sp_icon_export_data);
        item_export_data.set_title(app.carrot.L("export_data", "Export Data"));
        item_export_data.set_tip(app.carrot.L("export_data_tip", "Backup data as json data file"));
        item_export_data.set_act(() =>
        {
            app.carrot.play_sound_click();
            app.file.Set_filter(Carrot_File_Data.JsonData);
            app.file.Save_file(Act_export_data_done);
        });

        Carrot_Box_Item item_import_substitution = this.box.create_item("item_import_substitution");
        item_import_substitution.set_icon(app.sp_icon_import_data);
        item_import_substitution.set_title(app.carrot.L("substitution_import", "Substitution import"));
        item_import_substitution.set_tip(app.carrot.L("substitution_import_tip", "Recover data by replacing data using json files"));
        item_import_substitution.set_act(() =>
        {
            app.carrot.play_sound_click();
            app.file.Set_filter(Carrot_File_Data.JsonData);
            app.file.Open_file((paths) =>
            {
                Act_import_data_done(paths, true);
            });
        });

        Carrot_Box_Item item_import_additional = this.box.create_item("item_import_additional");
        item_import_additional.set_icon(app.sp_icon_import_data);
        item_import_additional.set_title(app.carrot.L("additional_import","Additional import"));
        item_import_additional.set_tip(app.carrot.L("additional_import_tip", "Recover data by adding data using json files"));
        item_import_additional.set_act(() =>
        {
            app.carrot.play_sound_click();
            app.file.Set_filter(Carrot_File_Data.JsonData);
            app.file.Open_file((paths) =>
            {
                Act_import_data_done(paths, false);
            });
        });
    }

    private void Act_export_data_done(string[] s_path)
    {
        app.carrot.play_sound_click();
        List<IDictionary> list_data=app.playlist_offline.get_list_all_type();
        FileHelper.WriteAllText(s_path[0],Json.Serialize(list_data));
        app.carrot.Show_msg("Export","Export json data success!\n" + s_path[0],Msg_Icon.Alert);
    }

    private void Act_import_data_done(string[] s_path,bool is_replacing)
    {
        app.carrot.play_sound_click();
        string s_data=FileHelper.ReadAllText(s_path[0]);
        IList list_item =(IList) Json.Deserialize(s_data);
        if (is_replacing) this.app.playlist_offline.Clear_All_data();
        for (int i=0; i < list_item.Count; i++)
        {
            IDictionary data_song=(IDictionary) list_item[i];
            this.app.playlist_offline.Add(data_song);
        }
        app.carrot.Show_msg("Import", "Import json data success!\n" + s_path[0], Msg_Icon.Alert);
        app.carrot.delay_function(2f, ()=>{
            app.playlist_offline.Show();
        });
    }
}
