using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TrustPathWebService.Infrastructure.CustomException;
using TrustPathWebService.Infrastructure.IService;

namespace TrustPathWebService.Controllers
{
    [Authorize, RoutePrefix("api/BusinessType")]
    public class BusinessTypeController : ApiController
    {
        private readonly IBusinessTypeService _service;

        public BusinessTypeController(IBusinessTypeService service)
        {
            _service = service;
        }

        [HttpPost, Route("GetAll")]
        public HttpResponseMessage GetAll(Domain.BusinessTypeLister mLister)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetAll(mLister));
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

        [HttpPost]
        public HttpResponseMessage Upsert(Domain.BusinessType mBusinessType)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mBusinessType));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unexpected Error: " + ex.Message);
            }
            return response;
        }

        [HttpGet, Route("{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Get(id));
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

        [HttpDelete, Route("{id:int}/LoginUser/{loginUserId:int}")]
        public HttpResponseMessage Delete(int id, int loginUserId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                _service.Delete(id, loginUserId);
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

        [HttpGet]
        public HttpResponseMessage GetActiveBusinessType()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetActiveBusiness());
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
