using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_key_search : MonoBehaviour
{
    public Text txt_name;
    public void click()
    {
        GameObject.Find("App").GetComponent<App>().set_text_inp_search(this.txt_name.text);
    }
}
