﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UstbBox.Services.TeachServices
{
    using UstbBox.Models.Teach;

    public interface ITeachService
    {
        IObservable<TeachNewsItem> GetLatestNews();
    }
}
