using UnityEngine.UI;

namespace GoPlay.Framework.Extensions
{
    public static class ButtonExtensions
    {
        public static void ChangText(this Button Btn, string replaceText)
        {
            var _btnText = Btn.GetComponentInChildren<Text>(false);
            if (!_btnText) 
                return;
            _btnText.text = replaceText;
        }
    }
}