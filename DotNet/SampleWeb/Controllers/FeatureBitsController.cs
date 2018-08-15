using System.Linq;
using FeatureBits.Core;
using Microsoft.AspNetCore.Mvc;

namespace SampleWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureBitsController : ControllerBase
    {
        private readonly IFeatureBitEvaluator _evaluator;

        public FeatureBitsController(IFeatureBitEvaluator evaluator)
        {
            _evaluator = evaluator;
        }

        [HttpGet("/{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var definition = _evaluator.Definitions.SingleOrDefault(d => d.Id == id);

            if (definition != null)
            {
                bool isEnabled = _evaluator.IsEnabled((Features) id, 0);
                return new JsonResult(isEnabled);
            }

            return NotFound();
        }
    }
}