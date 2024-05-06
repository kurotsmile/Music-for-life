using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel_more_item : MonoBehaviour
{
    public Text txt_title;

    public void click()
    {
        GameObject.Find("App").GetComponent<App>().show_more_list_music();
        Destroy(this.gameObject);
    }
}
