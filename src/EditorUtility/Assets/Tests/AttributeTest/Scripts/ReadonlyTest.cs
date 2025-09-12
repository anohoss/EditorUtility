using Anoho.Attributes;
using UnityEngine;

public class ReadonlyTest : MonoBehaviour
{
    [Readonly]
    [SerializeField]
    private int readonlyByDefault;

    [SerializeField]
    private bool isReadonly;

    [Readonly(nameof(isReadonly))]
    [SerializeField]
    private int readonlyByField;

    [Readonly(nameof(IsReadonly))]
    [SerializeField]
    private int readonlyByMethod;

    private bool IsReadonly()
    {
        return isReadonly;
    }
}
