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
        this.box_input=this.app.carrot.show_search(Act_done, app.carrot.L("search_tip", "You can search for song title, singer, genre or radio channel by search keyword."));
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

        this.app.playlist.Search(s_key);
    }

    IList get_list(string s_data)
    {
        IDictionary data = (IDictionary)Json.Deserialize(s_data);
        return (IList)data["all_item"];
    }
}
