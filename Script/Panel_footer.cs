using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_footer : MonoBehaviour
{
    public GameObject panel_menu_full;
    public Transform area_menu_portrait;
    public Transform area_menu_landscape;
    public Transform area_menu_full;

    public void show_menu_full()
    {
        GameObject.Find("App").GetComponent<App>().carrot.clear_contain(this.area_menu_full);
        foreach(Transform item_menu_mini in this.area_menu_portrait)
        {
            GameObject item_menu_full = Instantiate(item_menu_mini.gameObject);
            item_menu_full.transform.SetParent(this.area_menu_full);
            item_menu_full.transform.localPosition = new Vector3(item_menu_full.transform.localPosition.x, item_menu_full.transform.localPosition.y, 0f);
            item_menu_full.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_menu_full.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        this.panel_menu_full.SetActive(true);
    }

    public void hide_menu_full()
    {
        this.panel_menu_full.SetActive(false);
    }

    public void show_menu_for_portrait()
    {

    }

    public void show_menu_for_landscape()
    {
        GameObject.Find("App").GetComponent<App>().carrot.clear_contain(this.area_menu_landscape);
        foreach (Transform item_menu_mini in this.area_menu_portrait)
        {
            GameObject item_menu_landscape = Instantiate(item_menu_mini.gameObject);
            item_menu_landscape.transform.SetParent(this.area_menu_landscape);
            item_menu_landscape.transform.localPosition = new Vector3(item_menu_landscape.transform.localPosition.x, item_menu_landscape.transform.localPosition.y, 0f);
            item_menu_landscape.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_menu_landscape.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
