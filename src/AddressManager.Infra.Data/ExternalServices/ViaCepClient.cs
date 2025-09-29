using AddressManager.Application.Interfaces;
using AddressManager.Application.Models;
using AddressManager.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AddressManager.Infra.Data.ExternalServices;

public class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepClient> _logger;

    public ViaCepClient(HttpClient httpClient, ILogger<ViaCepClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ViaCepData> GetAddressByZipCodeAsync(string zipCode)
    {
        var url = $"ws/{zipCode}/json/";
        _logger.LogInformation("Starting ViaCEP API request for ZipCode {ZipCode}", zipCode);

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("ViaCEP API request failed for ZipCode {ZipCode} with status code {StatusCode}", zipCode, response.StatusCode);

            throw new ExternalServiceException($"Serviço de consulta de CEP temporariamente indisponível.");
        }

        var content = await response.Content.ReadAsStringAsync();

        var objResponse = JsonSerializer.Deserialize<ViaCepResponse>(content);

        if (objResponse == null || objResponse?.Erro?.ToLower() == "true")
        {
            _logger.LogWarning("ZipCode {ZipCode} not found in ViaCEP API", zipCode);

            throw new AddressNotFoundException($"CEP '{zipCode}' não encontrado");
        }

        _logger.LogInformation("Successfully retrieved address for ZipCode {ZipCode} from ViaCEP API", zipCode);

        return new ViaCepData(objResponse!.Cep, objResponse.Logradouro, objResponse.Bairro, objResponse.Localidade, objResponse.Estado, objResponse.Uf, objResponse.Regiao);
    }
}
