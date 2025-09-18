using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NWH.Common.Input
{
    /// <summary>
    ///     Adds clicked and pressed flags to the standard Unity UI Button.
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public class MobileInputButton : Button
    {
        public bool hasBeenClicked;
        public bool isPressed;

        public bool deactive;

        public UnityEvent onDown = new (),
            onUp = new ();


        protected override void OnEnable()
        {
            base.OnEnable();
            deactive = false;
        }
        private void Update()
        {
            if(!deactive) isPressed = IsPressed();
            hasBeenClicked = false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onDown.Invoke();
            hasBeenClicked = true;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onUp.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isPressed = false;
        }

    }
}