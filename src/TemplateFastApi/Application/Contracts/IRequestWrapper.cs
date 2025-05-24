using MediatR;

namespace TemplateFastApi.Application.Contracts;

public interface IRequestWrapper<T> : IRequest<T>
{
}