using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Models.Models.Response;
using Web.Api.Models.OperationResult;
using Web.Api.Models.Settings;
using Web.Api.RepositoryTrax.Core;

namespace WEBAPITRAX.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        CoreApi implement = null;
        private readonly ILogger logger;
        //private readonly NegocioExpedienteElectronico negocio;

        public UsersController(IOptions<AppSettings> _appSettings, ILogger<UsersController> logger)
        {
            implement = new CoreApi(_appSettings);
            this.logger = logger;
        }
        [HttpPost]
        [Route("Api/GetUsers")]
        //[Authorize]
        public ObtenerUsuariosResponseDTO GetUsers()
        {
            ObtenerUsuariosResponseDTO _Response = new ObtenerUsuariosResponseDTO ();
            try
            {
                if (!ModelState.IsValid)
                {
                    var _Errors = ModelState.Values.SelectMany(x => x.Errors).ToList();
                    _Response.Result.SetStatusCode(OperationResult.StatusCodesEnum.BAD_REQUEST);
                    _Errors.ForEach(x => { if (x.Exception == null) _Response.Result.AddException(new Exception(x.ErrorMessage)); else _Response.Result.AddException(x.Exception); });
                    return _Response;
                }
               
                _Response = implement.ObtenerUsuarios();
            }
            catch (Exception ex)
            {
                //this._Logger.LogError(ex);
                _Response.Result.SetStatusCode(OperationResult.StatusCodesEnum.INTERNAL_SERVER_ERROR);
                _Response.Result.AddException(ex);
            }
            return _Response;
        }


    }
}
