using AddressManager.Application.DTOs;
using AddressManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AddressManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    /// <summary>
    /// Obtém uma lista de todos os endereços com paginação.
    /// </summary>
    /// <remarks>
    /// Este endpoint retorna uma lista paginada de todos os endereços cadastrados.
    /// </remarks>
    /// <param name="pageNumber">O número da página a ser retornada.</param>
    /// <param name="pageSize">O número de endereços por página.</param>
    /// <returns>Uma lista de endereços.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AddressDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> Get([FromRoute] int pageNumber = 1, [FromRoute] int pageSize = 25)
    {
        var addresses = await _addressService.GetAllAddressesAsync(pageNumber, pageSize);

        return Ok(addresses);
    }

    /// <summary>
    /// Obtém um endereço específico pelo o ID.
    /// </summary>
    /// <remarks>
    /// Retorna os dados de um endereço correspondente ao ID fornecido.
    /// </remarks>
    /// <param name="id">O ID único do endereço a ser retornado.</param>
    /// <returns>O endereço correspondente ao ID, ou 404 Not Found se o endereço não for encontrado.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> Get([FromRoute] Guid id)
    {
        var address = await _addressService.GetAddressByIdAsync(id);

        return Ok(address);
    }

    /// <summary>
    /// Cria um novo endereço a partir de um CEP.
    /// </summary>
    /// <remarks>
    /// Este endpoint simplifica o processo de cadastro de endereços. Basta fornecer o CEP, e a API automaticamente busca e preenche os dados restantes do endereço (logradouro, bairro, etc.) através de uma integração com o serviço ViaCEP.
    ///
    /// Você também pode incluir informações adicionais e opcionais, como número, complemento e ponto de referência, para um cadastro mais completo.
    /// </remarks>
    /// <param name="addressDto">Os dados do endereço a ser criado.</param>
    /// <returns>O endereço criado com um ID único.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AddressDto>> Post([FromBody] CreateAddressDto addressDto)
    {
        var createdAddress = await _addressService.CreateAddressAsync(addressDto);

        return CreatedAtAction(nameof(Get), new { id = createdAddress.Id }, createdAddress);
    }

    /// <summary>
    /// Atualiza um endereço existente.
    /// </summary>
    /// <remarks>
    /// Este endpoint permite que você atualize dados complementares de um endereço já existente, como o número, complemento e ponto de referência.
    /// </remarks>
    /// <param name="id">O ID do endereço a ser atualizado. Deve corresponder ao ID no corpo da requisição.</param>
    /// <param name="addressDto">Os novos dados para os campos opcionais (número, complemento e referência).</param>
    /// <returns>Retorna 204 No Content se a atualização for bem-sucedida, ou 404 Not Found se o endereço não for encontrado.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateAddressDto addressDto)
    {
        if (id != addressDto.Id)
        {
            return BadRequest("O ID do endereço não corresponde ao ID fornecido na URL.");
        }

        await _addressService.UpdateAddressAsync(addressDto);

        return NoContent();
    }

    /// <summary>
    /// Exclui um endereço pelo o ID.
    /// </summary>
    /// <remarks>
    /// Este endpoint realiza a exclusão de um endereço cadastrado.
    /// </remarks>
    /// <param name="id">O ID único do endereço a ser excluído.</param>
    /// <returns>Retorna 204 No Content se a exclusão for bem-sucedida, ou 404 Not Found se o endereço não for encontrado.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _addressService.DeleteAddressAsync(id);

        return NoContent();
    }
}
