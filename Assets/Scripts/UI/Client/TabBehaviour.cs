using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ubv.ui.client
{
    public class TabBehaviour : MonoBehaviour
    {

        private EventSystem system;

        private void Start()
        {
            system = EventSystem.current;
        }

        protected virtual void Update()
        {
            if (system?.currentSelectedGameObject == null || !Input.GetKeyDown(KeyCode.Tab))
                return;

            Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
            if (current == null)
                return;

            bool up = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            Selectable next = up ? current.FindSelectableOnUp() : current.FindSelectableOnDown();

            if (next == null)
            {
                next = current;

                Selectable pnext;
                if (up) while ((pnext = next.FindSelectableOnDown()) != null) next = pnext;
                else while ((pnext = next.FindSelectableOnUp()) != null) next = pnext;
            }

            // Simulate Inputfield MouseClick
            InputField inputfield = next.GetComponent<InputField>();
            if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));

            // Select the next item in the taborder of our direction
            system.SetSelectedGameObject(next.gameObject);
        }
    }
}
