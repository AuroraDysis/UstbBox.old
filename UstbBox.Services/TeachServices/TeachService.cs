using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.TeachServices
{
    using UstbBox.Models.Teach;

    public class TeachService : ITeachService
    {
        public IObservable<List<TeachNewsItem>> GetLatestNews()
        {
        }
    }
}
