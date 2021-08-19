using Catalogue.API.Entities;
using Catalogue.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalogue.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogueController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogueController> _logger;
        public CatalogueController(IProductRepository repository, ILogger<CatalogueController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _repository.GetProductById(id);
            if(product == null)
            {
                _logger.LogError($"Product with id {id} was not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet]
        [Route("[action]/{category}")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var product = await _repository.GetProductsByCategory(category);
            if (product == null)
            {
                _logger.LogError($"Product with category {category} was not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>>CreateProduct([FromBody]Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<IActionResult>UpdateProduct([FromBody]Product product)
        {
            return Ok(await _repository.UpdateProduct(product));
        }

        [HttpDelete("{id:length(24)}", Name ="DeleteProduct")]
        [ProducesResponseType(typeof(Product),(int)HttpStatusCode.OK)]
        public async Task<IActionResult>DeleteProductById(string id)
        {
            return Ok(await _repository.DeleteProduct(id));
        }

    }
}
