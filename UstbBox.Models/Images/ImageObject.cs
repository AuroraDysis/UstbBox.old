using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Models.Images
{
    public class ImageObject
    {
        public ImageObject()
        {
        }

        public ImageObject(string name, string uri)
        {
            this.Name = name;
            this.Uri = uri;
        }

        public string Name { get; set; }

        public string Uri { get; set; }

        public static bool operator ==(ImageObject a, ImageObject b)
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
            return a.Name == b.Name && a.Uri == b.Uri;
        }

        public static bool operator !=(ImageObject a, ImageObject b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            ImageObject p = obj as ImageObject;
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return this.Name == p.Name && this.Uri == p.Uri;
        }

        public bool Equals(ImageObject p)
        {
            // Return true if the fields match:
            return this.Name == p.Name && this.Uri == p.Uri;
        }

        public override int GetHashCode()
        {
            return (this.Name + this.Uri).GetHashCode();
        }
    }
}
