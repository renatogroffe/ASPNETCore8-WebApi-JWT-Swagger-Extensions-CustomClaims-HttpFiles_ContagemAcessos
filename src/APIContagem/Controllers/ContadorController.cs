using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using APIContagem.Models;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize("Bearer")]
public class ContadorController : ControllerBase
{
    private static Contador _CONTADOR = new Contador();

    [HttpGet]
    [ProducesResponseType(typeof(ResultadoContador), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public ResultadoContador Get(
        [FromServices] ILogger<ContadorController> logger,
        [FromServices] IConfiguration configuration)
    {
        int valorAtualContador;
        lock (_CONTADOR)
        {
            _CONTADOR.Incrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }
        logger.LogInformation($"Contador - Valor atual: {valorAtualContador}");

        if (Convert.ToBoolean(Convert.ToBoolean(configuration["SimularFalha"])) &&
            valorAtualContador % 10 == 0)
            throw new Exception("Simulação de falha");

        return new()
        {
            ValorAtual = _CONTADOR.ValorAtual,
            Local = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Mensagem = configuration["Saudacao"],
            Framework = _CONTADOR.Framework
        };
    }

    [HttpGet("decrementar")]
    [ProducesResponseType(typeof(ResultadoContador), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public ResultadoContador GetDecrementar(
        [FromServices] ILogger<ContadorController> logger,
        [FromServices] IConfiguration configuration)
    {
        int valorAtualContador;
        lock (_CONTADOR)
        {
            _CONTADOR.Decrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }
        logger.LogInformation($"Contador (Decremento) - Valor atual: {valorAtualContador}");

        if (Convert.ToBoolean(Convert.ToBoolean(configuration["SimularFalha"])) &&
            valorAtualContador % 10 == 0)
            throw new Exception("Simulação de falha");

        return new()
        {
            ValorAtual = _CONTADOR.ValorAtual,
            Local = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Mensagem = configuration["Saudacao"],
            Framework = _CONTADOR.Framework
        };
    }
}