using Common.Exchanges.Response;

namespace TemplateFastApi.Application.Exchanges.Response;

public class SendTemplateResponse(bool success,  int statusCode, string? message) : BaseResponse(success, statusCode, message);