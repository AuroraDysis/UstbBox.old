using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Models.Teach
{
    public class TeachNewsItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Link { get; set; }

        public string Date { get; set; }

        public static bool operator ==(TeachNewsItem a, TeachNewsItem b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name == b.Name && a.Link == b.Link && a.Date == b.Date;
        }

        public static bool operator !=(TeachNewsItem a, TeachNewsItem b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            TeachNewsItem p = obj as TeachNewsItem;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Name == p.Name && this.Link == p.Link && this.Date == p.Date;
        }

        public bool Equals(TeachNewsItem p)
        {
            // Return true if the fields match:
            return this.Name == p.Name && this.Link == p.Link && this.Date == p.Date;
        }

        public override int GetHashCode()
        {
            return (this.Name + this.Link + this.Date).GetHashCode();
        }
    }
}
