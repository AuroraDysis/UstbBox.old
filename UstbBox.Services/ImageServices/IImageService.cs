using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.ImageServices
{
    using UstbBox.Models.Images;

    public interface IImageService
    {
        IObservable<ImageObject> GetCampusMap();

        IObservable<List<ImageObject>> GetSchoolCalendars();
    }
}
