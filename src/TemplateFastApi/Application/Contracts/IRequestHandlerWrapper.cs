using MediatR;

namespace TemplateFastApi.Application.Contracts;

public interface IRequestHandlerWrapper<TIn, TOut> : IRequestHandler<TIn, TOut>
    where TIn : IRequestWrapper<TOut>
{
}