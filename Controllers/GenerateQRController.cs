using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Models.Models.Request;
using Web.Api.Models.Models.Response;
using Web.Api.Models.OperationResult;
using Web.Api.Models.Settings;
using Web.Api.RepositoryTrax.Core;

namespace WEBAPITRAX.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GenerateQRController : Controller
    {
        public IConfiguration Configuration { get; }
        public IConfiguration _configuration;
        //private readonly DataBaseContext _context;
        CoreApi implement = null;
        private readonly IOptions<AppSettings> settings;

        private readonly ILogger logger;

        public GenerateQRController(IOptions<AppSettings> _appSettings, ILogger<UsersController> logger)
        {
            implement = new CoreApi(_appSettings);
            settings = _appSettings;
            this.logger = logger;
        }
        [HttpPost]
        [Route("Api/GenerateCodeQR")]
        //[Authorize]
        public QRGenerateResponseDTO GenerateCodeQR(QRGenerateRequestDTO _Request)
        {
            QRGenerateResponseDTO _Response = new QRGenerateResponseDTO();
            try
            {
                if (!ModelState.IsValid)
                {
                    var _Errors = ModelState.Values.SelectMany(x => x.Errors).ToList();
                    _Response.Result.SetStatusCode(OperationResult.StatusCodesEnum.BAD_REQUEST);
                    _Errors.ForEach(x => { if (x.Exception == null) _Response.Result.AddException(new Exception(x.ErrorMessage)); else _Response.Result.AddException(x.Exception); });
                    return _Response;
                }

                _Response = implement.GenerateCodeQR(_Request);
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
