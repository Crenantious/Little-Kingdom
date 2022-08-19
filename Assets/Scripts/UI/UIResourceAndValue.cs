using TMPro;
using UnityEngine;

public class UIResourceAndValue : MonoBehaviour
{
    [SerializeField] TMP_Text resourceName;
    [SerializeField] TMP_Text resourceValue;

    public Resource Resource { get; private set; }
    public int Value { get; private set; }

    public void Initialise(Resource resource, int value)
    {
        Resource = resource;
        Value = value;
        SetResource(resource);
        SetValue(value);
        gameObject.SetActive(true);
    }

    public void SetValue(int value) =>
        resourceValue.text = value.ToString();

    public void SetResource(Resource resource) =>
        resourceName.text = resource.name;
}
