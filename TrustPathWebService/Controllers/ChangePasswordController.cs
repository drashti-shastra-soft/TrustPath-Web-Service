using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrustPathWebService.Infrastructure.CustomException;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Controllers
{
    [Authorize, RoutePrefix("api/ChangePassword")]
    public class ChangePasswordController : ApiController
    {
        private readonly IChangePasswordService _service;

        public ChangePasswordController(IChangePasswordService service)
        {
            _service = service;
        }

        [HttpPut]
        public HttpResponseMessage Upsert(Domain.ChangePassword mChangePassword)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Upsert(mChangePassword);
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }
    }
}
