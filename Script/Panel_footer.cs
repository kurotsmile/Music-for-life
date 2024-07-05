using UnityEngine;

public class Panel_footer : MonoBehaviour
{
    public GameObject panel_menu_full;
    public Transform area_menu_portrait;
    public Transform area_menu_landscape;
    public Transform area_menu_full;
    public Transform[] tr_btn_menu;

    public void show_menu_full()
    {
        foreach(Transform item_menu in this.tr_btn_menu)
        {
            item_menu.transform.SetParent(this.area_menu_full);
            item_menu.transform.localPosition = new Vector3(item_menu.transform.localPosition.x, item_menu.transform.localPosition.y, 0f);
            item_menu.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_menu.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        this.panel_menu_full.SetActive(true);
    }

    public void hide_menu_full()
    {
        this.panel_menu_full.SetActive(false);
    }

    public void show_menu_for_landscape()
    {
        foreach (Transform item_menu in this.tr_btn_menu)
        {
            item_menu.transform.SetParent(this.area_menu_landscape);
            item_menu.transform.localPosition = new Vector3(item_menu.transform.localPosition.x, item_menu.transform.localPosition.y, 0f);
            item_menu.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_menu.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void show_menu_for_portrait()
    {
        foreach (Transform item_menu in this.tr_btn_menu)
        {
            item_menu.transform.SetParent(this.area_menu_portrait);
            item_menu.transform.localPosition = new Vector3(item_menu.transform.localPosition.x, item_menu.transform.localPosition.y, 0f);
            item_menu.transform.localRotation = Quaternion.Euler(Vector3.zero);
            item_menu.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
