using Carrot;
using System.Collections;
using UnityEngine;

public class Playlist_Search : MonoBehaviour
{
    public App app;
    private Carrot_Window_Input box_input;
    public void Show()
    {
        this.app.carrot.play_sound_click();
        this.box_input=this.app.carrot.show_search(Act_done, "Enter the name of the song, radio channel or audio you want to search for");
    }

    private void Act_done(string s_key)
    {
        this.box_input.close();
        this.app.carrot.play_sound_click();
        Debug.Log("Key search:" + s_key);
        app.clear_all_contain();

        Carrot.Carrot_Box_Item item_title = app.Create_item("title");
        item_title.set_icon(this.app.carrot.icon_carrot_search);
        item_title.set_title("Search Results");
        item_title.set_tip("54 Results Found");

        string s_data_artist = this.app.playlist.get_s_data_artist();
        if (s_data_artist != "")
        {
            IList list_artist = this.get_list(s_data_artist);
            for(int i = 0; i < list_artist.Count; i++)
            {
                IDictionary atrist = list_artist[i] as IDictionary;
                if (atrist["name"].ToString().IndexOf(s_key)>-1)
                {
                    Carrot.Carrot_Box_Item item_art=app.Create_item("atrist_"+i);
                    item_art.set_title(atrist["name"].ToString());
                    item_art.set_icon(app.sp_icon_artist);
                }
            }
        }
    }

    IList get_list(string s_data)
    {
        IDictionary data = (IDictionary)Json.Deserialize(s_data);
        return (IList)data["all_item"];
    }
}
