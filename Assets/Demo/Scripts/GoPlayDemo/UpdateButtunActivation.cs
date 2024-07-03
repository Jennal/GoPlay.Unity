using UnityEngine;
using UnityEngine.UI;

public class UpdateButtunActivation : MonoBehaviour
{
    public GoPlayClient goPlayClient;
    
    public Button[] connectedButtons;
    public Button[] disconnectedButtons;

    private void Update()
    {
        foreach (var button in connectedButtons)
        {
            button.interactable = goPlayClient.Client?.IsConnected ?? false;
        }
        
        foreach (var button in disconnectedButtons)
        {
            button.interactable = !goPlayClient.Client?.IsConnected ?? true;
        }
    }
}
