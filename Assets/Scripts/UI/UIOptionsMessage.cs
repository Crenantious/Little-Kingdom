using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIOptionsMessage : MonoBehaviour
{
    [SerializeField] Transform buttonParent;
    [SerializeField] TMP_Text message;
    [SerializeField] Button button;

    public void Initialise(string message, List<string> options, UnityAction<int> callback)
    {
        this.message.text = message;

        for (int i = 0; i < options.Count; i++)
        {
            int j = i;
            Button b = Instantiate(button, buttonParent);
            b.gameObject.SetActive(true);
            b.transform.GetChild(0).GetComponent<TMP_Text>().text = options[i];
            b.onClick.AddListener(() => Callback(j, callback));
        }
    }

    void Callback(int option, UnityAction<int> callback)
    {
        callback(option);
        Destroy(gameObject);
    }
}
