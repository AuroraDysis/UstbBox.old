using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.App.ViewModels.Models
{
    public class ResourceViewModel
    {
        public string Name { get; set; }
    }

    public class ResourceNotDowloadedViewModel : ResourceViewModel
    {

    }

    public class ResourceDownloadingViewModel : ResourceViewModel
    {

    }

    public class ResourceDownloadedViewModel : ResourceViewModel
    {

    }
}
