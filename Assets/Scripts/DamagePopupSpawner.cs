using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public GameObject popupPrefab;

    public void Spawn(Vector3 worldPosition, int amount, Color color, bool isCrit = false)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject popup = Instantiate(popupPrefab, screenPos, Quaternion.identity, GameObject.Find("Canvas").transform);
        popup.GetComponent<DamagePopup>().Setup(amount, color, isCrit);
    }
}
