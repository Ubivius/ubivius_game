using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace ubv.ui.client
{
    [RequireComponent(typeof(Button))]
    public class LinkBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
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
            this.GetComponentInChildren<TMP_Text>().color = Color.black;
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            this.GetComponentInChildren<TMP_Text>().color = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            this.GetComponentInChildren<TMP_Text>().color = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        }
    }
}