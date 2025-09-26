using AddressManager.Application.DTOs;
using AddressManager.Application.Interfaces;
using AddressManager.Domain.Entities;
using AddressManager.Domain.Exceptions;
using AddressManager.Domain.Interfaces;
using AddressManager.Domain.Value_Objects;
using AddressManager.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AddressManager.Application.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAddressDto> _createValidator;
    private readonly IValidator<UpdateAddressDto> _updateValidator;
    private readonly IViaCepClient _viaCepClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<AddressService> _logger;

    public AddressService(IUnitOfWork unitOfWork, IViaCepClient viaCepClient, IValidator<CreateAddressDto> createValidator, IValidator<UpdateAddressDto> updateValidator, IMemoryCache memoryCache, ILogger<AddressService> logger)
    {
        _unitOfWork = unitOfWork;
        _viaCepClient = viaCepClient;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _cache = memoryCache;
        _logger = logger;
    }

    public async Task<IEnumerable<AddressDto>> GetAllAddressesAsync(int pageNumber, int pageSize)
    {
        var addresses = await _unitOfWork.AddressRepository.GetAllAsync(pageNumber, pageSize);

        return addresses.Select( address => 
            new AddressDto(
                Id: address.Id,
                ZipCode: address.ZipCode.Value,
                Street: address.Street.Value,
                Number: address.Number,
                Complement: address.Complement,
                Reference: address.Reference,
                Neighborhood: address.Neighborhood.Value,
                City: address.City.Value,
                State: address.State.Name,
                Region: address.Region.Value
            ));
    }

    public async Task<AddressDto> GetAddressByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching address with ID {AddressId}", id);

        var cacheKey = $"address:{id}";

        var address = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            return await _unitOfWork.AddressRepository.GetByIdAsNoTrackingAsync(id);
        });

        if (address is null)
        {
            _logger.LogWarning("Address with ID {AddressId} not found", id);

            throw new AddressNotFoundException($"Endereço com o ID '{id}' não encontrado");
        }

        return new AddressDto(
            Id: address.Id,
            ZipCode: address.ZipCode.Value,
            Street: address.Street.Value,
            Number: address.Number,
            Complement: address.Complement,
            Reference: address.Reference,
            Neighborhood: address.Neighborhood.Value,
            City: address.City.Value,
            State: address.State.Name,
            Region: address.Region.Value
        );
    }

    public async Task<AddressDto> CreateAddressAsync(CreateAddressDto addressDto)
    {
        _logger.LogInformation("Creating address from ZipCode {ZipCode}", addressDto.ZipCode);

        var result = _createValidator.Validate(addressDto);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var normalizedZipCode = addressDto.ZipCode.Replace("-", "").Trim();

        var cacheKey = $"viacepdata:{normalizedZipCode}";

        _logger.LogInformation("Fetching ViaCEP data for ZipCode {ZipCode}", addressDto.ZipCode);

        var viaCepData = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            _logger.LogInformation("Cache miss for ZipCode {ZipCode} - fetching from ViaCEP", addressDto.ZipCode);

            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            return await _viaCepClient.GetAddressByZipCodeAsync(normalizedZipCode);
        });

        _logger.LogInformation("Creating Address entity from ViaCEP data for ZipCode {ZipCode}", addressDto.ZipCode);

        var address = new Address(
            id: Guid.NewGuid(),
            ZipCode.Create(addressDto.ZipCode),
            Street.Create(viaCepData.Street),
            Neighborhood.Create(viaCepData.Neighborhood),
            City.Create(viaCepData.City),
            State.Create(viaCepData.State, viaCepData.StateAbbreviation),
            Region.Create(viaCepData.Region)
        );

        var addressWithOptionalFields = UpdateAddressOptionalFields(address, addressDto.Number, addressDto.Complement, addressDto.Reference);

        var addressCreated = await _unitOfWork.AddressRepository.AddAsync(addressWithOptionalFields);
        await _unitOfWork.Commit();

        _logger.LogInformation("Address {AddressId} created successfully for ZipCode {ZipCode}", addressCreated.Id, addressDto.ZipCode);

        return new AddressDto(
            Id: addressCreated.Id,
            ZipCode: addressCreated.ZipCode.Value,
            Street: addressCreated.Street.Value,
            Number: addressCreated.Number,
            Complement: addressCreated.Complement,
            Reference: addressCreated.Reference,
            Neighborhood: addressCreated.Neighborhood.Value,
            City: addressCreated.City.Value,
            State: addressCreated.State.Name,
            Region: addressCreated.Region.Value
        );
    }

    public async Task UpdateAddressAsync(UpdateAddressDto addressDto)
    {
        _logger.LogInformation("Updating address {AddressId}", addressDto.Id);

        var result = _updateValidator.Validate(addressDto);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var address = await GetAddressEntityByIdAsync(addressDto.Id);

        var addressWithOptionalFieldsUpdated = UpdateAddressOptionalFields(address, addressDto.Number, addressDto.Complement, addressDto.Reference);

        await _unitOfWork.AddressRepository.UpdateAsync(addressWithOptionalFieldsUpdated);
        await _unitOfWork.Commit();

        _logger.LogInformation("Address {AddressId} updated successfully", addressDto.Id);

        var cacheKey = $"address:{addressDto.Id}";
        _cache.Remove(cacheKey);
    }

    public async Task DeleteAddressAsync(Guid id)
    {
        _logger.LogInformation("Deleting address {AddressId}", id);
        
        var address = await GetAddressEntityByIdAsync(id);

        await _unitOfWork.AddressRepository.DeleteAsync(address);
        await _unitOfWork.Commit();

        _logger.LogInformation("Address {AddressId} deleted successfully", id);

        var cacheKey = $"address:{id}";
        _cache.Remove(cacheKey);
    }

    private async Task<Address> GetAddressEntityByIdAsync(Guid id)
    {
        var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

        if (address is null)
        {
            _logger.LogWarning("Address with ID {AddressId} not found", id);

            throw new AddressNotFoundException($"Endereço com o ID '{id}' não encontrado");
        }

        return address;
    }

    private Address UpdateAddressOptionalFields(Address address, string? number, string? complement, string? reference)
    {
        if (!string.IsNullOrWhiteSpace(number))
        {
            address.UpdateNumber(number);
        }

        if (!string.IsNullOrWhiteSpace(complement))
        {
            address.UpdateComplement(complement);
        }

        if (!string.IsNullOrWhiteSpace(reference))
        {
            address.UpdateReference(reference);
        }

        return address;
    }
}
