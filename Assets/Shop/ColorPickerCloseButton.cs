namespace Shop
{
    public class ColorPickerCloseButton : TaliduButton
    {
        protected override void OnClickedButton()
        {
            UIManager.CloseColorPickerEvent.Invoke();
        }
    }
}