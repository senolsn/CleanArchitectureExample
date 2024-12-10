using Application.Abstractions.Messaging;
using System;


namespace Application.Webinars.Queries.GetWebinarById
{
    public sealed record GetWebinarByIdQuery(Guid WebinarId) : IQuery<WebinarResponse>;
   
}
