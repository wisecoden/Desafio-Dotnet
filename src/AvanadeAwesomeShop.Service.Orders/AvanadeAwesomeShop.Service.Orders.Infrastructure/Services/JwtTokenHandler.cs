using Microsoft.Extensions.Logging;

namespace AvanadeAwesomeShop.Service.Orders.Infrastructure.Services
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<JwtTokenHandler> _logger;
        private string? _cachedToken;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public JwtTokenHandler(IJwtTokenService jwtTokenService, ILogger<JwtTokenHandler> logger)
        {
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("JwtTokenHandler: Processing request to {RequestUri}", request.RequestUri);
            
            // Verificar se precisamos de um novo token
            if (string.IsNullOrEmpty(_cachedToken) || DateTime.UtcNow >= _tokenExpiry.AddMinutes(-5))
            {
                _logger.LogInformation("Generating new service JWT token");
                _cachedToken = _jwtTokenService.GenerateServiceToken();
                _tokenExpiry = DateTime.UtcNow.AddHours(24);
                _logger.LogInformation("Generated token: {Token}", _cachedToken?.Substring(0, Math.Min(50, _cachedToken.Length)) + "...");
            }

            // Adicionar token ao header Authorization
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cachedToken);
            
            _logger.LogInformation("Added JWT token to request for {RequestUri}", request.RequestUri);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}