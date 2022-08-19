using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIOptionsMessageManager : MonoBehaviour
{
    [SerializeField] Transform optionsMessage;

    /// <summary>
    /// Display a message with an arbitrary number of buttons below, in the centre of the screen.
    /// </summary>
    /// <param name="message">The message to display atop the buttons.</param>
    /// <param name="options">Each option in the List is the name of a button. Buttons are displayed in the order of the List.</param>
    /// <param name="callback">The Action to call when a button is pressed. The index of the button that was pressed is passed as a parameter.</param>
    public void Display(string message, List<string> options, UnityAction<int> callback)
    {
        Transform newContainer = Instantiate(optionsMessage, transform);
        Utilities.ResetTransformLocally(newContainer);
        newContainer.gameObject.SetActive(true);
        newContainer.GetComponent<UIOptionsMessage>().Initialise(message, options, callback);
    }
}
