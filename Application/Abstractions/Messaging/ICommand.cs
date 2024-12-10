using MediatR;


namespace Application.Abstractions.Messaging
{
    //Out Keyword : ICommand<TResponse> arayüzünün herhangi bir yöntemi veya özelliği, TResponse türünde bir değer döndürebilir, ancak bu türde bir parametre alamaz.
    public interface ICommand<out TResponse> : IRequest<TResponse>
    {
    }
}
