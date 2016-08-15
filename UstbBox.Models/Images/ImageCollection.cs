using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Models.Images
{
    using System.Reactive.Subjects;

    public class ImageCollection
    {
        public string Name { get; set; }

        public List<ImageObject> Images { get; set; }
    }
}
