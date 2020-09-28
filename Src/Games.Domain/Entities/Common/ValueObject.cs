using System.Collections.Generic;
using System.Linq;

namespace Games.Domain.Entities.Common
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityValues();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            var thisValues = GetEqualityValues().GetEnumerator();
            var otherValues = other.GetEqualityValues().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (ReferenceEquals(thisValues.Current, null) ^ ReferenceEquals(otherValues.Current, null))
                {
                    return false;
                }

                if (thisValues.Current != null && !thisValues.Current.Equals(otherValues.Current))
                {
                    return false;
                }
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }

            return ReferenceEquals(left, null) || ReferenceEquals(left, right) || left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return GetEqualityValues()
                .Select(v => v != null ? v.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        public virtual ValueObject GetCopy()
        {
            return (ValueObject)this.MemberwiseClone();
        }
    }
}