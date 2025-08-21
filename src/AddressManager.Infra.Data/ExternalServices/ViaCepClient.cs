using AddressManager.Application.Interfaces;
using AddressManager.Application.Models;
using System.Text.Json;

namespace AddressManager.Infra.Data.ExternalServices;

public class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _httpClient;

    public ViaCepClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ViaCepData?> GetAddressByZipCodeAsync(string zipCode)
    {
        var url = $"ws/{zipCode}/json/";

        var response = await _httpClient.GetAsync(url);

        var content = await response.Content.ReadAsStringAsync();

        var objResponse = JsonSerializer.Deserialize<ViaCepResponse>(content);

        if (objResponse == null || objResponse?.Erro?.ToLower() == "true")
        {
            return null;
        }

        return new ViaCepData(objResponse.Cep, objResponse.Logradouro, objResponse.Bairro, objResponse.Localidade, objResponse.Estado, objResponse.Uf, objResponse.Regiao);
    }
}
