using UnityEngine;
using Anoho.Attributes;

namespace ClassSelectorTest
{
    [System.Serializable]
    public abstract class BaseClass
    {
        [SerializeField]
        private uint id;
    }

    [System.Serializable]
    public class SubClass : BaseClass
    {
        [SerializeField]
        private string name;
    }

    public class ClassSelectorTest : MonoBehaviour
    {
        [System.Serializable]
        public class NestedClass : BaseClass
        {
            [SerializeField]
            private int count;

            [System.Serializable]
            public class DeeplyNestedClass : BaseClass
            {
                [SerializeField]
                private float value;
            }
        }

        [SerializeReference, ClassSelector]
        private BaseClass instance;

        [SerializeReference, ClassSelector]
        private BaseClass[] instances;
    }
}