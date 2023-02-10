using CustomInput;

namespace Interactables
{
    public class FlipItemButton : TaliduButton
    {
        protected override void OnClickedButton()
        {
            OnClick();
        }

        void OnClick()
        {
            SelectionManager.FLIP_OBJECT_EVENT.Invoke();
        }
    }
}
