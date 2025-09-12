using UnityEngine;

namespace Anoho.Attributes
{
    public sealed class ReadonlyAttribute : PropertyAttribute
    {
        public string MemberName { get; }

        public ReadonlyAttribute() : this(null) { }

        public ReadonlyAttribute(string memberName)
        {
            MemberName = memberName;
        }
    }
}
