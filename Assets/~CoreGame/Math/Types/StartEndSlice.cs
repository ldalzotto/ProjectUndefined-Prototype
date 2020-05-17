using UnityEngine;

namespace CoreGame
{
    public class StartEndSlice
    {
        private float beginIncluded;
        private float endExcluded;

        public StartEndSlice(float beginIncluded, float endExcluded)
        {
            this.beginIncluded = beginIncluded;
            this.endExcluded = endExcluded;
        }

        public StartEndSlice(int beginIncluded, int endExcluded)
        {
            this.beginIncluded = beginIncluded;
            this.endExcluded = endExcluded;
        }

        public float BeginIncluded { get => beginIncluded; }
        public float EndExcluded { get => endExcluded; }

        public bool Up()
        {
            return beginIncluded <= endExcluded;
        }

        public bool Down()
        {
            return beginIncluded >= endExcluded;
        }

        public bool Contains(float a)
        {
            return a >= beginIncluded && a < endExcluded;
        }

        public bool AngleDegreeContains(float a)
        {
            if (Up())
            {
                if (endExcluded == 360f)
                {
                    return a >= beginIncluded && a <= endExcluded;
                }
                else
                {
                    return a >= beginIncluded && a < endExcluded;
                }
            }
            else
            {
                if (endExcluded == 360f)
                {
                    return a <= beginIncluded && a >= endExcluded;
                }
                else
                {
                    return a < beginIncluded && a >= endExcluded;
                }
            }

        }

        public float AngleDiffNotSigned()
        {
            return Mathf.Abs(endExcluded - beginIncluded);
        }

        public override string ToString()
        {
            return "b : " + beginIncluded + " e : " + endExcluded;
        }

        public override bool Equals(object obj)
        {
            var slice = obj as StartEndSlice;
            return slice != null &&
                   beginIncluded == slice.beginIncluded &&
                   endExcluded == slice.endExcluded;
        }

        public override int GetHashCode()
        {
            var hashCode = -8683044;
            hashCode = hashCode * -1521134295 + beginIncluded.GetHashCode();
            hashCode = hashCode * -1521134295 + endExcluded.GetHashCode();
            return hashCode;
        }
    }
}
