using AddressManager.Application.DTOs;
using AddressManager.Application.Interfaces;
using AddressManager.Domain.Entities;
using AddressManager.Domain.Exceptions;
using AddressManager.Domain.Interfaces;
using AddressManager.Domain.Value_Objects;
using AddressManager.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;

namespace AddressManager.Application.Services;

public class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateAddressDto> _createValidator;
    private readonly IValidator<UpdateAddressDto> _updateValidator;
    private readonly IViaCepClient _viaCepClient;
    private readonly IMemoryCache _cache;

    public AddressService(IUnitOfWork unitOfWork, IViaCepClient viaCepClient, IValidator<CreateAddressDto> createValidator, IValidator<UpdateAddressDto> updateValidator, IMemoryCache memoryCache)
    {
        _unitOfWork = unitOfWork;
        _viaCepClient = viaCepClient;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _cache = memoryCache;
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
        var address = await _unitOfWork.AddressRepository.GetByIdAsNoTrackingAsync(id);

        if (address is null)
        {
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
        var result = _createValidator.Validate(addressDto);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var normalizedZipCode = addressDto.ZipCode.Replace("-", "").Trim();

        var cacheKey = $"address_{normalizedZipCode}";

        var viaCepData = await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            return await _viaCepClient.GetAddressByZipCodeAsync(normalizedZipCode);
        });

        if (viaCepData is null)
        {
            throw new AddressNotFoundException($"CEP '{addressDto.ZipCode}' não encontrado");
        }

        var address = new Address(
            id: Guid.NewGuid(),
            ZipCode.Create(addressDto.ZipCode),
            Street.Create(viaCepData.Street),
            Neighborhood.Create(viaCepData.Neighborhood),
            City.Create(viaCepData.City),
            State.Create(viaCepData.State, viaCepData.StateAbbreviation),
            Region.Create(viaCepData.Region)
        );

        if (!string.IsNullOrWhiteSpace(addressDto.Number))
        {
            address.UpdateNumber(addressDto.Number);
        }

        if (!string.IsNullOrWhiteSpace(addressDto.Complement))
        {
            address.UpdateComplement(addressDto.Complement);
        }

        if (!string.IsNullOrWhiteSpace(addressDto.Reference))
        {
            address.UpdateReference(addressDto.Reference);
        }

        var addressCreated = await _unitOfWork.AddressRepository.AddAsync(address);
        await _unitOfWork.Commit();

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
        var result = _updateValidator.Validate(addressDto);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }

        var address = await GetAddressEntityByIdAsync(addressDto.Id);

        if (address is null)
        {
            throw new AddressNotFoundException($"Endereço com o ID '{addressDto.Id}' não encontrado");
        }

        if (!string.IsNullOrWhiteSpace(addressDto.Number))
        {
            address.UpdateNumber(addressDto.Number);
        }

        if (!string.IsNullOrWhiteSpace(addressDto.Complement))
        {
            address.UpdateComplement(addressDto.Complement);
        }

        if (!string.IsNullOrWhiteSpace(addressDto.Reference))
        {
            address.UpdateReference(addressDto.Reference);
        }

        await _unitOfWork.AddressRepository.UpdateAsync(address);
        await _unitOfWork.Commit();
    }

    public async Task DeleteAddressAsync(Guid id)
    {
        var address = await GetAddressEntityByIdAsync(id);

        await _unitOfWork.AddressRepository.DeleteAsync(address);
        await _unitOfWork.Commit();
    }

    private async Task<Address> GetAddressEntityByIdAsync(Guid id)
    {
        var address = await _unitOfWork.AddressRepository.GetByIdAsync(id);

        if (address is null)
        {
            throw new AddressNotFoundException($"Endereço com o ID '{id}' não encontrado");
        }
        return address;
    }
}
