using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_country_item : MonoBehaviour
{
    public Text txt_name;
    public Image icon;
    public string key_lang;
    public int type = 0;
    public void click()
    {
        PlayerPrefs.SetString("lang_music", this.key_lang);
        if (this.type == 0)
        {
            GameObject.Find("App").GetComponent<App>().open_menu_footer(0);
        }
        else
        {
            GameObject.Find("App").GetComponent<App>().open_menu_footer(1);
        }
    }
}
