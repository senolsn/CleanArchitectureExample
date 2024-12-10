﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Webinars.Queries.GetWebinarById
{
    public sealed record WebinarResponse(Guid Id,  string Name, DateTime ScheduledOn);
}
