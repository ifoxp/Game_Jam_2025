using UnityEngine;

public class WindZoneShow : MonoBehaviour
{
    private void Update()
    {
        // ѕерев≥рка натисканн€ клав≥ш≥ V
        if (Input.GetKeyDown(KeyCode.V))
        {
            ShowWindZone();
        }
    }
    public void ShowWindZone()
    {
        // ќтримуЇмо поточне значенн€, за замовчуванн€м 0
        int currentValue = PlayerPrefs.GetInt("WindZoneShow", 0);

        // «м≥нюЇмо значенн€ на протилежне (0 -> 1 або 1 -> 0)
        int newValue = currentValue == 0 ? 1 : 0;

        // «бер≥гаЇмо нове значенн€ в PlayerPrefs
        PlayerPrefs.SetInt("WindZoneShow", newValue);
        PlayerPrefs.Save(); // «бер≥гаЇмо зм≥ни на диск
    }
}
