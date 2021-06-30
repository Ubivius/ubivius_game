using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ubv.ui.client
{
    [RequireComponent(typeof(Selectable))]
    public class HighlightBehavior : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!EventSystem.current.alreadySelecting)
            {
                EventSystem.current.SetSelectedGameObject(this.gameObject);
            }
        }


        public void OnSelect(BaseEventData eventData)
        {
            this.GetComponentInChildren<TMP_Text>().color = Color.white;
        }


        public void OnDeselect(BaseEventData eventData)
        {
            this.GetComponent<Selectable>().OnPointerExit(null);
            this.GetComponentInChildren<TMP_Text>().color = new Color(100f / 255f, 100f / 255f, 100f / 255f);
        }
    }
}