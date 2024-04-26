using HealthInsuranceSystem.Core.Extensions;

using Microsoft.AspNetCore.Mvc;

namespace HealthInsuranceSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Base controller
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected new IActionResult Ok()
        {
            return base.Ok(Envelope.Ok());
        }

        /// <summary>
        /// Response for okay with result type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult Ok<T>(T result)
        {
            return base.Ok(Envelope.Ok(result));
        }

        /// <summary>
        /// Response for Conflict with result type T at all bussiness errors
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        protected IActionResult BussinessError<T>(T result)
        {
            return base.Conflict(Envelope.Ok(result));
        }

        /// <summary>
        /// Envelop for error messages
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected IActionResult Error(string errorMessage)
        {
            return BadRequest(Envelope.Error(errorMessage));
        }

        /// <summary>
        /// Envelop for error messages
        /// </summary>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        protected IActionResult ErrorList(List<string> errorMessages)
        {
            return BadRequest(Envelope.ErrorList(errorMessages));
        }
    }
}
